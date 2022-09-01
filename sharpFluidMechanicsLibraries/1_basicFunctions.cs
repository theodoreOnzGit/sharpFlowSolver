using System;
using EngineeringUnits;
using EngineeringUnits.Units;

namespace sharpFluidMechanicsLibraries{
	// the end user only need look at the following class to see what
	// functionality is available in this code.
	public class PipeFrictionFactor
	{

		public double darcy(double Re, double roughnessRatio){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.darcy(Re,roughnessRatio);
		}

		public double moody(double Re, double roughnessRatio){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.moody(Re,roughnessRatio);
		}

		public double fanning(double Re, double roughnessRatio){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.fanning(Re,roughnessRatio);
		}

		public double fLDK(double Re, double roughnessRatio,
				double lengthToDiameterRatio,
				double K){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.fLDK(Re,roughnessRatio,
					lengthToDiameterRatio, 
					K);
		}

	}

	public class PipeReAndBe{

		public double getBe(double Re, double roughnessRatio,
				double lengthToDiameterRatio,
				double K){
			IPipeReAndBe frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.getBe(Re,roughnessRatio,
					lengthToDiameterRatio, 
					K);
		}

		public double getRe(double Be_D,
				double roughnessRatio,
				double lengthToDiameterRatio,
				double K){
			IPipeReAndBe frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.getRe(Be_D,
					roughnessRatio,
					lengthToDiameterRatio, 
					K);
		}
	}

	public class PipePressureLossAndMassFlowrate{

		public MassFlow getMassFlow(Pressure pressureLoss,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLoss pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getMassFlow(pressureLoss,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio,
						formLossK);
		}

		public Pressure getPressureLoss(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLoss pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getPressureLoss(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						roughnessRatio,
						formLossK);
		}
	}


}
