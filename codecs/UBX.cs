namespace csNMEA
{
    // Wrapper class for UBX00, UBX03, UBX04
    public class UBXPacket
    {
        private UBX00Packet packet00;
        private UBX03Packet packet03;
        private UBX04Packet packet04;
        private int pid;

        public UBXPacket(string[] fields) {
            fields[0] += fields[1];
            pid = int.Parse(fields[1]);
            switch (pid) {
                case 0:
                    packet00 = new UBX00Packet(fields);
                    break;
                case 3:
                    packet03 = new UBX03Packet(fields);
                    break;
                case 4:
                    packet04 = new UBX04Packet(fields);
                    break;
            }
        }

        public int PacketId {
            get { return pid; }
        }
        public UBX00Packet UBX00Packet {
            get { return packet00; }
        }
        public UBX03Packet UBX03Packet {
            get { return packet03; }
        }
        public UBX04Packet UBX04Packet {
            get { return packet04; }
        }     
    }
}