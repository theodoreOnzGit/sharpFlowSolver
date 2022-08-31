
namespace sharpFluidMechanicsLibraries{
	// the end user only need look at the following class to see what
	// functionality is available in this code.
	public class GetFrictionFactor
	{
		public double darcy(double Re, double roughnessRatio){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.darcy(Re,roughnessRatio);
		}
		public double moody(double Re, double roughnessRatio){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.moody(Re,roughnessRatio);
		}
		public double fanning(double Re, double roughnessRatio){
			IFrictionFactor frictionFactorCalcObj = 
				new ChurchillFrictionFactor();
			return frictionFactorCalcObj.fanning(Re,roughnessRatio);
		}
	}
}
