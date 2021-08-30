using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;


namespace csNMEA
{  
    public delegate void SerialDataCallback(SerialData data);

    class Program 
    {
        public static void Main(string[] args) 
        {
            int baudrate = 0; 
            string port = ""; 
            bool processUbx = false;

            bool hasArgs = false; // used to test for either args or csGPSconfig file

            if (args.Length > 0) {  
                // in Linux even if no args were passed, there will be a "csNMEA" arg :/
                // so we'll iterate and check for valid arg(s)
                foreach (string s in args) {
                    if (s.Length > 0 ) {
                        if (int.TryParse(s, out int testint)) {
                            baudrate = testint;
                            hasArgs = true;
                        }
                        else if (s.Contains("/dev")) {
                            port = s;
                            hasArgs = true;
                        }
                        else if (bool.TryParse(s, out bool b)) {
                            processUbx = b;
                        }
                    }
                }
            }

            // if no valid args were found, read the csGPSconfig.json file
            if (!hasArgs) {
                try {
                    JObject jo = JObject.Parse(File.ReadAllText("./csNMEA.json"));
                    port = (string)jo["csNMEA"]["serialport"]["portname"];
                    baudrate = (int)jo["csNMEA"]["serialport"]["baudrate"];
                    processUbx = (bool)jo["csNMEA"]["processOptions"]["ubxMessages"];
                }
                catch (System.IO.FileNotFoundException) {
                    Console.WriteLine("No csGPSconfig.json file and no args! Exiting.");
                    return;   
                }
                catch (Newtonsoft.Json.JsonReaderException) {
                    Console.WriteLine("Bad value(s) in csGPSconfig.json. Exiting.");
                    return;
                }
            }
            
            SerialDataCallback callback = new SerialDataCallback(SerialDataReceived);
            SerialReader reader = new SerialReader(port, baudrate, processUbx, callback);
            Thread readerThread = new Thread(new ThreadStart(reader.Run));

            readerThread.Start();

            // Pressing any key will stop the application
            Console.ReadLine();

            reader.Stop();
            readerThread = null;
            reader = null;
        }

        private static void SerialDataReceived(SerialData data) {
            string json = "";
            
            if (!data.isValid) return;

            if (data.sentenceId.Contains("APB")) {
                json = new APBPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("BWC")) { 
                json = new BWCPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("DBT")) { 
                json = new DBTPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("DTM")) { 
                json = new DTMPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("GGA")) { 
                json = new GGAPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("GLL")) { 
                json = new GLLPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("GNS")) {
                json = new GNSPacket(data.fields).getJson(); 
            }
            else if (data.sentenceId.Contains("GSA")) {
                json = new GSAPacket(data.fields).getJson(); 
            }
            else if (data.sentenceId.Contains("GST")) {
                json = new GSTPacket(data.fields).getJson(); 
            }
            else if (data.sentenceId.Contains("GSV")) { 
                json = new GSVPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("HDG")) { 
                json = new HDGPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("HDM")) { 
                json = new HDMPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("HDT")) {
                json = new HDTPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("MTK")) { 
                json = new MTKPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("MWV")) { 
                json = new MWVPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("RDID")) { 
                json = new RDIDPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("RMC")) { 
                json = new RMCPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("VHW")) { 
                json = new VHWPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("VTG")) { 
                json = new VTGPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("ZDA")) { 
                json = new ZDAPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("TXT")) { 
                json = new TXTPacket(data.fields).getJson();
            }
            else if (data.sentenceId.Contains("UBX")) { 
                UBXPacket p = new UBXPacket(data.fields);
                switch (p.PacketId) {
                    case 0:
                        json = p.UBX00Packet.getJson();
                        break;
                    case 3:
                        json = p.UBX03Packet.getJson();
                        break;
                    case 4:
                        json = p.UBX04Packet.getJson();
                        break;
                    default:
                        return;
                }
            }

            Console.WriteLine(json);
        }
    }
}