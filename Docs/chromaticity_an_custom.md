### Analytically computing the chromaticity using a custom radiance spectrum
* Uses a user specified spectral radiance function to compute the chromaticity(xyY) of a given temperature(in Kelvins)
  ```c#
  // Custom spectral radiance function
  double CustomSpectrum(double wavelength, double temperature)
  {
      // Some Code
      return spectral_radiant_exitance;
  }
  
  double temp = 20000.0;
  xyYColor chromaticity1 = CCTConverter.GetChromaticityOfTemperature_With_SpectralRadiance(temp, CustomSpectrum);
  // Choosing a different precision
  // false -> low precision (default) | true -> high precision
  bool precision = true;
  xyYColor chromaticity2 = CCTConverter.GetChromaticityOfTemperature_With_SpectralRadiance(temp, CustomSpectrum, precision);
  ```
