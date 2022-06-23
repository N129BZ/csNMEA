using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === GLL - Geographic position - latitude and longitude ===
     *
     * ------------------------------------------------------------------------------
     *         1       2 3        4 5         6 7  8
     *         |       | |        | |         | |  |
     *  $--GLL,llll.ll,a,yyyyy.yy,a,hhmmss.ss,a,m,*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     *
     * 1. Latitude
     * 2. N or S (North or South)
     * 3. Longitude
     * 4. E or W (East or West)
     * 5. Universal Time Coordinated (UTC)
     * 6. Status
     *    A - Data Valid
     *    V - Data Invalid
     * 7. FAA mode indicator (NMEA 2.3 and later)
     * 8. Checksum
     */
    public class GLLPacket : Decoder
    {
        public GLLPacket(string[] fields) {
            try {
                sentenceId = "GLL";
                sentenceName = "Geographic position - latitude and longitude";
                latitude = Helpers.parseLatitude(fields[1], fields[2]);
                longitude = Helpers.parseLongitude(fields[3], fields[4]);
                time = Helpers.parseTime(fields[5], "");
                status = fields[6] == "A" ? "valid" : "invalid";
                faaMode = fields[7];
            }
            finally {}
        }
        
        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float latitude { get; set; }
        public float longitude { get; set; }
        public DateTime time { get; set; }
        public string status { get; set; }
        public string faaMode { get; set; }
    }
}