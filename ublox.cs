using System;
using System.Diagnostics;
using System.Management;
using System.IO;


namespace csNMEA {

    public static class UBlox 
    {
        public const string UBLOX_5 = "u-blox 5";
        public const string UBLOX_6 = "u-blox 6";
        public const string UBLOX_7 = "u-blox 7";
        public const string UBLOX_8 = "u-blox 8";
        public const string UBLOX_9 = "u-blox 9";
        public const string UBLOX_NONE = "none";
        
        public static string GetUbloxVersion() {
            /*##################################################################
              This method is a Linux specific KLUDGE. NET Core doesn't have 
              any good way of iterating the port device data to get a valid PID,
              so this just uses redirecting the console standard output and parsing 
              the results of a lsusb command. 
             ####################################################################*/
            
            var upid = UBLOX_NONE;
            var process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.FileName = "/usr/bin/lsusb";
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            string strOutput = process.StandardOutput.ReadToEnd();
            string[] list = strOutput.Split(Environment.NewLine);
            foreach(string s in list) {
                if (s.Contains(UBLOX_6)) {
                    upid = UBLOX_6;
                }
                else if (s.Contains(UBLOX_7)) {
                    upid = UBLOX_7;
                } 
                else if (s.Contains(UBLOX_8)) {
                    upid = UBLOX_8;
                }
                else if (s.Contains(UBLOX_9)) {
                    upid = UBLOX_9;
                }
            }
            
            if (upid != UBLOX_NONE) {
                Console.WriteLine($"{upid} detected.");
            }

            return upid;
        }
    }
}