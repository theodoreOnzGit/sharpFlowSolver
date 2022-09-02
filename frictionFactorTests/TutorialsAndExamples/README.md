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

### Dealing with Pipes with Tilt and Internal Source

Now pipes don't usually exist in a vacuum, 
sometimes they are attached to pumps, and sometimes they are inclined
at an angle so that you have to take hydrostatic pressure into account.

Normally the Darcy Weisbach equation, when nondimensionalised
will help you calculate the pressure loss:

$$Be_{D} = 0.5 Re_{D}^2 (f_{Darcy} \frac{L}{D} +K )$$

$$ \Delta P_{loss} = 0.5 \rho u^2 (f_{Darcy} \frac{L}{D} +K )$$

Often times also, it's more convenient to work with mass flowrate
as it reduces the confusion of which velocity should i take.
At the same time, flowmeters often show you mass or volumetric flowrate.

Therefore, I have designed this code to work with mass flowrate rather
than velocity

$$ \Delta P_{loss} = 0.5 \frac{\dot{m}}{\rho A_{XS}^2} (f_{Darcy} \frac{L}{D} +K )$$

In the basic pipe equations, we deal with pressure loss from a given 
Reynold's number, mass flowrate or velocity.

However, when hydrostatic pressure is taken into account, we may need
to change the way we consider pressure.

A good way is to consider absolute pressure change between start and
end of a pipe section.

$$\Delta P_{change} = -\Delta P_{loss} + \Delta P_{hydrostatic}$$

In the case we have a pressure source within the pipe segment, eg.
a pump or some other source, we can consider this:

$$\Delta P_{change} = -\Delta P_{loss} + \Delta P_{hydrostatic} 
+ \Delta P_{source}$$

This will be the underlying equation for which i design this code.

Any time i refer to pressure change within the code, the above equation
will be used.

Here are some examples on how to use the code:

```csharp
// suppose you had a pipe and somehow it was attached to a pump
// it could be quite convenient to represent that pump and pipe 
// as some sort of single component.
//
// Also, in real life, pipes can move fluid against or with the 
// direction of gravity
//
// as such you do need to take hydrostatic pressure into account
//
// these examples show you how to do just that.
// example (1/6)
[Fact]
public void ObtainMassFlowFromPressureChangePipeWithTiltAndInternalSource(){

	// As usual, step 1 is the setup phase,
	//
	// Step 1:
	//
	// get all the pipe parameters ready

	Pressure pressureChange = new Pressure(
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

	// let's first initiate the nondimensionalPipeObj
	// and also our objects to nondimensionalise fluid mass flowrate


	// now we need to calculate hydrostatic pressure, 
	// note this equation:
	// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
	//
	
	double inclineAngleDegrees = 20.0;
	Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);

	Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
					AngleUnit.Radian));

	// for gravity i'll use 9.81 m/s^2 as my constant.

	Acceleration earthAcceleration9_81 =
		new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

	Pressure hydrostaticPressure = fluidDensity*
		earthAcceleration9_81*
		heightChange;

	double internalPumpSourceHeadGainValue = -1;
	Length headGain = new Length(internalPumpSourceHeadGainValue,
			LengthUnit.Meter);



	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;






	// Step 2: 
	//
	// now let's initiate our pipeloss object
	UniformTemperaturePipe pipeLossObject =
		new UniformTemperaturePipe();

	// Step 3:
	//
	// obtain mass flow given a pressure change across this pipe

	MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureChange,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					headGain,
					roughnessRatio,
					formLossK);




}

// example 
// (2/6)
//
// While it's common to represent pump head in meters,
// you might be inclined to use kinematic pressure as well
// i have included the option of using kinematic pressure 
// to represent pump head if you so desire
// 
[Fact]
public void ObtainMassFlowrateForPipeWithTiltAndInternalSourceKinematicPressure(){


	// As usual, step 1 is the setup phase,
	//
	// Step 1:
	//
	// get all the pipe parameters ready

	Pressure pressureChange = new Pressure(
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

	// let's first initiate the nondimensionalPipeObj
	// and also our objects to nondimensionalise fluid mass flowrate


	// now we need to calculate hydrostatic pressure, 
	// note this equation:
	// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
	//
	
	double inclineAngleDegrees = 20.0;
	Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);

	Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
					AngleUnit.Radian));

	// for gravity i'll use 9.81 m/s^2 as my constant.

	Acceleration earthAcceleration9_81 =
		new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

	Pressure hydrostaticPressure = fluidDensity*
		earthAcceleration9_81*
		heightChange;

	double internalPumpSourceHeadGainValue = -1;
	Length headGain = new Length(internalPumpSourceHeadGainValue,
			LengthUnit.Meter);

	SpecificEnergy internalPumpSourceKinematicPressure = 
		headGain * earthAcceleration9_81;



	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;






	// Step 2: 
	//
	// now let's initiate our pipeloss object
	UniformTemperaturePipe pipeLossObject =
		new UniformTemperaturePipe();

	// Step 3:
	//
	// obtain mass flow given a pressure change across this pipe

	MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureChange,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					internalPumpSourceKinematicPressure,
					roughnessRatio,
					formLossK);

}

// Example
// (3/6)
//
// And for those of you who love to use dynamic pressure, 
// here's yet another example:

[Fact]
public void ObtainMassFlowrateForPipeWithTiltAndInternalSourceDynPressure(){


	// As usual, step 1 is the setup phase,
	//
	// Step 1:
	//
	// get all the pipe parameters ready

	Pressure pressureChange = new Pressure(
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

	// let's first initiate the nondimensionalPipeObj
	// and also our objects to nondimensionalise fluid mass flowrate


	// now we need to calculate hydrostatic pressure, 
	// note this equation:
	// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
	//
	
	double inclineAngleDegrees = 20.0;
	Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);

	Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
					AngleUnit.Radian));

	// for gravity i'll use 9.81 m/s^2 as my constant.

	Acceleration earthAcceleration9_81 =
		new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

	Pressure hydrostaticPressure = fluidDensity*
		earthAcceleration9_81*
		heightChange;

	double internalPumpSourceHeadGainValue = -1;
	Length headGain = new Length(internalPumpSourceHeadGainValue,
			LengthUnit.Meter);

	SpecificEnergy internalPumpSourceKinematicPressure = 
		headGain * earthAcceleration9_81;

	// Here's where i calculate dynamic pressure for use!
	//

	Pressure internalPumpSourceDynamicPressure = 
		internalPumpSourceKinematicPressure*fluidDensity;


	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;






	// Step 2: 
	//
	// now let's initiate our pipeloss object
	UniformTemperaturePipe pipeLossObject =
		new UniformTemperaturePipe();

	// Step 3:
	//
	// obtain mass flow given a pressure change across this pipe

	MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureChange,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					internalPumpSourceDynamicPressure,
					roughnessRatio,
					formLossK);

}

// Now let's say you wanted to start calculating pressure change
// given a mass flowrate,
// here's how you would do that
// if you have an internal pump head
//
// Example
// (4/6)
[Fact]
public void ObtainPressureChangeFromMassFlowrateForPipeWithTiltAndInternalSource(){


	MassFlow fluidMassFlowrate = new MassFlow(
			-0.15, MassFlowUnit.KilogramPerSecond);

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

	// now we need to calculate hydrostatic pressure, 
	// note this equation:
	// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
	//
	
	double inclineAngleDegrees = -10.0;
	Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);

	Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
					AngleUnit.Radian));

	// for gravity i'll use 9.81 m/s^2 as my constant.

	Acceleration earthAcceleration9_81 =
		new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

	Pressure hydrostaticPressure = fluidDensity*
		earthAcceleration9_81*
		heightChange;

	// and now for the pressure source term

	double internalPumpSourceHeadGainValue = 2;
	Length headGain = new Length(internalPumpSourceHeadGainValue,
			LengthUnit.Meter);





	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

	// then let's obtain the pipe Bejan Number
	// given the Re
	// now for this test case, i need to take care of reverse flow manually



	
	// Step 2:
	// now let's initiate our pipeloss object

	UniformTemperaturePipe pipeLossObject =
		new UniformTemperaturePipe();

	// Step 3:
	//
	// calculate pressure change

	Pressure resultPressureChange = 
		pipeLossObject.getPressureChange(fluidMassFlowrate,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					headGain,
					roughnessRatio,
					formLossK);




}

// Example
// (5/6)
//
// As before, i also made provisions in case you wanted to use
// kinematic pressure
[Fact]
public void ObtainPressureChangePipeWithTiltAndInternalSourceKinPressure(){


	MassFlow fluidMassFlowrate = new MassFlow(
			-0.15, MassFlowUnit.KilogramPerSecond);

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

	// now we need to calculate hydrostatic pressure, 
	// note this equation:
	// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
	//
	
	double inclineAngleDegrees = -10.0;
	Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);

	Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
					AngleUnit.Radian));

	// for gravity i'll use 9.81 m/s^2 as my constant.

	Acceleration earthAcceleration9_81 =
		new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

	Pressure hydrostaticPressure = fluidDensity*
		earthAcceleration9_81*
		heightChange;

	// and now for the pressure source term

	double internalPumpSourceHeadGainValue = 2;
	Length headGain = new Length(internalPumpSourceHeadGainValue,
			LengthUnit.Meter);

	SpecificEnergy internalKinematicPressureSource = 
		headGain *
		earthAcceleration9_81 ;




	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

	// then let's obtain the pipe Bejan Number
	// given the Re
	// now for this test case, i need to take care of reverse flow manually



	
	// Step 2:
	// now let's initiate our pipeloss object

	UniformTemperaturePipe pipeLossObject =
		new UniformTemperaturePipe();

	// Step 3:
	//
	// calculate pressure change

	Pressure resultPressureChange = 
		pipeLossObject.getPressureChange(fluidMassFlowrate,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					internalKinematicPressureSource,
					roughnessRatio,
					formLossK);




}

// Example
// (6/6)
//
// Last but not least, an example with dynamic pressure:
[Fact]
public void ObtainPressureChangePipeWithTiltAndInternalSourceDynPressure(){


	MassFlow fluidMassFlowrate = new MassFlow(
			-0.15, MassFlowUnit.KilogramPerSecond);

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

	// now we need to calculate hydrostatic pressure, 
	// note this equation:
	// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
	//
	
	double inclineAngleDegrees = -10.0;
	Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);

	Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
					AngleUnit.Radian));

	// for gravity i'll use 9.81 m/s^2 as my constant.

	Acceleration earthAcceleration9_81 =
		new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

	Pressure hydrostaticPressure = fluidDensity*
		earthAcceleration9_81*
		heightChange;

	// and now for the pressure source term

	double internalPumpSourceHeadGainValue = 2;
	Length headGain = new Length(internalPumpSourceHeadGainValue,
			LengthUnit.Meter);

	Pressure internalPressureSource = 
		headGain *
		earthAcceleration9_81 *
		fluidDensity;




	double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

	// then let's obtain the pipe Bejan Number
	// given the Re
	// now for this test case, i need to take care of reverse flow manually



	
	// Step 2:
	// now let's initiate our pipeloss object

	UniformTemperaturePipe pipeLossObject =
		new UniformTemperaturePipe();

	// Step 3:
	//
	// calculate pressure change

	Pressure resultPressureChange = 
		pipeLossObject.getPressureChange(fluidMassFlowrate,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					internalPressureSource,
					roughnessRatio,
					formLossK);




}
```
