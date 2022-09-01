using Xunit;
using Xunit.Abstractions;
using System;
using sharpFluidMechanicsLibraries;
using EngineeringUnits;
using EngineeringUnits.Units;


namespace frictionFactorTests;
public partial class FrictionFactorTests : testOutputHelper
{
	[Fact]
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

	[Fact]
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

	[Fact]
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


	[Fact]
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


	[Fact]
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

	[Fact]
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

	[Fact]
	public void ConvertVelocityIntoReynoldsNumber(){
		// Setup
		double velocityMeterPerSecond = 2.0;
		double densityKgPerMeterCubed = 1000;
		double hydraulicDiameterMeters = 0.1;
		double viscosityPascalSecond = 1.05e-3;


		Speed fluidVelocity = new Speed(velocityMeterPerSecond,
				SpeedUnit.SI);

		Density fluidDensity = new Density(densityKgPerMeterCubed,
				DensityUnit.SI);
		
		Length hydraulicDiameter = new Length(hydraulicDiameterMeters,
				LengthUnit.SI);

		DynamicViscosity fluidViscosity = new DynamicViscosity(
				viscosityPascalSecond, DynamicViscosityUnit.SI);

		PipeReynoldsNumber testObj = new PipeReynoldsNumber();

		double resultRe = testObj.getRe(fluidDensity,
				fluidVelocity,
				hydraulicDiameter,
				fluidViscosity);


	}

	[Fact]
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

	[Fact]
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

	[Fact]
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
}
