using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === VTG - Track made good and ground speed ===
     *
     * ------------------------------------------------------------------------
		     x[7]: 230394       Date - 23rd of March 1994------
     *        1     2 3     4 5   6 7   8 9  10
     *        |     | |     | |   | |   | |  |
     * $--VTG,xxx.x,T,xxx.x,M,x.x,N,x.x,K,m,*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     *
     * 1. Track Degrees
     * 2. T = True
     * 3. Track Degrees
     * 4. M = Magnetic
     * 5. Speed Knots
     * 6. N = Knots
     * 7. Speed Kilometers Per Hour
     * 8. K = Kilometers Per Hour
     * 9. FAA mode indicator (NMEA 2.3 and later)
     * 10. Checksum
     */
    public class VTGPacket : Decoder
    {
        public VTGPacket(string[] fields) {
            try {
                sentenceId = "VTG";
                sentenceName = "Track made good and ground speed";
                trackTrue = Helpers.parseFloatSafe(fields[1]);
                trackMagnetic = Helpers.parseFloatSafe(fields[3]);
                speedKnots = Helpers.parseFloatSafe(fields[5]);
                speedKmph = Helpers.parseFloatSafe(fields[7]);
                faaMode = fields[9];
            }
            finally {}   
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float trackTrue { get; set; }
        public float trackMagnetic { get; set; }
        public float speedKnots { get; set; }
        public float speedKmph { get; set; }
        public string faaMode { get; set; }
    }
}