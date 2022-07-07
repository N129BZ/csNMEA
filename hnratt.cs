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
        private bool writeFile = false;

        public HNRATT(bool writefile) {
            var fpath = $"{Program.CSVFilePath}/UBX-ATT.csv";
            var existed = File.Exists(fpath);
            writeFile = writefile;
            if (writefile) {
                sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
                if (!existed)
                {
                    sw.WriteLine("timestamp,roll,pitch,heading,accRoll,accPitch,accHeading");
                }
            }
        }

        public async void WriteAsync(byte[] fielddata) {
            
            var handle = GCHandle.Alloc(fielddata, GCHandleType.Pinned);
            tHNRATT tatt = (tHNRATT)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(tHNRATT));
            
            var ts = Program.GetTimestamp();
            var r = (tatt.roll * .00001).ToString("0.##");
            var p = (tatt.pitch * .00001).ToString("0.##"); 
            var h = (tatt.heading * .00001).ToString("0.##");
            
            var ra = (tatt.accRoll * .00001).ToString("0.##");
            var pa = (tatt.accPitch * .00001).ToString("0.##"); 
            var ha = (tatt.accHeading * .00001).ToString("0.##");

            if (writeFile) {
                await sw.WriteLineAsync($"{ts},{r},{p},{h},{ra},{pa},{ha}");
                await sw.FlushAsync();
            }
            else {
                Console.WriteLine($"HNR-ATT {ts} - Roll|Accuracy: {r}|{ra} Pitch|Accuracy: {p}|{pa} Heading|Accuracy: {h}|{ha}");
            }
        }
    }
}

