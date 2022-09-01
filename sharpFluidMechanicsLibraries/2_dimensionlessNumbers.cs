using EngineeringUnits;
using EngineeringUnits.Units;
using System;

namespace sharpFluidMechanicsLibraries{
	// the end user only need look at the following class to see what
	// functionality is available in this code.
	public class PipeReynoldsNumber
	{
		// these nondimensionalise and redimensionalise
		// velocity to Re and back
		public double getRe(Density fluidDensity,
				Speed averageVelocity,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity){

			if(fluidDensity.As(DensityUnit.SI ) <= 0)
					throw new ArgumentOutOfRangeException(
						"density <= 0, nonphysical");
			if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"fluid Viscosity <= 0, nonphysical");
			if(hydraulicDiameter.As(LengthUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"hydraulic Diameter <= 0, nonphysical");

			double Re = fluidDensity*
				averageVelocity*
				hydraulicDiameter/
				fluidViscosity;
			return Re;
		}

		public Speed getAverageVelocity(Density fluidDensity,
				double Re,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity){
			if(fluidDensity.As(DensityUnit.SI) <= 0)
					throw new ArgumentOutOfRangeException(
						"density <= 0, nonphysical");
			if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"fluid Viscosity <= 0, nonphysical");
			if(hydraulicDiameter.As(LengthUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"hydraulic Diameter <= 0, nonphysical");

			Speed averageVelocity = fluidViscosity/
				fluidDensity/
				hydraulicDiameter*
				Re;

			return averageVelocity;
		}
		
		// These nondimensionalise and redimensionalise
		// mass flowrate to Re and back
		public double getRe(MassFlow fluidMassFlowrate,
				Area crossSectionalArea,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity){

			if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"fluid Viscosity <= 0, nonphysical");
			if(hydraulicDiameter.As(LengthUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"hydraulic Diameter <= 0, nonphysical");
			if(crossSectionalArea.As(AreaUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"pipe Area <= 0, nonphysical");

			double Re = fluidMassFlowrate/
				crossSectionalArea*
				hydraulicDiameter/
				fluidViscosity;
			return Re;
		}

		public MassFlow getMassFlowrate(Area crossSectionalArea,
				double Re,
				Length hydraulicDiameter,
				DynamicViscosity fluidViscosity){

			if(fluidViscosity.As(DynamicViscosityUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"fluid Viscosity <= 0, nonphysical");
			if(hydraulicDiameter.As(LengthUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"hydraulic Diameter <= 0, nonphysical");
			if(crossSectionalArea.As(AreaUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"pipe Area <= 0, nonphysical");

			MassFlow fluidMassFlowrate = fluidViscosity*
				crossSectionalArea/
				hydraulicDiameter*
				Re;

			return fluidMassFlowrate;
		}

	}

	public class PipeBejanNumber{

		public double getBe(SpecificEnergy kinematicPressure,
				Length hydraulicDiameter,
				KinematicViscosity fluidKinematicViscosity){

			if(fluidKinematicViscosity.As(KinematicViscosityUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"fluidKinematicViscosity <= 0, nonphysical");
			if(hydraulicDiameter.As(LengthUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"hydraulic Diameter <= 0, nonphysical");

			double Be = kinematicPressure*
				hydraulicDiameter.Pow(2)/
				fluidKinematicViscosity.Pow(2);

			return Be;
		}

		public double getBe(Pressure fluidPressure,
				Length hydraulicDiameter,
				Density fluidDensity,
				KinematicViscosity fluidKinematicViscosity){

			SpecificEnergy fluidKinematicPressure = fluidPressure/
				fluidDensity;

			double Be = this.getBe(fluidKinematicPressure,
					hydraulicDiameter,
					fluidKinematicViscosity);

			return Be;
		}

		public double getBe(Pressure fluidPressure,
				Length hydraulicDiameter,
				Density fluidDensity,
				DynamicViscosity fluidDynamicViscosity){

			SpecificEnergy fluidKinematicPressure = fluidPressure/
				fluidDensity;

			KinematicViscosity fluidKinematicViscosity = 
				fluidDynamicViscosity/
				fluidDensity;

			double Be = this.getBe(fluidKinematicPressure,
					hydraulicDiameter,
					fluidKinematicViscosity);

			return Be;
		}

		// the following gets kinematic and dyanmic pressure back from
		// Bejan number
		public SpecificEnergy getFluidKinematicPressure(double Be_D,
				Length hydraulicDiameter,
				KinematicViscosity fluidKinematicViscosity){

			if(fluidKinematicViscosity.As(KinematicViscosityUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"fluidKinematicViscosity <= 0, nonphysical");
			if(hydraulicDiameter.As(LengthUnit.SI) <= 0)
				throw new ArgumentOutOfRangeException(
						"hydraulic Diameter <= 0, nonphysical");

			SpecificEnergy fluidKinematicPressure =
				fluidKinematicViscosity.Pow(2)/
				hydraulicDiameter.Pow(2)*
				Be_D;
			return fluidKinematicPressure;
		}

		public Pressure getFluidPressure(double Be_D,
				Length hydraulicDiameter,
				Density fluidDensity,
				DynamicViscosity fluidDynamicViscosity){

			KinematicViscosity fluidKinematicViscosity = 
				fluidDynamicViscosity/fluidDensity;

			SpecificEnergy fluidKinematicPressure =
				this.getFluidKinematicPressure(
						Be_D,
						hydraulicDiameter,
						fluidKinematicViscosity);

			Pressure fluidPressure = fluidKinematicPressure*
				fluidDensity;

			return fluidPressure;

		}

		public Pressure getFluidPressure(double Be_D,
				Length hydraulicDiameter,
				Density fluidDensity,
				KinematicViscosity fluidKinematicViscosity){


			SpecificEnergy fluidKinematicPressure =
				this.getFluidKinematicPressure(
						Be_D,
						hydraulicDiameter,
						fluidKinematicViscosity);

			Pressure fluidPressure = fluidKinematicPressure*
				fluidDensity;

			return fluidPressure;

		}


	}

}
