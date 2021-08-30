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
using System.IO.Ports;
using System.Diagnostics;


namespace csNMEA
{
    class SerialReader {
        private string deviceSerialPort = "";
        private int baudRate = 9600;
        private int readTimeout = 5000;
        private bool running = false;
        private bool processUbx = false;
        
        private SerialDataCallback callback;
        
        // c'tor
        public SerialReader(string port, int baudrate, bool processUbxMessages, SerialDataCallback sdcallback) {
            deviceSerialPort = port;
            baudRate = baudrate;
            callback = sdcallback;
            processUbx = processUbxMessages;
        }
        
        public void Stop() {
            running = false;
        }
    		
        public void Run() {
            SerialPort serialPort = new SerialPort();
            running = true;
            bool portFound = false;
            
            String[] PortNames = SerialPort.GetPortNames();
            
            foreach (string s in PortNames) {
                if (s == deviceSerialPort) {
                    portFound = true;
                    break;
                }
            }
            
            if (portFound) {
                serialPort.PortName = deviceSerialPort;
                serialPort.BaudRate = baudRate;
                serialPort.Parity = Parity.None;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.ReadTimeout = readTimeout;
                serialPort.NewLine = "\r\n";
                try {
                    serialPort.Open();   
                }
                catch (System.IO.IOException ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Exiting program, try again after the port is fully initialized by the OS.");
                    running = false;
                    return;
                } 

                // loop right here until the port is open
                while (!serialPort.IsOpen) { }
                Console.WriteLine("Port " + deviceSerialPort + " opened at " + baudRate + " baud");
                
                // this will write the desired configuration to the UBLOX device
                using (UbloxConfigurator cfg = new UbloxConfigurator(serialPort, processUbx)){
                    cfg.WriteConfig();
                }

                while (running) {
                    callback?.Invoke(new SerialData(serialPort.ReadLine()));
                }
            }
        }
    }

    public class SerialData {
        
        public SerialData(string sentence)
        {
            string s = new string(sentence);
            
            if (!s.StartsWith("$")) {
                fields = new string[] {""};
                sentenceId = "";
            }
            else {
                // trim the asterisk and the 2-digit checksum
                fields = s.Substring(0, sentence.Length - 3).Split(",");
                sentenceId = fields[0];
            }
        }

        public bool isValid { 
            get {
                return sentenceId.Length > 1;
            }
        }
        public string sentenceId { get; set; }
        public string[] fields { get; set; }
    }   
}

