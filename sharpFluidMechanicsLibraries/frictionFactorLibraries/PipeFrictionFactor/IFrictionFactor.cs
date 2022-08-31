// Here is a class for Fanning Friction Factor using Churchill's Correlation
//

namespace sharpFluidMechanicsLibraries{
	public interface IFrictionFactor
	{
		double fanning(double ReynoldsNumber, double roughnessRatio);
		double moody(double ReynoldsNumber, double roughnessRatio);
		double darcy(double ReynoldsNumber, double roughnessRatio);
		double fLDK(double ReynoldsNumber,
				double roughnessRatio,
				double lengthToDiameterRatio,
				double K);
	}
	public interface IFrictionFactorGetRe
	{
		double getRe(double Be, double roughnessRatio, 
				double lengthToDiameter);
	}

	public interface IfLDKFactorPipe {

		double generic_fLDK(double ReynoldsNumber, double roughnessRatio,
				double lengthToDiameter,
				double K);

		double generic_fLDK_ReSq(double ReynoldsNumber, double roughnessRatio,
				double lengthToDiameter,
				double K);
	}

	public interface IfLDKFactorGetRePipe
	{
		double generic_getRe(double Be, double roughnessRatio, 
				double lengthToDiameter,
				double K);
	}
}
