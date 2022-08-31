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
		double getRe(double Be, double roughnessRatio, 
				double lengthToDiameter);

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
