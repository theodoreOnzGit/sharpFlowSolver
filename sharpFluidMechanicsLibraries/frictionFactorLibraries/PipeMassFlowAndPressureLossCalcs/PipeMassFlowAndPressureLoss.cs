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
		IPipeMassFlowAndPressureLoss,
		IPipeMassFlowAndPressureLossTilted,
		IPipeMassFlowAndPressureLossTiltedWithSource
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

			// now before i calculate Re, i want to make sure that
			// reverse flow is accounted for
			// this is true when fluidMassFlowrate is less than 0
			bool reverseFlow = (fluidMassFlowrate.As(
						MassFlowUnit.KilogramPerSecond) < 0.0);

			// so if i have reverse flow, i will make the fluidMassFlowrate
			// positive
			// and return the negative value
			// of pressureLoss
			if(reverseFlow)
				fluidMassFlowrate *= -1;

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


			// now before i exit, i want to make sure reverse flow is taken care
			// of
			if(reverseFlow)
				return pressureLoss * -1.0;

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

		/*************************************************
		 * the follwing methods implement 
		 * calculating pressure change (not pressure loss)
		 *
		 * from mass flowrate
		 * and vice versa
		 *
		 * this is for a tilted pipe or inclined pipe
		 *
		 *
		 * ***********************************************/
		// tilted pipe
		// (1/2)
		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				double roughnessRatio,
				double formLossK){

			// note: for validation, i'm not going to check the incline Angle 
			// values,
			// they can be positive or negative and at any value the user
			// specifies.
			// the math is taken care of in the sine part

			// the formula here for pressure change is:
			//
			// pressure change  = - pressureloss + hydrostaticPressure
			// + sourcePressure (eg. pump)

			Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

			// for gravity i'll use 9.81 m/s^2 as my constant.

			Acceleration earthAcceleration9_81 =
				new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

			Pressure hydrostaticPressureChange = fluidDensity*
				earthAcceleration9_81*
				heightChange;

			Pressure pressureLoss = hydrostaticPressureChange - pressureChange;

			MassFlow fluidMassFlowrate = this.getMassFlow(pressureLoss,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					roughnessRatio,
					formLossK);


			return fluidMassFlowrate;

		}

		// tilted pipe
		// (2/2)
		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				double roughnessRatio,
				double formLossK){
			// note: for validation, i'm not going to check the incline Angle 
			// values,
			// they can be positive or negative and at any value the user
			// specifies.
			// the math is taken care of in the sine part

			// Math.Abs(pressure loss) 
			// = pressure change - hydrostaticPressureChange

			Length heightChange = pipeLength * Math.Sin(inclineAngle.As(
						AngleUnit.Radian));

			// for gravity i'll use 9.81 m/s^2 as my constant.

			Acceleration earthAcceleration9_81 =
				new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

			Pressure hydrostaticPressureChange = fluidDensity*
				earthAcceleration9_81*
				heightChange;

			// now let's get the pressure loss term

			Pressure pressureLoss = this.getPressureLoss(
					fluidMassFlowrate,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					roughnessRatio,
					formLossK);

			// the formula here for pressure change is:
			//
			// pressure change  = - pressureloss + hydrostaticPressure
			// + sourcePressure (eg. pump)

			return hydrostaticPressureChange - pressureLoss;
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
		// tilted pipe with internal source 
		// (1/6)
		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Pressure sourceOrPumpPressure,
				double roughnessRatio,
				double formLossK){

			// the formula here for pressure change is:
			//
			// pressure change  = - pressureloss + hydrostaticPressure
			// + sourcePressure (eg. pump)
			Pressure pressureChangeWithoutSource = 
				pressureChange - sourceOrPumpPressure;
			return this.getMassFlow(pressureChangeWithoutSource,
					crossSectionalArea,
					hydraulicDiameter,
					fluidViscosity,
					fluidDensity,
					pipeLength,
					inclineAngle,
					roughnessRatio,
					formLossK);
		}

		// tilted pipe with internal source 
		// (2/6)
		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Pressure sourceOrPumpPressure,
				double roughnessRatio,
				double formLossK){
			// the formula here for pressure change is:
			//
			// pressure change  = - pressureloss + hydrostaticPressure
			// + sourcePressure (eg. pump)
			Pressure pressureChangeWithoutSource = 
				this.getPressureChange(fluidMassFlowrate,
						crossSectionalArea,
						hydraulicDiameter,
						fluidViscosity,
						fluidDensity,
						pipeLength,
						inclineAngle,
						roughnessRatio,
						formLossK);

			return pressureChangeWithoutSource + sourceOrPumpPressure;

		}


		// tilted pipe with internal source 
		// (3/6)
		// commonly for pumps, kinematic pressure and pump head is used
		// so i'll have those overloads also

		// kinematic pressure overloads
		//
		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				SpecificEnergy sourceOrPumpKinematicPressure,
				double roughnessRatio,
				double formLossK){

			Pressure sourceOrPumpPressure = sourceOrPumpKinematicPressure*
				fluidDensity;

			return this.getMassFlow(pressureChange,
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
		// tilted pipe with internal source 
		// (4/6)

		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				SpecificEnergy sourceOrPumpKinematicPressure,
				double roughnessRatio,
				double formLossK){

			Pressure sourceOrPumpPressure = 
				sourceOrPumpKinematicPressure* 
				fluidDensity;

				return this.getPressureChange(fluidMassFlowrate,
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
		
		// tilted pipe with internal source 
		// (5/6)
		// pump head overloads

		public MassFlow getMassFlow(Pressure pressureChange,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Length sourceOrPumpHead,
				double roughnessRatio,
				double formLossK){

			Acceleration earthAcceleration9_81 =
				new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

			SpecificEnergy sourceOrPumpKinematicPressure = 
				earthAcceleration9_81*
				sourceOrPumpHead;


			return this.getMassFlow(pressureChange,
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

		// tilted pipe with internal source 
		// (6/6)
		public Pressure getPressureChange(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity,
				Density fluidDensity,
				Length pipeLength,
				Angle inclineAngle,
				Length sourceOrPumpHead,
				double roughnessRatio,
				double formLossK){

			Acceleration earthAcceleration9_81 =
				new Acceleration(9.81, AccelerationUnit.MeterPerSecondSquared);

			SpecificEnergy sourceOrPumpKinematicPressure = 
				earthAcceleration9_81*
				sourceOrPumpHead;


				return this.getPressureChange(fluidMassFlowrate,
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








	}
}
