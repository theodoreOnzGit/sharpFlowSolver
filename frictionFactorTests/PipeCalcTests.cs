using Xunit;
using Xunit.Abstractions;
using System;
using sharpFluidMechanicsLibraries;

using EngineeringUnits;
using EngineeringUnits.Units;


namespace frictionFactorTests;
public partial class PipeCalcTests : testOutputHelper
{
	public PipeCalcTests(ITestOutputHelper outputHelper):base(outputHelper){

		// this constructor is just here to load the test output helper
		// which is just an object which helps me print code
		// when i run
		//' dotnet watch test --logger "console;verbosity=detailed"

		// now i'll also create dependencies in the constructor
		// 
	}

	/*************************************************************
	 * These tests deal with dimensionalising Re and back
	 *
	 *
	 * *************************************************************/

	[Fact]
	public void WhenGetReExpectCorrectRe(){
		// Setup
		double velocityMeterPerSecond = 2.0;
		double densityKgPerMeterCubed = 1000;
		double hydraulicDiameterMeters = 0.1;
		double viscosityPascalSecond = 1.05e-3;

		double expectedRe = densityKgPerMeterCubed*
			velocityMeterPerSecond*
			hydraulicDiameterMeters/
			viscosityPascalSecond;

		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);
		
		Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
				LengthUnit.SI);

		DynamicViscosity fluidViscosity = new DynamicViscosity(
				viscosityPascalSecond, DynamicViscosityUnit.SI);

		PipeReynoldsNumber testObj = new PipeReynoldsNumber();
		// Act
		double resultRe = testObj.getRe(fluidDensity,
				fluidVelocity,
				hydraulicDiameter,
				fluidViscosity);


		// Assert

		double error = Math.Abs(
				expectedRe - resultRe)/
			Math.Abs(expectedRe);
		double errorMax = 0.001;


		if(error < errorMax){
			Assert.Equal(expectedRe,resultRe, 5);
			return;
		}


		throw new Exception("getRe test failed");
	}

	

	[Fact]
	public void WhenGetVelocityExpectCorrectVelocity(){
		// Setup
		double velocityMeterPerSecond = 2.0;
		double densityKgPerMeterCubed = 1000;
		double hydraulicDiameterMeters = 0.1;
		double viscosityPascalSecond = 1.05e-3;

		double expectedRe = densityKgPerMeterCubed*
			velocityMeterPerSecond*
			hydraulicDiameterMeters/
			viscosityPascalSecond;

		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);
		
		Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
				LengthUnit.SI);

		DynamicViscosity fluidViscosity = new DynamicViscosity(
				viscosityPascalSecond, DynamicViscosityUnit.SI);

		PipeReynoldsNumber testObj = new PipeReynoldsNumber();


		double expectedVelocity = velocityMeterPerSecond;
		// Act
		Speed resultVelocity = testObj.getAverageVelocity(fluidDensity,
				expectedRe,
				hydraulicDiameter,
				fluidViscosity);

		double resultVelocityMetersPerSecond = 
			resultVelocity.As(SpeedUnit.SI);


		// Assert

		double error = Math.Abs(
				expectedVelocity - resultVelocityMetersPerSecond)/
			Math.Abs(expectedVelocity);
		double errorMax = 0.001;


		if(error < errorMax){
			Assert.Equal(expectedVelocity,
					resultVelocityMetersPerSecond, 5);
			return;
		}


		throw new Exception("getVelocity test failed");
	}

	[Fact]
	public void WhenGetReExpectCorrectRe_MassFlow(){
		// Setup
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

		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);
		
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
		double resultRe = testObj.getRe(fluidMassFlowrate,
				crossSectionalArea,
				hydraulicDiameter,
				fluidViscosity);


		// Assert

		double error = Math.Abs(
				expectedRe - resultRe)/
			Math.Abs(expectedRe);
		double errorMax = 0.001;


		if(error < errorMax){
			Assert.Equal(expectedRe,resultRe, 5);
			return;
		}


		throw new Exception("getRe test failed");
	}

	[Fact]
	public void WhenGetMassFlowExpectCorrectMassFlow_Re(){
		// Setup
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

		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);
		
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

		double resultMassFlowKgPerSecond = 
			resultMassFlow.As(MassFlowUnit.SI);


		// Assert

		double error = Math.Abs(
				fluidMassFlowrate.As(MassFlowUnit.SI) 
				- resultMassFlowKgPerSecond)/
			Math.Abs(fluidMassFlowrate.As(MassFlowUnit.SI));
		double errorMax = 0.001;


		if(error < errorMax){
			Assert.Equal(fluidMassFlowrate.As(MassFlowUnit.SI)
					,resultMassFlowKgPerSecond, 5);
			return;
		}


		throw new Exception("getRe test failed");
	}

	// the following tests for exception handling for 
	// getting Re from Mass flow and vice versa
	[Theory]
	[InlineData(0,10,0.1,1.05e-3)]
	[InlineData(-2,10,0,1.05e-3)]
	[InlineData(2,0,0.1,1.05e-3)]
	[InlineData(2,-1,0,1.05e-3)]
	[InlineData(2,10,0,1.05e-3)]
	[InlineData(2,11,-0,1.05e-3)]
	public void WhenGetReFromMassFlowUndesirableValueExpectException(
		double crossSectionalAreaMeterSq,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double massFlowrateKgPerSecond){


		// Setup
		PipeReynoldsNumber dimensionlessTestObj;
		dimensionlessTestObj = new PipeReynoldsNumber();


		
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


		// Act
		//
		// Assert

		if(crossSectionalArea.As(AreaUnit.SI) <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getRe(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(hydraulicDiameter.As(LengthUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getRe(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getRe(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}

		throw new Exception("exception not caught");
	}

	[Theory]
	[InlineData(0,10,0.1,1.05e-3)]
	[InlineData(-2,10,0,1.05e-3)]
	[InlineData(2,0,0.1,1.05e-3)]
	[InlineData(2,-1,0,1.05e-3)]
	[InlineData(2,10,0,1.05e-3)]
	[InlineData(2,11,-0,1.05e-3)]
	public void WhenGetMassFlowFromReUndesirableValueExpectException(
		double crossSectionalAreaMeterSq,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double massFlowrateKgPerSecond){


		// Setup
		PipeReynoldsNumber dimensionlessTestObj;
		dimensionlessTestObj = new PipeReynoldsNumber();


		
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

		double Re = fluidMassFlowrate/crossSectionalArea*
			hydraulicDiameter/
			fluidViscosity;


		// Act
		//
		// Assert

		if(crossSectionalArea.As(AreaUnit.SI) <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getMassFlowrate(crossSectionalArea,
						Re,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(hydraulicDiameter.As(LengthUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getMassFlowrate(crossSectionalArea,
						Re,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getMassFlowrate(crossSectionalArea,
						Re,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}

		throw new Exception("exception not caught");
	}

	// the following tests check for exceptions for the
	// getting Re from Velocity and vice versa
	[Theory]
	[InlineData(0,10,0.1,1.05e-3)]
	[InlineData(-2,10,0,1.05e-3)]
	[InlineData(2,0,0.1,1.05e-3)]
	[InlineData(2,-1,0,1.05e-3)]
	[InlineData(2,10,0,1.05e-3)]
	[InlineData(2,11,-0,1.05e-3)]
	public void WhenGetReFromVeloityUndesirableValueExpectException(
		double densityKgPerMeterCubed,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double velocityMeterPerSecond){


		// Setup
		PipeReynoldsNumber dimensionlessTestObj;
		dimensionlessTestObj = new PipeReynoldsNumber();

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);

		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);
		
		Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
				LengthUnit.SI);

		DynamicViscosity fluidViscosity = new DynamicViscosity(
				viscosityPascalSecond, DynamicViscosityUnit.SI);


		// Act
		//
		// Assert

		if(fluidDensity.As(DensityUnit.SI) <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getRe(fluidDensity,
						fluidVelocity,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(hydraulicDiameter.As(LengthUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getRe(fluidDensity,
						fluidVelocity,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getRe(fluidDensity,
						fluidVelocity,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}

		throw new Exception("exception not caught");
	}

	[Theory]
	[InlineData(0,10,0.1,1.05e-3)]
	[InlineData(-2,10,0,1.05e-3)]
	[InlineData(2,0,0.1,1.05e-3)]
	[InlineData(2,-1,0,1.05e-3)]
	[InlineData(2,10,0,1.05e-3)]
	[InlineData(2,11,-0,1.05e-3)]
	public void WhenGetVelocityFromReUndesirableValueExpectException(
		double densityKgPerMeterCubed,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double velocityMeterPerSecond){


		// Setup
		PipeReynoldsNumber dimensionlessTestObj;
		dimensionlessTestObj = new PipeReynoldsNumber();

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);

		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);
		
		Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
				LengthUnit.SI);

		DynamicViscosity fluidViscosity = new DynamicViscosity(
				viscosityPascalSecond, DynamicViscosityUnit.SI);

		double Re = fluidDensity*
			fluidVelocity*
			hydraulicDiameter/
			fluidViscosity;

		// Act
		//
		// Assert

		if(fluidDensity.As(DensityUnit.SI) <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getAverageVelocity(fluidDensity,
						Re,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(hydraulicDiameter.As(LengthUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getAverageVelocity(fluidDensity,
						Re,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}
		if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getAverageVelocity(fluidDensity,
						Re,
						hydraulicDiameter,
						fluidViscosity)
					);
			return;
		}

		throw new Exception("exception not caught");
	}


	// the following section tests for Bejan number nondimensionalisation
	// into dynamic pressure and back
	// using both kinematic and dynamic viscosity


	// the following section tests for exceptions in bejan number
	//

	[Theory]
	[InlineData(0,10,0.1,1.05e3)]
	[InlineData(-2,10,0,1.05e3)]
	[InlineData(2,0,0.1,1.05e3)]
	[InlineData(2,-1,0,1.05e3)]
	[InlineData(2,10,0,1.05e3)]
	[InlineData(2,11,-0,1.05e3)]
	public void WhenGetBeFromPressureDropUndesirableValueExpectException(
		double densityKgPerMeterCubed,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double dynamicPressurePascal){


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

		double Be = fluidPressure*
			hydraulicDiameter.Pow(2)*
			fluidDensity/
			fluidViscosity.Pow(2);

		// Act
		//
		// Assert

		if(fluidDensity.As(DensityUnit.SI) <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getBe(fluidPressure,
						hydraulicDiameter,
						fluidDensity,
						fluidViscosity)
					);
			return;
		}
		if(hydraulicDiameter.As(LengthUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getBe(fluidPressure,
						hydraulicDiameter,
						fluidDensity,
						fluidViscosity)
					);
			return;
		}
		if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getBe(fluidPressure,
						hydraulicDiameter,
						fluidDensity,
						fluidViscosity)
					);
			return;
		}

		throw new Exception("exception not caught");
	}


	[Theory]
	[InlineData(0,10,0.1,1.05e3)]
	[InlineData(-2,10,0,1.05e3)]
	[InlineData(2,0,0.1,1.05e3)]
	[InlineData(2,-1,0,1.05e3)]
	[InlineData(2,10,0,1.05e3)]
	[InlineData(2,11,-0,1.05e3)]
	public void WhenGetPressureDropFromBeUndesirableValueExpectException(
		double densityKgPerMeterCubed,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double dynamicPressurePascal){


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

		double Be = fluidPressure*
			hydraulicDiameter.Pow(2)*
			fluidDensity/
			fluidViscosity.Pow(2);

		// Act
		//
		// Assert

		if(fluidDensity.As(DensityUnit.SI) <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getFluidPressure(Be,
						hydraulicDiameter,
						fluidDensity,
						fluidViscosity)
					);
			return;
		}
		if(hydraulicDiameter.As(LengthUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getFluidPressure(Be,
						hydraulicDiameter,
						fluidDensity,
						fluidViscosity)
					);
			return;
		}
		if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					dimensionlessTestObj.
					getFluidPressure(Be,
						hydraulicDiameter,
						fluidDensity,
						fluidViscosity)
					);
			return;
		}

		throw new Exception("exception not caught");
	}


	// the following section tests if getting Be from pressure drop
	// and vice versa is working correctly
	[Theory]
	[InlineData(2,10,0.1,1.05e3)]
	[InlineData(2,10,2.1,1.05e3)]
	public void WhenGetBeFromPressureDropExpectCorrectValue(
		double densityKgPerMeterCubed,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double dynamicPressurePascal){


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

		double expectedBe = fluidPressure*
			hydraulicDiameter.Pow(2)*
			fluidDensity/
			fluidViscosity.Pow(2);

		// Act
		double resultBe = dimensionlessTestObj.getBe(
				fluidPressure,
				hydraulicDiameter,
				fluidDensity,
				fluidViscosity);
		// Assert
		Assert.Equal(expectedBe,resultBe);
	}

	[Theory]
	[InlineData(2,10,0.1,1.05e3)]
	[InlineData(2,10,2.1,-0.05e3)]
	public void WhenGetPressureDropFromBeExpectCorrectValue(
		double densityKgPerMeterCubed,
		double viscosityPascalSecond,
		double hydraulicDiameterMeters,
		double dynamicPressurePascal){


		// Setup
		PipeBejanNumber dimensionlessTestObj;
		dimensionlessTestObj = new PipeBejanNumber();

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);

		
		Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
				LengthUnit.SI);

		DynamicViscosity fluidViscosity = new DynamicViscosity(
				viscosityPascalSecond, DynamicViscosityUnit.SI);

		Pressure expectedFluidPressure = new Pressure(
				dynamicPressurePascal, PressureUnit.SI);

		double expectedBe = expectedFluidPressure*
			hydraulicDiameter.Pow(2)*
			fluidDensity/
			fluidViscosity.Pow(2);

		// Act
		Pressure resultFluidPressure = dimensionlessTestObj.
			getFluidPressure(expectedBe,
					hydraulicDiameter,
					fluidDensity,
					fluidViscosity);


		// Assert
		Assert.Equal(expectedFluidPressure.As(PressureUnit.SI)
				,resultFluidPressure.As(PressureUnit.SI));
	}

	// these tests check the PipeMassFlowAndPressureLoss 
	// as well as the wrapper
	// to see if they yield the correct flowrate.
	// I'm assuming the wrapper objects for PipeBeAndRe,
	// PipeReynoldsNumber
	// PipeBejanNumber are all OK
	// only doing ONE test here
	[Fact]
	public void WhenPipeCalcPressureLossExpectCorrectValue(){

		// Setup
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

		
		// now let's initiate our pipeloss object

		IPipeMassFlowAndPressureLoss pipeLossObject =
			new PipeMassFlowAndPressureLossDefaultImplementation();

		// Act

		Pressure resultPressureLoss = 
			pipeLossObject.getPressureLoss(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedPressureLoss.As(PressureUnit.SI),
				resultPressureLoss.As(PressureUnit.SI));


	}

	[Fact]
	public void WhenPipeCalcPressureLossWrapperExpectCorrectValue(){

		// Setup
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

		
		// now let's initiate our pipeloss object

		PipePressureLossAndMassFlowrate pipeLossObject =
			new PipePressureLossAndMassFlowrate();

		// Act

		Pressure resultPressureLoss = 
			pipeLossObject.getPressureLoss(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedPressureLoss.As(PressureUnit.SI),
				resultPressureLoss.As(PressureUnit.SI));


	}

	// now we want to calculate mass flowrate from a given pressure
	//
	[Fact]
	public void WhenPipeCalcMassFlowExpectCorrectValue(){

		// Setup
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

		// let's first initiate the nondimensionalPipeObj
		// and also our objects to nondimensionalise fluid mass flowrate
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();


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

		IPipeMassFlowAndPressureLoss pipeLossObject =
			new PipeMassFlowAndPressureLossDefaultImplementation();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureLoss,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}

	[Fact]
	public void WhenPipeCalcMassFlowWrapperExpectCorrectValue(){

		// Setup
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

		// let's first initiate the nondimensionalPipeObj
		// and also our objects to nondimensionalise fluid mass flowrate
		PipeReAndBe nondimensionalPipeObj = 
			new PipeReAndBe();

		PipeReynoldsNumber pipeReObject = 
			new PipeReynoldsNumber();

		PipeBejanNumber pipeBeObject = 
			new PipeBejanNumber();


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

		PipePressureLossAndMassFlowrate pipeLossObject =
			new PipePressureLossAndMassFlowrate();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureLoss,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio,
						formLossK);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}

	// basically i have this for convenience,
	// if i don't supply K or roughness ratios,
	// a smooth pipe is assumed and form loss is zero
	[Fact]
	public void WhenPipeCalcMassFlowWrapperNoFormLossSmoothPipeExpectCorrectValue(){

		// Setup
		Pressure pressureLoss = new Pressure(
				50, PressureUnit.Pascal);

		Area crossSectionalArea = new Area(
				2.88e-4, AreaUnit.SI);

		Length hydraulicDiameter = crossSectionalArea.Sqrt()*2.0/
			Math.Pow(Math.PI,0.5);

		DynamicViscosity fluidViscosity = new
			DynamicViscosity(1.05e-3, DynamicViscosityUnit.PascalSecond);

		double roughnessRatio = 0;

		Length pipeLength = new Length(0.5, LengthUnit.SI);


		double formLossK = 0;

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

		PipePressureLossAndMassFlowrate pipeLossObject =
			new PipePressureLossAndMassFlowrate();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureLoss,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}
	// basically i have this for convenience,
	// if i don't supply K 
	// form loss is zero
	[Fact]
	public void WhenPipeCalcMassFlowWrapperNoFormLossExpectCorrectValue(){

		// Setup
		Pressure pressureLoss = new Pressure(
				50, PressureUnit.Pascal);

		Area crossSectionalArea = new Area(
				2.88e-4, AreaUnit.SI);

		Length hydraulicDiameter = crossSectionalArea.Sqrt()*2.0/
			Math.Pow(Math.PI,0.5);

		DynamicViscosity fluidViscosity = new
			DynamicViscosity(1.05e-3, DynamicViscosityUnit.PascalSecond);

		double roughnessRatio = 0.11;

		Length pipeLength = new Length(0.5, LengthUnit.SI);


		double formLossK = 0;

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

		PipePressureLossAndMassFlowrate pipeLossObject =
			new PipePressureLossAndMassFlowrate();

		// Act

		MassFlow resultMassFlow = pipeLossObject.getMassFlow(pressureLoss,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio);


		// Assert
		//
		Assert.Equal(expectedMassFlow.As(MassFlowUnit.SI),
				resultMassFlow.As(MassFlowUnit.SI));


	}
}
