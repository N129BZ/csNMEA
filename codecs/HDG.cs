using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === HDG - Heading - deviation and variation ===
     *
     * ------------------------------------------------------------------------------
     *        1   2   3 4   5 6
     *        |   |   | |   | |
     * $--HDG,x.x,x.x,a,x.x,a*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Magnetic Sensor heading in degrees
     * 2. Magnetic Deviation, degrees
     * 3. Magnetic Deviation direction, E = Easterly, W = Westerly
     * 4. Magnetic Variation, degrees
     * 5. Magnetic Variation direction, E = Easterly, W = Westerly
     * 6. Checksum
     */
    public class HDGPacket : Decoder
    {
        public HDGPacket(string[] fields) {
            sentenceId = "HDG";
            sentenceName = "Heading - deviation and variation";
            heading = Helpers.parseFloatSafe(fields[1]);
            deviation = Helpers.parseFloatSafe(fields[2]);
            deviationDirection = fields[3] == "E" ? "E" : fields[3] == "W" ? "W" : "";
            variation = Helpers.parseFloatSafe(fields[4]);
            variationDirection = fields[5] == "E" ? "E" : fields[5] == "W" ? "W" : "";
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public float heading { get; set; }
        public float deviation { get; set; }
        public string deviationDirection { get; set; }
        public float variation { get; set; }
        public string variationDirection { get; set; }
    }
}