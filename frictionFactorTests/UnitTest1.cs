using Xunit;
using Xunit.Abstractions;
using System;
using sharpFluidMechanicsLibraries;


namespace frictionFactorTests;
public class FrictionFactorTests : testOutputHelper
{
	public FrictionFactorTests(ITestOutputHelper outputHelper):base(outputHelper){

		// this constructor is just here to load the test output helper
		// which is just an object which helps me print code
		// when i run
		//' dotnet watch test --logger "console;verbosity=detailed"

		// now i'll also create dependencies in the constructor
		// 
	}



	





	// this test will test the churchill correlation over some
	// values using an online colebrook calculator
	// https://www.engineeringtoolbox.com/colebrook-equation-d_1031.html
	// https://www.ajdesigner.com/php_colebrook/colebrook_equation.php#ajscroll
	// the online calculators return a darcy friction factor
	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224)]
	[InlineData(40000, 0.05, 0.072124054027755)]
	[InlineData(4e5, 0.05, 0.071608351787938)]
	[InlineData(4e6, 0.05,  0.071556444535705)]
	[InlineData(4e7, 0.05,  0.071551250389636)]
	[InlineData(4e8, 0.05, 0.071550730940769)]
	[InlineData(4e9, 0.05, 0.071550678995539)]
	[InlineData(4e3, 0.0, 0.039907014055631)]
	[InlineData(4e7, 0.00005, 0.010627694187016)]
	[InlineData(4e6, 0.001, 0.019714092419925)]
	[InlineData(4e5, 0.01, 0.038055838413508)]
	[InlineData(4e4, 0.03,  0.057933060738478)]
	public void Test_churchillFrictionFactorShouldBeAccurate_Turbulent(double Re,double roughnessRatio, double referenceFrictionFactor){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double referenceDarcyFactor = referenceFrictionFactor;

		// also the above values are visually inspected with respect to the graph
		IFrictionFactor frictionFactorObj;
		frictionFactorObj = new ChurchillFrictionFactor();

		// Act

		double resultDarcyFactor =  frictionFactorObj.darcy(Re,roughnessRatio);
		
		// Assert
		// Now by default, i can assert to a fixed number of decimal places
		// so comparing 99.98 and 99.99 are about the same to two decimal places
		// However, repeat this tactic with smaller numbers,eg
		// 0.00998 and 0.00999
		// this tactic will fail
		// to normalise everything I will use a normalise decimal place
		// I can take the logarithm base 10 of this number, round up
		// because the log10 of a number will give about the number of decimal 
		// places i need to correct for


		int normaliseDecimalPlace(double reference){

			double normaliseDouble = Math.Log10(reference);
			normaliseDouble = Math.Ceiling(normaliseDouble);
			int normaliseInteger;

			normaliseInteger = (int)normaliseDouble;
			// at this stage, i will get the number of decimal places i need to subtract
			// i want to add the correct number of decimal places,
			// so i will just use a negative sign
			normaliseInteger = -normaliseInteger;

			return normaliseInteger;
		}

		int decimalPlaceTest = 1 + normaliseDecimalPlace(referenceDarcyFactor);


		Assert.Equal(referenceDarcyFactor,resultDarcyFactor,decimalPlaceTest);
	}


	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224)]
	[InlineData(40000, 0.05, 0.072124054027755)]
	[InlineData(4e5, 0.05, 0.071608351787938)]
	[InlineData(4e6, 0.05,  0.071556444535705)]
	[InlineData(4e7, 0.05,  0.071551250389636)]
	[InlineData(4e8, 0.05, 0.071550730940769)]
	[InlineData(4e9, 0.05, 0.071550678995539)]
	[InlineData(4e3, 0.0, 0.039907014055631)]
	[InlineData(4e7, 0.00005, 0.010627694187016)]
	[InlineData(4e6, 0.001, 0.019714092419925)]
	[InlineData(4e5, 0.01, 0.038055838413508)]
	[InlineData(4e4, 0.03,  0.057933060738478)]
	public void Test_churchillFrictionFactorErrorNotMoreThan2Percent_Turbulent(double Re,double roughnessRatio, double referenceFrictionFactor){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double referenceDarcyFactor = referenceFrictionFactor;

		// also the above values are visually inspected with respect to the graph
		IFrictionFactor frictionFactorObj;
		frictionFactorObj = new ChurchillFrictionFactor();

		double errorMax = 0.02;
		// Act

		double resultDarcyFactor =  frictionFactorObj.darcy(Re,roughnessRatio);
		

		double error = Math.Abs(referenceDarcyFactor - resultDarcyFactor)/referenceDarcyFactor;

		// Assert
		//

		Assert.True(error < errorMax);




	}

	[Theory]
	[InlineData(100, 0.05)]
	[InlineData(200, 0.05)]
	[InlineData(300, 0.05)]
	[InlineData(400, 0.05)]
	[InlineData(400, 0.0)]
	[InlineData(500, 0.05)]
	[InlineData(600, 0.05)]
	[InlineData(800, 0.05)]
	[InlineData(1000, 0.05)]
	[InlineData(1200, 0.05)]
	[InlineData(1400, 0.05)]
	[InlineData(1600, 0.05)]
	[InlineData(1800, 0.05)]
	[InlineData(2000, 0.05)]
	public void Test_churchillFrictionFactorErrorNotMoreThan2Percent_Laminar(double Re,double roughnessRatio){
		// this tests the churchill relation against the 
		// laminar flow friction factor
		// fanning is 16/Re
		// and no matter the roughness ratio, I should get the same result
		// however, roughness ratio should not exceed 0.1
		// as maximum roughness ratio in charts is about 0.05
		//
		// Setup

		// this test asserts that the error should not be more than 2%

		double referenceFanning = 16/Re;

		IFrictionFactor frictionFactorObj;
		frictionFactorObj = new ChurchillFrictionFactor();

		double errorMax = 0.02;

		// Act

		double resultFanning = frictionFactorObj.fanning(Re,roughnessRatio);

		// Assert
		//
		// I want to use a 10 percent difference rather than absolute value
		// Assert.Equal(referenceFanning,resultFanning,4);

		double error;
		error = Math.Abs(resultFanning - referenceFanning)/referenceFanning;
		
		Assert.True(error < errorMax);
		// I have asserted that the churchill friction factor correlation is accurate to 
		// 10% up to Re=2200 with the laminar flow correlation,
		// this is good
	}

	[Theory]
	[InlineData(100, 0.05)]
	[InlineData(200, 0.05)]
	[InlineData(300, 0.05)]
	[InlineData(400, 0.05)]
	[InlineData(400, 0.0)]
	[InlineData(500, 0.05)]
	[InlineData(600, 0.05)]
	[InlineData(800, 0.05)]
	[InlineData(1000, 0.05)]
	[InlineData(1200, 0.05)]
	[InlineData(1400, 0.05)]
	[InlineData(1600, 0.05)]
	[InlineData(1800, 0.05)]
	[InlineData(2000, 0.05)]
	[InlineData(2200, 0.05)]
	public void Test_churchillFrictionFactorErrorNotMoreThan4Percent_Laminar(double Re,double roughnessRatio){
		// this tests the churchill relation against the 
		// laminar flow friction factor
		// fanning is 16/Re
		// and no matter the roughness ratio, I should get the same result
		// however, roughness ratio should not exceed 0.1
		// as maximum roughness ratio in charts is about 0.05
		//
		// Setup

		// this test asserts that the error should not be more than 2%

		double referenceFanning = 16/Re;

		IFrictionFactor frictionFactorObj;
		frictionFactorObj = new ChurchillFrictionFactor();

		double errorMax = 0.04;

		// Act

		double resultFanning = frictionFactorObj.fanning(Re,roughnessRatio);

		// Assert
		//
		// I want to use a 10 percent difference rather than absolute value
		// Assert.Equal(referenceFanning,resultFanning,4);

		double error;
		error = Math.Abs(resultFanning - referenceFanning)/referenceFanning;
		
		Assert.True(error < errorMax);
		// I have asserted that the churchill friction factor correlation is accurate to 
		// 10% up to Re=2200 with the laminar flow correlation,
		// this is good
	}


	[Theory]
	[InlineData(100, 0.05)]
	[InlineData(200, 0.05)]
	[InlineData(300, 0.05)]
	[InlineData(400, 0.05)]
	[InlineData(400, 0.0)]
	[InlineData(500, 0.05)]
	[InlineData(600, 0.05)]
	[InlineData(800, 0.05)]
	[InlineData(1000, 0.05)]
	[InlineData(1200, 0.05)]
	[InlineData(1400, 0.05)]
	[InlineData(1600, 0.05)]
	[InlineData(1800, 0.05)]
	[InlineData(2000, 0.05)]
	[InlineData(2200, 0.05)]
	public void Test_churchillFrictionFactorShouldBeAccurate_Laminar(double Re,double roughnessRatio){
		// this tests the churchill relation against the 
		// laminar flow friction factor
		// fanning is 16/Re
		// and no matter the roughness ratio, I should get the same result
		// however, roughness ratio should not exceed 0.1
		// as maximum roughness ratio in charts is about 0.05
		//
		// Setup

		double referenceFrictionFactor = 16/Re;

		IFrictionFactor frictionFactorObj;
		frictionFactorObj = new ChurchillFrictionFactor();

		// Act

		double resultFrictionFactor = frictionFactorObj.fanning(Re,roughnessRatio);

		// Assert
		//
		// Assert.Equal(referenceFrictionFactor,resultFrictionFactor,4);

		double resultErrorFraction;
		resultErrorFraction = Math.Abs(resultFrictionFactor - referenceFrictionFactor)/referenceFrictionFactor;
		
		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.035);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
		}
		// I have asserted that the churchill friction factor correlation is accurate to 
		// 3.5% up to Re=2200 with the laminar flow correlation,
		// this is good
		// the anomalous data point is at Re=2200
		throw new Exception("result unsatisfactory");
	}

	[Theory]
	[InlineData(100, 0.05)]
	[InlineData(200, 0.05)]
	[InlineData(300, 0.05)]
	[InlineData(400, 0.05)]
	[InlineData(400, 0.0)]
	[InlineData(500, 0.05)]
	[InlineData(600, 0.05)]
	[InlineData(800, 0.05)]
	[InlineData(1000, 0.05)]
	[InlineData(1200, 0.05)]
	[InlineData(1400, 0.05)]
	[InlineData(1600, 0.05)]
	[InlineData(1800, 0.05)]
	[InlineData(2000, 0.05)]
	[InlineData(2200, 0.05)]
	public void WhenUserWrapperFanningShouldBeAccurate_Laminar(
			double Re,double roughnessRatio){
		// this tests the churchill relation against the 
		// laminar flow friction factor
		// fanning is 16/Re
		// and no matter the roughness ratio, I should get the same result
		// however, roughness ratio should not exceed 0.1
		// as maximum roughness ratio in charts is about 0.05
		//
		// Setup

		double referenceFrictionFactor = 16/Re;

		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultFrictionFactor = frictionFactorObj.fanning(Re,roughnessRatio);

		// Assert
		//
		// I want to use a 10 percent difference rather than absolute value
		// Assert.Equal(referenceFrictionFactor,resultFrictionFactor,4);

		double resultErrorFraction;
		resultErrorFraction = Math.Abs(resultFrictionFactor - referenceFrictionFactor)/referenceFrictionFactor;
		
		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.035);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
		}
		// I have asserted that the churchill friction factor correlation is accurate to 
		// 3.5% up to Re=2200 with the laminar flow correlation,
		// this is good
		// the anomalous data point is at Re=2200
		throw new Exception("result unsatisfactory");
	}

	[Theory]
	[InlineData(100, 0.05)]
	[InlineData(200, 0.05)]
	[InlineData(300, 0.05)]
	[InlineData(400, 0.05)]
	[InlineData(400, 0.0)]
	[InlineData(500, 0.05)]
	[InlineData(600, 0.05)]
	[InlineData(800, 0.05)]
	[InlineData(1000, 0.05)]
	[InlineData(1200, 0.05)]
	[InlineData(1400, 0.05)]
	[InlineData(1600, 0.05)]
	[InlineData(1800, 0.05)]
	[InlineData(2000, 0.05)]
	[InlineData(2200, 0.05)]
	public void WhenUserWrapperDarcyShouldBeAccurate_Laminar(
			double Re,double roughnessRatio){
		// this tests the churchill relation against the 
		// laminar flow friction factor
		// fanning is 16/Re
		// and no matter the roughness ratio, I should get the same result
		// however, roughness ratio should not exceed 0.1
		// as maximum roughness ratio in charts is about 0.05
		//
		// Setup

		double referenceFrictionFactor = 64.0/Re;

		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultFrictionFactor = frictionFactorObj.darcy(Re,roughnessRatio);

		// Assert
		//
		// I want to use a 10 percent difference rather than absolute value
		// Assert.Equal(referenceFrictionFactor,resultFrictionFactor,4);

		double resultErrorFraction;
		resultErrorFraction = Math.Abs(resultFrictionFactor - referenceFrictionFactor)/referenceFrictionFactor;
		
		Assert.Equal(0.0, resultErrorFraction,1);
		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.035);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
		}
		// 3.5% up to Re=2200 with the laminar flow correlation,
		// this is good
		// the anomalous data point is at Re=2200
		throw new Exception("result unsatisfactory");
	}
	[Theory]
	[InlineData(100, 0.05)]
	[InlineData(200, 0.05)]
	[InlineData(300, 0.05)]
	[InlineData(400, 0.05)]
	[InlineData(400, 0.0)]
	[InlineData(500, 0.05)]
	[InlineData(600, 0.05)]
	[InlineData(800, 0.05)]
	[InlineData(1000, 0.05)]
	[InlineData(1200, 0.05)]
	[InlineData(1400, 0.05)]
	[InlineData(1600, 0.05)]
	[InlineData(1800, 0.05)]
	[InlineData(2000, 0.05)]
	[InlineData(2200, 0.05)]
	public void WhenUserWrapperMoodyShouldBeAccurate_Laminar(
			double Re,double roughnessRatio){
		// this tests the churchill relation against the 
		// laminar flow friction factor
		// fanning is 16/Re
		// and no matter the roughness ratio, I should get the same result
		// however, roughness ratio should not exceed 0.1
		// as maximum roughness ratio in charts is about 0.05
		//
		// Setup

		double referenceFrictionFactor = 16/Re;

		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultFrictionFactor = frictionFactorObj.fanning(Re,roughnessRatio);

		// Assert
		//
		// I want to use a 10 percent difference rather than absolute value
		// Assert.Equal(referenceFrictionFactor,resultFrictionFactor,4);

		double resultErrorFraction;
		resultErrorFraction = Math.Abs(
				resultFrictionFactor - referenceFrictionFactor)/
			referenceFrictionFactor;
		
		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.035);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
		}
		// I have asserted that the churchill friction factor correlation is accurate to 
		// 3.5% up to Re=2200 with the laminar flow correlation,
		// this is good
		// the anomalous data point is at Re=2200
		throw new Exception("result unsatisfactory");
	}

	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224)]
	[InlineData(40000, 0.05, 0.072124054027755)]
	[InlineData(4e5, 0.05, 0.071608351787938)]
	[InlineData(4e6, 0.05,  0.071556444535705)]
	[InlineData(4e7, 0.05,  0.071551250389636)]
	[InlineData(4e8, 0.05, 0.071550730940769)]
	[InlineData(4e9, 0.05, 0.071550678995539)]
	[InlineData(4e3, 0.0, 0.039907014055631)]
	[InlineData(4e7, 0.00005, 0.010627694187016)]
	[InlineData(4e6, 0.001, 0.019714092419925)]
	[InlineData(4e5, 0.01, 0.038055838413508)]
	[InlineData(4e4, 0.03,  0.057933060738478)]
	public void WhenDarcyWrapperTestedTurbulentErrorLessThan2Percent(
			double Re,double roughnessRatio, double referenceFrictionFactor){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double referenceDarcyFactor = referenceFrictionFactor;

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultDarcyFactor =  frictionFactorObj.darcy(Re,roughnessRatio);
		

		double resultErrorFraction = Math.Abs(
				referenceDarcyFactor - resultDarcyFactor)/referenceDarcyFactor;

		// Assert
		//

		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.02);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
			// 4 cases fall under this category
		}
		throw new Exception("result unsatisfactory");

	}

	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224)]
	[InlineData(40000, 0.05, 0.072124054027755)]
	[InlineData(4e5, 0.05, 0.071608351787938)]
	[InlineData(4e6, 0.05,  0.071556444535705)]
	[InlineData(4e7, 0.05,  0.071551250389636)]
	[InlineData(4e8, 0.05, 0.071550730940769)]
	[InlineData(4e9, 0.05, 0.071550678995539)]
	[InlineData(4e3, 0.0, 0.039907014055631)]
	[InlineData(4e7, 0.00005, 0.010627694187016)]
	[InlineData(4e6, 0.001, 0.019714092419925)]
	[InlineData(4e5, 0.01, 0.038055838413508)]
	[InlineData(4e4, 0.03,  0.057933060738478)]
	public void WhenMoodyWrapperTestedTurbulentErrorLessThan2Percent(
			double Re,double roughnessRatio, double referenceFrictionFactor){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double referenceMoodyFactor = referenceFrictionFactor;

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultMoodyFactor =  frictionFactorObj.moody(Re,roughnessRatio);
		

		double resultErrorFraction = Math.Abs(
				referenceMoodyFactor - resultMoodyFactor)/referenceMoodyFactor;

		// Assert
		//

		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.02);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
			// 4 cases fall under this category
		}
		throw new Exception("result unsatisfactory");

	}

	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224)]
	[InlineData(40000, 0.05, 0.072124054027755)]
	[InlineData(4e5, 0.05, 0.071608351787938)]
	[InlineData(4e6, 0.05,  0.071556444535705)]
	[InlineData(4e7, 0.05,  0.071551250389636)]
	[InlineData(4e8, 0.05, 0.071550730940769)]
	[InlineData(4e9, 0.05, 0.071550678995539)]
	[InlineData(4e3, 0.0, 0.039907014055631)]
	[InlineData(4e7, 0.00005, 0.010627694187016)]
	[InlineData(4e6, 0.001, 0.019714092419925)]
	[InlineData(4e5, 0.01, 0.038055838413508)]
	[InlineData(4e4, 0.03,  0.057933060738478)]
	public void WhenFanningWrapperTestedTurbulentErrorLessThan2Percent(
			double Re,double roughnessRatio, double referenceFrictionFactor){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double referenceFanningFactor = referenceFrictionFactor/4.0;

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultFanningFactor =  frictionFactorObj.fanning(Re,roughnessRatio);
		

		double resultErrorFraction = Math.Abs(
				referenceFanningFactor - resultFanningFactor)/
			referenceFanningFactor;

		// Assert
		//

		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		// if error is more than 1%, check the maximum error
		bool resultSomewhatSatisfactory = (resultErrorFraction < 0.02);
		if(resultSomewhatSatisfactory){
			Assert.True(resultSomewhatSatisfactory);
			return;
			// 4 cases fall under this category
		}
		throw new Exception("result unsatisfactory");

	}

	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224, 5,1)]
	[InlineData(40000, 0.05, 0.072124054027755,6,3)]
	[InlineData(4e5, 0.05, 0.071608351787938, 7, 2)]
	[InlineData(4e6, 0.05,  0.071556444535705, 4.5, 1.5)]
	[InlineData(4e7, 0.05,  0.071551250389636, 8.8,9)]
	[InlineData(4e8, 0.05, 0.071550730940769, 10, 70)]
	[InlineData(4e9, 0.05, 0.071550678995539, 100, 1000)]
	[InlineData(4e3, 0.0, 0.039907014055631, 77 , 84)]
	[InlineData(4e7, 0.00005, 0.010627694187016, 123, 123)]
	[InlineData(4e6, 0.001, 0.019714092419925, 0.5, 0.5)]
	[InlineData(4e5, 0.01, 0.038055838413508, 0.8, 83)]
	[InlineData(4e4, 0.03,  0.057933060738478, 1.4, 7.5)]
	public void WhenfLDKWrapperExpectCorrectValue(
			double Re,double roughnessRatio, 
			double referenceFrictionFactor,
			double lengthToDiameter,
			double formLossK){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double reference_fLDK = referenceFrictionFactor*
			lengthToDiameter + formLossK;

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		double resultfLDKFactor =  frictionFactorObj.
			fLDK(Re,roughnessRatio,
					lengthToDiameter,
					formLossK);
		

		double resultErrorFraction = Math.Abs(
				reference_fLDK - resultfLDKFactor)/
			reference_fLDK;

		// Assert
		//

		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		Assert.Equal(reference_fLDK,resultfLDKFactor);
		throw new Exception("result unsatisfactory");
	}

	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224, 5,1)]
	[InlineData(40000, 0.05, 0.072124054027755,6,3)]
	[InlineData(4e5, 0.05, 0.071608351787938, 7, 2)]
	[InlineData(4e6, 0.05,  0.071556444535705, 4.5, 1.5)]
	[InlineData(4e7, 0.05,  0.071551250389636, 8.8,9)]
	[InlineData(4e8, 0.05, 0.071550730940769, 10, 70)]
	[InlineData(4e9, 0.05, 0.071550678995539, 100, 1000)]
	[InlineData(4e3, 0.0, 0.039907014055631, 77 , 84)]
	[InlineData(4e7, 0.00005, 0.010627694187016, 123, 123)]
	[InlineData(4e6, 0.001, 0.019714092419925, 0.5, 0.5)]
	[InlineData(4e5, 0.01, 0.038055838413508, 0.8, 83)]
	[InlineData(4e4, 0.03,  0.057933060738478, 1.4, 7.5)]
	public void WhenBeWrapperExpectCorrectValue(
			double Re,double roughnessRatio, 
			double referenceFrictionFactor,
			double lengthToDiameter,
			double formLossK){
		// i'm making the variable explicit so the user can see
		// it's darcy friction factor, no ambiguity here

		// Setup
		double reference_fLDK = referenceFrictionFactor*
			lengthToDiameter + formLossK;
		double referenceBe = 
			reference_fLDK*0.5*
			Math.Pow(Re,2.0);

		// also the above values are visually inspected with respect to the graph
		PipeReAndBe frictionFactorObj;
		frictionFactorObj = new PipeReAndBe();

		// Act

		double resultBe =  frictionFactorObj.
			getBe(Re,roughnessRatio,
					lengthToDiameter,
					formLossK);
		

		double resultErrorFraction = Math.Abs(
				referenceBe - resultBe)/
			referenceBe;

		// Assert
		//

		// I want to check if the error is less than 1%
		bool resultSatisfactory = (resultErrorFraction < 0.01);
		if(resultSatisfactory){
			Assert.True(resultSatisfactory);
			return;
		}
		Assert.Equal(referenceBe,resultBe);
		throw new Exception("result unsatisfactory");
	}

	[Theory]
	[InlineData(0.0,0.1)]
	[InlineData(-1800.0,0.1)]
	[InlineData(1800.0,-0.1)]
	public void WhenWrapperDarcyUndesirableValueExpectException(
			double Re,
			double roughnessRatio){

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		if(Re == 0.0){
			Assert.Throws<DivideByZeroException>(
					() => 
					frictionFactorObj.
					darcy(Re,roughnessRatio)
					);
			return;
		}
		if(Re < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					darcy(Re,roughnessRatio)
					);
			return;
		}
		if(roughnessRatio < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					darcy(Re,roughnessRatio)
					);
			return;
		}

		throw new Exception("exception not caught");
	}
	

	[Theory]
	[InlineData(0.0,0.1)]
	[InlineData(-1800.0,0.1)]
	[InlineData(1800.0,-0.1)]
	public void WhenWrapperMoodyUndesirableValueExpectException(
			double Re,
			double roughnessRatio){

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		if(Re == 0.0){
			Assert.Throws<DivideByZeroException>(
					() => 
					frictionFactorObj.
					moody(Re,roughnessRatio)
					);
			return;
		}
		if(Re < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					moody(Re,roughnessRatio)
					);
			return;
		}
		if(roughnessRatio < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					moody(Re,roughnessRatio)
					);
			return;
		}

		throw new Exception("exception not caught");
	}

	[Theory]
	[InlineData(0.0,0.1)]
	[InlineData(-1800.0,0.1)]
	[InlineData(1800.0,-0.1)]
	public void WhenWrapperFanningUndesirableValueExpectException(
			double Re,
			double roughnessRatio){

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		if(Re == 0.0){
			Assert.Throws<DivideByZeroException>(
					() => 
					frictionFactorObj.
					fanning(Re,roughnessRatio)
					);
			return;
		}
		if(Re < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					fanning(Re,roughnessRatio)
					);
			return;
		}
		if(roughnessRatio < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					fanning(Re,roughnessRatio)
					);
			return;
		}

		throw new Exception("exception not caught");
	}


	[Theory]
	[InlineData(0.0,0.1,10,10)]
	[InlineData(-1800.0,0.1,10,10)]
	[InlineData(1800.0,-0.1,10,10)]
	[InlineData(1800.0,0.1,-10,10)]
	[InlineData(1800.0,0.1,10,-10)]
	[InlineData(1800.0,0.1,0,-10)]
	public void WhenWrapperfLDKUndesirableValueExpectException(
			double Re,
			double roughnessRatio,
			double lengthToDiameter,
			double formLossCoefficientK){

		// also the above values are visually inspected with respect to the graph
		PipeFrictionFactor frictionFactorObj;
		frictionFactorObj = new PipeFrictionFactor();

		// Act

		if(Re == 0.0){
			Assert.Throws<DivideByZeroException>(
					() => 
					frictionFactorObj.
					fLDK(Re,roughnessRatio,
						lengthToDiameter,
						formLossCoefficientK)
					);
			return;
		}
		if(Re < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					fLDK(Re,roughnessRatio,
						lengthToDiameter,
						formLossCoefficientK)
					);
			return;
		}
		if(roughnessRatio < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					fLDK(Re,roughnessRatio,
						lengthToDiameter,
						formLossCoefficientK)
					);
			return;
		}
		if(formLossCoefficientK < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					fLDK(Re,roughnessRatio,
						lengthToDiameter,
						formLossCoefficientK)
					);
			return;
		}
		if(lengthToDiameter <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					fLDK(Re,roughnessRatio,
						lengthToDiameter,
						formLossCoefficientK)
					);
			return;
		}

		throw new Exception("exception not caught");
	}


	// this part of the tests deal with getRe functions
	//
	[Theory]
	[InlineData(4000, 0.05, 0.076986834889224, 4.0)]
	[InlineData(40000, 0.05, 0.07212405402775,5.0)]
	[InlineData(4e5, 0.05, 0.071608351787938, 10.0)]
	[InlineData(4e6, 0.05,  0.071556444535705, 20.0)]
	[InlineData(4e7, 0.05,  0.071551250389636, 100.0)]
	[InlineData(4e8, 0.05, 0.071550730940769, 1000.0)]
	[InlineData(4e9, 0.05, 0.071550678995539, 65.0)]
	[InlineData(4e3, 0.0, 0.039907014055631, 20.0 )]
	[InlineData(4e7, 0.00005, 0.010627694187016, 35.0)]
	[InlineData(4e6, 0.001, 0.019714092419925, 8.9)]
	[InlineData(4e5, 0.01, 0.038055838413508, 50.0)]
	[InlineData(4e4, 0.03,  0.057933060738478, 1.0e5)]
	public void Test_churchillFrictionFactorShouldGetAccurateReTurbulent(
			double Re,
			double roughnessRatio, 
			double referenceDarcyFrictionFactor,
			double lengthToDiameter){
		// the objective of this test is to test the
		// accuracy of getting Re using the getRe function
		//
		// we have a reference Reynold's number
		//
		// and we need to get a Re using
		// fanning friction factor
		// and roughness Ratio
		//
		// we already have roughness ratio
		// but we need Bejan number and L/D
		//
		// Bejan number would be known in real life.
		// however, in this case, we cannot arbitrarily
		// specify it
		// the only equation that works now
		// is Be = f*Re^2*(4L/D)^3/32.0
		// That means we just specify a L/D ratio
		// and that would specify everything.
		// So I'm going to randomly specify L/D ratios and hope that
		// works
		

		// setup
		//
		double referenceRe = Re;

		IPipeReAndBe testObject;
		testObject = new ChurchillFrictionFactor();


		double fanningFrictionFactor = 0.25*referenceDarcyFrictionFactor;
		double Be_L = fanningFrictionFactor*Math.Pow(Re,2.0);
		Be_L *= Math.Pow(4.0*lengthToDiameter,3);
		Be_L *= 1.0/32.0;

		// act

		double resultRe;
		resultRe = testObject.getRe(
				Be_L,
				roughnessRatio,
				lengthToDiameter);

		// Assert (manual test)

		// Assert.Equal(referenceRe, resultRe);

		// Assert (auto test)
		// test if error is within 1% of actual Re
		double errorFraction = Math.Abs(resultRe - referenceRe)/Math.Abs(referenceRe);
		double errorTolerance = 0.01;

		Assert.True(errorFraction < errorTolerance);


	}

	[Theory]
	[InlineData(0.0,0.1,-10)]
	public void WhengetReNoFormLossUndesirableValueExpectException(
			double Re,
			double roughnessRatio,
			double lengthToDiameter){

		// Setup
		// also the above values are visually inspected with respect to the graph
		IPipeReAndBe frictionFactorObj;
		frictionFactorObj = new ChurchillFrictionFactor();

		// let's calculate Be manually first:
		double Be_D = frictionFactorObj.
			getBe(Re,roughnessRatio,lengthToDiameter,
					0.0);
		double Be_L = Be_D*
			Math.Pow(lengthToDiameter, 2.0);


		// Assert

		if(roughnessRatio < 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					getRe(Be_L,
						roughnessRatio,
						lengthToDiameter)
					);
			return;
		}
		if(lengthToDiameter <= 0.0){
			Assert.Throws<ArgumentOutOfRangeException>(
					() => 
					frictionFactorObj.
					getRe(Be_L,
						roughnessRatio,
						lengthToDiameter)
					);
			return;
		}

		throw new Exception("exception not caught");
	}
}
