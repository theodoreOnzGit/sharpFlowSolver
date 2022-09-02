using Xunit;
using Xunit.Abstractions;
using System;
using sharpFluidMechanicsLibraries;
using EngineeringUnits;
using EngineeringUnits.Units;


namespace frictionFactorTests;
public partial class PipeCalcTests : testOutputHelper
{
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
}
