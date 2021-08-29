using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === GNS - GNSS fix data ===
     *
     * ------------------------------------------------------------------------------
     *                                                        11
     *        1         2       3 4        5 6 7  8   9  10   |    12  13
     *        |         |       | |        | | |  |   |   |   |    |   |
     * $--GNS,hhmmss.ss,llll.ll,N,yyyyy.yy,W,x,xx,x.x,x.x,x.x,null,xxxx*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Time (UTC)
     * 2. Latitude
     * 3. N or S (North or South)
     * 4. Longitude
     * 5. E or W (East or West)
     * 6. Mode Indicator - Variable Length,
     *    N - fix not available,
     *    A - GPS fix,
     *    D - Differential GPS fix
     *    P = PPS fix
     *    R = Real Time Kinematic
     *    F = Float RTK
     *    E = Estimated (dead reckoning)
     *    M = Manual input mode
     *    S = Simulation mode
     * 7. Number of satellites in view, 00 - 12
     * 8. Horizontal Dilution of precision
     * 9. Orthometric height in meters (MSL reference)
     * 10. Geoidal separation in meters - the difference between the earth ellipsoid surface and mean-sea-level (geoid) surface
     *     defined by the reference datum used in the position solution
     * 11. Age of differential data - Null if talker ID is GN, additional GNS messages follow with GP and/or GL Age of differential data
     * 12. Reference station ID1, range 0000-4095
     * 13. Checksum
    */
    public class GNSPacket : Decoder
    {
        public GNSPacket(string[] fields) {
            sentenceId = "GNS";
            sentenceName = "GNSS fix data";
            time =  Helpers.parseTime(fields[1], "");
            latitude =  Helpers.parseLatitude(fields[2], fields[3]);
            longitude =  Helpers.parseLongitude(fields[4], fields[5]);
            modeIndicator = fields[6];
            satellitesInView =  Helpers.parseIntSafe(fields[7]);
            horizontalDilution =  Helpers.parseFloatSafe(fields[8]);
            altitudeMeters =  Helpers.parseFloatSafe(fields[9]);
            geoidalSeperation =  Helpers.parseFloatSafe(fields[10]);
            differentialAge =  Helpers.parseFloatSafe(fields[11]);
            differentialRefStn = fields[12];
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public DateTime time { get; set; }   
        public float latitude { get; set; }  
        public float longitude { get; set; }   
        public string modeIndicator { get; set; }
        public int satellitesInView { get; set; }  
        public float horizontalDilution { get; set; } 
        public float altitudeMeters { get; set; }   
        public float geoidalSeperation { get; set; } 
        public float differentialAge { get; set; }  
        public string differentialRefStn { get; set; }
    }
}