using System;
using System.IO.Ports;
using System.Text;

namespace csNMEA
{
    class SerialReader {
        private string deviceSerialPort = "";
        private int baudRate = 9600;
        private int readTimeout = 5000;
        private bool running = false;
        private bool processUbx = false;
        private bool createCSVfiles = false;

        private SerialDataCallback callback;
        
        // c'tor
        public SerialReader(string port, int baudrate, bool processUbxMessages, bool createfiles, SerialDataCallback sdcallback) {
            deviceSerialPort = port;
            baudRate = baudrate;
            callback = sdcallback;
            processUbx = processUbxMessages;
            createCSVfiles = createfiles;
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

                // loop right here until the port is openvar
                // this will write the desired configuration to the UBLOX device
                using (UbloxConfigurator cfg = new UbloxConfigurator(serialPort, processUbx)){
                    cfg.WriteConfig();
                }

                var hnratt = new HNRATT(createCSVfiles);
                var hnrpvt = new HNRPVT(createCSVfiles);
                ESFINS esf = new ESFINS(createCSVfiles);

                while (running) {
                    var header = new int[2];
                    header[0] = serialPort.ReadByte();
                    header[1] = serialPort.ReadByte(); 
                    if (header[0] == 0x24 && header[1] == 0x47) {  // "$G" is a GPS message
                        var line = serialPort.ReadLine();
                        string msg = "$G" + line;
                        //callback?.Invoke(new SerialData(msg));
                    }
                    else if (header[0] == 0xB5 && header[1] == 0x62) {  // UBX message
                        var clsmid = new byte[2];
                        var mlen = new byte[2];
                        var chksum = new byte[2];
                        
                        serialPort.Read(clsmid, 0, 2);
                        
                        if ((clsmid[0] == 0x05 && clsmid[1] == 0x01)  || 
                            (clsmid[0] == 0x05 && clsmid[1] == 0x00)) {  // ACK-ACK-NAK, continue
                            serialPort.Read(chksum, 0, 2); // read past the 2-byte chksum
                            Console.WriteLine("ACK-ACK or ACK-NAK received!");
                        }
                        else {
                            serialPort.Read(mlen, 0, 2); // get message payload length
                            var len = BitConverter.ToInt16(mlen, 0);
                            var msgdata = new byte[len]; // prepare payload receive buffer 
                            serialPort.Read(msgdata, 0, len); // get the payload
                            serialPort.Read(chksum, 0, 2); // read past the 2-byte chksum
                            
                            // now check class id and message id
                            if (clsmid[0] == 0x28 && clsmid[1] == 0x01) {  // HNR-ATT message
                                hnratt.Write(msgdata);
                            }
                            else if (clsmid[0] == 0x28 && clsmid[1] == 0x00) {  // HNR-PVT message
                                hnrpvt.Write(msgdata);
                            }
                            else if (clsmid[0] == 0x10 && clsmid[1] == 0x015) {  // EFS-INS message
                                esf.Write(msgdata);
                            } 
                        }
                    }
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

