### Approximating the chromaticity (xyY excluding Y) of a given temperature (in Kelvins)
  * Uses plank's law to approximate the chromaticity
  ```c#
  // Approximates the chromaticity
  double temp = 20000.0;
  xyYColor chromaticity1 = CCTConverter.GetChromaticityOfTemperature_Approximate(temp);
  ```
### Approximating the CCT of a given chromaticity
  ```c#
  // Approximates the CCT of a given chromaticity (luminance ignored)
  double temperature = CCTConverter.GetCCTofChromaticity(new xyYColor(0.5,0.2,0.0));
  ```
### Converting the XYZ color of light with a given wavelength (in nano meters)
  ```c#
  // wavelength should be between 360nm and 830nm
  double wavelength = 680;
  XYZColor xyz = CCTConverter.WaveLengthToXYZ(wavelength);
  ```
