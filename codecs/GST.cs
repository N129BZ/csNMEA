using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === GST - GPS pseudorange noise statistics ===
     *
     * ------------------------------------------------------------------------------
     *        1         2   3   4   5   6   7   8    9
     *        |         |   |   |   |   |   |   |    |
     * $--GST,hhmmss.ss,x.x,x.x,x.x,x.x,x.x,x.x,x.x,*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. UTC time of associated GGA fix
     * 2. RMS value of the standard deviation of the range inputs to the navigation
     *    process (range inputs include pseudoranges varror ellipse, meters
     * 4. Standard deviation of semi-minor axis of error ellipse, meters
     * 5. Orientation of semi-major axis of error ellipse, degrees from true north
     * 6. Standard deviation of latitude error, meters
     * 7. Standard deviation of longitude error, meters
     * 8. Standard deviation of altitude error, meters
     * 9. Checksum
     */
    public class GSTPacket : Decoder
    {
        public GSTPacket(string[] fields) {
            sentenceId = "GST";
            sentenceName = "GPS pseudorange noise statistics";
            time = Helpers.parseTime(fields[1], "");
            totalRms = Helpers.parseFloatSafe(fields[2]);
            semiMajorError = Helpers.parseFloatSafe(fields[3]);
            semiMinorError = Helpers.parseFloatSafe(fields[4]);
            orientationOfSemiMajorError = Helpers.parseFloatSafe(fields[5]);
            latitudeError = Helpers.parseFloatSafe(fields[6]);
            longitudeError = Helpers.parseFloatSafe(fields[7]);
            altitudeError = Helpers.parseFloatSafe(fields[8]);
            
        }
        
        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public DateTime time { get; set; }
        public float totalRms { get; set; }
        public float semiMajorError { get; set; }
        public float semiMinorError { get; set; }
        public float orientationOfSemiMajorError { get; set; }
        public float latitudeError { get; set; }
        public float longitudeError { get; set; }
        public float altitudeError { get; set; }
    }
}