using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace csNMEA
{

    public class UBX00Packet : Decoder
    {
        public UBX00Packet(string[] fields) {
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

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this);
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