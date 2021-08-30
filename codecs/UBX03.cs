using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace csNMEA
{
    /*
     * === UBX03 - Satellite status ===
     */
    public class Satellite {
        public Satellite(int sid,
                         string typ,
                         string stat,
                         int azi,
                         int elev,
                         int sig,
                         int clt) {
            satelliteId = sid;
            satelliteType = typ;
            status = stat;
            azimuth = azi;
            elevation = elev;
            signalStrength = sig;
            carrierLockTime = clt;
        }

        public int satelliteId { get; set; }
        public string satelliteType { get; set; }
        public string status { get; set; }
        public int azimuth { get; set; }
        public int elevation { get; set; }
        public int signalStrength { get; set; }
        public int carrierLockTime { get; set; }
    }

    public class UBX03Packet : Decoder
    {
        public UBX03Packet(string[] fields) {
            sentenceId = "UBX03";
            sentenceName = "Satellite status"; 

            if (int.TryParse(fields[2], out int numsats)) {
                int offset = 3;
                List<Satellite> sats = new List<Satellite>(numsats);
                for (int i = 0; i < numsats; i++) {
                    if(int.TryParse(fields[offset], out int satid)) {
                        sats.Add(new Satellite(
                            satid,
                            getSatelliteType(satid),
                            fields[offset + 1],
                            Helpers.parseIntSafe(fields[offset + 2]),
                            Helpers.parseIntSafe(fields[offset + 3]),
                            Helpers.parseIntSafe(fields[offset + 4]),
                            Helpers.parseIntSafe(fields[offset + 5])));
                    }
                    offset += 6;
                }
                satellites = sats;
            }
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public List<Satellite> satellites { get; set; }

        private string getSatelliteType(int satid) {
            string satstr = "";
            if (satid < 33) {
                satstr = "GPS";
            }
            else if(satid < 65) { // indicates SBAS: WAAS, EGNOS, MSAS, etc.
                satstr = "SBAS";
            }
            else if (satid < 97) { // GLONASS
                satstr = "GLONASS";
            }
            else if (satid >= 120 && satid < 162) { // indicates SBAS: WAAS, EGNOS, MSAS, etc.
                satstr = "SBAS";
            }
            else if (satid > 210) {
                satstr = "GALILEO";
            }
            else {
                satstr = "UNKNOWN";
            }
            return satstr;
        }
    }
}