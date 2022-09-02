using Xunit;
using Xunit.Abstractions;
using System;
using sharpFluidMechanicsLibraries;

using EngineeringUnits;
using EngineeringUnits.Units;


namespace frictionFactorTests;
public partial class PipeCalcTests : testOutputHelper
{
	// suppose now i have a tilted pipe, meaning to say
	// that that the pipe itself is angled upwards or downwards,
	// now hydrostatic pressure will play a role in the pressure loss
	// or gain and resulting flowrate
	//
	//
	// The following tests help with this using the tilted pipe class
	//
	// Tilted pipe test
	// (1/4) 
	// this test helps us calculate corect mass flow if i were to supply a
	// pressure change to a tilted pipe. 
	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(-90)]
	[InlineData(90)]
	[InlineData(270)]
	[InlineData(-270)]
	[InlineData(180)]
	public void WhenTiltedPipeCalcMassFlowExpectCorrectValue(
			double inclineAngleDegrees){

		// Setup
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
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();


		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		// in the case of a tilted pipe pressure_source = 0 (no pump etc.)
		//
		// pressure change = pressure_hydrostatic - pressure_loss
		//
		// pressure_loss = pressure_hydrostatic - pressure change
		//
		// note that hydrostatic pressure is rho g z
		//
		// z is the height change
		//
		//
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;

		Pressure pressureLoss = hydrostaticPressure - pressureChange;


		// and let's get the Be_D and L/D
		double Be_D = pipeBeObject.getBe(pressureLoss,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re

		double Re = nondimensionalPipeObj.getRe(Be_D,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);



		// once we get Re, we can get mass flow quite easily
		//

		MassFlow expectedMassFlow = pipeReObject.getMassFlowrate(
				crossSectionalArea,
				Re,
				hydraulicDiameter,
				fluidViscosity);


		
		// now let's initiate our pipeloss object
		IPipeMassFlowAndPressureLossTilted pipeLossObject =
			new PipeMassFlowAndPressureLossDefaultImplementation();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}

	// tilted pipe test
	// (2/4)
	// this test helps us calculate corect mass flow if i were to supply a
	// pressure change to a tilted pipe. 
	// in the reverse direction
	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(-90)]
	[InlineData(90)]
	[InlineData(270)]
	[InlineData(-270)]
	[InlineData(180)]
	public void WhenTiltedPipeCalcMassFlowReverseFlowExpectCorrectValue(
			double inclineAngleDegrees){

		// Setup
		Pressure pressureChange = new Pressure(
				-50, PressureUnit.Pascal);

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
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();


		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		// in the case of a tilted pipe pressure_source = 0 (no pump etc.)
		//
		// pressure change = pressure_hydrostatic - pressure_loss
		//
		// note that hydrostatic pressure is rho g z
		//
		// z is the height change
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;

		Pressure pressureLoss =  hydrostaticPressure - pressureChange;


		// and let's get the Be_D and L/D
		double Be_D = pipeBeObject.getBe(pressureLoss,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re

		double Re = nondimensionalPipeObj.getRe(Be_D,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);



		// once we get Re, we can get mass flow quite easily
		//

		MassFlow expectedMassFlow = pipeReObject.getMassFlowrate(
				crossSectionalArea,
				Re,
				hydraulicDiameter,
				fluidViscosity);


		
		// now let's initiate our pipeloss object
		IPipeMassFlowAndPressureLossTilted pipeLossObject =
			new PipeMassFlowAndPressureLossDefaultImplementation();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}
	// tilted pipe test
	// (3/4)
	// this test ensures that I get the correct pressure Change
	// from a pipe with some internal flow
	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(-90)]
	[InlineData(90)]
	[InlineData(270)]
	[InlineData(-270)]
	[InlineData(180)]
	public void WhenTiltedPipeCalcPressureChangeExpectCorrectValue(
			double inclineAngleDegrees){

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

		// let's first initiate the nondimensionalPipeObj
		// and also our objects to nondimensionalise fluid mass flowrate
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();
		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		// in the case of a tilted pipe pressure_source = 0 (no pump etc.)
		//
		// pressure change = pressure_hydrostatic - pressure_loss
		//
		// note that hydrostatic pressure is rho g z
		//
		// z is the height change
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;



		// and let's get the Re and L/D
		double Re = pipeReObject.getRe(fluidMassFlowrate,
				crossSectionalArea,
				hydraulicDiameter,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re

		double Be = nondimensionalPipeObj.getBe(
				Re,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);

		// once we get Be, we can get the pressure loss terms
		//
		Pressure expectedPressureLoss = pipeBeObject.getFluidPressure(
				Be,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);
		// now that we have obtained hydrostatic pressure change
		// and expected pressure Loss
		// we can calculate 
		// expected pressure change
		// pressure change = pressure_hydrostatic - pressure_loss

		Pressure expectedPressureChange = hydrostaticPressure -
			expectedPressureLoss;

		
		// now let's initiate our pipeloss object

		IPipeMassFlowAndPressureLossTilted pipeLossObject =
			new PipeMassFlowAndPressureLossDefaultImplementation();

		// Act

		Pressure resultPressureChange = 
			pipeLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedPressureChange.As(PressureUnit.SI),
				resultPressureChange.As(PressureUnit.SI));


	}
	// Tilted pipe test
	//(4/4) 
	//
	// i need to ensure that pressure change is working even with reverse
	// flow
	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(-90)]
	[InlineData(90)]
	[InlineData(270)]
	[InlineData(-270)]
	[InlineData(180)]
	public void WhenTiltedPipeCalcPressureChangeReverseFlowExpectCorrectValue(
			double inclineAngleDegrees){

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

		// let's first initiate the nondimensionalPipeObj
		// and also our objects to nondimensionalise fluid mass flowrate
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();
		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		// in the case of a tilted pipe pressure_source = 0 (no pump etc.)
		//
		// pressure change = pressure_hydrostatic - pressure_loss
		//
		// note that hydrostatic pressure is rho g z
		//
		// z is the height change
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;



		// and let's get the Re and L/D
		double Re = pipeReObject.getRe(fluidMassFlowrate,
				crossSectionalArea,
				hydraulicDiameter,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re
		// now for this test case, i need to take care of reverse flow manually
		bool reverseFlow = (Re < 0.0);
		if(reverseFlow)
			Re *= -1.0;

		double Be = nondimensionalPipeObj.getBe(
				Re,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);

		// once we get Be, we can get the pressure loss terms
		//
		Pressure expectedPressureLoss = pipeBeObject.getFluidPressure(
				Be,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);
		// now that we have obtained hydrostatic pressure change
		// and expected pressure Loss
		// we can calculate 
		// expected pressure change
		// pressure change = pressure_hydrostatic - pressure_loss
		if(reverseFlow)
			expectedPressureLoss *= -1.0;

		Pressure expectedPressureChange = hydrostaticPressure -
			expectedPressureLoss;


		
		// now let's initiate our pipeloss object

		IPipeMassFlowAndPressureLossTilted pipeLossObject =
			new PipeMassFlowAndPressureLossDefaultImplementation();

		// Act

		Pressure resultPressureChange = 
			pipeLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedPressureChange.As(PressureUnit.SI),
				resultPressureChange.As(PressureUnit.SI));


	}

	// wrapper test 
	// (1/2)
	// get pressure change given a pipe mass flowrate
	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(-90)]
	[InlineData(90)]
	[InlineData(270)]
	[InlineData(-270)]
	[InlineData(180)]
	public void WhenWrapperCalcPressureChangeReverseFlowExpectCorrectValue(
			double inclineAngleDegrees){

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

		// let's first initiate the nondimensionalPipeObj
		// and also our objects to nondimensionalise fluid mass flowrate
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();
		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		// in the case of a tilted pipe pressure_source = 0 (no pump etc.)
		//
		// pressure change = pressure_hydrostatic - pressure_loss
		//
		// note that hydrostatic pressure is rho g z
		//
		// z is the height change
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;



		// and let's get the Re and L/D
		double Re = pipeReObject.getRe(fluidMassFlowrate,
				crossSectionalArea,
				hydraulicDiameter,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re
		// now for this test case, i need to take care of reverse flow manually
		bool reverseFlow = (Re < 0.0);
		if(reverseFlow)
			Re *= -1.0;

		double Be = nondimensionalPipeObj.getBe(
				Re,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);

		// once we get Be, we can get the pressure loss terms
		//
		Pressure expectedPressureLoss = pipeBeObject.getFluidPressure(
				Be,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);
		// now that we have obtained hydrostatic pressure change
		// and expected pressure Loss
		// we can calculate 
		// expected pressure change
		// pressure change = pressure_hydrostatic - pressure_loss
		if(reverseFlow)
			expectedPressureLoss *= -1.0;

		Pressure expectedPressureChange = hydrostaticPressure -
			expectedPressureLoss;


		
		// now let's initiate our pipeloss object

		UniformTemperaturePipe pipeLossObject =
			new UniformTemperaturePipe();

		// Act

		Pressure resultPressureChange = 
			pipeLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedPressureChange.As(PressureUnit.SI),
				resultPressureChange.As(PressureUnit.SI));


	}
	// wrapper test
	// (2/2)
	//
	// this tests the wrapper to see if i can get
	// mass flow given a pressure change
	[Theory]
	[InlineData(10)]
	[InlineData(0)]
	[InlineData(-10)]
	[InlineData(-90)]
	[InlineData(90)]
	[InlineData(270)]
	[InlineData(-270)]
	[InlineData(180)]
	public void WhenWrapperCalcMassFlowExpectCorrectValue(
			double inclineAngleDegrees){

		// Setup
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
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();


		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		// in the case of a tilted pipe pressure_source = 0 (no pump etc.)
		//
		// pressure change = pressure_hydrostatic - pressure_loss
		//
		// pressure_loss = pressure_hydrostatic - pressure change
		//
		// note that hydrostatic pressure is rho g z
		//
		// z is the height change
		//
		//
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;

		Pressure pressureLoss = hydrostaticPressure - pressureChange;


		// and let's get the Be_D and L/D
		double Be_D = pipeBeObject.getBe(pressureLoss,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re

		double Re = nondimensionalPipeObj.getRe(Be_D,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);



		// once we get Re, we can get mass flow quite easily
		//

		MassFlow expectedMassFlow = pipeReObject.getMassFlowrate(
				crossSectionalArea,
				Re,
				hydraulicDiameter,
				fluidViscosity);


		
		// now let's initiate our pipeloss object
		UniformTemperaturePipe pipeLossObject =
			new UniformTemperaturePipe();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}



	/*************************************************************
	 * The following section tests the wrapper for calculating pipe
	 * pressure change and mass flowrates with the source
	 *
	 * I will only test the wrapper and not the underlying code
	 * since soooo many unit tests have been done on the underlying code
	 *
	 * I will test it for the method for obtaining the mass flowrate
	 * given an internal head loss or gain,
	 *
	 * this is because in the default implementation,
	 * pipe head loss or gain will recursively invoke
	 *
	 * ***********************************************************/

	// Wrapper tilted pipe with internal source (head gain)
	// (1/2)
	[Theory]
	[InlineData(10,0.0)]
	[InlineData(0, -1.0)]
	[InlineData(-10, -2.5)]
	[InlineData(-90, 1.0)]
	[InlineData(90, 3.5)]
	[InlineData(270, 10.9)]
	[InlineData(-270, -2)]
	[InlineData(180, -1)]
	public void WhenWrapperCalcMassFlowWithInternalSourceExpectCorrectValue(
			double inclineAngleDegrees,
			double internalPumpSourceHeadGainValue){

		// Setup
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
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();


		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		
		Angle inclineAngle = new Angle(inclineAngleDegrees, AngleUnit.Degree);
	
		Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

		// for gravity i'll use 9.81 m/s^2 as my constant.

		Acceleration earthAcceleration9_81 =
			new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

		Pressure hydrostaticPressure = fluidDensity*
			earthAcceleration9_81*
			heightChange;

		Length headGain = new Length(internalPumpSourceHeadGainValue,
				LengthUnit.Meter);

		Pressure internalPressureSource = 
			headGain *
			earthAcceleration9_81 *
			fluidDensity;


		Pressure pressureLoss = internalPressureSource +
			hydrostaticPressure - pressureChange;


		// and let's get the Be_D and L/D
		double Be_D = pipeBeObject.getBe(pressureLoss,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re

		double Re = nondimensionalPipeObj.getRe(Be_D,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);



		// once we get Re, we can get mass flow quite easily
		//

		MassFlow expectedMassFlow = pipeReObject.getMassFlowrate(
				crossSectionalArea,
				Re,
				hydraulicDiameter,
				fluidViscosity);


		
		// now let's initiate our pipeloss object
		UniformTemperaturePipe pipeLossObject =
			new UniformTemperaturePipe();

		// Act

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


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}

	// Wrapper tilted pipe with internal source (head gain)
	// (2/2)
	[Theory]
	[InlineData(10,0.0)]
	[InlineData(0, -1.0)]
	[InlineData(-10, -2.5)]
	[InlineData(-90, 1.0)]
	[InlineData(90, 3.5)]
	[InlineData(270, 10.9)]
	[InlineData(-270, -2)]
	[InlineData(180, -1)]
	public void WhenWrapperCalcPressureChgWithInternalSourceExpectCorrectValue(
			double inclineAngleDegrees,
			double internalPumpSourceHeadGainValue){


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

		// let's first initiate the nondimensionalPipeObj
		// and also our objects to nondimensionalise fluid mass flowrate
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();
		// now we need to calculate hydrostatic pressure, 
		// note this equation:
		// pressure change = pressure_source + pressure_hydrostatic - pressure_loss
		//
		
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

		Length headGain = new Length(internalPumpSourceHeadGainValue,
				LengthUnit.Meter);

		Pressure internalPressureSource = 
			headGain *
			earthAcceleration9_81 *
			fluidDensity;



		// and let's get the Re and L/D
		double Re = pipeReObject.getRe(fluidMassFlowrate,
				crossSectionalArea,
				hydraulicDiameter,
				fluidViscosity);

		double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

		// then let's obtain the pipe Bejan Number
		// given the Re
		// now for this test case, i need to take care of reverse flow manually
		bool reverseFlow = (Re < 0.0);
		if(reverseFlow)
			Re *= -1.0;

		double Be = nondimensionalPipeObj.getBe(
				Re,
				roughnessRatio,
				lengthToDiameterRatio,
				formLossK);

		// once we get Be, we can get the pressure loss terms
		//
		Pressure expectedPressureLoss = pipeBeObject.getFluidPressure(
				Be,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);
		// now that we have obtained hydrostatic pressure change
		// and expected pressure Loss
		// we can calculate 
		// expected pressure change
		// pressure change = internalSource + 
		// pressure_hydrostatic - pressure_loss
		//
		if(reverseFlow)
			expectedPressureLoss *= -1.0;

		Pressure expectedPressureChange = internalPressureSource +
			hydrostaticPressure -
			expectedPressureLoss;


		
		// now let's initiate our pipeloss object

		UniformTemperaturePipe pipeLossObject =
			new UniformTemperaturePipe();

		// Act

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


		// Assert
		//
		Assert.Equal(expectedPressureChange.As(PressureUnit.SI),
				resultPressureChange.As(PressureUnit.SI));


	}
}
