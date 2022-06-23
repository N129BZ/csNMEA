using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === GSV - Satellites in view ===
     *
     * ------------------------------------------------------------------------------
     *         1 2 3  4  5  6  7   8  9  10 11  12 13 14 15  16 17 18 19  20 21
     *         | | |  |  |  |  |   |  |  |  |   |  |  |  |   |  |  |  |   |  |
     *  $--GSA,x,x,xx,xx,xx,xx,xxx,xx,xx,xx,xxx,xx,xx,xx,xxx,xx,xx,xx,xxx,xx*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     *
     * 1. Number of sentences for full data
     * 2. Sentence number out of total
     * 3. Number of satellites in view
     * 4. PRN of satellite used for fix (may be blank)
     *
     * 5. Satellite PRN number     \
     * 6. Elevation, degrees       +- Satellite 1
     * 7. Azimuth, degrees         |
     * 8. Signal to noise ratio    /
     *
     * 9. Satellite PRN number     \
     * 10. Elevation, degrees      +- Satellite 2
     * 11. Azimuth, degrees        |
     * 12. Signal to noise ratio   /
     *
     * 13. Satellite PRN number    \
     * 14. Elevation, degrees      +- Satellite 3
     * 15. Azimuth, degrees        |
     * 16. Signal to noise ratio   /
     *
     * 17. Satellite PRN number    \
     * 18. Elevation, degrees      +- Satellite 4
     * 19. Azimuth, degrees        |
     * 20. Signal to noise ratio   /
     *
     * 21. Checksum
     */

    public class GSVSatellite
    {
        public GSVSatellite(int prn, int ele, int azi, int snr) {
            prnNumber = prn;
            elevationDegrees = ele;
            azimuthTrue = azi;
            SNRdB = snr;
        }

        public int prnNumber { get; set; }
        public int elevationDegrees { get; set; }
        public int azimuthTrue { get; set; }
        public int SNRdB { get; set; }
    }

    public class GSVPacket : Decoder
    {
        public GSVPacket(string[] fields) {
            try {
                sentenceId = "GSV";
                sentenceName = "Satellites in view";
                int numRecords = (fields.Length - 4) / 4;
                List<GSVSatellite> sats = new List<GSVSatellite>();

                for (int i = 0; i < numRecords; i++) {
                    int offset = i * 4 + 4;
                    sats.Add(new GSVSatellite(Helpers.parseIntSafe(fields[offset]),
                                            Helpers.parseIntSafe(fields[offset + 1]),
                                            Helpers.parseIntSafe(fields[offset + 2]),
                                            Helpers.parseIntSafe(fields[offset + 3])));
                
                }
                
                numberOfMessages = Helpers.parseIntSafe(fields[1]);
                messageNumber = Helpers.parseIntSafe(fields[2]);
                satellitesInView = Helpers.parseIntSafe(fields[3]);
                satellites = sats;
            }
            finally {}
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public int numberOfMessages { get; set; }
        public int messageNumber { get; set; }
        public int satellitesInView { get; set; }
        public List<GSVSatellite> satellites { get; set; }
    }
}