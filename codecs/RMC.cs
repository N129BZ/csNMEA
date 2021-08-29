using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === RMC - Recommended minimum navigation information ===
     *
     * ------------------------------------------------------------------------------
     *                                                              12
     *        1         2 3       4 5        6 7   8   9      10  11|  13
     *        |         | |       | |        | |   |   |      |   | |  |
     * $--RMC,hhmmss.ss,A,llll.ll,a,yyyyy.yy,a,x.x,x.x,ddmmyy,x.x,a,m,*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. UTC Time
     * 2. Status
     *    A = Valid
     *    V = Navigation receiver warning
     * 3. Latitude
     * 4. N or S
     * 5. Longitude
     * 6. E or W
     * 7. Speed over ground, knots
     * 8. Track made good, degrees true
     * 9. Date, ddmmyy
     * 10. Magnetic Variation, degrees
     * 11. E or W
     * 12. FAA mode indicator (NMEA 2.3 and later)
     * 13. Checksum
     */
    public class RMCPacket : Decoder
    {
        public RMCPacket(string[] fields) {
            sentenceId = "RMC";
            sentenceName = "Recommended minimum navigation information";
            datetime = Helpers.parseDatetime(fields[9], fields[1]);
            status = fields[2] == "A" ? "valid" : "warning";
            latitude = Helpers.parseLatitude(fields[3], fields[4]);
            longitude = Helpers.parseLongitude(fields[5], fields[6]);
            speedKnots = Helpers.parseFloatSafe(fields[7]);
            trackTrue = Helpers.parseFloatSafe(fields[8]);
            variation = Helpers.parseFloatSafe(fields[10]);
            variationPole = fields[11] == "E" ? "E" : fields[11] == "W" ? "W" : "";
            faaMode = fields[12];
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public DateTime datetime { get; set; }
        public string status { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float speedKnots { get; set; }
        public float trackTrue { get; set; }
        public float variation { get; set; }
        public string variationPole { get; set; }
        public string faaMode { get; set; }
    }
}