using System;
using EngineeringUnits;
using EngineeringUnits.Units;

namespace sharpFluidMechanicsLibraries{
	// the end user only need look at the following class to see what
	// functionality is available in this code.
	//
	//
	// Over here, i have constant temperature pipes, ie
	// the pipe has a constant viscosity and density throughout the pipe
	// this pipe is able to take in hydrostatic pressure into account as well
	//
	public class UniformTemperaturePipe
	{

		// this method gets a mass flow
		// given a pressure Change
		// this is NOT pressure drop
		//
		// pressure change  = pressure(final) - pressure(initial)

		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTilted pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);
		}

		// the following method gets a pressure Change
		// given a mass flowrate
		// This is not a pressure loss term, but rather pressure
		// change
		// which takes into account hydrostatic pressure as well

		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTilted pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);
		}

		/*************************************************
		 * the follwing methods implement 
		 * calculating pressure change (not pressure loss)
		 *
		 * from mass flowrate
		 * and vice versa
		 *
		 * this is for a tilted pipe or inclined pipe
		 * with an internal source. Maybe
		 * there could be a pump within the pipe or something
		 *
		 * source can be in the form of actual pressure
		 * or head loss,
		 * kinematic pressure is also included for convenience
		 *
		 *
		 * ***********************************************/


		// tilted pipe with source
		// (1/6)
		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Pressure sourceOrPumpPressure,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTiltedWithSource pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						sourceOrPumpPressure,
						roughnessRatio,
						formLossK);
		}

		// tilted pipe  with source
		// (2/6)

		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Pressure sourceOrPumpPressure,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTiltedWithSource pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						sourceOrPumpPressure,
						roughnessRatio,
						formLossK);
		}


		// tilted pipe with source
		// (3/6)
		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				SpecificEnergy sourceOrPumpKinematicPressure,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTiltedWithSource pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						sourceOrPumpKinematicPressure,
						roughnessRatio,
						formLossK);
		}

		// tilted pipe  with source
		// (4/6)

		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				SpecificEnergy sourceOrPumpKinematicPressure,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTiltedWithSource pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						sourceOrPumpKinematicPressure,
						roughnessRatio,
						formLossK);
		}


		// tilted pipe with source
		// (5/6)
		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Length sourceOrPumpHead,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTiltedWithSource pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getMassFlow(pressureChange,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						sourceOrPumpHead,
						roughnessRatio,
						formLossK);
		}

		// tilted pipe  with source
		// (6/6)

		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Length sourceOrPumpHead,
				double roughnessRatio = 0.0,
				double formLossK = 0.0){

			IPipeMassFlowAndPressureLossTiltedWithSource pressureLossObject =
				new PipeMassFlowAndPressureLossDefaultImplementation();

			return pressureLossObject.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						sourceOrPumpHead,
						roughnessRatio,
						formLossK);
		}
	}



}
