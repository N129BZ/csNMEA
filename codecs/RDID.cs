using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === PRDID - RDI proprietary heading, pitch, and roll ===
     *
     * ------------------------------------------------------------------------------
     *        1   2   3   4
     *        |   |   |   |
     * $PRDID,x.x,x.x,x.x*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Roll
     * 2. Pitch
     * 3. Heading
     * 4. Checksum
     */
    public class RDIDPacket : Decoder
    {
        public RDIDPacket(string[] fields) {
            try {
                sentenceId = "RDID";
                sentenceName = "RDI proprietary heading, pitch, and roll";
                roll = Helpers.parseFloatSafe(fields[1]);
                pitch = Helpers.parseFloatSafe(fields[2]);
                heading = Helpers.parseFloatSafe(fields[3]);
            }
            finally {}
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float roll { get; set; }
        public float pitch { get; set; }
        public float heading { get; set; }
    }
}