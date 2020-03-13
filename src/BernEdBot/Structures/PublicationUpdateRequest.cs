using BernEdBot.Converters;
using Newtonsoft.Json;
using System;

namespace BernEdBot.Structures
{
    [Serializable]
    public class PublicationUpdateRequest
    {
        [JsonProperty("requestId")]
        public int RequestId { get; set; }

        [JsonProperty("oldPublisherJson")]
        public Publication OldPublication { get; set; }

        [JsonProperty("newPublisherJson")]
        public Publication NewPublication { get; set; }

        [JsonProperty("redditPostId")]
        public string RedditPostID { get; set; }

        [JsonProperty("redditVerifiedCommentId")]
        public string RedditVerifiedCommentId { get; set; }

        [JsonProperty("approvedBy")]
        public string ApprovedBy { get; set; }

        [JsonProperty("requestReceived")]
        public DateTime RequestReceived { get; set; }

        [JsonProperty("isSpam")]
        [JsonConverter(typeof(IntBoolConvert))]
        public bool IsSpam { get; set; }

        [JsonProperty("ip")]
        public string IP { get; set; }

        [JsonProperty("applied")]
        public DateTime? Applied { get; set; }

        public bool Equals(PublicationUpdateRequest publicationUpdateRequest)
        {
            return (publicationUpdateRequest != null 
                && publicationUpdateRequest.RequestId.Equals(RequestId) 
                && JsonConvert.SerializeObject(publicationUpdateRequest.NewPublication).Equals(JsonConvert.SerializeObject(NewPublication))
                && JsonConvert.SerializeObject(publicationUpdateRequest.OldPublication).Equals(JsonConvert.SerializeObject(OldPublication)));
        }
    }
}
