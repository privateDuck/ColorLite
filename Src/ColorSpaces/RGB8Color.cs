using System.Globalization;
using System.Drawing;
using static System.Math;

namespace ColorLite
{
	public struct RGB8
	{
		public readonly byte R;
		public readonly byte G;
		public readonly byte B;

		public RGB8(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
		}

		public RGB8(sRGBColor sRGB)
		{
			this.R = System.Convert.ToByte(Min(sRGB.r * 255, 255));
			this.G = System.Convert.ToByte(Min(sRGB.g * 255, 255));
			this.B = System.Convert.ToByte(Min(sRGB.b * 255, 255));
		}

		public RGB8(LinearRGBColor linearRGB)
		{
			this.R = System.Convert.ToByte(linearRGB.r * 255);
			this.G = System.Convert.ToByte(linearRGB.g * 255);
			this.B = System.Convert.ToByte(linearRGB.b * 255);
		}

		public RGB8(System.Drawing.Color color)
		{
			R = color.R;
			G = color.G;
			B = color.B;
		}

		public int ToInt32 { get => (255 << 24) | (R << 16) | (G << 8) | B; }
		public Color ToSystemColor { get => Color.FromArgb(this.ToInt32); }
		public override string ToString() => string.Format(CultureInfo.InvariantCulture, "RGB8 [R={0:}, G={1:}, B={2:}]", R, G, B);
	}
}
