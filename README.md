# ColorLite
A simple .NET library for working with common color spaces

# Documentation
### Working Color Spaces
* ColorLite offers 4 of the most commonly used Color Spaces
  * [CIE XYZ (1931)](https://en.wikipedia.org/wiki/CIE_1931_color_space#Meaning_of_X,_Y_and_Z)
  * [CIE xyY](https://en.wikipedia.org/wiki/CIE_1931_color_space#CIE_xy_chromaticity_diagram_and_the_CIE_xyY_color_space)
  * Linear RGB
  * [sRGB](https://en.wikipedia.org/wiki/SRGB#sRGB_definition)
* **Illuminant D65** is used as the white point

### Conversion Between Color Spaces
* All working spaces implement the IColorSpace interface
* The interface contains methods for converting one ColorSpace to another
```c#
// Example
LinearRGBColor linrgb = new LinearRGBColor(0.2,0,0.4);
XYZColor xyz = linrgb.To_XYZ();
```
* You can also chain conversion methods
```c#
LinearRGBColor linrgb = new LinearRGBColor(0.2,0,0.4);
XYZColor xyz = linrgb.To_XYZ();
sRGBColor srgb = linrgb.To_XYZ().To_sRGB();
```

### CCT (Correlated Colour Temperature) Calculations
* ColorLite offers the **CCTConverter** static class for working with CCTs
* This mainly offers 4 functionalities
  * Analytically computing the chromaticity (xyY) of a given temperature (in Kelvins)
     ```c#
     // Integrates the plankian radiance spectrum to compute the chromaticity
     double temp = 20000.0;
     xyYColor chromaticity1 = CCTConverter.GetChromaticityOfTemperature_Analytical(temp);
     // Choosing a different precision
     // false -> low precision (default)
     // true -> high precision
     xyYColor chromaticity2 = CCTConverter.GetChromaticityOfTemperature_Analytical(temp, true);
     ```
  * Approximating the chromaticity (xyY excluding Y) of a given temperature (in Kelvins)
    ```c#
     // Approximates the chromaticity
     double temp = 20000.0;
     xyYColor chromaticity1 = CCTConverter.GetChromaticityOfTemperature_Approximate(temp);
    ```
  * Approximating the CCT of a given chromaticity
    ```c#
     // Approximates the CCT of a given chromaticity (luminance ignored)
     double temperature = CCTConverter.GetCCTofChromaticity(new xyYColor(0.5,0.2,0.0));
    ```
  * Converting the XYZ color of light with a given wavelength (in nano meters)
    ```c#
     // wavelength must be between 360nm and 830nm
     double wavelength = 680;
     XYZColor xyz = CCTConverter.WaveLengthToXYZ(wavelength);
    ```
## CIE Color Matching Tables
* ColorLite does allow you to use both the 5nm and the 1nm CIE Color Matching Tables
* Use the provided **CIETables** static class to use these look-up tables
* Although the static method **CCTConverter.WaveLengthToXYZ(double wavelength);** does more or less the same thing as the static methods contained in this class
  ```C#
  // Example of using the 1nm table
  double x,y,z;
  double wavelength = 560;
  CIETables.CIE_COLOR_MATCHING_1NM(wavelength, out x, out y, out z);
  ```
