using System;
using System.IO;

namespace csNMEA {

    public class HNRPVT {

        private System.IO.StreamWriter sw;
        
        public HNRPVT() {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var fpath = "/home/bro/Programming/UBX-PVT_" + unixTimestamp.ToString() + ".csv";
            sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
            sw.WriteLine("Latitude,Longitude,GPSAlt,MSLAlt,GroundSpeed,3DSpeed,HeadingMotion,VehicleMotion");
        }
        
        ~HNRPVT() {
            sw.Close();
        }

        public void Write(byte[] fielddata) {

            byte[] blat = new byte[4];
            byte[] blon = new byte[4];
            byte[] bhtGPS = new byte[4];
            byte[] bhtMSL = new byte[4];
            byte[] bspGND = new byte[4];
            byte[] bsp3D = new byte[4]; 
            byte[] bmotH = new byte[4];
            byte[] bmotV = new byte[4];
            
            Array.Copy(fielddata, 20, blat, 0, 4);
            Array.Copy(fielddata, 24, blon, 0, 4);
            Array.Copy(fielddata, 28, bhtGPS, 0, 4);
            Array.Copy(fielddata, 32, bhtMSL, 0, 4);
            Array.Copy(fielddata, 36, bspGND, 0, 4);
            Array.Copy(fielddata, 40, bsp3D, 0, 4);
            Array.Copy(fielddata, 44, bmotH, 0, 4);
            Array.Copy(fielddata, 48, bmotV, 0, 4);
            
            var lat = ((BitConverter.ToInt32(blat)) * .0000001).ToString("0.#####"); 
            var lon = ((BitConverter.ToInt32(blon)) * .0000001).ToString("0.#####");;
            var htGPS = ((BitConverter.ToInt32(bhtGPS)) * 0.00328084).ToString("0.##");
            var htMSL = ((BitConverter.ToInt32(bhtMSL)) * 0.00328084).ToString("0.##"); 
            var spGND = ((BitConverter.ToInt32(bspGND)) * 0.00223694).ToString("0.##");
            var sp3D = ((BitConverter.ToInt32(bsp3D)) * 0.00223694).ToString("0.##");
            var motH = ((BitConverter.ToInt32(bmotH)) * .00001).ToString("0.##");
            var motV = ((BitConverter.ToInt32(bmotV)) * .00001).ToString("0.##");
            
            var line = lat + "," + lon + "," + htGPS + "," + htMSL + "," + spGND + "," + sp3D + "," + motH + "," + motV;
            sw.WriteLine(line);
            Console.WriteLine(line);
        }
    }
}
