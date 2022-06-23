using System;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === GGA - Global positioning system fix data ===
     *
     * ------------------------------------------------------------------------------
     *                                                      11
     *        1         2       3 4        5 6 7  8   9  10 |  12 13  14   15
     *        |         |       | |        | | |  |   |   | |   | |   |    |
     * $--GGA,hhmmss.ss,llll.ll,a,yyyyy.yy,a,x,xx,x.x,x.x,M,x.x,M,x.x,xxxx*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Time (UTC)
     * 2. Latitude
     * 3. N or S (North or South)
     * 4. Longitude
     * 5. E or W (East or West)
     * 6. GPS Quality Indicator,
     *    0 - fix not available,
     *    1 - GPS fix,
     *    2 - Differential GPS fix
     *    3 = PPS fix
     *    4 = Real Time Kinematic
     *    5 = Float RTK
     *    6 = Estimated (dead reckoning)
     *    7 = Manual input mode
     *    8 = Simulation mode
     * 7. Number of satellites in view, 00 - 12
     * 8. Horizontal Dilution of precision
     * 9. Antenna Altitude above/below mean-sea-level (geoid)
     * 10. Units of antenna altitude, meters
     * 11. Geoidal separation, the difference between the WGS-84 earth
     *     ellipsoid and mean-sea-level (geoid), "-" means mean-sea-level below ellipsoid
     * 12. Units of geoidal separation, meters
     * 13. Age of differential GPS data, time in seconds since last SC104
     *     type 1 or 9 update, null field when DGPS is not used
     * 14. Differential reference station ID, 0000-1023
     * 15. Checksum
    */
    public class GGAPacket : Decoder
    {
        string[] FixTypes = new string[] { "none", "fix", "delta", "pps", "rtk", "frtk", "estimated", "manual", "simulation" };
        
        public GGAPacket(string[] fields) {
            try {
                sentenceId = "GGA";;
                sentenceName = "Global positioning system fix data";
                time = Helpers.parseTime(fields[1], "");
                latitude = Helpers.parseLatitude(fields[2], fields[3]);
                longitude = Helpers.parseLongitude(fields[4], fields[5]);
                fixType = FixTypes[Helpers.parseIntSafe(fields[6])];
                satellitesInView = Helpers.parseIntSafe(fields[7]);
                horizontalDilution = Helpers.parseFloatSafe(fields[8]);
                altitudeMeters = Helpers.parseFloatSafe(fields[9]);
                geoidalSeperation = Helpers.parseFloatSafe(fields[11]);
                differentialAge = Helpers.parseFloatSafe(fields[13]);
                differentialRefStn = fields[14];
            }
            finally {}
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public DateTime time { get; set; } //:  helpers_1.parseTime(fields[1]),
        public float latitude { get; set; } //: helpers_1.parseLatitude(fields[2], fields[3]),
        public float longitude { get; set; } //: helpers_1.parseLongitude(fields[4], fields[5]),
        public string fixType { get; set; } //: FixTypes[helpers_1.parseIntSafe(fields[6])],
        public int satellitesInView { get; set; } //: helpers_1.parseIntSafe(fields[7]),
        public float horizontalDilution { get; set; } //horizontalDilution: helpers_1.parseFloatSafe(fields[8]),
        public float altitudeMeters { get; set; }  //: helpers_1.parseFloatSafe(fields[9]),
        public float geoidalSeperation { get; set; }  //: helpers_1.parseFloatSafe(fields[11]),
        public float differentialAge { get; set; }  //: helpers_1.parseFloatSafe(fields[13]),
        public string differentialRefStn { get; set; }  //: fields[14]
    }
}
