using System;
using System.Runtime.InteropServices;
using System.IO;

namespace csNMEA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct tEFSINS {

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 bitfield;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
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
        private bool writeFile = false;

        public ESFINS(bool writefile) {

            var fpath = $"{Program.CSVFilePath}/ESF-INS.csv";
            var exists = File.Exists(fpath);
            writeFile = writefile;
            if (writefile) {
                sw = new System.IO.StreamWriter(new System.IO.FileStream(fpath, FileMode.OpenOrCreate));
                if (!exists) {
                    sw.WriteLine("timestamp,xAccel,yAccel,zAccel,xAngRate,yAngRate,zAngRate");
                }
            }
        }
        
        public async void WriteAsync(byte[] fielddata) {
            var handle = GCHandle.Alloc(fielddata, GCHandleType.Pinned);
            tEFSINS tins = (tEFSINS)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(tEFSINS));
            
            var ts = Program.GetTimestamp();
            var bf = Convert.ToString(tins.bitfield, 2);
            var xa = (tins.xAccel * .0001).ToString("0.###");
            var ya = (tins.yAccel * .0001).ToString("0.###");
            var za = (tins.zAccel * .0001).ToString("0.###");

            var xr = (tins.xAngRate * .001).ToString("0.##");
            var yr = (tins.yAngRate * .001).ToString("0.##");
            var zr = (tins.zAngRate * .001).ToString("0.##");
            if (writeFile) {
                await sw.WriteLineAsync($"{ts},{bf},{xa},{ya},{za},{xr},{yr},{zr}");
                await sw.FlushAsync();
            }
            else {
                Console.WriteLine($"EFS-INS {ts} - Bitfield: {bf}, X AngRate|Accel: {xa}|{xr}, Y AngRate|Accel: {ya}|{yr}, Z AngRate|Accel: {za}|{zr}");
            }
        }
    }
}