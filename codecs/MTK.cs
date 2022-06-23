using System;
using System.Linq;
using Newtonsoft.Json;


namespace csNMEA
{
     /*
     * === MTK - Configuration packet ===
     *
     * ------------------------------------------------------------------------------
     *       1   2 ... n n+1
     *       |   |     | |
     * $--MTKxxx,a,...,a*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Packet type (000-999)
     * 2. - n. Data fields; meaning and quantity vary depending on the packet type
     * n+1. Checksum
     */
    public class MTKPacket : Decoder
    {
        public MTKPacket(string[] fields) {
            try {
                sentenceId = "MTK";
                sentenceName = "Configuration packet";
                packetType = Helpers.parseIntSafe(fields[0].Substring(3));
                //data = fields.Take(1).Select(Helpers.parseNumberOrString().ToArray());
            }
            finally {}            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public int packetType { get; set; }
        public string[] data { get; set; }
    }
}