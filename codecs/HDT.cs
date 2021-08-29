using System;
using Newtonsoft.Json;


namespace csNMEA 
{
    public class HDTPacket : Decoder
    {
        /*
         * === HDT - Heading - true ===
         *
         * ------------------------------------------------------------------------------
         *        1   2 3
         *        |   | |
         * $--HDT,x.x,T*hh<CR><LF>
         * ------------------------------------------------------------------------------
         *
         * Field Number:
         * 1. Heading degrees, true
         * 2. T = True
         * 3. Checksum
         */

        public HDTPacket(string[] fields) {
            sentenceId = "HDT";
            sentenceName = "Heading - true";
            heading = Helpers.parseFloatSafe(fields[1]);
            
        }
        
        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float heading { get; set; } 
    }
}
