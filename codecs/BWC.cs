using System;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === BWC - Bearing and distance to waypoint - great circle ===
     *
     * ------------------------------------------------------------------------------
     *                                                         12
     *        1         2       3 4        5 6   7 8   9 10  11|    13 14
     *        |         |       | |        | |   | |   | |   | |    |  |
     * $--BEC,hhmmss.ss,llll.ll,a,yyyyy.yy,a,x.x,T,x.x,M,x.x,N,c--c,m,*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. UTC time
     * 2. Waypoint Latitude
     * 3. N = North, S = South
     * 4. Waypoint Longitude
     * 5. E = East, W = West
     * 6. Bearing, True
     * 7. T = True
     * 8. Bearing, Magnetic
     * 9. M = Magnetic
     * 10. Nautical Miles
     * 11. N = Nautical Miles
     * 12. Waypoint ID
     * 13. FAA mode indicator (NMEA 2.3 and later, optional)
     * 14. Checksum
     */
    public class BWCPacket : Decoder
    { 
        public BWCPacket(string[] fields) {
            sentenceId = "BWC";
            sentenceName = "Bearing and distance to waypoint - great circle";
            time = Helpers.parseTime(fields[1], "");
            bearingLatitude = Helpers.parseLatitude(fields[2], fields[3]);
            bearingLongitude = Helpers.parseLongitude(fields[4], fields[5]);
            bearingTrue = Helpers.parseFloatSafe(fields[6]);
            bearingMagnetic = Helpers.parseFloatSafe(fields[8]);
            distanceNm = Helpers.parseFloatSafe(fields[10]);
            waypointId = fields[12];
            faaMode = fields[13];
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public DateTime time { get; set; } 
        public float bearingLatitude { get; set; }
        public float bearingLongitude { get; set; } 
        public float bearingTrue  { get; set; } 
        public float bearingMagnetic { get; set; } 
        public float distanceNm { get; set; } 
        public string waypointId { get; set; }
        public string faaMode { get; set; } 
    }
}
