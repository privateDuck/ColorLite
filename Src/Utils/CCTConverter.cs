using System;

namespace ColorLite
{
	public static class CCTConverter
	{
		private static double pow2(in double d) => d * d;
		private static double pow3(in double d) => d * d * d;
		private static double pow4(in double d) => d * d * d * d;
		private static double pow5(in double d) => d * d * d * d * d;
		private static double pow6(in double d) => d * d * d * d * d * d;

		private const double pi = Math.PI;
		private const double h = 6.62607015e-34;
		private const double c = 299792458.0;
		private const double kB = 1.380649e-23;

		/// <summary>
		/// Returns the approximate chromaticity of a temperature. (works best in 1667K - 25000K range)
		/// </summary>
		/// <param name="temperature">Temperature of the balckbody in Kelvins</param>
		/// <returns>xy chromaticity (Y = 1)</returns>
		public static xyYColor GetChromaticityOfTemperature_Approximate(double temperature)
		{
			double x = 0, y = 0;
			if(temperature <= 4000)
			{
				x = -0.2661239 * 1e+9 / pow3(in temperature) - 0.2343589 * 1e+6 / pow2(in temperature) + 0.8776956 * (1e+3 / temperature) + 0.179910;
			}
			else
			{
				x = -3.0258469 * (1e+9 / pow3(in temperature)) + 2.1070379 * (1e+6 / pow2(in temperature)) + 0.2226347 * (1e+3 / temperature) + 0.240390;
			}

			if (temperature <= 2222)
			{
				y = -1.1063814 * pow3(x) - 1.34811020 * pow2(x) + 2.18555832 * x - 0.20219683;
			}
			else if (temperature <= 4000)
			{
				y = -0.9549476 * pow3(x) - 1.37418593 * pow2(x) + 2.09137015 * x - 0.16748867;
			}
			else
			{
				y = +3.0817580 * pow3(x) - 5.87338670 * pow2(x) + 3.75112997 * x - 0.37001483;
			}

			return new xyYColor(x,y,1);
		}

		/// <summary>
		/// Returns the analytically computed chromaticity of a temperature.
		/// </summary>
		/// <param name="temperature">Temperature of the balckbody in Kelvins</param>
		/// <param name="highPrecision">Precision of the lookup tables (high = 1nm, low = 5nm)</param>
		/// <returns>xyY chromaticity</returns>
		public static xyYColor GetChromaticityOfTemperature_Analytical(double temperature, bool highPrecision = false)
		{
			double X = 0, Y = 0, Z = 0;

			int length = (highPrecision) ? CIETables.COLOR_MATCHING_TABLE_LENGTH_1NM : CIETables.COLOR_MATCHING_TABLE_LENGTH_5NM;
			double wavelength = (highPrecision) ? CIETables.kLambdaStart_1nm : CIETables.kLambdaStart_5nm;
			double dLambda = (highPrecision) ? 1 : 5;

			for (int i = 0; i < length; i++)
			{
				double Xf, Yf, Zf;
				if(highPrecision) CIETables.CIE_COLOR_MATCHING_1NM(wavelength, out Xf, out Yf, out Zf);
				else CIETables.CIE_COLOR_MATCHING_5NM(wavelength, out Xf, out Yf, out Zf);

				double spectralRadiance = PlanksSpectralRadiance(wavelength * 1e-9, temperature);
				X += Xf * spectralRadiance;
				Y += Yf * spectralRadiance;
				Z += Zf * spectralRadiance;
				wavelength += dLambda;
			}

			X *= dLambda * 1e-9 / temperature;
			Y *= dLambda * 1e-9 / temperature;
			Z *= dLambda * 1e-9 / temperature;

			return new XYZColor(X, Y, Z).To_xyY();
		}

		/// <summary>
		/// Returns the analytically computed chromaticity of a temperature using a custom Spectral Radiance Function
		/// </summary>
		/// <param name="temperature">Temperature of the balckbody in Kelvins</param>
		/// <param name="SpectralRadianceGetter">Spectral Radiance Function (wavelength(in nm), temperature(in K))</param>
		/// <param name="highPrecision">Precision of the lookup tables (high = 1nm, low = 5nm)</param>
		/// <returns></returns>
		public static xyYColor GetChromaticityOfTemperature_With_SpectralRadiance(double temperature, Func<double,double,double> SpectralRadianceGetter, bool highPrecision = false)
		{
			double X = 0, Y = 0, Z = 0;

			int length = (highPrecision) ? CIETables.COLOR_MATCHING_TABLE_LENGTH_1NM : CIETables.COLOR_MATCHING_TABLE_LENGTH_5NM;
			double wavelength = (highPrecision) ? CIETables.kLambdaStart_1nm : CIETables.kLambdaStart_5nm;
			double dLambda = (highPrecision) ? 1 : 5;

			for (int i = 0; i < length; i++)
			{
				double Xf, Yf, Zf;
				if (highPrecision) CIETables.CIE_COLOR_MATCHING_1NM(wavelength, out Xf, out Yf, out Zf);
				else CIETables.CIE_COLOR_MATCHING_5NM(wavelength, out Xf, out Yf, out Zf);

				double spectralRadiance = SpectralRadianceGetter(wavelength, temperature);
				X += Xf * spectralRadiance;
				Y += Yf * spectralRadiance;
				Z += Zf * spectralRadiance;
				wavelength += dLambda;
			}

			X *= dLambda * 1e-9 / temperature;
			Y *= dLambda * 1e-9 / temperature;
			Z *= dLambda * 1e-9 / temperature;

			return new XYZColor(X, Y, Z).To_xyY();
		}

		private static readonly double[] xe = new double[] { 0.3366, 0.3356 };
		private static readonly double[] ye = new double[] { 0.1735, 0.1691 };
		private static readonly double[] A0 = new double[] { -949.86315, 36284.48953 };
		private static readonly double[] A1 = new double[] { 6253.80338, 0.00228 };
		private static readonly double[] t1 = new double[] { 0.92159, 0.07861 };
		private static readonly double[] A2 = new double[] { 28.70599, 5.4535*1e-36 };
		private static readonly double[] t2 = new double[] { 0.20039, 0.01543 };
		private const double A3 = 0.00004;
		private const double t3 = 0.07125;

		/// <summary>
		/// Returns the approximate CCT of a chromaticity
		/// </summary>
		/// <param name="chromaticity">xy Chromaticity (Y luminance is ignored)</param>
		/// <returns>CCT in Kelvins</returns>
		public static double GetCCTofChromaticity(xyYColor chromaticity)
		{
			double n = (chromaticity.x - xe[0]) / (chromaticity.y - ye[0]);
			double T = A0[0] + A1[0] * Math.Exp(-n / t1[0]) + A2[0] * Math.Exp(-n / t2[0]) + A3 * Math.Exp(-n / t3);

			if (T >= 50000)
			{
				n = (chromaticity.x - xe[1]) / (chromaticity.y - ye[1]);
				T = A0[1] + A1[1] * Math.Exp(-n / t1[1]) + A2[1] * Math.Exp(-n / t2[1]) + A3 * Math.Exp(-n / t3);
			}
			
			return T;
		}

		/// <summary>
		/// Returns The XYZ Color Representation of a Particular Wavelength
		/// </summary>
		/// <param name="wavelength">Wavelength</param>
		/// <returns>XYZ Color of the wavelength</returns>
		public static XYZColor WaveLengthToXYZ(double wavelength)
		{
			double x, y, z;
			CIETables.CIE_COLOR_MATCHING_1NM(wavelength, out x, out y, out z);
			return new XYZColor(x, y, z);
		}

		private static double PlanksSpectralRadiance(double lambda, double T)
		{
			const double c1 = 2 * pi * h * c * c;
			const double c2 = h * c / kB;

			return c1 / (pow5(lambda) * (Math.Exp(c2 / (lambda * T)) - 1));
		}
	}
}
