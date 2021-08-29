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
                    }
                }
            }

            // if no valid args were found, read the csGPSconfig.json file
            if (!hasArgs) {
                try {
                    JObject jo = JObject.Parse(File.ReadAllText("./csGPSconfig.json"));
                    port = (string)jo["csGPSconfig"]["serialport"]["portname"];
                    baudrate = (int)jo["csGPSconfig"]["serialport"]["baudrate"];
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
            SerialReader reader = new SerialReader(port, baudrate, callback);
            Thread readerThread = new Thread(new ThreadStart(reader.Run));

            readerThread.Start();

            // Pressing any key will stop the application
            Console.ReadLine();

            reader.Stop();
            readerThread = null;
            reader = null;
        }

        private static void SerialDataReceived(SerialData data) {
            string sentenceid = "";
            string s = data.fields[0];
            int fl = s.Length;

            switch (fl) {
                case 0:  // Garbage message, get out now
                    return;
                case 8:  // Example: $PUBX00
                    sentenceid = data.fields[0].Substring(3, 5);
                    break;
                case 7:  // Example: $PRDID
                    sentenceid = data.fields[0].Substring(3, 4);
                    break;
                case 6:  // Example: $GXXX (all the rest)
                default:  
                    sentenceid = data.fields[0].Substring(3, 3);
                    break;
            }

            if (sentenceid.Contains("APB")) {
                s = new APBPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("BWC")) { 
                s = new BWCPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("DBT")) { 
                s = new DBTPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("DTM")) { 
                s = new DTMPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("GGA")) { 
                s = new GGAPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("GLL")) { 
                s = new GLLPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("GNS")) {
                s = new GNSPacket(data.fields).getJson(); 
            }
            else if (sentenceid.Contains("GSA")) {
                s = new GSAPacket(data.fields).getJson(); 
            }
            else if (sentenceid.Contains("GST")) {
                s = new GSTPacket(data.fields).getJson(); 
            }
            else if (sentenceid.Contains("GSV")) { 
                s = new GSVPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("HDG")) { 
                s = new HDGPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("HDM")) { 
                s = new HDMPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("HDT")) {
                s = new HDTPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("MTK")) { 
                s = new MTKPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("MWV")) { 
                s = new MWVPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("RDID")) { 
                s = new RDIDPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("RMC")) { 
                s = new RMCPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("VHW")) { 
                s = new VHWPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("VTG")) { 
                s = new VTGPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("ZDA")) { 
                s = new ZDAPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("TXT")) { 
                s = new TXTPacket(data.fields).getJson();
            }
            else if (sentenceid.Contains("UBX00")) { 
                s = new UBX00Packet(data.fields).getJson();
            }
            else if (sentenceid.Contains("UBX03")) { 
                s = new UBX03Packet(data.fields).getJson();
            }
            else if (sentenceid.Contains("UBX04")) { 
                s = new UBX04Packet(data.fields).getJson();
            }

            Console.WriteLine(s);
        }
    }
}