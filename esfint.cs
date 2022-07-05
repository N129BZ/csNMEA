using System;
using System.Runtime.InteropServices;
using System.IO;

namespace csNMEA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct tEFSINS {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 bitField;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public byte[] reserved;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 iTOW;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 xAngRate;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 yAngRate;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 zAngRate;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 xAccel;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 yAccel;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 zAccel;
    }

    public class ESFINS
    {
        private StreamWriter sw;
        private bool createFile = false;

        public ESFINS(bool createfile) {

            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var fpath = "/home/bro/Programming/ESF-INS_" + unixTimestamp.ToString() + ".csv";
            createFile = createfile;
            if (createfile) {
                sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
                sw.WriteLine("xAccel,yAccel,zAccel,xAngRate,yAngRate,zAngRate");
            }
        }
        
        public void Write(byte[] fielddata) {

            var handle = GCHandle.Alloc(fielddata, GCHandleType.Pinned);
            tEFSINS tint = (tEFSINS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(tEFSINS));

            var xa = (tint.xAccel * .0001).ToString("0.###");
            var ya = (tint.yAccel * .0001).ToString("0.###");
            var za = (tint.zAccel * .0001).ToString("0.###");

            var xr = (tint.xAngRate * .001).ToString("0.##");
            var yr = (tint.yAngRate * .001).ToString("0.##");
            var zr = (tint.zAngRate * .001).ToString("0.##");
            if (createFile) {
                sw.WriteLine($"{xa},{ya},{za},{xr},{yr},{zr}");
                sw.Flush();
            }
            Console.WriteLine($"EFS-INT - X AngRate|Accel: {xa}|{xr}, Y AngRate|Accel: {ya}|{yr}, Z AngRate|Accel: {za}|{zr}");
        }
    }
}