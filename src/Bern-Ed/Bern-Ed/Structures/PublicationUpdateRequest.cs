using Bern_Ed.Converters;
using Newtonsoft.Json;
using System;

namespace Bern_Ed.Structures
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

        public PublicationUpdateRequest(PublicationChangeReport publicationChangeReport, Publication oldPublication)
        {
            OldPublication = oldPublication;
            NewPublication = new Publication
            {
                PubID = publicationChangeReport.PubID,
                Name = publicationChangeReport.Name,
                City = publicationChangeReport.City,
                StateAbbr = publicationChangeReport.StateAbbr,
                Email = publicationChangeReport.Email,
                Phone = publicationChangeReport.Phone,
                POCTitle = publicationChangeReport.POCTitle,
                POC = publicationChangeReport.POC,
                WordLimit = publicationChangeReport.WordLimit,
                DaysWaitAfterPublish = publicationChangeReport.DaysWaitAfterPublish,
                RequiresLocalTieIn = publicationChangeReport.RequiresLocalTieIn,
                Website = publicationChangeReport.Website,
                Notes = oldPublication.Notes + "|" + publicationChangeReport.AddNote,
                ContactURL = publicationChangeReport.Source, 
                Enabled = oldPublication.Enabled
            };
            RequestReceived = DateTime.Now;
        }

        public PublicationUpdateRequest() { }
    }
}
