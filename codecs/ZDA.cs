using System;
using System.Linq;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === ZDA - Time & Date - UTC, day, month, year and local time zone ===
     *
     * ------------------------------------------------------------------------------
     *	      1         2  3  4    5  6  7
     *        |         |  |  |    |  |  |
     * $--ZDA,hhmmss.ss,dd,mm,yyyy,zz,zz*hh<CR><LF>
     * ------------------------------------------------------------------------------
     *
     * Field Number:
     * 1. UTC time (hours, minutes, seconds, may have fractional subsecond)
     * 2. Day, 01 to 31
     * 3. Month, 01 to 12
     * 4. Year (4 digits)
     * 5. Local zone description, 00 to +- 13 hours
     * 6. Local zone minutes description, 00 to 59, apply same sign as local hours
     * 7. Checksum
     */
    public class ZDAPacket : Decoder
    {
        public ZDAPacket(string[] fields) {
            sentenceId = "ZDA";
            sentenceName = "UTC, day, month, year, and local time zone";
            datetime = Helpers.parseTime(fields[1], ""); //fields.Skip(2).Take(5));
            localZoneHours = Helpers.parseIntSafe(fields[5]);
            localZoneMinutes = Helpers.parseIntSafe(fields[6]);
            
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public DateTime datetime { get; set; }
        public int localZoneHours { get; set; }
        public int localZoneMinutes { get; set; }
    }
}