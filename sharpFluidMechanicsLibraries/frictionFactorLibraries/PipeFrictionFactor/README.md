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


Unfortunately, we don't quite have one input and one output. So we'll
have to change types.

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


Here's the result:

```csharp

double pressureDropRoot(double Re){

	// fanning term
	//
	double fanningTerm;
	fanningTerm = this.fanning(Re, this.roughnessRatio);
	fanningTerm *= Math.Pow(Re,2.0);


	//  BejanTerm
	//
	double bejanTerm;
	bejanTerm = 32.0 * this.bejanNumber;
	bejanTerm *= Math.Pow(4.0*this.lengthToDiameter,-3);

	// to set this to zero, we need:
	//
	return fanningTerm - bejanTerm;

}
```

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

And then 1e8 as the maximum. That's the upper limit of the moody
chart.

```csharp
double ReynoldsNumber;
ReynoldsNumber = FindRoots.OfFunction(pressureDropRoot, 1, 1e8);
```

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












