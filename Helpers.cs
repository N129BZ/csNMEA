// Copied from from https://github.com/nherment/node-nmea/blob/master/lib/Helper.js

using System;

namespace csNMEA 
{ 
    public static class Helpers 
    {
        public static string[] m_hex = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"};

        public static string toHexString(ushort v) {
            byte msn = (byte)((v >> 4) & 0x0f);
            byte lsn = (byte)((v >> 0) & 0x0f);

            return m_hex[msn] + m_hex[lsn];
        }

        public static string padLeft(string value, int length, string paddingCharacter) {
            string result = value;
            while (result.Length < length) {
                result = paddingCharacter + result;
            }

            return result;
        }

        public static string padLeft(int value, int length, string paddingCharacter) {
            string result = value.ToString();
            return padLeft(result, length, paddingCharacter);
        }

        // =========================================
        // checksum related functions
        // =========================================

        /**
         * Checks that the given NMEA sentence has a valid checksum.
         */
        public static bool validNmeaChecksum(string nmeaSentence) {
            string[] a = nmeaSentence.Split("*");
            string sentenceWithoutChecksum = a[0], checksumString = a[1];
            ushort correctChecksum = computeNmeaChecksum(sentenceWithoutChecksum); //sentenceWithoutChecksum);

            // checksum is a 2 digit hex value
            ushort actualChecksum = ushort.Parse(a[1]);

            return (correctChecksum == actualChecksum);
        }

        /**
         * Generate a checksum for an NMEA sentence without the trailing "*xx".
         */
       public static ushort computeNmeaChecksum(string sentenceWithoutChecksum) {
            // init to first character value after the $
            char[] testchars = sentenceWithoutChecksum.ToCharArray();
            byte checksum = (byte)testchars[1]; 

            // process rest of characters, zero delimited
            for (int i = 2; i < testchars.Length; i += 1) {
                checksum = (byte)(checksum ^ (byte)testchars[i]);
            }

            // checksum is between 0x00 and 0xff
            checksum = (byte)(checksum & 0xff);

            return checksum;
        }

        /**
         * Generate the correct trailing "*xx" footer for an NMEA sentence.
         */
        public static string createNmeaChecksumFooter(string sentenceWithoutChecksum) {
            return "*" + toHexString(computeNmeaChecksum(sentenceWithoutChecksum));
        }

        // =========================================
        // field encoders
        // =========================================

        public static string encodeFixed(float value, int decimalPlaces) {
            string fmt = "{0:0.";
            for (int i = 0; i < decimalPlaces; i++) {
                fmt += "0";
            }
            fmt += "}"; 
            return string.Format(fmt, value);
        }

        /**
         * Encodes the latitude in the standard NMEA format "ddmm.mmmmmm".
         *
         * @param latitude Latitude in decimal degrees.
         */
        public static string encodeLatitude(float latitude) {
            string hemisphere = "";
            if (latitude < 0) {
                hemisphere = "S";
                latitude = -latitude;
            } else {
                hemisphere = "N";
            }

            // get integer degrees
            double x = (double)latitude;
            int d = (int)Math.Floor(x);
            // latitude degrees are always 2 digits
            string s = padLeft(d, 2, "0");

            // get fractional degrees
            float f = latitude - d;
            // convert to fractional minutes
            float m = (float)(f * 60.0);
            // format the fixed point fractional minutes "mm.mmmmmm"
            string t = padLeft(string.Format("{0:000000}", m), 9, "0");

            s = s + t + "," + hemisphere;
            return s;
        }

        /**
         * Encodes the longitude in the standard NMEA format "dddmm.mmmmmm".
         *
         * @param longitude Longitude in decimal degrees.
         */
        public static string encodeLongitude(float longitude) {
            string hemisphere = "";
            if (longitude < 0) {
                hemisphere = "W";
                longitude = -longitude;
            } else {
                hemisphere = "E";
            }

            // get integer degrees
            double x = (double)longitude;
            int d = (int)Math.Floor(x);
            // longitude degrees are always 3 digits
            string s = padLeft(d, 3, "0");

            // get fractional degrees
            float f = longitude - d;
            // convert to fractional minutes and round up to the specified precision
            float m = (float)(f * 60.0);
            // format the fixed point fractional minutes "mm.mmmmmm"
            string t = padLeft(string.Format("{0:000000}", m), 9, "0");

            s = s + t + "," + hemisphere;
            return s;
        }

        // 1 decimal, always meters
        public static string encodeAltitude(float alt) {
            return string.Format("{0:0.0}", alt) + ",M";
        }

        // Some encodings don't want the unit
        public static string encodeAltitudeNoUnits(float alt) {
            return string.Format("{0:0.0}", alt);
        }

        // 1 decimal, always meters
        public static string encodeGeoidalSeperation(float geoidalSep) {
            return string.Format("{0:0.0}", geoidalSep) + ",M";
        }

        // Some encodings don't want the unit
        public static string encodeGeoidalSeperationNoUnits(float geoidalSep) {
            return string.Format("{0:0.0}", geoidalSep);
        }

        // degrees
        public static string encodeDegrees(float degrees) {
            return padLeft(string.Format("{0:0.00}", degrees), 6, "0");
        }

        public static string encodeDate(DateTime date) {
            int yr = date.Year;
           
            if (yr < 73) {
                yr += 2000;
            }
            else {
                yr += 1900;
            }
            string year = yr.ToString(); //date.ToUniversalTime().Year.ToString();  
            string month = date.ToUniversalTime().Month.ToString();  
            string day = date.ToUniversalTime().Day.ToString();  

            return padLeft(day, 2, "0") + padLeft(month, 2, "0") + year.ToString().Substring(2);
        }

        public static string encodeTime(DateTime date) {
            string hours = date.ToUniversalTime().Hour.ToString(); 
            string minutes = date.ToUniversalTime().Minute.ToString();
            string seconds = date.ToUniversalTime().Second.ToString();  

            return padLeft(hours, 2, "0") + padLeft(minutes, 2, "0") + padLeft(seconds, 2, "0");
        }

        public static string encodeValue(object value) {
            if (value == null) {
                return "";
            }

            return value.ToString();
        }

        // =========================================
        // field traditionalDecoders
        // =========================================

        /**
         * Parse the given string to a float, returning 0 for an empty string.
         */
        public static float parseFloatSafe(string str) {
            if (float.TryParse(str, out float x)) {
                return x;
            }
            return 0.0f;
        }

        /**
         * Parse the given string to a integer, returning 0 for an empty string.
         */
        public static int parseIntSafe(string i) {
            if (int.TryParse(i, out int z)) {
                return z;
            }
            return 0;
        }

        /**
         * Parse the given string to a float if possible, returning 0 for an undefined
         * value and a string the the given string cannot be parsed.
         */
        public static string parseNumberOrString(string str) {
            return float.TryParse(str, out float x) ? x.ToString() : "";
        }
        public static string parseNumberOrString(float nbr) {
            return nbr.ToString();
        }

        /**
         * Parses latitude given as "ddmm.mm", "dmm.mm" or "mm.mm" (assuming zero
         * degrees) along with a given hemisphere of "N" or "S" into decimal degrees,
         * where north is positive and south is negetive.
         */
        public static float parseLatitude(string lat, string hemi) {
            float hemisphere = (float)((hemi == "N") ? 1.0 : -1.0);

            string[] a = lat.Split(".");

            string degrees = "";
            string minutes = "";
            if (a[0].Length == 4) {
                // two digits of degrees
                degrees = lat.Substring(0, 2);
                minutes = lat.Substring(2);
            } else if (a[0].Length == 3) {
                // 1 digit of degrees (in case no leading zero)
                degrees = lat.Substring(0, 1);
                minutes = lat.Substring(1);
            } else {
                // no degrees, just minutes (nonstandard but a buggy unit might do this)
                degrees = "0";
                minutes = lat;
            }
            float d = 0, m = 0;
            float.TryParse(degrees, out d);
            float.TryParse(minutes, out m);
            return d + (m / 60) * hemisphere;
        }

        /**
         * Parses latitude given as "dddmm.mm", "ddmm.mm", "dmm.(
        */
        public static float parseLongitude(string lon, string hemi) {
            float h = (float)((hemi == "E") ? 1.0 : -1.0);

            string[] a = lon.Split(".");

            string degrees, minutes;

            if (a[0].Length == 5) {
                // three digits of degrees
                degrees = lon.Substring(0, 3);
                minutes = lon.Substring(3);
            } else if (a[0].Length == 4) {
                // 2 digits of degrees (in case no leading zero)
                degrees = lon.Substring(0, 2);
                minutes = lon.Substring(2);
            } else if (a[0].Length == 3) {
                // 1 digit of degrees (in case no leading zero)
                degrees = lon.Substring(0, 1);
                minutes = lon.Substring(1);
            } else {
                // no degrees, just minutes (nonstandard but a buggy unit might do this)
                degrees = "0";
                minutes = lon;
            }
            float d = 0, m = 0;
            float.TryParse(degrees, out d);
            float.TryParse(minutes, out m);
            return (float)(d + (m / 60.0) * h);
        }

        /**
         * Parses a UTC time and optionally a date and returns a Date
         * object.
         * @param {String} time Time the format "hhmmss" or "hhmmss.ss"
         * @param {String=} date Optional date in format the ddmmyyyy or ddmmyy
         * @returns {Date}
         */
        public static DateTime parseTime(string stime, string sdate) {
            DateTime ret = new DateTime();
            if (stime == "") {
                return ret;
            }

            int year = 0; 
            int month = 0; 
            int day = 0;

            if (sdate.Length == 8) {
                year = parseIntSafe(sdate.Substring(4));
                month = parseIntSafe(sdate.Substring(2, 4));
                day = parseIntSafe(sdate.Substring(0, 2)); 
            }
            else {
                year = ret.Year;
                month = ret.Month;
                day = ret.Day;
            }

            int hrs = parseIntSafe(stime.Substring(0, 2));
            int mins = parseIntSafe(stime.Substring(2, 2));
            int secs = parseIntSafe(stime.Substring(4, 2));
            
            // Extract the milliseconds, since they can be not present, be 3 decimal place, or 2 decimal places, or other?
            string msStr = stime.Substring(7);
            int msExp = msStr.Length;
            int ms = 0;
            if (msExp != 0) {
                ms = (int)(float.Parse(msStr) * Math.Pow(10, 3 - msExp));
            }
            
            if (sdate.Length == 0) {
                return ret;    //(hour: hrs, minute: mins, second: secs, millisecond: ms);
            }
            else {
                return new DateTime(year, month, day, hrs, mins, secs, ms); 
            }
        }

        /**
         * Parses a date in the format "yyMMdd" along with a time in the format
         * "hhmmss" or "hhmmss.ss" and returns a Date object.
         */
        public static DateTime parseDatetime(string sdate, string stime) {
            
            if (sdate.Length == 0 && stime.Length == 0) {
                return new DateTime();
            }
 
            int day = parseIntSafe(sdate.Substring(0, 2));
            int month = parseIntSafe(sdate.Substring(2, 2));
            int year = parseIntSafe(sdate.Substring(4, 2));

            if (year < 73) {
                year += 2000;
            }
            else {
                year += 1900;
            }

            if (stime.Length == 0) {
                return new DateTime(year, month, day);
            }
            else {
                int hours = parseIntSafe(stime.Substring(0, 2));   // 0,1
                int minutes = parseIntSafe(stime.Substring(2, 2)); // 2,3
                int seconds = parseIntSafe(stime.Substring(4, 2)); // 4,5
                int milliseconds = 0;
                if (stime.Length >= 8) {
                    milliseconds = parseIntSafe(stime.Substring(7)) * 10;
                }

                return new DateTime(year, month, day, hours, minutes, seconds, milliseconds).ToUniversalTime();
            }
        }
    }
}