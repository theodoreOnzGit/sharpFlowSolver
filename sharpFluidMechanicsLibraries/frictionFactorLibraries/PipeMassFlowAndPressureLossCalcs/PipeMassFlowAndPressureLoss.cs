// Here is a class for Fanning Friction Factor using Churchill's Correlation
//

using System;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using System.Collections.Generic;
using EngineeringUnits;
using EngineeringUnits.Units;


namespace sharpFluidMechanicsLibraries{

	public partial class PipeMassFlowAndPressureLossDefaultImplementation :
		IPipeMassFlowAndPressureLoss
	{


		/*************************************************
		 * the follwing methods implement calculate pressure loss
		 * from mass flowrate
		 *
		 *
		 * ***********************************************/

		public Pressure getPressureLoss(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				double roughnessRatio,
				double formLossK){
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
			Pressure pressureLoss = pipeBeObject.getFluidPressure(
					Be,
					hydraulicDiameter,
					fluidDensity,
					fluidViscosity);

			return pressureLoss;
		}


		public MassFlow getMassFlow(Pressure pressureLoss,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				double roughnessRatio,
				double formLossK){
			// let's first initiate the nondimensionalPipeObj
			// and also our objects to nondimensionalise fluid mass flowrate
			PipeReAndBe nondimensionalPipeObj = 
				new PipeReAndBe();

			PipeReynoldsNumber pipeReObject = 
				new PipeReynoldsNumber();

			PipeBejanNumber pipeBeObject = 
				new PipeBejanNumber();

			// and let's get the Be_D and L/D

			double lengthToDiameterRatio = pipeLength/hydraulicDiameter;

			double Be_D = pipeBeObject.getBe(pressureLoss,
					hydraulicDiameter,
					fluidDensity,
					fluidViscosity);

			// let's get Re
			double Re_D = nondimensionalPipeObj.getRe(Be_D,
					roughnessRatio,
					lengthToDiameterRatio,
					formLossK);


			// and finally return mass flowrate
			//
			MassFlow fluidMassFlowrate = 
				pipeReObject.getMassFlowrate(crossSectionalArea,
						Re_D,
						hydraulicDiameter,
						fluidViscosity);

			return fluidMassFlowrate;

		}

	}
}
