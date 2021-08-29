using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === UBX04 - Time of day and clock information ===
     *
     * UBX ID = "04"
     * ------------------------------------------------------------------------------------------------------------
     

    * Field Number:
     *
     * 1. Propietary message identifier: 04
     * 2. UTC Time
     * 3. UTC Date
     * 4. UTC time of week
     * 5. UTC week number, continues beyond 1023
     * 6. Leap seconds
     * 7. Receiver clock bias
     * 8. Receiver clock drift
     * 9. Time pulse granularity
     * 10. Checksum
     */
    public class UBX04Packet : Decoder
    {
        public UBX04Packet(string[] fields) {
            sentenceId = "UBX04";
            sentenceName = "Time of day and clock information";
            utcDateTime = Helpers.parseDatetime(fields[3], fields[2]);
            utcTow = Helpers.parseIntSafe(fields[4]);
            utcWeek = Helpers.parseIntSafe(fields[5]);
            leapSec = Helpers.parseIntSafe(fields[6]);
            clkBias = Helpers.parseIntSafe(fields[7]);
            clkDrift = Helpers.parseIntSafe(fields[8]);
            tpGranularity = Helpers.parseIntSafe(fields[9]);
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public DateTime utcDateTime { get; set; }
        public int utcTow { get; set; }
        public int utcWeek { get; set; }
        public int leapSec { get; set; }
        public int clkBias { get; set; }
        public int clkDrift { get; set; }
        public int tpGranularity { get; set; }
    }
}