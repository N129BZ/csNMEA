using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === VHW – Water speed and heading ===
     *
     * ------------------------------------------------------------------------------
     *        1   2 3   4 5   6 7   8 9
     *        |   | |   | |   | |   | |
     * $--VHW,x.x,T,x.x,M,x.x,N,x.x,K*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. Degress True
     * 2. T = True
     * 3. Degrees Magnetic
     * 4. M = Magnetic
     * 5. Knots (speed of vessel relative to the water)
     * 6. N = Knots
     * 7. Kilometers (speed of vessel relative to the water)
     * 8. K = Kilometers
     * 9. Checksum
     */
    public class VHWPacket : Decoder
    {
        public VHWPacket(string[] fields) {
            sentenceId = "VHW";
            sentenceName = "Water speed and heading";
            degreesTrue = Helpers.parseFloatSafe(fields[1]);
            degreesMagnetic = Helpers.parseFloatSafe(fields[3]);
            speedKnots = Helpers.parseFloatSafe(fields[5]);
            speedKmph = Helpers.parseFloatSafe(fields[7]);
               
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float degreesTrue { get; set; }
        public float degreesMagnetic { get; set; }
        public float speedKnots { get; set; }
        public float speedKmph { get; set; }
    }
}