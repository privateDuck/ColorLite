using System.Collections.ObjectModel;
using System.Globalization;
using static System.Math;

namespace ColorLite
{
	public struct XYZColor : IColorSpace
	{
		//public readonly double X;
		//public readonly double Y;
		//public readonly double Z;
		
		private readonly double[] data;
		/// <summary>
		/// D65 XYZ to LinearRGB matrix
		/// </summary>
		public static double[][] xyz2rgb = new double[][]
		{
			new double[]{3.2404542, -1.5371385,-0.4985314},
			new double[]{-0.9692660, 1.8760108, 0.0415560},
			new double[]{0.0556434, -0.2040259, 1.0572252}
		};

		public XYZColor(XYZColor color)
		{
			data = new double[3] { color.X, color.Y, color.Z };
		}

		/// <summary>
		/// Initializes an instance of a XYZ Color
		/// </summary>
		/// <param name="X">X Value</param>
		/// <param name="Y">Y Value</param>
		/// <param name="Z">Z Value</param>
		public XYZColor(double X, double Y, double Z)
		{
			data = new double[3] { X, Y, Z };
		}

		public double this[int i] => data[i];
		public double X { get => data[0]; }
		public double Y { get => data[1]; }
		public double Z { get => data[2]; }
		public ReadOnlyCollection<double> Vector => new ReadOnlyCollection<double>(data);
		/// <summary>
		/// Equivalent to xyz(xyY : z redundant) space
		/// </summary>
		public XYZColor NormalizedXYZ { get => this * inverseSum; }
		
		/// <summary>
		/// Sum of the components
		/// </summary>
		public double Sum { get => X + Y + Z; }

		/// <summary>
		/// Inverse of the sum of the components
		/// </summary>
		public double inverseSum { get => (Sum != 0.0) ? 1.0 / Sum : 1; }

		#region Conversion Methods
		public LinearRGBColor To_LinearRGB()
		{
			double r = xyz2rgb[0][0] * this.X + xyz2rgb[0][1] * this.Y + xyz2rgb[0][2] * this.Z;
			double g = xyz2rgb[1][0] * this.X + xyz2rgb[1][1] * this.Y + xyz2rgb[1][2] * this.Z;
			double b = xyz2rgb[2][0] * this.X + xyz2rgb[2][1] * this.Y + xyz2rgb[2][2] * this.Z;

			return new LinearRGBColor(r, g, b);
		}
		public sRGBColor To_sRGB()
		{
			LinearRGBColor linear = To_LinearRGB();
			double R = (linear.r <= 0.0031308) ? 12.92 * linear.r : Pow(linear.r, (1 / 2.4)) * 1.055 - 0.055;
			double G = (linear.g <= 0.0031308) ? 12.92 * linear.g : Pow(linear.g, (1 / 2.4)) * 1.055 - 0.055;
			double B = (linear.b <= 0.0031308) ? 12.92 * linear.b : Pow(linear.b, (1 / 2.4)) * 1.055 - 0.055;

			return new sRGBColor(R, G, B);
		}
		public xyYColor To_xyY()
		{
			double x = X / Sum;
			double y = Y / Sum;
			return new xyYColor(x, y, Y);
		}
		public XYZColor To_XYZ() => this;
		
		#endregion


		#region operators and overloads
		public bool Equals(XYZColor other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		public override bool Equals(object obj) => obj is XYZColor other && Equals(other);
		public static bool operator ==(XYZColor a, XYZColor b) => Equals(a, b);
		public static bool operator !=(XYZColor a, XYZColor b) => !Equals(a, b);
		public static XYZColor operator +(XYZColor a, XYZColor b) => new XYZColor(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static XYZColor operator -(XYZColor a, XYZColor b) => new XYZColor(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		public static XYZColor operator *(XYZColor a, double b) => new XYZColor(a.X * b, a.Y * b, a.Z * b);
		public static XYZColor operator /(XYZColor a, double b) => new XYZColor(a.X / b, a.Y / b, a.Z / b);
		public override string ToString() => string.Format(CultureInfo.InvariantCulture, "XYZ [X={0:0.##}, Y={1:0.##}, Z={2:0.##}]", X, Y, Z);
		public override int GetHashCode() => base.GetHashCode();
		#endregion
	}
}
