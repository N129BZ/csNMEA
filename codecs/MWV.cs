using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === MWV - Wind speed and angle ===
     *
     * ------------------------------------------------------------------------------
     *        1   2 3   4 5
     *        |   | |   | |
     * $--MWV,x.x,a,x.x,a*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     *
     * 1. Wind Angle, 0 to 360 degrees
     * 2. Reference, R = Relative, T = True
     * 3. Wind Speed
     * 4. Wind Speed Units, K/M/N
     * 5. Status, A = Data Valid
     * 6. Checksum
     */
    public class MWVPacket : Decoder
    {
        public MWVPacket(string[] fields) {
            sentenceId = "MWV";
            sentenceName = "Wind speed and angle";
            windAngle = Helpers.parseFloatSafe(fields[1]);
            reference = fields[2] == "R" ? "relative" : "true";
            speed = Helpers.parseFloatSafe(fields[3]);
            units = fields[4] == "K" ? "K" : fields[4] == "M" ? "M" : "N";
            status = fields[5] == "A" ? "valid" : "invalid";
               
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public float windAngle { get; set; }
        public string reference { get; set; }
        public float speed { get; set; }
        public string units { get; set; }
        public string status { get; set; }
    }
}