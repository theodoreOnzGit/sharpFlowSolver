# sharpFlowSolver
C# .NetStandard2.0 libraries and tests for functions usable in fluid mechanics

## Licensing

In these libraries, I use MadsKirkFoged EngineeringUnits libraries. These were
released under the following MIT license:

MIT License

Copyright (c) 2021 Mads Kirk Foged

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.

His EngineeringUnits nuget package is released under MIT license and is
meant to be kept that way. j

I also use the MathNET C# libraries in order to perform iterative calculations
and interpolations. Their license is presented here:

Copyright (c) 2002-2022 Math.NET

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.


However, this C# library, the sharpFlowSolver library
is released under GNU GPLv3. Meaning all derivative works released must be
released under GNU GPLv3. And the code must be open-sourced.


## Quick Start

### installation and use
```csharp
using sharpFluidMechanicsLibraries;
using EngineeringUnits;
using EngineeringUnits.Units;
```
After installing the nuget package,
append the above namespaces to your code, and start using the functions 
below.

### usage

The purpose of this library is to help you calculate common 
fluid mechanics based problems. 
Eg. 
1. Calculating friction factor
2. Calculating mass flowrate from pressure loss
3. Claculating pressure loss from mass flowrate


To help you do this, here are some useful functions:


#### Friction Factor
Firstly, darcy, moody and fanning friction factor.

In actual fact, darcy and moody friction factor are the same
in this code.

ie, in the laminar region, it defaults to 64/Re.
(Re<2300)

I just included both of these in case one might get confused about
them.

The fanning friction factor defaults to 16/Re in the laminar
region. (Re<2300)


```csharp
public void moodyFrictionFactor(){
	// Step 1, initiate the friction factor object
	PipeFrictionFactor frictionFactorObj;
	frictionFactorObj = new PipeFrictionFactor();

	// Step 2, define Re and roughness ratio
	double Re = 1800;
	double roughnessRatio = 0.0005;

	// Step 3, calculate friction factor

	double moody = frictionFactorObj.moody(Re,roughnessRatio);
}

public void darcyFrictionFactor(){
	// Step 1, initiate the friction factor object
	PipeFrictionFactor frictionFactorObj;
	frictionFactorObj = new PipeFrictionFactor();

	// Step 2, define Re and roughness ratio
	double Re = 4e9;
	double roughnessRatio = 0.0005;

	// Step 3, calculate friction factor

	double darcy = frictionFactorObj.darcy(Re,roughnessRatio);
	// note that darcy and moody friction factors are the same
	// i just created an extra function in case people get confused.
}

public void fanningFrictionFactor(){

	// Step 1, initiate the friction factor object
	PipeFrictionFactor frictionFactorObj;
	frictionFactorObj = new PipeFrictionFactor();

	// Step 2, define Re and roughness ratio
	double Re = 25;
	double roughnessRatio = 0.0005;

	// Step 3, calculate friction factor

	double fanning = frictionFactorObj.fanning(Re,roughnessRatio);

}
```


Now it's very common for us when we calculate flow losses in pipes
that there are form losses in addition to the usual pipe losses.

As such, we can include the constant K to help us:

$$\Delta P_{loss} = \frac{1}{2} \rho u^2 (f_{darcy} \frac{L}{D}+K)$$
$$\Delta P_{loss} = \frac{1}{2} \rho u^2 (f_{fanning} \frac{4L}{D}+K)$$

The term in the brackets is what i refer to as fLDK factor.

You can obtain it using the code below:


```csharp
public void obtaining_fLDK(){

	// Step 1, initiate the friction factor object
	PipeFrictionFactor frictionFactorObj;
	frictionFactorObj = new PipeFrictionFactor();

	// Step 2, define Re and roughness ratio
	// as well as lengthToDiameter ratio
	// and form loss coefficent K
	double Re = 250;
	double roughnessRatio = 0.0005;
	double lengthToDiameter = 5.98;
	double formLossCoefficientK = 4.25;

	// Step 3, calculate fLDK term


	double fLDK = frictionFactorObj.fLDK(
			Re,
			roughnessRatio,
			lengthToDiameter,
			formLossCoefficientK);

}
```

#### dimensionless pressure loss calculations

Now, we want to calculate pressure losses using a given reynold's number,

So we use the Darcy Weisbach equation:


$$\Delta P_{loss} (Pa) = 0.5 \rho (kg/m^3) u^2 (m^2/s^2) 
(f_{darcy} \frac{L}{D} + K)$$

The problem with the above equation is units!

One way to get around the plethora of units we can use is to simply
work with nondimensional numbers.

I can nondimensionalise pressure loss as follows:

$$Be_{D} = \frac{\Delta P D_{H}^2}{\mu \nu}$$

This nondimensional number is the bejan number.

$\mu$ is dynamic viscosity, $\nu$ is kinematic viscosity.

$D_{H}$ is the hydraulic mean diameter.

I would then nondimensionalise velocity using the Reynold's number:

$$Re_{D} = \frac{\rho u D_{H}}{\mu}$$

Now of course, there are many expressions for us to calculate Re,
but how we get here is immaterial; what matters is that we can
nondimensionalise the Darcy Weisbach equation to a simpler form:

$$Be_{D} = 0.5 Re^2 (f_{darcy} \frac{L}{D} + K)$$

Thus, if we want to calculate nondimensionalised pressure loss,
we can do so:

Note: this function will not work with negative Reynold's numbers.

```csharp
public void obtain_dimensionlessPressureLoss_Be_given_Re(){


	// Step 1, initiate the PipeReAndBe object
	PipeReAndBe frictionFactorObj;
	frictionFactorObj = new PipeReAndBe();

	// Step 2, define Re and roughness ratio
	// as well as lengthToDiameter ratio
	// and form loss coefficent K
	// Reynold's number here is nondimensionalised
	// based on hydraulic diameter
	double Re = 250;
	double roughnessRatio = 0.0005;
	double lengthToDiameter = 5.98;
	double formLossCoefficientK = 4.25;

	// Step 3, calculate Be,
	// note that it will return values nondimensionalised based on
	// hydraulic diameter rather than pipe length
	// it's more convenient that way
	double bejanNumber_D = frictionFactorObj.getBe(
			Re,
			roughnessRatio,
			lengthToDiameter,
			formLossCoefficientK);

}
```

#### getting Re from nondimensionalised pressure loss

If we wanted to use the following equation:
$$Be_{D} = 0.5 Re^2 (f_{darcy} \frac{L}{D} + K)$$

And guess Re from Be, we would have to do so using numerical
methods. 

You can try it out in the following code simplifies that:

```csharp

public void obtain_Re_given_dimensionlessPressureLoss_Be(){


	// Step 1, initiate the PipeReAndBe object
	PipeReAndBe frictionFactorObj;
	frictionFactorObj = new PipeReAndBe();

	// Step 2, define Be_D and roughness ratio
	// as well as lengthToDiameter ratio
	// and form loss coefficent K
	// Bejan number here is nondimensionalised
	// based on diameter, not length
	double Be_D = 3500;
	double roughnessRatio = 0.0005;
	double lengthToDiameter = 5.98;
	double formLossCoefficientK = 4.25;

	// Step 3, calculate Be,
	// note that it will return values nondimensionalised based on
	// hydraulic diameter rather than pipe length
	// it's more convenient that way
	double Re = frictionFactorObj.getRe(
			Be_D,
			roughnessRatio,
			lengthToDiameter,
			formLossCoefficientK);

}

```
Note: this works with negative Bejan numbers also, since we can have
negative pressure loss causing reverse flow.

This would work okay assuming the pipe is hydraulically symmetrical,
meaning to say a 500 Pa pressure loss would produce the same magnitude
of flowrate as a -500 Pa pressure loss. The only difference is a change
in flow direction.


### Nondimensionalisation and Redimensionalisation

To help with getting pressure losses and velocity from their 
nondimensional forms, i have a few classes which deal with that.

You can get Re from velocity or mass flowrate and vice versa:
```csharp

public void getReFromMassFlowrate(){
	double velocityMeterPerSecond = 2.0;
	double densityKgPerMeterCubed = 1000;
	double hydraulicDiameterMeters = 0.1;
	double viscosityPascalSecond = 1.05e-3;

	double expectedRe = densityKgPerMeterCubed*
		velocityMeterPerSecond*
		hydraulicDiameterMeters/
		viscosityPascalSecond;

	double crossSectionalAreaMeterSq = 
		Math.PI/4.0 * Math.Pow(hydraulicDiameterMeters,2.0);
	double massFlowrateKgPerSecond = 
		crossSectionalAreaMeterSq*
		velocityMeterPerSecond*
		densityKgPerMeterCubed;

	
	Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
			LengthUnit.SI);

	DynamicViscosity fluidViscosity = new DynamicViscosity(
			viscosityPascalSecond, DynamicViscosityUnit.SI);

	Area crossSectionalArea = new Area(
			crossSectionalAreaMeterSq,
			AreaUnit.SI);

	MassFlow fluidMassFlowrate = new MassFlow(
			massFlowrateKgPerSecond,
			MassFlowUnit.SI);

	PipeReynoldsNumber testObj = new PipeReynoldsNumber();

	double resultRe = testObj.getRe(fluidMassFlowrate,
			crossSectionalArea,
			hydraulicDiameter,
			fluidViscosity);


}

public void GetMassFlowrateFromRe(){
	double velocityMeterPerSecond = 2.0;
	double densityKgPerMeterCubed = 1000;
	double hydraulicDiameterMeters = 0.1;
	double viscosityPascalSecond = 1.05e-3;

	double expectedRe = densityKgPerMeterCubed*
		velocityMeterPerSecond*
		hydraulicDiameterMeters/
		viscosityPascalSecond;

	double crossSectionalAreaMeterSq = 
		Math.PI/4.0 * Math.Pow(hydraulicDiameterMeters,2.0);
	double massFlowrateKgPerSecond = 
		crossSectionalAreaMeterSq*
		velocityMeterPerSecond*
		densityKgPerMeterCubed;

	
	Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
			LengthUnit.SI);

	DynamicViscosity fluidViscosity = new DynamicViscosity(
			viscosityPascalSecond, DynamicViscosityUnit.SI);

	Area crossSectionalArea = new Area(
			crossSectionalAreaMeterSq,
			AreaUnit.SI);

	MassFlow fluidMassFlowrate = new MassFlow(
			massFlowrateKgPerSecond,
			MassFlowUnit.SI);

	PipeReynoldsNumber testObj = new PipeReynoldsNumber();
	// Act
	MassFlow resultMassFlow = testObj.getMassFlowrate(crossSectionalArea,
			expectedRe,
			hydraulicDiameter,
			fluidViscosity);
}
```
You can also do the same with velocity.
The syntax is as follows:

```csharp

PipeReynoldsNumber testObj = new PipeReynoldsNumber();

double Re = testObj.getRe(Density fluidDensity,
		Speed averageVelocity,
		Length hydraulicDiameter,
		DynamicViscosity fluidViscosity);

Speed velocity = testObj.getAverageVelocity(Density fluidDensity,
		double Re,
		Length hydraulicDiameter,
		DynamicViscosity fluidViscosity);
```

The next thing is to obtain pressure loss from bejan number and vice versa

```csharp

public void getBejanNumberFromPressureLoss(){

	double densityKgPerMeterCubed = 2000;
	double viscosityPascalSecond = 0.005;
	double hydraulicDiameterMeters = 0.5;
	double dynamicPressurePascal = 1.05e-3;

	// Setup
	PipeBejanNumber dimensionlessTestObj;
	dimensionlessTestObj = new PipeBejanNumber();

	Density fluidDensity = new Density(densityKgPerMeterCubed,
			DensityUnit.SI);

	
	Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
			LengthUnit.SI);

	DynamicViscosity fluidViscosity = new DynamicViscosity(
			viscosityPascalSecond, DynamicViscosityUnit.SI);

	Pressure fluidPressure = new Pressure(
			dynamicPressurePascal, PressureUnit.SI);

	// calculate Be
	double resultBe = dimensionlessTestObj.getBe(
			fluidPressure,
			hydraulicDiameter,
			fluidDensity,
			fluidViscosity);
}

[Fact]
public void getPressureLossesFromBejanNumber(){

	double densityKgPerMeterCubed = 2000;
	double viscosityPascalSecond = 1.05e-3;
	double hydraulicDiameterMeters = 0.05;
	double BejanNumber = 5000;

	// Setup
	PipeBejanNumber dimensionlessTestObj;
	dimensionlessTestObj = new PipeBejanNumber();

	Density fluidDensity = new Density(densityKgPerMeterCubed,
			DensityUnit.SI);

	
	Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
			LengthUnit.SI);

	DynamicViscosity fluidViscosity = new DynamicViscosity(
			viscosityPascalSecond, DynamicViscosityUnit.SI);


	// Act
	Pressure resultFluidPressure = dimensionlessTestObj.
		getFluidPressure(BejanNumber,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);


}

```

You can also use kinematic viscosity and kinematic pressure. But I'm omitting
it from the quick start guide. Go to the source code to see how it is done.

### Pipe Loss and Mass Flowrate calculations

You can directly calculate the mass flowrate and pressure loss terms
for a pipe if you have the relevant parameters.

See the example below:

```csharp


[Fact]
public void ObtainMassFlowrateFromPressureLoss(){

	// Here's the tedious part; getting all the relevant information
	Pressure pressureLoss = new Pressure(
			50, PressureUnit.Pascal);

	Area crossSectionalArea = new Area(
			2.88e-4, AreaUnit.SI);

	Length hydraulicDiameter = crossSectionalArea.Sqrt()*2.0/
		Math.Pow(Math.PI,0.5);

	DynamicViscosity fluidViscosity = new
		DynamicViscosity(1.05e-3, DynamicViscosityUnit.PascalSecond);

	double roughnessRatio = 0.005;

	Length pipeLength = new Length(0.5, LengthUnit.SI);


	double formLossK = 5.55;

	Density fluidDensity = new Density(1000, 
			DensityUnit.SI);

	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;
	
	// Step 2: intitate the pipeLossObject

	PipePressureLossAndMassFlowrate pipeLossObject =
		new PipePressureLossAndMassFlowrate();

	// Step3: calculate mass flowrate

	MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureLoss,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					roughnessRatio,
					formLossK);

}


[Fact]
public void ObtainPressureLossFromMassFlow(){


	// Step 1: obtain pipe parameters:
	MassFlow fluidMassFlowrate = new MassFlow(
			0.15, MassFlowUnit.KilogramPerSecond);

	Area crossSectionalArea = new Area(
			2.88e-4, AreaUnit.SI);

	Length hydraulicDiameter = crossSectionalArea.Sqrt()*2.0/
		Math.Pow(Math.PI,0.5);

	DynamicViscosity fluidViscosity = new
		DynamicViscosity(1.05e-3, DynamicViscosityUnit.PascalSecond);

	double roughnessRatio = 0.005;

	Length pipeLength = new Length(0.5, LengthUnit.SI);


	double formLossK = 5.55;

	Density fluidDensity = new Density(1000, 
			DensityUnit.SI);

	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

	
	// Step 2: now let's initiate our pipeloss object

	PipePressureLossAndMassFlowrate pipeLossObject =
		new PipePressureLossAndMassFlowrate();

	// Step 3: calculate pressure loss

	Pressure resultPressureLoss = 
		pipeLossObject.getPressureLoss(fluidMassFlowrate,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					roughnessRatio,
					formLossK);




}

```

You also have the option of not including roughness ratio or form losses,
if you don't supply them, their default value is zero.

```csharp

[Fact]
public void ObtainMassFlowrateFromPressureLossNoFormLossSmoothPipe(){

	// you also have the option of NOT supplying any form losses
	// or roughness ratio
	// in that case, K = 0 and smooth pipe is assumed
	
	// Here's the tedious part; getting all the relevant information
	Pressure pressureLoss = new Pressure(
			50, PressureUnit.Pascal);

	Area crossSectionalArea = new Area(
			2.88e-4, AreaUnit.SI);

	Length hydraulicDiameter = crossSectionalArea.Sqrt()*2.0/
		Math.Pow(Math.PI,0.5);

	DynamicViscosity fluidViscosity = new
		DynamicViscosity(1.05e-3, DynamicViscosityUnit.PascalSecond);


	Length pipeLength = new Length(0.5, LengthUnit.SI);


	Density fluidDensity = new Density(1000, 
			DensityUnit.SI);

	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;
	
	// Step 2: intitate the pipeLossObject

	PipePressureLossAndMassFlowrate pipeLossObject =
		new PipePressureLossAndMassFlowrate();

	// Step3: calculate mass flowrate

	MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureLoss,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength);

	

}
