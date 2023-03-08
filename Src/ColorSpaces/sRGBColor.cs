using System.Globalization;
using System.Drawing;
using static System.Math;
using System.Collections.ObjectModel;

namespace ColorLite
{
	public struct sRGBColor : IColorSpace
	{
		private readonly double[] data;
		public sRGBColor(sRGBColor color)
		{
			data = new double[3] { color.r, color.g, color.b };
		}

		/// <summary>
		/// Initializes an instance of a sRGB encoded RGB color
		/// </summary>
		/// <param name="r">R channel (0 - 1)</param>
		/// <param name="g">G channel (0 - 1)</param>
		/// <param name="b">B channel (0 - 1)</param>
		public sRGBColor(double r, double g, double b)
		{
			data = new double[3] { r, g, b };
		}

		public sRGBColor(Color color)
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
		public sRGBColor NormalizeIntensity()
		{
			var maxChannel = Max(r, Max(g, b));
			if (maxChannel == 0)
			{
				maxChannel = 1;
			}

			var r1 = Max(0, (Min(1, (r / maxChannel))));
			var g1 = Max(0, (Min(1, (g / maxChannel))));
			var b1 = Max(0, (Min(1, (b / maxChannel))));
			return new sRGBColor(r1, g1, b1);
		}

		/// <summary>
		/// Normalize the intensity of the color (Brings all channels to 0-1 Range)
		/// </summary>
		public sRGBColor NormalizedIntensity { get => NormalizeIntensity(); }

		/// <summary>
		/// Divide the channels by the sum of the channels
		/// </summary>
		public sRGBColor NormalizedRGB
		{
			get
			{
				double s = r + g + b;
				s = s.Equals(0.0) ? 1.0 : s;
				return new sRGBColor(r / s, g / s, b / s);
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
				sRGBColor color = NormalizedIntensity;
				return (255 << 24) | byteClamp(color.r * 255.0) << 16 | byteClamp(color.g * 255.0) << 8 | byteClamp(color.b * 255.0);
			}
		}

		/// <summary>
		/// Clamp all channels to 0-1 range
		/// </summary>
		public sRGBColor Clamped { get => new sRGBColor(clamp01(r), clamp01(g), clamp01(b)); }

		private int byteClamp(double a) => ((int)Min(Max(a, 0), 255));
		private double clamp01(double a) => Min(Max(a, 0), 1);

		#region Conversion Methods
		public LinearRGBColor To_LinearRGB()
		{
			double R = (r <= 0.04045) ? r / 12.92 : Pow((r + 0.055) / (1 + 0.055), 2.4);
			double G = (g <= 0.04045) ? g / 12.92 : Pow((g + 0.055) / (1 + 0.055), 2.4);
			double B = (b <= 0.04045) ? b / 12.92 : Pow((b + 0.055) / (1 + 0.055), 2.4);

			return new LinearRGBColor(R, G, B);
		}
		public sRGBColor To_sRGB() => this;
		public xyYColor To_xyY()
		{
			return To_XYZ().To_xyY();
		}

		public XYZColor To_XYZ()
		{
			LinearRGBColor linear = To_LinearRGB();
			return linear.To_XYZ();
		}
		#endregion

		#region operators and overloads
		public bool Equals(sRGBColor other) => r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b);
		public override bool Equals(object obj) => obj is sRGBColor other && Equals(other);
		public static bool operator ==(sRGBColor a, sRGBColor b) => Equals(a, b);
		public static bool operator !=(sRGBColor a, sRGBColor b) => !Equals(a, b);
		public override string ToString() => string.Format(CultureInfo.InvariantCulture, "sRGB [R={0:0.##}, G={1:0.##}, B={2:0.##}]", r, g, b);
		public override int GetHashCode() => base.GetHashCode();
		#endregion

		#region Conversion Operators
		public static explicit operator LinearRGBColor(sRGBColor srgb) => srgb.To_LinearRGB();
		#endregion

		#region Factory Methods
		public Color ToSystemColor { get => Color.FromArgb(this.ToInt32); }
		public static implicit operator Color(sRGBColor color) => Color.FromArgb(color.ToInt32);
		public static explicit operator sRGBColor(Color color) => new sRGBColor(color);
		#endregion
	}
}
