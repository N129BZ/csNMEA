using System;
using System.IO;
using System.Runtime.InteropServices;

namespace csNMEA {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct tHNRPVT {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 iTOW;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 year;

        [MarshalAs(UnmanagedType.U1)]
        public byte month;

        [MarshalAs(UnmanagedType.U1)]
        public byte day;

        [MarshalAs(UnmanagedType.U1)]
        public byte hour;

        [MarshalAs(UnmanagedType.U1)]
        public byte minute;

        [MarshalAs(UnmanagedType.U1)]
        public byte sec;

        [MarshalAs(UnmanagedType.I1)]
        public byte valid;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 nano;

        [MarshalAs(UnmanagedType.U1)]
        public byte gpsFix;

        [MarshalAs(UnmanagedType.U1)]
        public byte flags;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved1;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 lon;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 lat;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 htGPS;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 htMSL;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 speedGnd;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 speed3D;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 headMot;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 headVeh;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 hAcc;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 vAcc;

        [MarshalAs(UnmanagedType.U4)]
        public Int32 sAcc;

        [MarshalAs(UnmanagedType.U4)]
        public Int32 headAcc;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] reserved2;
    }

    public class HNRPVT {

        private StreamWriter sw;
        private bool createFile = false;

        public HNRPVT(bool createfile) {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var fpath = "/home/bro/Programming/UBX-PVT_" + unixTimestamp.ToString() + ".csv";
            createFile = createfile;
            if (createFile) {
                sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
                sw.WriteLine("lon,lat,altGPS,altMSL,speedGND,speed3D,motHead,motVeh");
            }
        }
        
        public void Write(byte[] fielddata) {

            var handle = GCHandle.Alloc(fielddata, GCHandleType.Pinned);
            tHNRPVT tpvt = (tHNRPVT)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(tHNRPVT));

            var lon = (tpvt.lon * .0000001).ToString("0.#####"); 
            var lat = (tpvt.lat * .0000001).ToString("0.#####");;
            var altGPS = (tpvt.htGPS * 0.00328084).ToString("0.##");
            var altMSL = (tpvt.htMSL * 0.00328084).ToString("0.##"); 
            var spGND = (tpvt.speedGnd * 0.00223694).ToString("0.##");
            var sp3D = (tpvt.speed3D * 0.00223694).ToString("0.##");
            var motH = (tpvt.headMot * .00001).ToString("0.##");
            var motV = (tpvt.headVeh * .00001).ToString("0.##");
            
            if (createFile) {
                sw.WriteLine($"{lon},{lat},{altGPS},{altMSL},{spGND},{sp3D},{motH},{motV}");
                sw.Flush();
            }
            Console.WriteLine($"HNR-PVT - {lon},{lat},{altGPS},{altMSL},{spGND},{sp3D},{motH},{motV}");
        }
    }
}
