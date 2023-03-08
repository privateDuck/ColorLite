namespace ColorLite
{
	public interface IColorSpace
	{
		XYZColor To_XYZ();
		sRGBColor To_sRGB();
		LinearRGBColor To_LinearRGB();
		xyYColor To_xyY();
	}

	public enum ColorSpace { XYZ, LinearRGB, sRGB, xyY}
}
