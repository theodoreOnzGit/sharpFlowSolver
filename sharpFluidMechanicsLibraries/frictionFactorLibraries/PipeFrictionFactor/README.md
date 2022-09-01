# ChurchillFrictionFactor Readme


## Thread Safety Warning!

Thread Safety is not tested nor guaranteed for this code. You
have been warned. It is not currently a goal of this code as of
the time of writing this readme (Aug 2022). If you use it in 
parallel processing, use at your own risk. Or at least supply
a different copy of the library for each thread.

## Design and Interfaces

The main goal of the ChurchillFrictionFactor class is to
provide the darcy, moody and fanning friction factor given
a Reynold's number and roughness ratio.

However, since there are many other ways of finding friction factor
I have written an IFrictionFactor interface so that we can
have many ways of implementing the friction factor.

Eg. Colebrook Formula.



### IFrictionFactor Interface

The IFrictionFactor interface just returns the fanning, moody
and darcy friction factor given a Re and roughness Ratio 
$\frac{\varepsilon}{D}$.

Note that moody and darcy friction factor are basically the same
thing. 

But I put it there for ease of use.

```csharp
public interface IFrictionFactor
{
	double fanning(double ReynoldsNumber, double roughnessRatio);
	double moody(double ReynoldsNumber, double roughnessRatio);
	double darcy(double ReynoldsNumber, double roughnessRatio);
}
```

The moody and darcy friction factor are identitcal in formula,
it's just that the difference in names may cause some confusion at
times, and people may wonder if it's a different friction factor.

In the laminar regime [(Brown, 2002)](#brown2002):

$$f_{darcy} = f_{moody} = \frac{64}{Re}$$

And the fanning friction factor is as follows 
[(Welty et al. 2020)](#welty2020):
$$f_{fanning} = \frac{16}{Re}$$

While there are several friction factor methods, the Churchill 
friction factor is able to use a singular equation for laminar,
transitional and turbulent flow and also have a mean error compared
to the colebrook correlation of about 0.475 - 0.48%  
[(Winning And Coole, 2013)](#winning2013). This figure is largely
dependent on roughness ratio but has not been reported to exceed
0.48% [(Turgut et al., 2014)](#turgut2014).

Given this satisfactory performance (I define satisfactory as <1% 
error compared to colebrook), I have opted to stick with the
Churchill correlation as it can provide a continuous function
to help estimate friction factors between laminar and turbulent 
regions.

# ChurchHillFrictionFactor.cs
Churchill friction factor [(Winning and Coole, 2013)](#winning2013)
is defined by:


$$f_{fanning} = 2 \left[\\
\left( \frac{8}{Re} \right)^{12} + \\
\left( \frac{1}{A+B}\right)^{3/2} \\
\right]^{1/12} $$

$$f_{Darcy} = 8 \left[\\
\left( \frac{8}{Re} \right)^{12} + \\
\left( \frac{1}{A+B}\right)^{3/2} \\
\right]^{1/12} $$

$$A = \left[ 2.457 \ln \frac{1}{\left( (7/Re)^{0.9} + \\
0.27 \frac{\varepsilon}{D} \right)} \\
\right]^{16}\ \ ; \ \ \\
B = \left( \frac{37530}{Re} \\ 
\right)^{16} $$


## Defining A and B in the Code

For now, all the parameters needed for the Churchill Correlation
are dimensionless. Therefore we will just use doubles to represent
dimensionless numbers.

$$A = \left[ 2.457 \ln \frac{1}{\left( (7/Re)^{0.9} + \\
0.27 \frac{\varepsilon}{D} \right)} \\
\right]^{16}\ \ ; \ \ \\$$

A is defined in code as the following:

```csharp
private double A(double Re, double roughnessRatio){
	// first i need the logarithm of a number

	double reynoldsTerm =  Math.Pow( (7.0/Re), 0.9);
	double roughnessTerm = 0.27*roughnessRatio;

	double logFraction = 1.0/(reynoldsTerm+roughnessTerm);
	double innerBracketTerm = 2.457*Math.Log(logFraction);
	double A = Math.Pow(innerBracketTerm,16);
	
	return A;
}

```


$$B = \left( \frac{37530}{Re} \\ 
\right)^{16} $$

```csharp

private double B(double Re){
	double numerator = Math.Pow(37530,16);
	double denominator = Math.Pow(Re,16);
	return numerator/denominator;
}


```
##  intermediate calculation

Now, to keep the code neat, such that we don't have too much
math going on in one line, i have seprated out the calculations
for the A and B term into different functions. Also, I have
made this innerTerm function to calculate the following parameter.



$$innerTerm =  \left[\\
\left( \frac{8}{Re} \right)^{12} + \\
\left( \frac{1}{A+B}\right)^{3/2} \\
\right] $$

```csharp

private double churchillInnerTerm(double Re, double roughnessRatio){

	double laminarTerm;
	laminarTerm = Math.Pow(8.0/Re, 12);

	double turbulentTerm;
	double Aterm = this.A(Re,roughnessRatio);
	double Bterm = this.B(Re);

	turbulentTerm = Math.Pow( 1.0/(Aterm + Bterm), 3.0/2);

	return laminarTerm + turbulentTerm;


}


```

If this inner term is calculated well, we can just use the Math.Pow
formula to calculate the fanning friction factor.

## fanning friction factor

So to calculate fanning friction factor,

```csharp
public double fanning(double ReynoldsNumber, double roughnessRatio){

	double fanningFrictionFactor;
	fanningFrictionFactor = 2 * Math.Pow(this.churchillInnerTerm(ReynoldsNumber,roughnessRatio), 1.0/12);
	return fanningFrictionFactor;
}
```

$$f_{fanning} = 2 \left[\\
\left( \frac{8}{Re} \right)^{12} + \\
\left( \frac{1}{A+B}\right)^{3/2} \\
\right]^{1/12} $$

## Darcy and Moody Friction factor methods
Darcy friction  factor just multiples fanning friction factor
by 4..

```csharp
public double darcy(double ReynoldsNumber, double roughnessRatio){

	// darcy friction factor is 4x fanning friction factor
	// https://neutrium.net/fluid-flow/pressure-loss-in-pipe/
	double darcyFrictionFactor;
	darcyFrictionFactor = 4 * this.fanning(ReynoldsNumber,roughnessRatio);
	return darcyFrictionFactor;
}
```

Moody Friction factor just calls the Darcy friction factor method.

```csharp

public double moody(double ReynoldsNumber, double roughnessRatio){

	// apparently the moody friciton factor is same as the darcy friction factor

	return this.darcy(ReynoldsNumber,roughnessRatio);
}
```

## Usage 

Just instantiate the object and use the fanning friction factor term
straightaway.

## Caution for Re <= 0

When calculating friction factor, it is important that Re>0. 
Otherwise, we will have a divide by zero exception.

Thus, this correlation becomes undefined at zero flow. Caution
should be exercised when using this correlation to calculate
pressure losses at zero mass flowrate or Re.

Also, when Re is negative, or flow is perhaps reversed, the equation
will also not make any sense. Thus we must be careful to watch out
when using this equation for zero flow or reverse flow.

### Exception handling for invalid values
To sort this issue out, i'll make sure the code lets you 
know that invalid values are given.

Also a roughnessRatio less than zero also doesn't make physical sense.

I'll also ensure the code throws an exception to let you know that's the case.

```csharp
public double darcy(double ReynoldsNumber, 
		double roughnessRatio){
	if(ReynoldsNumber == 0)
		throw new DivideByZeroException("Re = 0");

	if(ReynoldsNumber < 0)
		throw new ArgumentOutOfRangeException("Re<0");
	
	if(roughnessRatio < 0)
		throw new ArgumentOutOfRangeException("roughnessRatio<0");

	// darcy friction factor is 4x fanning friction factor
	// https://neutrium.net/fluid-flow/pressure-loss-in-pipe/
	double darcyFrictionFactor;
	darcyFrictionFactor = 4 * this.fanning(ReynoldsNumber,roughnessRatio);
	return darcyFrictionFactor;
}
```

# finding Re from pressure drop (nondimensional pressure drop Be)
Now in finding our friction factor from Reynold's number, it is
a relatively simple affair when we calculate churchill friction 
factor. Nevertheless, if we wanted to find the mass flowrate, 
velocity or Re from a given friction factor, we would be in trouble
because one friction factor value can have multiple Re which
yield the same friction factor within laminar, 
transition and turbulent region.

Therefore, in practice, it is better to supply a pressure loss
term, or pressure drop term, or the nondimensionalised equivalent
to solve for an explicit value of Re.

## getBe and fLDK

For pressure drop, we have the explicit correlation
$$f_{fanning}(Re,\frac{\varepsilon}{D})* Re^2 = \frac{32 Be_L}{ (\frac{4L}{D})^3 
}$$

This is derived via nondimensionalising the Darcy Weisbach equation
[(Turgut et al., 2014)](#turgut2014):

$$f_{fanning} \frac{4L}{D} \  \frac{1}{2} \rho u^2 = \Delta P_{loss}$$
$$f_{darcy} \frac{L}{D} \  \frac{1}{2} \rho u^2 = \Delta P_{loss}$$

I note here the $\Delta P_{losses}$ is referring to pressure losses.

However, when it comes to writing code, we may not wish to leave it
in dimensional form as it is very easy to make mistakes with units.

If we nondimensionalise the pressure loss terms and velocities, we
may avoid such unit based errors within the code. 

We shall conveniently nondimensionalise velocity using Re,

$$Re = \frac{u D_H \rho}{\mu}$$

And this results in:

$$f_{fanning}  Re^2 = \frac{\Delta P_{loss}}{ (\frac{4L}{D}) \  
\frac{1}{2} \rho (\frac{ \mu}{\rho D})^2 }$$
To nondimensionalise the pressure loss terms,
The [Bejan Number](https://www.sciencedirect.com/science/article/abs/pii/S0735193321000075)
[(Zimparov et al., 2021)](#zimparov2021)
is a number that represents a sort of dimensionless pressure drop.

It was originally derived for heat transfer, but can just 
as easily apply for fluid mechanics.

For heat transfer, it is defined as:

$$Be_L = \frac{\Delta P L^2 }{\mu \alpha }$$


For our case:
$$Be_L = \frac{\Delta P_{loss} L^2}{\nu^2\rho} $$
$$Be_L = \frac{\Delta P_{loss} L^2 \rho}{\mu^2} $$

Subtituting this definition of the Bejan number results in:

$$f_{fanning}(Re,\frac{\varepsilon}{D})* Re^2 = \frac{32 Be_L}{ (\frac{4L}{D})^3 
}$$

Now this is okay for pipes without any form losses. But for pipes with
form losses, there is an extra K term which may appear inside:

$$(f_{darcy} \frac{L}{D} +K)  \frac{1}{2} \rho u^2 = \Delta P_{loss}$$

Therefore i have taken the liberty to redefine the Bejan number so that
it can fit our case:

$$Be_D = \frac{\Delta P_{loss} D^2}{\nu^2\rho} $$

With this, we can nondimensionalise:
$$(f_{fanning}  \frac{4L}{D}+K) Re^2 = \frac{\Delta P_{loss}}{   
\frac{1}{2} \rho (\frac{ \mu}{\rho D})^2 }$$

Into:

$$Be_D = 0.5 (f_{fanning}  \frac{4L}{D}+K) Re^2$$
$$Be_D = 0.5 (f_{darcy} \frac{L}{D}+K) Re^2$$


This has the additional advantage of being easier to remember as there
isn't a cube length to diameter term, or a constant of 32.

Thus, the solution procedure when it comes to finding pipe mass flowrates
and pressure losses is often to nondimensionalise the pressure loss and 
velocity/mass flowrate terms and use this equation to solve for Be or Re.

And then redimensionalise the terms.

With this in mind, i'd like to have quick methods to calculate fLDK terms:


$$fLDK = (f_{darcy} \frac{L}{D} +K) = (f_{fanning} \frac{4L}{D} +K)$$

This term is convenient for us because it will negate the constant need
to differentiate between $f_{darcy}$ and $f_{fanning}$. 

This will be available to the end user to calculate.

That about sums it up for the pipe friction factor.

Here's the code with all the exception handling etc.

```csharp
public double fLDK(double ReynoldsNumber,
		double roughnessRatio,
		double lengthToDiameterRatio,
		double K){
	if(ReynoldsNumber == 0)
		throw new DivideByZeroException();

	if(ReynoldsNumber < 0)
		throw new ArgumentOutOfRangeException();

	if(roughnessRatio < 0)
		throw new ArgumentOutOfRangeException("roughnessRatio<0");

	if(lengthToDiameterRatio <= 0)
		throw new ArgumentOutOfRangeException(
				"lengthToDiameterRatio<=0");

	if(K < 0)
		throw new ArgumentOutOfRangeException(
				"Form loss coefficient K < 0");

	double fLDK;
	double f = this.darcy(ReynoldsNumber,
			roughnessRatio);
	fLDK = f*lengthToDiameterRatio + K;

	return fLDK;
}
```

Basically, if the form loss coefficents, lengthToDiameter ratios are less
than zero, i will also throw exceptions.

However, we still want for us to find the bejan number. That's just the
fLDK multiplied by $0.5 Re^2$ 

$$Be = 0.5* fLDK * Re^2$$

One would think that just substituting this equation into a function would work.

However, if Re = 0, we would get an error again.

To sort this out, I would then return Be = 0  if Re = 0.

Lastly, i made a tweak, basically to save on some calculation
cost when Re is in the laminar region.

When in the laminar region, 
$$f_{darcy} = \frac{64}{Re}$$
$$fLDK*Re^2 = \frac{L}{D} * 64Re + K*Re^2 $$

Nevertheless, there is potential discontinuity for 64Re and the
value given by the churchill friction factor.

My solution then is to perform linear interpolation, taking
Re=0 and $f_{darcy}*Re^2=0$ and Re=1800 and 
$f_{darcy}*Re^2 = 64*1800$ as fixed points

```csharp
public double getBe(double ReynoldsNumber,
		double roughnessRatio,
		double lengthToDiameterRatio,
		double K){

	if(ReynoldsNumber == 0)
		return 0.0;

	if(ReynoldsNumber < 0)
		throw new ArgumentOutOfRangeException("Re < 0");

	if(roughnessRatio < 0)
		throw new ArgumentOutOfRangeException("roughnessRatio<0");

	if(lengthToDiameterRatio <= 0)
		throw new ArgumentOutOfRangeException(
				"lengthToDiameterRatio<=0");

	if(K < 0)
		throw new ArgumentOutOfRangeException(
				"Form loss coefficient K < 0");

	// i'm including an improvement for Re<1800
	// so that we linearly interpolate the churchill
	// friction factor from 1800 onwards
	// basically, we are interpolating
	// f_{darcy}*Re^2  = 64Re
	// at lower Re values to increase accuracy and save computation
	// cost
	if(ReynoldsNumber <1800){
		double ReTransition = 1800.0;
		IInterpolation _linear_f_ReSq;

		IList<double> xValues = new List<double>();
		IList<double> yValues = new List<double>();
		xValues.Add(0.0);
		xValues.Add(ReTransition);

		yValues.Add(0.0);
		yValues.Add(64.0*ReTransition);

		_linear_f_ReSq = Interpolate.Linear(xValues,yValues);
		double fLDReSq = _linear_f_ReSq.Interpolate(
				ReynoldsNumber)*lengthToDiameterRatio;

		double kReSq = K*Math.Pow(ReynoldsNumber,2.0);
		
		return 0.5*(kReSq + fLDReSq);

	}

	double fLDK;
	double f = this.darcy(ReynoldsNumber,
			roughnessRatio);
	fLDK = f*lengthToDiameterRatio + K;

	double Be = 0.5*fLDK*
		Math.Pow(ReynoldsNumber,2.0);

	return Be;
}
```




This will be under dimensionless calculations...

## getRe
We previously derived:
$$Be = 0.5* fLDK * Re^2$$
This becomes the equation we will solve iteratively for Re given a Be.

Now, i also developed the following code for a first iteration
based on the following equation:

$$f_{fanning}(Re,\frac{\varepsilon}{D})* Re^2 = \frac{32 Be_L}{ (\frac{4L}{D})^3 
}$$

While i favour the use of $Be_D$ now, i have use $Be_L$ in the above equation.

We can freely convert between $Be_L$ and $Be_D$ :
$$Be_L = \frac{\Delta P_{loss} L^2 \rho}{\mu^2} $$
$$Be_D = \frac{\Delta P_{loss} D^2 \rho}{\mu^2} $$
$$Be_L = Be_D *(\frac{L}{D})^2$$

Thus the code can still be made valid for pipes with zero form losses.

### Code details for Pipe with zero form loss

We want to use the Mathnet Numerics library

so in our csproj file we have:

```xml
<PackageReference Include="MathNet.Numerics" Version="5.0.0" />
```

I'm using the findRoots.cs [file](https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/FindRoots.cs):

```csharp

namespace MathNet.Numerics
{
    public static class FindRoots
    {
        /// <summary>Find a solution of the equation f(x)=0.</summary>
        /// <param name="f">The function to find roots from.</param>
        /// <param name="lowerBound">The low value of the range where the root is supposed to be.</param>
        /// <param name="upperBound">The high value of the range where the root is supposed to be.</param>
        /// <param name="accuracy">Desired accuracy. The root will be refined until the accuracy or the maximum number of iterations is reached. Example: 1e-14.</param>
        /// <param name="maxIterations">Maximum number of iterations. Example: 100.</param>
        public static double OfFunction(Func<double, double> f, double lowerBound, double upperBound, double accuracy = 1e-8, int maxIterations = 100)
        {
            if (!ZeroCrossingBracketing.ExpandReduce(f, ref lowerBound, ref upperBound, 1.6, maxIterations, maxIterations*10))
            {
                throw new NonConvergenceException("The algorithm has failed, exceeded the number of iterations allowed or there is no root within the provided bounds.");
            }

            if (Brent.TryFindRoot(f, lowerBound, upperBound, accuracy, maxIterations, out var root))
            {
                return root;
            }

            if (Bisection.TryFindRoot(f, lowerBound, upperBound, accuracy, maxIterations, out root))
            {
                return root;
            }

            throw new NonConvergenceException("The algorithm has failed, exceeded the number of iterations allowed or there is no root within the provided bounds.");
        }

```

So I'll be using the namespace MathNet.Numerics and use the static class
FindRoots.

The way to call it inline is:

```csharp

FindRoots.OfFunction(func<double,double> f, lowerBound, upperBound);
```


Unfortunately, we don't quite have a one input and one output function,
our Bejan number is dependent on L/D, Re, roughness ratio etc.

So we'll have to resort to some object oriented trickery in order to do this.

Meaning to say that all else constant, the function is such that
Reynold's number is the input and the LHS-RHS is the output.

Which doesn't quite give us pure functions so to speak, since we 
are using properties. But I'll try as far as i can.

Setting the equation to zero is


$$f_{fanning}(Re,\frac{\varepsilon}{D})* Re^2 - \frac{32 Be}{ (\frac{4L}{D})^3 
} = 0$$

So I'll need to create a function that takes Reynold's number as an input,
takes the Bejan number, roughness ratio and lengthToDiameter ratio
as constants. Then return the LHS-RHS as the output.


However, we know for a fact that zero pressure loss is possible. Nevertheless
this would imply that no flow is going through the pipe.

This means that Re = 0 will be supplied to the fanning friction factor 
function and we will get an error.

How can we circumvent this?

Basically in a no form loss scenario, (K = 0),

the fanning term can be reduced to:

$$f_{fanning} Re^2 = 16/Re *Re^2 = 16 Re$$

This means that we don't calculate fanning friction factor directly,
but just calculate the fanning term directly as 16Re.

Substituting Re = 0 in such an equation will be all right.

However, we also want this function to be continuous with the churchill 
friction factor as the fanning term calculated with 16Re may have some
discontinuities with the fanning term calculated using churchill correlation.

How can we then circumvent this?

We can use interpolation.

We use churchill term to calculate the fanning term in the laminar regime,
that's one fixed point, and using (Re = 0, fanningTerm = 0) 
as the second fixed point, perform linear interpolation.

Here's the result:

```csharp

	this.roughnessRatio = roughnessRatio;
	this.lengthToDiameter = lengthToDiameter;
	this.bejanNumber = Be_L;

	// I'll define a pressureDrop function with which to find
	// the Reynold's Number
	double pressureDropRoot(double Re){

		// fanning term
		//
		//
		// Now here is a potential issue for stability,
		// if Re = 0, the fanning friction factor is not well behaved,
		// Hence it's better to use the laminar term at low Reynold's number
		//
		// we note that in the laminar regime, 
		// f = 16/Re
		// so f*Re^2 = 16*Re
		double transitionPoint = 1800.0;
		double fanningTerm;

		if (Re > transitionPoint)
		{
			fanningTerm = this.fanning(
					Re, this.roughnessRatio);
			fanningTerm *= Math.Pow(Re,2.0);
		}
		else
		{
			// otherwise we return 16/Re*Re^2 or 16*Re
			// or rather an interpolated version to preserve the
			// continuity of the points.
			IInterpolation _linear;

			IList<double> xValues = new List<double>();
			IList<double> yValues = new List<double>();
			xValues.Add(0.0);
			xValues.Add(transitionPoint);

			yValues.Add(0.0);
			yValues.Add(this.fanning(transitionPoint,this.roughnessRatio)*
					Math.Pow(transitionPoint,2.0));

			_linear = Interpolate.Linear(xValues,yValues);
			fanningTerm = _linear.Interpolate(Re);
		}






		//  BejanTerm
		//
		double bejanTerm;
		bejanTerm = 32.0 * this.bejanNumber;
		bejanTerm *= Math.Pow(4.0*this.lengthToDiameter,-3);

		// to set this to zero, we need:
		//
		return fanningTerm - bejanTerm;

	}
}
```
So with the above code, we have successfully represented:
$$f_{fanning}(Re,\frac{\varepsilon}{D})* Re^2 - \frac{32 Be}{ (\frac{4L}{D})^3 
} = 0$$

The LHS is the fanningTerm. Which is

$$f_{fanning}(Re,\frac{\varepsilon}{D})* Re^2 $$

To calculate this i use the fanning friction factor function
within the churchill correlation class. (I'm adding methods to 
the same class)

Then I'm multiplying $Re^2$ to the fanning friction factor.

The RHS is the bejan term


$$ \frac{32 Be}{ (\frac{4L}{D})^3} $$

So I set the bejan term to 32.0*Be, the user's given Bejan number.
Then i multiplied that by four times the L/D ratio to the power
of -3 to obtain the 4L/D term in the denominator.

After that i use the FindRoots.OfFunction method
I set the minimum Re to be 1 (otherwise we'll get infinity in the
laminar region)

And then 1e12 as the maximum. That's the upper limit of the moody
chart.

```csharp
double maxRe = 1e12;
double ReynoldsNumber;
ReynoldsNumber = FindRoots.OfFunction(pressureDropRoot, 0, maxRe);
```

I can't make the upper limit too high, or else the bisection algorithm
won't converge. I think Re = 1e12 should cover all relevant pipe flow
cases especially for incompressible flow.

Unfortunately this means i have to use class parameters to share
variables within the function as constants.

```csharp

private double roughnessRatio;
private double lengthToDiameter;
private double bejanNumber;

```

I first use the function's variables to instantiate the values:

```csharp

this.roughnessRatio = roughnessRatio;
this.lengthToDiameter = lengthToDiameter;
this.bejanNumber = Be;
```

Then I perform calculations.
After I'm done, i set them all to zero.

```csharp

// once I'm done, i want to clean up all terms
this.roughnessRatio = 0.0;
this.lengthToDiameter = 0.0;
this.bejanNumber = 0.0;


// then let's return Re

return ReynoldsNumber;
```

This is to ensure that the object state doesn't really affect
the calculation, so it functions like a pure method or function.

And I also make sure to clean up every number after i'm done,
so that the memory is cleared.


The only thing that remains is to test it!

#### Valid values and Exceptions

I designed this code to be able to handle backflow, or negative Bejan numbers
due to negative pressure losses. (ie reverse flow)

I assume of course, that the pipe's pressure loss profile is symmetrical.
Meaning a 500 Pa pressure loss results in the same magnitude of flowrate
as a -500 Pa pressure loss.

If a negative Bejan number is supplied, then the bejan number is turned into
the postiive equivalent. And i capture the value in a boolean:

```csharp
bool isNegative;
if (Be_L < 0)
{
	Be_L *= -1;
	isNegative = true;
}
else 
{
	isNegative = false;
}

```

I then go on calculating as per normal, and then

```csharp
if (isNegative)
{
	return -ReynoldsNumber;
}
```

if the bejan number is negative, i return the negative Re equivalent.

For pipes without form losses, besides $Be_L$, i also consider roughness ratio
and length to diameter.

For pipes in particular, length to diameter of zero or less make no sense. We
do not have a pipe with zero length for sure. All pipes or components have
some lengthscale.

Also, we can have roughness ratio equal zero, but not less than zero.
That does not make physical sense.

So i will throw exceptions in those cases:


```csharp
if(lengthToDiameterRatio <= 0)
	throw new ArgumentOutOfRangeException(
			"lengthToDiameterRatio<=0");

if(roughnessRatio < 0)
	throw new ArgumentOutOfRangeException(
			"roughnessRatio<0");
```

In particular, we also don't want the Bejan number to exceed the amount
corresponding to Re=1e12. That is definitely out of range.

```csharp
	// this part deals with negative Be_L values
	// invalid Be_L values
	bool isNegative;
	if (Be_L < 0)
	{
		Be_L *= -1;
		isNegative = true;
	}
	else 
	{
		isNegative = false;
	}

	double maxRe = 1e12;

	// i calculate the Be_L corresponding to 
	// Re = 1e12
	double maxBe_D = this.getBe(maxRe,
			roughnessRatio, lengthToDiameter,
			0.0);
	double maxBe_L = maxBe_D*
		Math.Pow(lengthToDiameter,2.0);

	if(Be_L >= maxBe_L)
		throw new ArgumentOutOfRangeException(
				"Be too large");
```


### code details for pipes with form losses

Now that we've settled the base case of pipe Re vs Be, we can go on to 
build upon this code to include form losses.

I can of course define a special case for which there are zero form losses,
And that's where i will just use the no form loss code tested before.

i don't need to re-test it.

The code is as follows:

```csharp

public double getRe(double Be_D,
		double roughnessRatio,
		double lengthToDiameter,
		double formLossK){

	if(formLossK == 0){
		double Be_L = Be_D * Math.Pow(lengthToDiameter,
				2.0);
		return this.getRe(Be_L,
				roughnessRatio,
				lengthToDiameter);
	}

	if(lengthToDiameter <= 0)
		throw new ArgumentOutOfRangeException(
				"lengthToDiameterRatio<=0");

	if(roughnessRatio < 0)
		throw new ArgumentOutOfRangeException(
				"roughnessRatio<0");

	if(formLossK < 0)
		throw new ArgumentOutOfRangeException(
				"formLossK<0");

	// this part deals with negative Be_L values
	// invalid Be_L values
	bool isNegative;
	if (Be_D < 0)
	{
		Be_D *= -1;
		isNegative = true;
	}
	else 
	{
		isNegative = false;
	}

	double maxRe = 1e12;

	// i calculate the Be_D corresponding to 
	// Re = 1e12
	double maxBe_D = this.getBe(maxRe,
			roughnessRatio, lengthToDiameter,
			0.0);

	if(Be_D >= maxBe_D)
		throw new ArgumentOutOfRangeException(
				"Be too large");
	// the above checks for all the relevant exceptions
	// including formLossK < 0
	//
	// now we are ready to do root finding
	//
	// the underlying equation is 
	// Be = 0.5*fLDK*Re^2

	this.roughnessRatio = roughnessRatio;
	this.lengthToDiameter = lengthToDiameter;
	this.bejanNumber = Be_D;
	this.formLossK = formLossK;

	double pressureDropRoot(double Re){
		// i'm solving for
		// Be - 0.5*fLDK*Re^2 = 0 
		// the fLDK term can be calculated using
		// getBe
		//
		// now i don't really need the interpolation
		// term in here because when Re = 0,
		// Be = 0 in the getBe code.
		// so really, no need for fancy interpolation.

		double fLDKterm = this.getBe(Re,
				this.roughnessRatio,
				this.lengthToDiameter,
				this.formLossK);

		return this.bejanNumber - fLDKterm;

	}
	
	double ReynoldsNumber = 0.0;
	ReynoldsNumber = FindRoots.OfFunction(
			pressureDropRoot, 0, 
			maxRe);

	// now i'll clean everything up
	this.roughnessRatio = 0.0;
	this.lengthToDiameter = 0.0;
	this.bejanNumber = 0.0;
	this.formLossK = 0.0;


	if (isNegative)
	{
		return -ReynoldsNumber;
	}

	return ReynoldsNumber;
}
```





# Bibliography

##### Brown2002

Brown, G. O. (2002). The history of the Darcy-Weisbach equation for pipe flow resistance. Environmental and water resources history, 38(7), 34-43.

##### Welty2020

Welty, J., Rorrer, G. L., & Foster, D. G. (2020). Fundamentals of momentum, heat, and mass transfer. John Wiley & Sons.

##### Winning2013
Winning, H. K., & Coole, T. (2013). Explicit friction factor accuracy and computational efficiency for turbulent flow in pipes. Flow, turbulence and combustion, 90(1), 1-27.

##### Turgut2014

Turgut, O. E., Asker, M., & Coban, M. T. (2014). A review of non iterative friction factor correlations for the calculation of pressure drop in pipes. Bitlis Eren University Journal of Science and Technology, 4(1), 1-8.

##### Zimparov2021

Zimparov, V. D., Angelov, M. S., & Hristov, J. Y. (2021). Critical review of the definitions of the Bejan number-first law of thermodynamics. International Communications in Heat and Mass Transfer, 124, 105113.












