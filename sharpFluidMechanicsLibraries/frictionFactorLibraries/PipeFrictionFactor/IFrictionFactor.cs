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
				double lengthToDiameter,
				double formLossK);
	}
	public interface IPipeReAndBe
	{
		// this method gets Re in case of a 
		// pipe without form losses
		double getRe(double Be_L, double roughnessRatio, 
				double lengthToDiameter);

		// this overload gets Re in case of a pipe with form
		// losses
		// it is based on Be_D rather than Be_L

		double getRe(double Be_D,
				double roughnessRatio,
				double lengthToDiameter,
				double formLossK);

		double getBe(double ReynoldsNumber, 
				double roughnessRatio,
				double lengthToDiameter,
				double formLossK);
				

	}

	public interface IfLDKFactorPipe {

		double generic_fLDK(double ReynoldsNumber, double roughnessRatio,
				double lengthToDiameter,
				double formLossK);

		double generic_fLDK_ReSq(double ReynoldsNumber, double roughnessRatio,
				double lengthToDiameter,
				double formLossK);
	}

	public interface IfLDKFactorGetRePipe
	{
		double generic_getRe(double Be, double roughnessRatio, 
				double lengthToDiameter,
				double formLossK);
	}
}
