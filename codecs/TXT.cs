using System;
using Newtonsoft.Json;


namespace csNMEA
{
    /*
     * === TXT - Human readable text information for display purposes ===
     *
     * -------------------------------
     *         1  2  3  4   5
     *         |  |  |  |   |
     *  $--TXT,xx,xx,xx,c-c,*hh<CR><LF>
     * -------------------------------
     *
     * Field Number:
     *
     * 1. Total number of sentences
     * 2. Sentence number
     * 3. Text Id
     * 4. Message text, up to 61 characters
     * 5. Checksum
     */
    public class TXTPacket : Decoder
    {
        public TXTPacket(string[] fields) {
            try {
                sentenceId = "TXT";
                sentenceName = "Human readable text information for display purposes";
                numberOfSentences = Helpers.parseIntSafe(fields[1]);
                sentenceNumber = Helpers.parseIntSafe(fields[2]);
                textId = Helpers.parseIntSafe(fields[3]);
                textInformation = fields[4];
            }
            finally {}
        }

        public override string getJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        public int numberOfSentences { get; set; }
        public int sentenceNumber { get; set; }
        public int textId { get; set; }
        public string textInformation { get; set; }
    }
}