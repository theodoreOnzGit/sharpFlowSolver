// Here is a class for Fanning Friction Factor using Churchill's Correlation
//

using System;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using System.Collections.Generic;


namespace sharpFluidMechanicsLibraries{

	public partial class ChurchillFrictionFactor : 
		IFrictionFactor ,IPipeReAndBe
	{


		/*************************************************
		 * the follwing methods implement IFrictionFactor
		 *
		 *
		 *
		 * ***********************************************/


		// this particular implementation uses the churchill correlation
		public double fanning(double ReynoldsNumber, 
				double roughnessRatio){

			if(ReynoldsNumber == 0)
				throw new DivideByZeroException("Re = 0");

			if(ReynoldsNumber < 0)
				throw new ArgumentOutOfRangeException("Re<0");
			
			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException("roughnessRatio<0");

			double fanningFrictionFactor;
			fanningFrictionFactor = 2 * Math.Pow(this.churchillInnerTerm(ReynoldsNumber,roughnessRatio), 1.0/12);
			return fanningFrictionFactor;
		}

		public double moody(double ReynoldsNumber, 
				double roughnessRatio){

			// apparently the moody friciton factor is same as the darcy friction factor

			if(ReynoldsNumber == 0)
				throw new DivideByZeroException("Re = 0");

			if(ReynoldsNumber < 0)
				throw new ArgumentOutOfRangeException("Re<0");
			
			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException("roughnessRatio<0");

			return this.darcy(ReynoldsNumber,roughnessRatio);
		}

		public double darcy(double ReynoldsNumber, 
				double roughnessRatio){
			if(ReynoldsNumber == 0)
				throw new DivideByZeroException("Re = 0");

			if(ReynoldsNumber < 0)
				throw new ArgumentOutOfRangeException("Re<0");
			
			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException("roughnessRatio<0");

			// darcy friction factor is 4x fanning friction factor
			// https://neutrium.net/fluid-flow/pressure-loss-in-pipe/
			double darcyFrictionFactor;
			darcyFrictionFactor = 4 * this.fanning(ReynoldsNumber,roughnessRatio);
			return darcyFrictionFactor;
		}

		public double fLDK(double ReynoldsNumber,
				double roughnessRatio,
				double lengthToDiameterRatio,
				double K){
			if(ReynoldsNumber == 0)
				throw new DivideByZeroException("Re = 0");

			if(ReynoldsNumber < 0)
				throw new ArgumentOutOfRangeException("Re < 0");

			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException("roughnessRatio<0");

			if(lengthToDiameterRatio <= 0)
				throw new ArgumentOutOfRangeException(
						"lengthToDiameterRatio<=0");

			if(K < 0)
				throw new ArgumentOutOfRangeException(
						"Form loss coefficient K < 0");

			double fLDK;
			double f = this.darcy(ReynoldsNumber,
					roughnessRatio);
			fLDK = f*lengthToDiameterRatio + K;

			return fLDK;
		}

		/***********************************************
		 * The following methods implement IPipeBeAndRe
		 *
		 *
		 *
		 *
		 * ***********************************************/

		public double getBe(double ReynoldsNumber,
				double roughnessRatio,
				double lengthToDiameterRatio,
				double K){

			if(ReynoldsNumber == 0)
				return 0.0;

			if(ReynoldsNumber < 0)
				throw new ArgumentOutOfRangeException("Re < 0");

			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException("roughnessRatio<0");

			if(lengthToDiameterRatio <= 0)
				throw new ArgumentOutOfRangeException(
						"lengthToDiameterRatio<=0");

			if(K < 0)
				throw new ArgumentOutOfRangeException(
						"Form loss coefficient K < 0");

			// i'm including an improvement for Re<1800
			// so that we linearly interpolate the churchill
			// friction factor from 1800 onwards
			// basically, we are interpolating
			// f_{darcy}*Re^2  = 64Re
			// at lower Re values to increase accuracy and save computation
			// cost
			if(ReynoldsNumber <1800){
				double ReTransition = 1800.0;
				IInterpolation _linear_f_ReSq;

				IList<double> xValues = new List<double>();
				IList<double> yValues = new List<double>();
				xValues.Add(0.0);
				xValues.Add(ReTransition);

				yValues.Add(0.0);
				yValues.Add(64.0*ReTransition);

				_linear_f_ReSq = Interpolate.Linear(xValues,yValues);
				double fLDReSq = _linear_f_ReSq.Interpolate(
						ReynoldsNumber)*lengthToDiameterRatio;

				double kReSq = K*Math.Pow(ReynoldsNumber,2.0);
				
				return 0.5*(kReSq + fLDReSq);

			}

			double fLDK;
			double f = this.darcy(ReynoldsNumber,
					roughnessRatio);
			fLDK = f*lengthToDiameterRatio + K;

			double Be = 0.5*fLDK*
				Math.Pow(ReynoldsNumber,2.0);

			return Be;
		}

		public double getRe(double Be_D,
				double roughnessRatio,
				double lengthToDiameter,
				double formLossK){

			if(formLossK == 0){
				double Be_L = Be_D * Math.Pow(lengthToDiameter,
						2.0);
				return this.getRe(Be_L,
						roughnessRatio,
						lengthToDiameter);
			}

			if(lengthToDiameter <= 0)
				throw new ArgumentOutOfRangeException(
						"lengthToDiameterRatio<=0");

			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException(
						"roughnessRatio<0");

			if(formLossK < 0)
				throw new ArgumentOutOfRangeException(
						"formLossK<0");

			// this part deals with negative Be_L values
			// invalid Be_L values
			bool isNegative;
			if (Be_D < 0)
			{
				Be_D *= -1;
				isNegative = true;
			}
			else 
			{
				isNegative = false;
			}

			double maxRe = 1e12;

			// i calculate the Be_D corresponding to 
			// Re = 1e12
			double maxBe_D = this.getBe(maxRe,
					roughnessRatio, lengthToDiameter,
					0.0);

			if(Be_D >= maxBe_D)
				throw new ArgumentOutOfRangeException(
						"Be too large");
			// the above checks for all the relevant exceptions
			// including formLossK < 0
			//
			// now we are ready to do root finding
			//
			// the underlying equation is 
			// Be = 0.5*fLDK*Re^2

			this.roughnessRatio = roughnessRatio;
			this.lengthToDiameter = lengthToDiameter;
			this.bejanNumber = Be_D;
			this.formLossK = formLossK;

			double pressureDropRoot(double Re){
				// i'm solving for
				// Be - 0.5*fLDK*Re^2 = 0 
				// the fLDK term can be calculated using
				// getBe
				//
				// now i don't really need the interpolation
				// term in here because when Re = 0,
				// Be = 0 in the getBe code.
				// so really, no need for fancy interpolation.

				double fLDKterm = this.getBe(Re,
						this.roughnessRatio,
						this.lengthToDiameter,
						this.formLossK);

				return this.bejanNumber - fLDKterm;

			}
			
			double ReynoldsNumber = 0.0;
			ReynoldsNumber = FindRoots.OfFunction(
					pressureDropRoot, 0, 
					maxRe);

			// now i'll clean everything up
			this.roughnessRatio = 0.0;
			this.lengthToDiameter = 0.0;
			this.bejanNumber = 0.0;
			this.formLossK = 0.0;


			if (isNegative)
			{
				return -ReynoldsNumber;
			}

			return ReynoldsNumber;
		}

		// the following overload gets Re in case
		// there is no form loss
		// it also works based on Be_L rather than Be_D

		public double getRe(double Be_L, 
				double roughnessRatio,
				double lengthToDiameter){

			// now i want to make sure this function can handle negative 
			// pressure drop
			//
			// ie pressure drops in reverse direction, and this should
			// yield us reverse flow and negative Reynold's numbers
			// so what i'll do is this: if Be_L < 0,
			// then i'll make it positive
			//

			if(lengthToDiameter <= 0)
				throw new ArgumentOutOfRangeException(
						"lengthToDiameterRatio<=0");

			if(roughnessRatio < 0)
				throw new ArgumentOutOfRangeException(
						"roughnessRatio<0");

			// this part deals with negative Be_L values
			// invalid Be_L values
			bool isNegative;
			if (Be_L < 0)
			{
				Be_L *= -1;
				isNegative = true;
			}
			else 
			{
				isNegative = false;
			}

			double maxRe = 1e12;

			// i calculate the Be_L corresponding to 
			// Re = 1e12
			double maxBe_D = this.getBe(maxRe,
					roughnessRatio, lengthToDiameter,
					0.0);
			double maxBe_L = maxBe_D*
				Math.Pow(lengthToDiameter,2.0);

			if(Be_L >= maxBe_L)
				throw new ArgumentOutOfRangeException(
						"Be too large");



			this.roughnessRatio = roughnessRatio;
			this.lengthToDiameter = lengthToDiameter;
			this.bejanNumber = Be_L;

			// I'll define a pressureDrop function with which to find
			// the Reynold's Number
			double pressureDropRoot(double Re){

				// fanning term
				//
				//
				// Now here is a potential issue for stability,
				// if Re = 0, the fanning friction factor is not well behaved,
				// Hence it's better to use the laminar term at low Reynold's number
				//
				// we note that in the laminar regime, 
				// f = 16/Re
				// so f*Re^2 = 16*Re
				double transitionPoint = 1800.0;
				double fanningTerm;

				if (Re > transitionPoint)
				{
					fanningTerm = this.fanning(
							Re, this.roughnessRatio);
					fanningTerm *= Math.Pow(Re,2.0);
				}
				else
				{
					// otherwise we return 16/Re*Re^2 or 16*Re
					// or rather an interpolated version to preserve the
					// continuity of the points.
					IInterpolation _linear;

					IList<double> xValues = new List<double>();
					IList<double> yValues = new List<double>();
					xValues.Add(0.0);
					xValues.Add(transitionPoint);

					yValues.Add(0.0);
					yValues.Add(this.fanning(transitionPoint,this.roughnessRatio)*
							Math.Pow(transitionPoint,2.0));

					_linear = Interpolate.Linear(xValues,yValues);
					fanningTerm = _linear.Interpolate(Re);
				}






				//  BejanTerm
				//
				double bejanTerm;
				bejanTerm = 32.0 * this.bejanNumber;
				bejanTerm *= Math.Pow(4.0*this.lengthToDiameter,-3);

				// to set this to zero, we need:
				//
				return fanningTerm - bejanTerm;

			}

			double ReynoldsNumber;
			ReynoldsNumber = FindRoots.OfFunction(
					pressureDropRoot, 0, 
					maxRe);

			// once I'm done, i want to clean up all terms
			this.roughnessRatio = 0.0;
			this.lengthToDiameter = 0.0;
			this.bejanNumber = 0.0;


			// then let's return Re

			if (isNegative)
			{
				return -ReynoldsNumber;
			}

			return ReynoldsNumber;
		}



		public double roughnessRatio { get; private set; }
		public double lengthToDiameter { get; private set; }
		public double bejanNumber {get; private set; }
		public double formLossK {get; private set; }

		/***********************************************
		 * The following methods do the backend logic
		 * for the churchill Friction factor
		 *
		 *
		 *
		 *
		 * ***********************************************/

		private double churchillInnerTerm(double Re, double roughnessRatio){

			double laminarTerm;
			laminarTerm = Math.Pow(8.0/Re, 12);

			double turbulentTerm;
			double Aterm = this.A(Re,roughnessRatio);
			double Bterm = this.B(Re);

			turbulentTerm = Math.Pow( 1.0/(Aterm + Bterm), 3.0/2);

			return laminarTerm + turbulentTerm;


		}

		public double A(double Re, double roughnessRatio){
			// first i need the logarithm of a number

			double reynoldsTerm =  Math.Pow( (7.0/Re), 0.9);
			double roughnessTerm = 0.27*roughnessRatio;

			double logFraction = 1.0/(reynoldsTerm+roughnessTerm);
			double innerBracketTerm = 2.457*Math.Log(logFraction);
			double A = Math.Pow(innerBracketTerm,16);

			return A;
		}

		public double B(double Re){
			double numerator = Math.Pow(37530,16);
			double denominator = Math.Pow(Re,16);
			return numerator/denominator;
		}
		/*
		 ************************************************************* 
		 ************************************************************* 
		 ************************************************************* 
		 this part will help implement code to find Re given a specific
		 Bejan number and roughnessRatio
		 ************************************************************* 
		 ************************************************************* 
		 ************************************************************* 
		 ************************************************************* 
		 */


	}

}
