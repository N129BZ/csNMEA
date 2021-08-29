using System;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === DTM - Datum reference ===
     *
     * ------------------------------------------------------------------------------
     *           1  2  3   4  5   6  7  8    9
     *           |  |  |   |  |   |  |  |    |
     *  $ --DTM,ref,x,llll,c,llll,c,aaa,ref*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Local datum code.
     *    W84 - WGS84
     *    W72 - WGS72
     *    S85 - SGS85
     *    P90 - PE90
     *    999 - User defined IHO datum code
     * 2. Local datum subcode. May be blank.
     * 3. Latitude offset (minutes)
     * 4. N or S
     * 5. Longitude offset (minutes)
     * 6. E or W
     * 7. Altitude offset in meters
     * 8. Datum name. What’s usually seen here is "W84", the standard WGS84 datum used by GPS.
     * 9. Checksum
     */
    public class DTMPacket : Decoder
    {
        public DTMPacket(string[] fields) {
            sentenceId = "DTM"; 
            sentenceName = "Datum reference"; 
            datumCode = parseDatumCode(fields[1]);
            datumSubcode = fields[2];
            offsetLatitude = Helpers.parseLatitude(fields[3], fields[4]);
            offsetLongitude = Helpers.parseLongitude(fields[5], fields[6]);
            offsetAltitudeMeters = Helpers.parseFloatSafe(fields[7]);
            datumName = parseDatumCode(fields[8]);
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public string datumCode { get; set; }    
        public string datumSubcode { get; set; } 
        public float offsetLatitude { get; set; } 
        public float offsetLongitude { get; set; }  
        public float offsetAltitudeMeters { get; set; }  
        public string datumName { get; set; } 
    
        public string parseDatumCode(string field) {
            return field == "W84" ? "W84"
                : field == "W72" ? "W72"
                    : field == "S85" ? "S85"
                        : field == "P90" ? "P90"
                            : field == "999" ? "999"
                                : "";
        }

    }
}
