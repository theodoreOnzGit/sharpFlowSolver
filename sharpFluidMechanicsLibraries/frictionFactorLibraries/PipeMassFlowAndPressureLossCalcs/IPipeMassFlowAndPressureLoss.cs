using EngineeringUnits;

namespace sharpFluidMechanicsLibraries{

	public interface IPipeMassFlowAndPressureLoss
	{


		/*************************************************
		 * the follwing methods implement calculate pressure loss
		 * from mass flowrate
		 *
		 *
		 * ***********************************************/

		Pressure getPressureLoss(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				double roughnessRatio,
				double formLossK);

		MassFlow getMassFlow(Pressure pressureLoss,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				double roughnessRatio,
				double formLossK);

	}
}
