using System;
using Newtonsoft.Json;

namespace csNMEA 
{
    public abstract class Decoder {
        
        [JsonProperty(Order = -3)]
        public string sentenceId { get; set; }
        [JsonProperty(Order = -2)]
        public string sentenceName {get; set; }
        public abstract string getJson();
    }
}
