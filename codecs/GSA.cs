using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === GSA - Active satellites and dilution of precision ===
     *
     * ------------------------------------------------------------------------------
     *         1 2 3  4  5  6  7  8  9  10 11 12 13 14 15  16  17  18
     *         | | |  |  |  |  |  |  |  |  |  |  |  |  |   |   |   |
     *  $--GSA,a,x,xx,xx,xx,xx,xx,xx,xx,xx,xx,xx,xx,xx,x.x,x.x,x.x*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     *
     * 1. Selection of 2D or 3D fix
     *    A - Automatic
     *    M - Manual, forced to operate in 2D or 3D
     * 2. 3D fix
     *    1 - no fix
     *    2 - 2D fix
     *    3 - 3D fix
     * 3. PRN of satellite used for fix (may be blank)
     * ...
     * 14. PRN of satellite used for fix (may be blank)
     * 15. Dilution of precision
     * 16. Horizontal dilution of precision
     * 17. Vertical dilution of precision
     * 18. Checksum
     */

    public class GSAPacket : Decoder
    { 
        string[] ThreeDFixTypes = new string[] { "unknown", "none", "2D", "3D" };

        public GSAPacket(string[] fields) {
            List<string> sats = new List<string>();
            for (int i = 3; i < 15; i++) {
                if (fields[i].Length > 0) {
                    sats.Add(fields[i]);
                }
            }
            sentenceId = "GSA";
            sentenceName = "Active satellites and dilution of precision";
            selectionMode = fields[1] == "A" ? "automatic" : "manual";
            fixMode = ThreeDFixTypes[Helpers.parseIntSafe(fields[2])];
            satellites = sats.ToArray();
            PDOP = Helpers.parseFloatSafe(fields[15]);
            HDOP = Helpers.parseFloatSafe(fields[16]);
            VDOP = Helpers.parseFloatSafe(fields[17]);
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public string selectionMode { get; set; }
        public string fixMode { get; set; }
        public string[] satellites { get; set; }
        public float PDOP { get; set; }
        public float HDOP { get; set; }
        public float VDOP { get; set; }    
    }
}