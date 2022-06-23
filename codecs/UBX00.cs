using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === UBX00 -  Lat/Long position data ===
     *
     * -------------------------------
     *        1   2          3          4  5           6  7  8   9  10 11 12 13 14 15 16 17 18 19 20 21
     *        |   |          |          |  |           |  |  |   |  |  |  |  |  |  |  |  |  |  |  |  |
     *  $PUBX,00, hhmmss.ss, ddmm.mmmm, c, dddmm.mmmm, c, x, cc, x, x, x, x, x, x, x, x, x, x, x, x, *hh<CR><LF>
     * -------------------------------
     *
     * 1. Propietary message identifier: 00
     * 2. UTC Time, Current time
     * 3. Latitude, Degrees + minutes
     * 4. N/S Indicator, N=north or S=south
     * 5. Longitude, Degrees + minutes
     * 6. E/W Indicator, E=east or W=west
     * 7. Altitude above user datum ellipsoid
     * 8. Navigation Status, See Table below
     * 9. Horizontal accuracy estimate
     * 10. Vertical accuracy estimate
     * 11. SOG, Speed over ground
     * 12. COG, Course over ground
     * 13. Vertical velocity, positive=downwards
     * 14. Age of most recent DGPS corrections, empty = none available
     * 15. HDOP, Horizontal Dilution of Precision
     * 16. VDOP, Vertical Dilution of Precision
     * 17. TDOP, Time Dilution of Precision
     * 18. Number of GPS satellites used in the navigation solution
     * 19. Number of GLONASS satellites used in the navigation solution
     * 20. DR used
     * 21. Checksum
     */
    public class UBX00Packet : Decoder
    {
        public UBX00Packet(string[] fields) {
            try {
                sentenceId = "UBX00";
                sentenceName = "Lat/Long position data";
                utcTime = fields[2];
                latitude = Helpers.parseFloatSafe(fields[3]);
                nsIndicator = fields[4];
                longitude = Helpers.parseFloatSafe(fields[5]);
                ewIndicator = fields[6];
                altRef = Helpers.parseFloatSafe(fields[7]);
                navStatus = fields[8];
                hAccuracy = Helpers.parseFloatSafe(fields[9]);
                vAccuracy = Helpers.parseFloatSafe(fields[10]);
                speedOverGround = Helpers.parseFloatSafe(fields[11]);
                courseOverGround = Helpers.parseFloatSafe(fields[12]);
                vVelocity = Helpers.parseFloatSafe(fields[13]);
                ageCorrections = Helpers.parseFloatSafe(fields[14]);
                hdop = Helpers.parseFloatSafe(fields[15]);
                vdop = Helpers.parseFloatSafe(fields[16]);
                tdop = Helpers.parseFloatSafe(fields[17]);
                gpsSatellites = Helpers.parseFloatSafe(fields[18]);
                glonassSatellites = Helpers.parseFloatSafe(fields[19]);
                drUsed = Helpers.parseFloatSafe(fields[20]);
            }
            finally {}
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public string utcTime { get; set; }
        public float latitude { get; set; }
        public string nsIndicator { get; set; }
        public float longitude { get; set; }
        public string ewIndicator { get; set; }
        public float altRef { get; set; }
        public string navStatus { get; set; }
        public float hAccuracy { get; set; }
        public float vAccuracy { get; set; }
        public float speedOverGround { get; set; }
        public float courseOverGround { get; set; }
        public float vVelocity { get; set; }
        public float ageCorrections { get; set; }
        public float hdop { get; set; }
        public float vdop { get; set; }
        public float tdop { get; set; }
        public float gpsSatellites { get; set; }
        public float glonassSatellites { get; set; }
        public float drUsed { get; set; }
    }
}