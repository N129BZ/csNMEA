/*
	Copyright (c) 2015-2016 Christopher Young
	Distributable under the terms of The "BSD New" License
	that can be found in the LICENSE file, herein included
	as part of this header.

	This .NET Core version was adapted from b3nn0's EU version of Christopher Young's Stratux project at:
    
    https://github.com/b3nn0/stratux

    This C# code is an import of the UBLOX serial device initialization code from the module gps.go 
    
*/

using System;
using System.Diagnostics;
using System.IO.Ports;

namespace csNMEA
{
    public class UBLOX_PIDS 
    {
        public const string UBLOX_5 = "u-blox 5";
        public const string UBLOX_6 = "u-blox 6";
        public const string UBLOX_7 = "u-blox 7";
        public const string UBLOX_8 = "u-blox 8";
        public const string UBLOX_9 = "u-blox 9";
        public const string UBLOX_NONE = "none";
    }

    public class UbloxConfigurator : IDisposable
    {
        private string UBLOX_PID = UBLOX_PIDS.UBLOX_NONE;
        private bool processUbx = false;
        private SerialPort serialPort;
        private bool disposedValue;
        
        public UbloxConfigurator(SerialPort port, bool pubx) {
            serialPort = port; 
            processUbx = pubx;
        }

        public void WriteConfig() {

            getUbloxVersion();
            
            switch (UBLOX_PID) {
                case UBLOX_PIDS.UBLOX_6:
                case UBLOX_PIDS.UBLOX_7:
                    writeUblox7ConfigCommands();
                    break;
                case UBLOX_PIDS.UBLOX_8:
                    writeUblox8ConfigCommands();
                    break;
                case UBLOX_PIDS.UBLOX_9:
                    writeUblox9ConfigCommands();
                    break;
                default:
                    throw(new Exception("Invalid UBLOX pid. Cannot continue."));
            }
            
            writeUbloxGenericConfigCommands(10);
        }

        private void getUbloxVersion() {
            /*##################################################################
              This method is a Linux specific KLUDGE. NET Core doesn't have 
              any good way of iterating the port device data to get a valid PID,
              so this just uses redirecting the console standard output and parsing 
              the results of a lsusb command. 
             ####################################################################*/

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
                if (s.Contains(UBLOX_PIDS.UBLOX_6)) {
                    UBLOX_PID = UBLOX_PIDS.UBLOX_6;
                }
                else if (s.Contains(UBLOX_PIDS.UBLOX_7)) {
                    UBLOX_PID = UBLOX_PIDS.UBLOX_7;
                } 
                else if (s.Contains(UBLOX_PIDS.UBLOX_8)) {
                    UBLOX_PID = UBLOX_PIDS.UBLOX_8;
                }
                else if (s.Contains(UBLOX_PIDS.UBLOX_9)) {
                    UBLOX_PID = UBLOX_PIDS.UBLOX_9;
                }
            }
            
            if (UBLOX_PID != UBLOX_PIDS.UBLOX_NONE) {
                Console.WriteLine($"{UBLOX_PID} detected.");
            }
        }
        
        private void writeUbloxGenericConfigCommands(int navrate) {
            // Turn off "time pulse" (usually drives an LED).
            byte[] tp5 = new byte[32]; 
            tp5[1] = 0x01;  
            tp5[4] = 0x32; 
            tp5[8] = 0x40;
        	tp5[9] = 0x42;
        	tp5[10] = 0x0F;
        	tp5[12] = 0x40;
        	tp5[13] = 0x42;
        	tp5[14] = 0x0F;
            tp5[28] = 0xE7;
            serialPortWrite(makeUBXCFG(0x06, 0x31, 32, tp5));

            // UBX-CFG-NMEA (change NMEA protocol version to 4.0 extended)
        	serialPortWrite(makeUBXCFG(0x06, 0x17, 20, new byte[]{0x00, 0x40, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 
                                                                  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));

            // UBX-CFG-PMS
        	serialPortWrite(makeUBXCFG(0x06, 0x86, 8, new byte[]{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // Full Power Mode
        	// serialPortWrite(makeUBXCFG(0x06, 0x86, 8, new byte[]{0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})) // Balanced Power Mode

        	// UBX-CFG-NAV5                           |mask1...|  dyn
        	serialPortWrite(makeUBXCFG(0x06, 0x24, 36, new byte[]{0x01, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                                                  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                                                  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // Dynamic platform model: airborne with <2g acceleration

        	// UBX-CFG-SBAS (disable integrity, enable auto-scan)
        	serialPortWrite(makeUBXCFG(0x06, 0x16, 8, new byte[]{0x01, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00}));

            // UBX-CFG-MSG (NMEA Standard Messages)  msg   msg   Ports 1-6 (every 10th message over UART1, every message over USB)
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00})); // GGA - Global positioning system fix data
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // GLL - Latitude and longitude, with time of position fix and status
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x02, 0x00, 0x05, 0x00, 0x05, 0x00, 0x00})); // GSA - GNSS DOP and Active Satellites
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x03, 0x00, 0x05, 0x00, 0x05, 0x00, 0x00})); // GSV - GNSS Satellites in View
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x04, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00})); // RMC - Recommended Minimum data
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x05, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00})); // VGT - Course over ground and Ground speed
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // GRS - GNSS Range Residuals
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // GST - GNSS Pseudo Range Error Statistics
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // ZDA - Time and Date<
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // GBS - GNSS Satellite Fault Detection
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // DTM - Datum Reference
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // GNS - GNSS fix data
        	// serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})) // ???
        	serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[] {0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00})); // VLW - Dual ground/water distance

            if (processUbx) {
                // UBX-CFG-MSG (TURN ON NMEA PUBX Messages)      msg   msg   Ports 1-6
    	        //                                                 Class     ID  DDC  UART1 UART2  USB   I2C   Reseved
                serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[]{0xF1, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00})); // UBX00
                serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[]{0xF1, 0x03, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00})); // UBX03
                serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[]{0xF1, 0x04, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00})); // UBX04
            }
            else {
                // UBX-CFG-MSG (TURN OFF NMEA PUBX Messages)      msg   msg   Ports 1-6
    	        //                                                 Class     ID  DDC  UART1 UART2  USB   I2C   Reseved
                serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[]{0xF1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));
                serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[]{0xF1, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));
                serialPortWrite(makeUBXCFG(0x06, 0x01, 8, new byte[]{0xF1, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}));
            }
            
            if (navrate == 10) {
        		serialPortWrite(makeUBXCFG(0x06, 0x08, 6, new byte[] {0x64, 0x00, 0x01, 0x00, 0x01, 0x00})); // 100ms & 1 cycle -> 10Hz (UBX-CFG-RATE payload bytes: little endian!)
        	} else if (navrate == 5) {
        		serialPortWrite(makeUBXCFG(0x06, 0x08, 6, new byte[] {0xC8, 0x00, 0x01, 0x00, 0x01, 0x00})); // 200ms & 1 cycle -> 5Hz (UBX-CFG-RATE payload bytes: little endian!)
        	} else if (navrate == 2) {
        		serialPortWrite(makeUBXCFG(0x06, 0x08, 6, new byte[] {0xF4, 0x01, 0x01, 0x00, 0x01, 0x00})); // 500ms & 1 cycle -> 2Hz (UBX-CFG-RATE payload bytes: little endian!)
        	} else if (navrate == 1) {
        		serialPortWrite(makeUBXCFG(0x06, 0x08, 6, new byte[] {0xE8, 0x03, 0x01, 0x00, 0x01, 0x00})); // 1000ms & 1 cycle -> 1Hz (UBX-CFG-RATE payload bytes: little endian!)
        	}
        }

        private void writeUblox7ConfigCommands() {
            byte[] cfgGnss = new byte[]{0x00, 0x00, 0xFF, 0x04}; // numTrkChUse=0xFF: number of tracking channels to use will be set to number of tracking channels available in hardware
			byte[] gps = new byte[]{0x00, 0x04, 0xFF, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable GPS with 4-255 channels (ublox default)
			byte[] sbas = new byte[]{0x01, 0x01, 0x03, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable SBAS with 1-3 channels (ublox default)
			byte[] qzss = new byte[]{0x05, 0x00, 0x03, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable QZSS with 0-3 channel (ublox default)
			byte[] glonass = new byte[]{0x06, 0x08, 0xFF, 0x00, 0x00, 0x00, 0x01, 0x01}; // disable GLONASS (ublox default)
			cfgGnss = concatTypedArrays(cfgGnss, gps);
			cfgGnss = concatTypedArrays(cfgGnss, sbas);
			cfgGnss = concatTypedArrays(cfgGnss, qzss);
			cfgGnss = concatTypedArrays(cfgGnss, glonass);
			serialPortWrite(makeUBXCFG(0x06, 0x3E, cfgGnss.Length, cfgGnss));
        }

        private void writeUblox8ConfigCommands() {
            byte[] cfgGnss = new byte[]{0x00, 0x00, 0xFF, 0x05}; // numTrkChUse=0xFF: number of tracking channels to use will be set to number of tracking channels available in hardware
        	byte[] gps     = new byte[]{0x00, 0x08, 0x10, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable GPS with 8-16 channels (ublox default)
        	byte[] sbas    = new byte[]{0x01, 0x01, 0x03, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable SBAS with 1-3 channels (ublox default)
        	byte[] galileo = new byte[]{0x02, 0x08, 0x08, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable Galileo with 8-8 channels (ublox default: disabled and 4-8 channels)
        	byte[] beidou  = new byte[]{0x03, 0x08, 0x10, 0x00, 0x00, 0x00, 0x01, 0x01}; // disable BEIDOU
        	byte[] qzss    = new byte[]{0x05, 0x01, 0x03, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable QZSS 1-3 channels, L1C/A (ublox default: 0-3 channels)
        	byte[] glonass = new byte[]{0x06, 0x08, 0x10, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable GLONASS with 8-16 channels (ublox default: 8-14 channels)
        	
        	cfgGnss = concatTypedArrays(cfgGnss, gps);
        	cfgGnss = concatTypedArrays(cfgGnss, sbas);
        	cfgGnss = concatTypedArrays(cfgGnss, beidou);
        	cfgGnss = concatTypedArrays(cfgGnss, qzss);
        	cfgGnss = concatTypedArrays(cfgGnss, glonass);
        	serialPortWrite(makeUBXCFG(0x06, 0x3E, cfgGnss.Length, cfgGnss)); // Succeeds on all chips supporting GPS+GLO

        	cfgGnss[3] = 0x06;
        	cfgGnss = concatTypedArrays(cfgGnss, galileo);
        	serialPortWrite(makeUBXCFG(0x06, 0x3E, cfgGnss.Length, cfgGnss)); // Succeeds only on chips that support GPS+GLO+GAL
        }

        private void writeUblox9ConfigCommands() {
            byte[] cfgGnss = new byte[] {0x00, 0x00, 0xFF, 0x06}; // numTrkChUse=0xFF: number of tracking channels to use will be set to number of tracking channels available in hardware
        	byte[] gps     = new byte[] {0x00, 0x08, 0x10, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable GPS with 8-16 channels (ublox default)
        	byte[] sbas    = new byte[] {0x01, 0x03, 0x03, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable SBAS with 3-3 channels (ublox default)
        	byte[] galileo = new byte[] {0x02, 0x08, 0x10, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable Galileo with 8-16 channels (ublox default: 8-12 channels)
        	byte[] beidou  = new byte[] {0x03, 0x08, 0x10, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable BEIDOU with 8-16 channels (ublox default: 2-5 channels)
        	byte[] qzss    = new byte[] {0x05, 0x03, 0x04, 0x00, 0x01, 0x00, 0x05, 0x01}; // enable QZSS 3-4 channels, L1C/A & L1S (ublox default)
        	byte[] glonass = new byte[] {0x06, 0x08, 0x10, 0x00, 0x01, 0x00, 0x01, 0x01}; // enable GLONASS with 8-16 tracking channels (ublox default: 8-12 channels)
        	
        	cfgGnss = concatTypedArrays(cfgGnss, gps);
        	cfgGnss = concatTypedArrays(cfgGnss, sbas);
        	cfgGnss = concatTypedArrays(cfgGnss, beidou);
        	cfgGnss = concatTypedArrays(cfgGnss, qzss);
        	cfgGnss = concatTypedArrays(cfgGnss, glonass);
        	cfgGnss = concatTypedArrays(cfgGnss, galileo);
        	serialPortWrite(makeUBXCFG(0x06, 0x3E, cfgGnss.Length, cfgGnss));
        }

        private void reconfigureSerialPort() {
            // Reconfigure serial port.
    		byte[] cfg = new byte[20];
    		cfg[0] = 0x01; // portID.
    		cfg[1] = 0x00; // res0.
    		cfg[2] = 0x00; // res1.
    		cfg[3] = 0x00; // res1.

    			
    		// [   7   ] [   6   ] [   5   ] [   4   ]
    		// 0000 0000 0000 0000 0000 1000 1100 0000
    		// UART mode. 0 stop bits, no parity, 8 data bits. Little endian order.
    		cfg[4] = 0xC0;
    		cfg[5] = 0x08;
    		cfg[6] = 0x00;
    		cfg[7] = 0x00;

    		// Baud rate. Little endian order.
    		UInt32 bdrt = 115200;
    		cfg[11] = (byte)((bdrt >> 24) & 0xFF);   // = 0x00
    		cfg[10] = (byte)((bdrt >> 16) & 0xFF);   // = 0x01
    		cfg[9] = (byte)((bdrt >> 8) & 0xFF);     // = 0xC2
    		cfg[8] = (byte)(bdrt & 0xFF);            // = 0x00

    		// inProtoMask. NMEA and UBX. Little endian.
    		cfg[12] = 0x03;
    		cfg[13] = 0x00;

    		// outProtoMask. NMEA. Little endian.
    		cfg[14] = 0x02;
    		cfg[15] = 0x00;
        }

    	private byte[] concatTypedArrays(byte[] a, byte[] b) {
            byte[] newarray = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, newarray, 0, a.Length);
            Buffer.BlockCopy(b, 0, newarray, a.Length, b.Length);
            return newarray;
        }

        /*
        	makeUBXCFG()
        		creates a UBX-formatted package consisting of two sync characters,
        		class, ID, payload length in bytes (2-byte little endian), payload, and checksum.
        		See p. 95 of the u-blox M8 Receiver Description.
        */
       private byte[] makeUBXCFG(byte cls, byte id, int msglen, byte[] msg) {
        	byte[] retA = new byte[6];
            UInt16 mlen = (UInt16)msglen;
        	retA[0] = 0xB5;
        	retA[1] = 0x62;
        	retA[2] = cls;
        	retA[3] = id;
        	retA[4] = (byte)(mlen & 0xFF);
        	retA[5] = (byte)((mlen >> 8) & 0xFF);
            byte[] retC = concatTypedArrays(retA, msg);
        	byte[] chk = chksumUBX(retC, 2);
        	return concatTypedArrays(retC, chk);
        }

       private byte[] chksumUBX(byte[] msg, int startIndex) {
        	int a = 0;  int b = 0;
            byte[] chk = new byte[2]; 
        	for (var i = startIndex; i < msg.Length; i++) {
        		a += msg[i];
        		b += a;
        	}
            chk[0] = (byte)(a & 0xFF);
            chk[1] = (byte)(b & 0xFF);
            string s1 = $"{chk[0]:X}";
            string s2 = $"{chk[1]:X}";
            //Console.WriteLine(s1 + " " + s2);
        	return chk;
        }
   
    	private void serialPortWrite(byte[] msg) {
            serialPort.Write(msg, 0, msg.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                //if (disposing)
                //{
                //}

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}