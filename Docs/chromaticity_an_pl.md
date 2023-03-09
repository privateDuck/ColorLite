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
