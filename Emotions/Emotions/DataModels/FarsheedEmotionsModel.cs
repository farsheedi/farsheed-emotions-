using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emotions.DataModels
{
    public class FarsheedEmotionsModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "Emotion")]
        public string Emotion { get; set; }

      

    }
}
