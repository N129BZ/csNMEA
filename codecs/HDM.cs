using System;
using Newtonsoft.Json;


namespace csNMEA
{
     /*
     * === HDM - Heading - magnetic ===
     *
     * ------------------------------------------------------------------------------
     *        1   2 3
     *        |   | |
     * $--HDM,x.x,M*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Heading degrees, magnetic
     * 2. M = Magnetic
     * 3. Checksum
     */
    public class HDMPacket : Decoder
    {
        public HDMPacket(string[] fields) {
            sentenceId = "HDM";
            sentenceName = "Heading - magnetic";
            heading = Helpers.parseFloatSafe(fields[1]);    
                
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float heading { get; set; }
    }
}