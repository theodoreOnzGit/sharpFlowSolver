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
		IPipeMassFlowAndPressureLossTilted
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









	}
}
