## Analytically computing the chromaticity
* Uses plank's law to compute the chromaticity(xyY) of a given temperature (in Kelvins)
  ```c#
  double temp = 20000.0;
  xyYColor chromaticity1 = CCTConverter.GetChromaticityOfTemperature_Analytical(temp);
  // Choosing a different precision
  // false -> low precision (default) | true -> high precision
  xyYColor chromaticity2 = CCTConverter.GetChromaticityOfTemperature_Analytical(temp, true);
  ```
