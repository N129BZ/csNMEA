using System;
using System.IO;
using System.Runtime.InteropServices;

namespace csNMEA {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct tHNRATT {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 iTOW;

        [MarshalAs(UnmanagedType.I1)]
        public byte version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=3)]
        public byte[] reserved;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 roll;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 pitch;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 heading;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 accRoll;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 accPitch;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 accHeading;
    }

    public class HNRATT {

        private StreamWriter sw;
        private bool createFile = false;

        public HNRATT(bool createfile) {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var fpath = "/home/bro/Programming/UBX-ATT_" + unixTimestamp.ToString() + ".csv";
            createFile = createfile;
            if (createfile) {
                sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
                sw.WriteLine("roll,pitch,heading,accRoll,accPitch,accHeading");
            }
        }

        public void Write(byte[] fielddata) {
            
            var handle = GCHandle.Alloc(fielddata, GCHandleType.Pinned);
            tHNRATT tatt = (tHNRATT)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(tHNRATT));

            var r = (tatt.roll * .00001).ToString("0.##");
            var p = (tatt.pitch * .00001).ToString("0.##"); 
            var h = (tatt.heading * .00001).ToString("0.##");
            
            var ra = (tatt.accRoll * .00001).ToString("0.##");
            var pa = (tatt.accPitch * .00001).ToString("0.##"); 
            var ha = (tatt.accHeading * .00001).ToString("0.##");

            if (createFile) {
                sw.WriteLine($"{r},{p},{h},{ra},{pa},{ha}");
                sw.Flush();
            }
            Console.WriteLine($"HNR-ATT - Roll|Accuracy: {r}|{ra} Pitch|Accuracy: {p}|{pa} Heading|Accuracy: {h}|{ha}");
        }
    }
}

