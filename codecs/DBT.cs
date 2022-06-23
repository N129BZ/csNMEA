using System;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === DBT - Depth below transducer ===
     *
     * ------------------------------------------------------------------------------
     *        1   2 3   4 5   6 7
     *        |   | |   | |   | |
     * $--DBT,x.x,f,x.x,M,x.x,F*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Depth, feet
     * 2. f = feet
     * 3. Depth, meters
     * 4. M = meters
     * 5. Depth, Fathoms
     * 6. F = Fathoms
     * 7. Checksum
     */
    public class DBTPacket : Decoder
    {
        public DBTPacket(string[] fields) {
            try {
                sentenceId = "DBT";
                sentenceName = "Depth below transducer";
                depthFeet = Helpers.parseFloatSafe(fields[1]);
                depthMeters = Helpers.parseFloatSafe(fields[3]);
                depthFathoms = Helpers.parseFloatSafe(fields[5]);
            }
            finally {}
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public float depthFeet { get; set; }
        public float depthMeters { get; set; }
        public float depthFathoms { get; set; }
    }
}
