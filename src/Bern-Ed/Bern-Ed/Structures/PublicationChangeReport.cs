using Bern_Ed.Converters;
using Newtonsoft.Json;
using System;

namespace Bern_Ed.Structures
{
    [Serializable]
    public class PublicationChangeReport
    {
        [JsonProperty("pubId")]
        public int PubID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("stateAbbr")]
        public string StateAbbr { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("pocTitle")]
        public string POCTitle { get; set; }

        [JsonProperty("poc")]
        public string POC { get; set; }

        [JsonProperty("wordLimit")]
        public int? WordLimit { get; set; }

        [JsonProperty("daysWaitAfterPublish")]
        public int DaysWaitAfterPublish { get; set; }

        [JsonProperty("requiresLocalTieIn")]
        [JsonConverter(typeof(IntBoolConvert))]
        public bool RequiresLocalTieIn { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("addNote")]
        public string AddNote { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
}
