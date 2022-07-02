using System;
using System.IO;

namespace csNMEA {

    public class HNRATT {

        private StreamWriter sw;
        
        public HNRATT() {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var fpath = "/home/bro/Programming/UBX-ATT_" + unixTimestamp.ToString() + ".csv";
            sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
            sw.WriteLine("Roll,Pitch,Heading");
        }

        ~HNRATT() {
            sw.Close();
        }

        public void Write(byte[] fielddata) {
            
            byte[] broll = new byte[4];
            byte[] bpitch = new byte[4];
            byte[] bheading = new byte[4];
            
            Array.Copy(fielddata, 8, broll, 0, 4); 
            Array.Copy(fielddata, 12, bpitch, 0, 4); 
            Array.Copy(fielddata, 16, bheading, 0, 4); 
            
            var roll = ((BitConverter.ToInt32(broll)) * .00001).ToString("0.##");
            var pitch = ((BitConverter.ToInt32(bpitch)) * .00001).ToString("0.##"); 
            var heading = ((BitConverter.ToInt32(bheading)) * .00001).ToString("0.##");
            
            var line = roll + "," + pitch + "," + heading;
            sw.WriteLine(line);
            Console.WriteLine("Roll: {0} Pitch: {1} Heading: {2}", roll, pitch, heading);
        }
    }
}

