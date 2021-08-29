namespace csNMEA
{
    /*
     * === UBX00 -  Lat/Long position data ===
     *
     * -------------------------------
     *        1   2          3          4  5           6  7  8   9  10 11 12 13 14 15 16 17 18 19 20 21
     *        |   |          |          |  |           |  |  |   |  |  |  |  |  |  |  |  |  |  |  |  |
     *  $PUBX,00, hhmmss.ss, ddmm.mmmm, c, dddmm.mmmm, c, x, cc, x, x, x, x, x, x, x, x, x, x, x, x, *hh<CR><LF>
     * -------------------------------
     *
     * 1. Propietary message identifier: 00
     * 2. UTC Time, Current time
     * 3. Latitude, Degrees + minutes
     * 4. N/S Indicator, N=north or S=south
     * 5. Longitude, Degrees + minutes
     * 6. E/W Indicator, E=east or W=west
     * 7. Altitude above user datum ellipsoid
     * 8. Navigation Status, See Table below
     * 9. Horizontal accuracy estimate
     * 10. Vertical accuracy estimate
     * 11. SOG, Speed over ground
     * 12. COG, Course over ground
     * 13. Vertical velocity, positive=downwards
     * 14. Age of most recent DGPS corrections, empty = none available
     * 15. HDOP, Horizontal Dilution of Precision
     * 16. VDOP, Vertical Dilution of Precision
     * 17. TDOP, Time Dilution of Precision
     * 18. Number of GPS satellites used in the navigation solution
     * 19. Number of GLONASS satellites used in the navigation solution
     * 20. DR used
     * 21. Checksum
     */

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