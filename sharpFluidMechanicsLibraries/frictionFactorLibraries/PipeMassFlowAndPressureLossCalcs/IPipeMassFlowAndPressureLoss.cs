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

	public interface IPipeMassFlowAndPressureLossTilted :
		IPipeMassFlowAndPressureLoss{

		MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				double roughnessRatio,
				double formLossK);

		Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				double roughnessRatio,
				double formLossK);
		}

	public interface IPipeMassFlowAndPressureLossTiltedWithSource :
		IPipeMassFlowAndPressureLossTilted{

		MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Pressure sourceOrPumpPressure,
				double roughnessRatio,
				double formLossK);

		Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Pressure sourceOrPumpPressure,
				double roughnessRatio,
				double formLossK);


		// commonly for pumps, kinematic pressure and pump head is used
		// so i'll have those overloads also

		// kinematic pressure overloads
		//
		MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				SpecificEnergy sourceOrPumpKinematicPressure,
				double roughnessRatio,
				double formLossK);

		Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				SpecificEnergy sourceOrPumpKinematicPressure,
				double roughnessRatio,
				double formLossK);
		
		// pump head overloads

		MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Length sourceOrPumpHead,
				double roughnessRatio,
				double formLossK);

		Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Length sourceOrPumpHead,
				double roughnessRatio,
				double formLossK);
		}

}
