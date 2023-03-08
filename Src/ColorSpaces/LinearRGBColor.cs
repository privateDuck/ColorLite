using System.Globalization;
using System.Drawing;
using static System.Math;
using System.Collections.ObjectModel;

namespace ColorLite
{
	public struct LinearRGBColor : IColorSpace
	{
		private readonly double[] data;
		/// <summary>
		/// D65 Linear RGB to XYZ matrix
		/// </summary>
		public static double[][] rgb2xyz = new double[][]
		{
			new double[]{0.4124564,  0.3575761,  0.1804375},
			new double[]{0.2126729,  0.7151522,  0.0721750},
			new double[]{0.0193339,  0.1191920,  0.9503041 }
		};

		public LinearRGBColor(LinearRGBColor color)
		{
			data = new double[3] { color.r, color.g, color.b };
		}

		/// <summary>
		/// Initializes an instance of a linear RGB color
		/// </summary>
		/// <param name="r">R channel (0 - 1)</param>
		/// <param name="g">G channel (0 - 1)</param>
		/// <param name="b">B channel (0 - 1)</param>
		public LinearRGBColor(double r, double g, double b)
		{
			data = new double[3] { r, g, b };
		}

		public LinearRGBColor(Color color)
		{
			double r = System.Convert.ToDouble(color.R) / 255.0;
			double g = System.Convert.ToDouble(color.G) / 255.0;
			double b = System.Convert.ToDouble(color.B) / 255.0;
			data = new double[3] { r, g, b };
		}

		public double this[int i] => data[i];
		public double r { get => data[0]; }
		public double g { get => data[1]; }
		public double b { get => data[2]; }

		public ReadOnlyCollection<double> Vector => new ReadOnlyCollection<double>(data);

		/// <summary>
		/// Normalize the intensity of the color (Brings all channels to 0-1 Range)
		/// </summary>
		/// <returns>Normalized Color</returns>
		public LinearRGBColor NormalizeIntensity()
		{
			var maxChannel = Max(r, Max(g, b));
			if (maxChannel == 0)
			{
				maxChannel = 1;
			}

			var r1 = Max(0, (Min(1, (r / maxChannel))));
			var g1 = Max(0, (Min(1, (g / maxChannel))));
			var b1 = Max(0, (Min(1, (b / maxChannel))));
			return new LinearRGBColor(r1, g1, b1);
		}

		/// <summary>
		/// Normalize the intensity of the color (Brings all channels to 0-1 Range)
		/// </summary>
		public LinearRGBColor NormalizedIntensity { get => NormalizeIntensity(); }

		/// <summary>
		/// Divide the channels by the sum of the channels
		/// </summary>
		public LinearRGBColor NormalizedRGB
		{
			get
			{
				double s = r + g + b;
				s = s.Equals(0.0) ? 1.0 : s;
				return new LinearRGBColor(r / s, g / s, b / s);
			}
		}
		public RGB8 ToRGB8 { get { return new RGB8(this); } }

		/// <summary>
		/// Convert to 8bit format encoded to a int32
		/// </summary>
		public int ToInt32
		{
			get
			{
				LinearRGBColor color = NormalizedIntensity;
				return (255 << 24) | byteClamp(color.r * 255.0) << 16 | byteClamp(color.g * 255.0) << 8 | byteClamp(color.b * 255.0);
			}
		}

		/// <summary>
		/// Clamp all channels to 0-1 range
		/// </summary>
		public LinearRGBColor Clamped { get => new LinearRGBColor(clamp01(r), clamp01(g), clamp01(b)); }

		private int byteClamp(double a) => ((int)Min(Max(a, 0), 255));
		private double clamp01(double a) => Min(Max(a, 0), 1);

		#region Conversion Methods
		public LinearRGBColor To_LinearRGB() => this;
		public sRGBColor To_sRGB()
		{
			double R = (r <= 0.0031308) ? 12.92 * r : Pow(r, (1 / 2.4)) * 1.055 - 0.055;
			double G = (g <= 0.0031308) ? 12.92 * g : Pow(g, (1 / 2.4)) * 1.055 - 0.055;
			double B = (b <= 0.0031308) ? 12.92 * b : Pow(b, (1 / 2.4)) * 1.055 - 0.055;

			return new sRGBColor(R, G, B);
		}
		public xyYColor To_xyY() => To_XYZ().To_xyY();
		public XYZColor To_XYZ()
		{
			double x = rgb2xyz[0][0] * this.r + rgb2xyz[0][1] * this.g + rgb2xyz[0][2] * this.b;
			double y = rgb2xyz[1][0] * this.r + rgb2xyz[1][1] * this.g + rgb2xyz[1][2] * this.b;
			double z = rgb2xyz[2][0] * this.r + rgb2xyz[2][1] * this.g + rgb2xyz[2][2] * this.b;

			return new XYZColor(x, y, z);
		}
		#endregion

		#region operators and overloads
		public bool Equals(LinearRGBColor other) => r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b);
		public override bool Equals(object obj) => obj is LinearRGBColor other && Equals(other);
		public static bool operator ==(LinearRGBColor a, LinearRGBColor b) => Equals(a, b);
		public static bool operator !=(LinearRGBColor a, LinearRGBColor b) => !Equals(a, b);
		public override string ToString() => string.Format(CultureInfo.InvariantCulture, "LinearRGB [r={0:0.##}, g={1:0.##}, b={2:0.##}]", r, g, b);
		public override int GetHashCode() => base.GetHashCode();
		#endregion

		#region Conversion Operators
		public static explicit operator sRGBColor(LinearRGBColor linear) => linear.To_sRGB();
		#endregion

		#region Factory Methods
		public Color ToSystemColor { get => Color.FromArgb(this.ToInt32); }
		public static implicit operator Color(LinearRGBColor color) => Color.FromArgb(color.ToInt32);
		public static explicit operator LinearRGBColor(Color color) => new LinearRGBColor(color);
		#endregion
	}
}
