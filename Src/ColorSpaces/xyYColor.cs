using System.Collections.ObjectModel;
using System.Globalization;

namespace ColorLite
{
	public struct xyYColor : IColorSpace
	{
		private readonly double[] data;
		/// <summary>
		/// Initializes an instance of a xyY Color`
		/// </summary>
		/// <param name="x">x chromaticity</param>
		/// <param name="y">y chromaticity</param>
		/// <param name="Y">luminance</param>
		public xyYColor(double x, double y, double Y)
		{
			data = new double[3] { x, y, Y };
		}

		public double this[int i] => data[i];
		public double x { get => data[0]; }
		public double y { get => data[1]; }
		public double Y { get => data[2]; }

		public ReadOnlyCollection<double> Vector => new ReadOnlyCollection<double>(data);

		/// <summary>
		/// Get the co-related temperature of this color
		/// </summary>
		public double GetCCT => CCTConverter.GetCCTofChromaticity(this);

		#region Conversion Methods
		public LinearRGBColor To_LinearRGB() => To_XYZ().To_LinearRGB();
		public sRGBColor To_sRGB() => To_XYZ().To_sRGB();
		public xyYColor To_xyY() => this;
		public XYZColor To_XYZ()
		{
			if (Y <= 0 || y == 0)
			{
				return new XYZColor(0, 0, 0);
			}
			double X = x * Y / y;
			double Z = (1 - x - y) * Y / y;

			return new XYZColor(X, Y, Z);
		}

		#endregion

		#region operators and overloads
		public bool Equals(xyYColor other) => x.Equals(other.x) && y.Equals(other.y) && Y.Equals(other.Y);
		public override bool Equals(object obj) => obj is xyYColor other && Equals(other);
		public static bool operator ==(xyYColor a, xyYColor b) => Equals(a, b);
		public static bool operator !=(xyYColor a, xyYColor b) => !Equals(a, b);

		public override string ToString() => string.Format(CultureInfo.InvariantCulture, "xyY [x={0:0.##}, y={1:0.##}, Y={2:0.##}]", x, y, Y);
		public override int GetHashCode() => base.GetHashCode();
		#endregion
	}
}
