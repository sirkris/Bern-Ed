using BernEdBot.Structures;
using Newtonsoft.Json;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using Reddit.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BernEdBot
{
    public class Workflow
    {
        public bool Active { get; set; }
        public RedditClient RedditClient
        {
            get
            {
                if (redditClient == null 
                    && !string.IsNullOrWhiteSpace(RedditCredentials.RefreshToken))
                {
                    redditClient = new RedditClient(appId: RedditCredentials.AppId, refreshToken: RedditCredentials.RefreshToken, accessToken: RedditCredentials.AccessToken,
                        userAgent: "BernEdBot v" + GetVersion());
                }

                return redditClient;
            }
            private set
            {
                redditClient = value;
            }
        }
        private RedditClient redditClient;

        private RedditCredentials RedditCredentials
        {
            get
            {
                if (redditCredentials == null)
                {
                    redditCredentials = new RedditCredentials();
                }

                return redditCredentials;
            }
            set
            {
                redditCredentials = value;
            }
        }
        private RedditCredentials redditCredentials;

        private IList<PublicationUpdateRequest> RequestQueue
        {
            get
            {
                if (requestQueue == null
                    || requestQueue.Count.Equals(0)
                    || !RequestQueueLastUpdated.HasValue
                    || RequestQueueLastUpdated.Value.AddMinutes(15) < DateTime.Now)
                {
                    string res = Request.ExecuteRequest(Request.Prepare("/berned/pubChangeRequest?getQueue"));
                    if (!string.IsNullOrEmpty(res))
                    {
                        res = res.Replace("\\\"", "\"")
                            .Replace("\"oldPublisherJson\":\"", "\"oldPublisherJson\":")
                            .Replace("}\",\"newPublisherJson\":\"", "},\"newPublisherJson\":")
                            .Replace("}\",\"redditPostId\"", "},\"redditPostId\"");

                        RequestQueue = JsonConvert.DeserializeObject<IList<PublicationUpdateRequest>>(res);
                    }
                }

                return requestQueue;
            }
            set
            {
                requestQueue = value;
                RequestQueueLastUpdated = DateTime.Now;
            }
        }
        private IList<PublicationUpdateRequest> requestQueue;
        private DateTime? RequestQueueLastUpdated { get; set; }

        private DateTime? LastPostClean { get; set; }
        private DateTime? LastApprovalCheck { get; set; }

        private Request Request { get; set; }
        private Subreddit Subreddit { get; set; }

        // Indexed by RequestID.  --Kris
        private IDictionary<int, SelfPost> Posts { get; set; }

        private IDictionary<string, int> RequestIdsByPostId { get; set; }
        private HashSet<string> RequesterIPs { get; set; }
        private IDictionary<int, PublicationUpdateRequest> RequestsByRequestID { get; set; }

        private bool LogVerbose { get; set; } = false;

        private const string SUBREDDIT = "FreeOpinionSyndicate";
        private const string POST_TITLE_PREFIX = "NEW PUBLISHER UPDATE REPORT: ";
        private const int MAX_ACTIVE_POSTS = 3;

        private string NEWLINE
        {
            get
            {
                return (Environment.NewLine + Environment.NewLine);
            }
            set { }
        }

        public Workflow()
        {
            Intro();

            Log("Initializing....");

            if (RedditClient == null)
            {
                throw new Exception("Unable to load Reddit client!");
            }

            Posts = new Dictionary<int, SelfPost>();
            RequestIdsByPostId = new Dictionary<string, int>();
            RequesterIPs = new HashSet<string>();
            RequestsByRequestID = new Dictionary<int, PublicationUpdateRequest>();
            Request = new Request();
            Subreddit = RedditClient.Subreddit(SUBREDDIT);

            Log("Initialization complete.");
        }

        public void MainLoop()
        {
            Log("Commencing main loop....");

            // Check Reddit for any existing posts to monitor.  --Kris
            ScanPosts();

            Active = true;
            while (Active)
            {
                CleanPosts();
                CheckPostApprovals();
                ExecuteQueue();

                Thread.Sleep(60000);
            }

            IList<int> keys = new List<int>();
            foreach (int requestId in Posts.Keys)
            {
                keys.Add(requestId);
            }

            foreach (int requestId in keys)
            {
                UnmonitorPost(requestId);
            }

            Log("Main loop execution complete.");
        }

        private void ExecuteQueue()
        {
            if (RequestQueue != null 
                && Posts.Count <= MAX_ACTIVE_POSTS)
            {
                Log("Checking requests queue....");

                foreach (PublicationUpdateRequest publicationUpdateRequest in RequestQueue)
                {
                    if (string.IsNullOrEmpty(publicationUpdateRequest.RedditPostID))
                    {
                        MonitorPost(CreatePost(publicationUpdateRequest), publicationUpdateRequest);
                        if (Posts.Count.Equals(MAX_ACTIVE_POSTS))
                        {
                            break;
                        }
                    }
                }

                // Clear the queue.  --Kris
                RequestQueue = null;

                Log("Queue check complete.");
            }
        }

        private SelfPost CreatePost(PublicationUpdateRequest publicationUpdateRequest)
        {
            string body = "# " + publicationUpdateRequest.OldPublication.Name + NEWLINE;

            body += "## Proposed Changes" + NEWLINE;
            body += GetChanges(publicationUpdateRequest) + NEWLINE;

            body += "## Raw Data" + NEWLINE;
            body += "    " + GetRawData(publicationUpdateRequest) + "    " + NEWLINE;

            body += "## Instructions" + NEWLINE;
            body += GetInstructions() + NEWLINE;

            body += "**Thank you for helping to keep this public database accurate and up-to-date!**";

            SelfPost res = Subreddit.SelfPost(POST_TITLE_PREFIX + publicationUpdateRequest.OldPublication.Name, body).Submit();
            publicationUpdateRequest.RedditPostID = res.Id;

            res.Distinguish("yes");
            UpdateRequest(publicationUpdateRequest.RequestId, publicationUpdateRequest);

            Log("Created new Reddit post: " + res.Id);

            return res;
        }

        private string GetChanges(PublicationUpdateRequest publicationUpdateRequest)
        {
            string res = "";
            if (publicationUpdateRequest.OldPublication == null || publicationUpdateRequest.NewPublication == null
                || !publicationUpdateRequest.OldPublication.PubID.Equals(publicationUpdateRequest.NewPublication.PubID)
                || !publicationUpdateRequest.OldPublication.Name.Equals(publicationUpdateRequest.NewPublication.Name))
            {
                return "**THE DATA FROM THIS REQUEST IS CORRUPTED!  PLEASE REJECT TO AVOID COMPROMISING DATA INTEGRITY!**";
            }

            if (!publicationUpdateRequest.OldPublication.City.Equals(publicationUpdateRequest.NewPublication.City))
            {
                res += "### City" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.City + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.City + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.StateAbbr.Equals(publicationUpdateRequest.NewPublication.StateAbbr))
            {
                res += "### State" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.StateAbbr + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.StateAbbr + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.Email.Equals(publicationUpdateRequest.NewPublication.Email))
            {
                res += "### Email" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.Email + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.Email + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.Phone.Equals(publicationUpdateRequest.NewPublication.Phone))
            {
                res += "### Phone" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.Phone + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.Phone + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.POCTitle.Equals(publicationUpdateRequest.NewPublication.POCTitle))
            {
                res += "### Contact Title" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.POCTitle + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.POCTitle + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.POC.Equals(publicationUpdateRequest.NewPublication.POC))
            {
                res += "### Contact Name" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.POC + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.POC + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.WordLimit.Equals(publicationUpdateRequest.NewPublication.WordLimit))
            {
                res += "### Word Limit" + NEWLINE;
                res += (publicationUpdateRequest.OldPublication.WordLimit.HasValue ? "~~" + publicationUpdateRequest.OldPublication.WordLimit + "~~" + NEWLINE : "");
                res += (publicationUpdateRequest.NewPublication.WordLimit.HasValue ? "*" + publicationUpdateRequest.NewPublication.WordLimit + "*" + NEWLINE : "");
            }

            if (!publicationUpdateRequest.OldPublication.DaysWaitAfterPublish.Equals(publicationUpdateRequest.NewPublication.DaysWaitAfterPublish))
            {
                res += "### Publish Interval" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.DaysWaitAfterPublish.ToString() + " days" + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.DaysWaitAfterPublish.ToString() + " days" + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.City.Equals(publicationUpdateRequest.NewPublication.Notes))
            {
                res += "### Notes" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.Notes.Replace("|", "~~" + NEWLINE + "~~") + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.Notes.Replace("|", "*" + NEWLINE + "*") + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.ContactURL.Equals(publicationUpdateRequest.NewPublication.ContactURL))
            {
                res += "### Source" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.ContactURL + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.ContactURL + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.Website.Equals(publicationUpdateRequest.NewPublication.Website))
            {
                res += "### Website" + NEWLINE;
                res += "~~" + publicationUpdateRequest.OldPublication.Website + "~~" + NEWLINE;
                res += "*" + publicationUpdateRequest.NewPublication.Website + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.RequiresLocalTieIn.Equals(publicationUpdateRequest.NewPublication.RequiresLocalTieIn))
            {
                res += "### Requires Local Tie-In?" + NEWLINE;
                res += "~~" + (publicationUpdateRequest.OldPublication.RequiresLocalTieIn ? "Yes" : "No") + "~~" + NEWLINE;
                res += "*" + (publicationUpdateRequest.NewPublication.RequiresLocalTieIn ? "Yes" : "No") + "*" + NEWLINE;
            }

            if (!publicationUpdateRequest.OldPublication.Enabled.Equals(publicationUpdateRequest.NewPublication.Enabled))
            {
                res += "### Enabled?" + NEWLINE;
                res += "~~" + (publicationUpdateRequest.OldPublication.Enabled ? "Yes" : "No") + "~~" + NEWLINE;
                res += "*" + (publicationUpdateRequest.NewPublication.Enabled ? "Yes" : "No") + "*" + NEWLINE;
            }

            if (string.IsNullOrEmpty(res))
            {
                res = "**There are no proposed changes.  This request was likely submitted by mistake and should be rejected.**";
            }

            return res;
        }

        private string GetRawData(PublicationUpdateRequest publicationUpdateRequest)
        {
            publicationUpdateRequest.IP = "(hidden)";
            return JsonConvert.SerializeObject(publicationUpdateRequest);
        }

        private string GetInstructions()
        {
            return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "PostInstructions.md"));
        }

        private void ScanPosts()
        {
            Log("Scanning for active request posts....");

            // Sometimes, Reddit will return a 401 Unauthorized response when the servers are busy.  Don't be fooled.  --Kris
            int retry = 10;
            do
            {
                try
                {
                    foreach (Post post in Subreddit.Posts.GetNew(limit: 100))
                    {
                        if (IsActiveReportPost(post, out PublicationUpdateRequest publicationUpdateRequest))
                        {
                            MonitorPost((SelfPost)post, publicationUpdateRequest);
                        }
                    }
                }
                catch (RedditUnauthorizedException ex)
                {
                    if ((--retry).Equals(0))
                    {
                        throw ex;
                    }

                    Thread.Sleep(30000);
                }

                break;
            } while (!retry.Equals(0));

            Log("Post scan complete.  Found " + Posts.Count.ToString() + " post" + (!Posts.Count.Equals(1) ? "s" : "") + ".");
        }

        private void MonitorPost(SelfPost post, PublicationUpdateRequest publicationUpdateRequest)
        {
            if (post != null && !RequesterIPs.Contains(publicationUpdateRequest.IP))
            {
                Log("Monitoring post (postId=" + post.Id + ", requestId=" + publicationUpdateRequest.RequestId.ToString() + ")....");

                post.PostDataUpdated += C_PostUpdated;
                post.MonitorPostData(monitoringBaseDelayMs: 15000);

                post.PostScoreUpdated += C_PostUpdated;
                post.MonitorPostScore(monitoringBaseDelayMs: 15000);

                Posts.Add(publicationUpdateRequest.RequestId, post);
                RequestIdsByPostId.Add(post.Id, publicationUpdateRequest.RequestId);
                RequesterIPs.Add(publicationUpdateRequest.IP);
                RequestsByRequestID.Add(publicationUpdateRequest.RequestId, publicationUpdateRequest);
            }
        }

        private void UnmonitorPost(PublicationUpdateRequest publicationUpdateRequest)
        {
            if (Posts.ContainsKey(publicationUpdateRequest.RequestId))
            {
                Posts[publicationUpdateRequest.RequestId].MonitorPostData();
                Posts[publicationUpdateRequest.RequestId].PostDataUpdated -= C_PostUpdated;

                Posts[publicationUpdateRequest.RequestId].MonitorPostScore();
                Posts[publicationUpdateRequest.RequestId].PostScoreUpdated -= C_PostUpdated;

                Log("Stopped monitoring post (postId=" + Posts[publicationUpdateRequest.RequestId].Id + ", requestId=" + publicationUpdateRequest.RequestId.ToString() + ").");

                Posts.Remove(publicationUpdateRequest.RequestId);
                RequesterIPs.Remove(publicationUpdateRequest.IP);
            }
        }

        private void UnmonitorPost(int requestId)
        {
            UnmonitorPost(RequestsByRequestID[requestId]);
        }

        private void UnmonitorPost(Post post)
        {
            int? requestId = null;
            foreach (KeyValuePair<int, SelfPost> pair in Posts)
            {
                if (pair.Value.Id.Equals(post.Id))
                {
                    requestId = pair.Key;
                    break;
                }
            }

            if (requestId.HasValue)
            {
                UnmonitorPost(requestId.Value);
            }
        }

        private void CleanPosts()
        {
            if (!LastPostClean.HasValue
                || LastPostClean.Value.AddHours(1) < DateTime.Now)
            {
                Log("Cleaning posts....");

                IList<int> removeIds = new List<int>();
                foreach (KeyValuePair<int, SelfPost> pair in Posts)
                {
                    string res = Request.ExecuteRequest(Request.Prepare("/berned/pubChangeRequest?requestId=" + pair.Key.ToString()));
                    if (string.IsNullOrWhiteSpace(res))
                    {
                        removeIds.Add(pair.Key);
                    }
                    else
                    {

                        PublicationUpdateRequest publicationUpdateRequest = null;
                        try
                        {
                            publicationUpdateRequest = JsonConvert.DeserializeObject<PublicationUpdateRequest>(SanitizeReportJSON(res));
                        }
                        catch (Exception) { }

                        PublicationUpdateRequest report = RefreshRequest(GetReportData(pair.Value));
                        if (report == null
                            || !report.Equals(publicationUpdateRequest))
                        {
                            removeIds.Add(pair.Key);
                        }
                    }
                }

                foreach (int removeId in removeIds)
                {
                    UnmonitorPost(removeId);
                }

                LastPostClean = DateTime.Now;

                Log("Post clean complete.  Removed " + removeIds.Count.ToString() + " post" + (!removeIds.Count.Equals(1) ? "s" : "") + " from monitoring.");
            }
        }

        // Manually scans the posts for any that meet the criteria.  Neccessary because changes to "approved" won't show up in Reddit.NET standard monitoring.  --Kris
        private void CheckPostApprovals()
        {
            if (!LastApprovalCheck.HasValue
                || LastApprovalCheck.Value.AddHours(1) < DateTime.Now)
            {
                Log("Checking posts for approval....");

                IList<int> removeIds = new List<int>();
                foreach (KeyValuePair<int, SelfPost> pair in Posts)
                {
                    if (CheckPostIsReady(pair.Value.About()))
                    {
                        removeIds.Add(pair.Key);
                    }
                }

                foreach (int removeId in removeIds)
                {
                    UnmonitorPost(removeId);
                }

                RequestQueue = null;

                LastApprovalCheck = DateTime.Now;

                Log("Post approval scan complete.  Removed " + removeIds.Count.ToString() + " post" + (!removeIds.Count.Equals(1) ? "s" : "") + " from monitoring.");
            }
        }

        // Checks to see if the post meets the criteria for approval.  If it does, apply the publisher updates and return true.  --Kris
        private bool CheckPostIsReady(SelfPost post)
        {
            if (PostIsReady(post))
            {
                // Update our publisher in the db.  --Kris
                PublicationUpdateRequest publicationUpdateRequest = RefreshRequest(GetReportData(post));

                Log("Request #" + publicationUpdateRequest.RequestId.ToString() + " to update " + publicationUpdateRequest.OldPublication.Name + " (postId=" + post.Id + ") meets all criteria!");

                UpdatePublisher(publicationUpdateRequest.OldPublication.PubID,
                    (!publicationUpdateRequest.OldPublication.Email.Equals(publicationUpdateRequest.NewPublication.Email) ? publicationUpdateRequest.NewPublication.Email : null),
                    (!publicationUpdateRequest.OldPublication.Phone.Equals(publicationUpdateRequest.NewPublication.Phone) ? publicationUpdateRequest.NewPublication.Phone : null),
                    (!publicationUpdateRequest.OldPublication.POCTitle.Equals(publicationUpdateRequest.NewPublication.POCTitle) ? publicationUpdateRequest.NewPublication.POCTitle : null),
                    (!publicationUpdateRequest.OldPublication.POC.Equals(publicationUpdateRequest.NewPublication.POC) ? publicationUpdateRequest.NewPublication.POC : null),
                    (!publicationUpdateRequest.OldPublication.WordLimit.Equals(publicationUpdateRequest.NewPublication.WordLimit) ? publicationUpdateRequest.NewPublication.WordLimit : null),
                    (!publicationUpdateRequest.OldPublication.Notes.Equals(publicationUpdateRequest.NewPublication.Notes) ? publicationUpdateRequest.NewPublication.Notes : null),
                    (!publicationUpdateRequest.OldPublication.RequiresLocalTieIn.Equals(publicationUpdateRequest.NewPublication.RequiresLocalTieIn) 
                        ? publicationUpdateRequest.NewPublication.RequiresLocalTieIn : (bool?)null),
                    (!publicationUpdateRequest.OldPublication.DaysWaitAfterPublish.Equals(publicationUpdateRequest.NewPublication.DaysWaitAfterPublish) 
                        ? publicationUpdateRequest.NewPublication.DaysWaitAfterPublish : (int?)null),
                    (!publicationUpdateRequest.OldPublication.Website.Equals(publicationUpdateRequest.NewPublication.Website) ? publicationUpdateRequest.NewPublication.Website : null),
                    (!publicationUpdateRequest.OldPublication.ContactURL.Equals(publicationUpdateRequest.NewPublication.ContactURL) ? publicationUpdateRequest.NewPublication.ContactURL : null),
                    (!publicationUpdateRequest.OldPublication.City.Equals(publicationUpdateRequest.NewPublication.City) ? publicationUpdateRequest.NewPublication.City : null));

                // Mark the post as applied.  --Kris
                AcceptPost(post);

                publicationUpdateRequest.Applied = DateTime.Now;
                publicationUpdateRequest.ApprovedBy = post.Listing.ApprovedBy;

                // Update the request.  --Kris
                UpdateRequest(RequestIdsByPostId[post.Id], publicationUpdateRequest);

                return true;
            }
            else
            {
                return false;
            }
        }

        // Criteria:  Post must be approved by a moderator, not be removed, not be marked as spam, not be edited, be at least 3 hours old, and have a score > 0 with an upvote ratio > 50%.  --Kris
        private bool PostIsReady(SelfPost post)
        {
            return (post != null
                && post.Created.AddHours(3) <= DateTime.Now
                && post.Listing.Approved
                && !post.Removed 
                && !post.Spam 
                && post.Edited.Equals(default)
                && (string.IsNullOrEmpty(post.Listing.LinkFlairText) || !post.Listing.LinkFlairText.Equals("REJECTED"))
                && (string.IsNullOrEmpty(post.Listing.LinkFlairText) || !post.Listing.LinkFlairText.Equals("APPLIED"))
                && post.Score > 0 
                && (post.UpvoteRatio.Equals(0) || post.UpvoteRatio > 0.5));
        }

        private void UpdatePublisher(int pubId, string email = null, string phone = null, string pocTitle = null, string poc = null, int? wordLimit = null, string notes = null, 
            bool? requiresLocalTieIn = null, int? daysWaitAfterPublish = null, string webDomain = null, string contactURL = null, string city = null)
        {
            Log("Preparing to update publisher #" + pubId.ToString() + "....");

            string logMsg = "Updated publisher #" + pubId.ToString() + " with the following parameters:" + NEWLINE;

            RestRequest restRequest = Request.Prepare("/opmail/publications", Method.POST);

            restRequest.AddParameter("pubId", pubId);

            string logParams = "";
            if (!string.IsNullOrWhiteSpace(email))
            {
                restRequest.AddParameter("email", email);
                logParams += "email=" + email + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                restRequest.AddParameter("phone", phone);
                logParams += "phone=" + phone + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(pocTitle))
            {
                restRequest.AddParameter("pocTitle", pocTitle);
                logParams += "pocTitle=" + pocTitle + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(poc))
            {
                restRequest.AddParameter("poc", poc);
                logParams += "poc=" + poc + Environment.NewLine;
            }

            // To unset back to null, pass 0.  --Kris
            if (wordLimit.HasValue)
            {
                restRequest.AddParameter("wordLimit", (!wordLimit.Value.Equals(0) ? wordLimit.Value : (int?)null));
                logParams += "wordLimit=" + wordLimit.ToString() + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(notes))
            {
                restRequest.AddParameter("notes", notes);
                logParams += "notes=" + notes + Environment.NewLine;
            }

            if (requiresLocalTieIn.HasValue)
            {
                restRequest.AddParameter("requiresLocalTieIn", requiresLocalTieIn.Value);
                logParams += "requiresLocalTieIn=" + (requiresLocalTieIn.Value ? "yes" : "no") + Environment.NewLine;
            }

            if (daysWaitAfterPublish.HasValue)
            {
                restRequest.AddParameter("daysWaitAfterPublish", daysWaitAfterPublish.Value);
                logParams += "daysWaitAfterPublish=" + daysWaitAfterPublish.ToString() + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(webDomain))
            {
                restRequest.AddParameter("webDomain", webDomain);
                logParams += "webDomain=" + webDomain + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(contactURL))
            {
                restRequest.AddParameter("contactURL", contactURL);
                logParams += "contactURL=" + contactURL + Environment.NewLine;
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                restRequest.AddParameter("city", city);
                logParams += "city=" + city + Environment.NewLine;
            }

            if (string.IsNullOrEmpty(logParams))
            {
                Log("Warning: UpdatePublisher called for pubId #" + pubId.ToString() + " with no changes.");
            }
            else
            {
                Request.ExecuteRequest(restRequest);
                Log(logMsg + logParams);
            }
        }

        // Server config on ourproject.org is incompatible with our API PUT.  So if the record already exists, it'll be treated as an update.  --Kris
        private void UpdateRequest(int requestId, PublicationUpdateRequest publicationUpdateRequest)
        {
            RestRequest request = Request.Prepare("/berned/pubChangeRequest", Method.POST, "application/json");

            string json = JsonConvert.SerializeObject(publicationUpdateRequest);
            request.AddJsonBody(json);

            string res = Request.ExecuteRequest(request);

            Log("Updated request #" + requestId.ToString() + ".");
            Log("Request Data:" + json, true);
        }

        // TODO - Remove.  --Kris
        private void UpdateRequest(int requestId, string redditPostId = null, string redditVerifiedCommentId = null, string approvedBy = null, bool? isSpam = null, bool? applied = null)
        {
            Log("Preparing to update request #" + requestId.ToString() + "....");

            string logMsg = "Updated request #" + requestId.ToString() + " with the following parameters:" + NEWLINE;

            RestRequest restRequest = Request.Prepare("/berned/pubChangeRequest/" + requestId.ToString(), Method.PUT);

            string logParams = "";
            if (!string.IsNullOrWhiteSpace(redditPostId))
            {
                restRequest.AddParameter("redditPostId", redditPostId);
                logParams += "redditPostId=" + redditPostId + Environment.NewLine;
            }
            if (!string.IsNullOrWhiteSpace(redditVerifiedCommentId))
            {
                restRequest.AddParameter("redditVerifiedCommentId", redditVerifiedCommentId);
                logParams += "redditVerifiedCommentId=" + redditVerifiedCommentId + Environment.NewLine;
            }
            if (!string.IsNullOrWhiteSpace(approvedBy))
            {
                restRequest.AddParameter("approvedBy", approvedBy);
                logParams += "approvedBy=" + approvedBy + Environment.NewLine;
            }
            if (isSpam.HasValue)
            {
                restRequest.AddParameter("isSpam", (isSpam.Value ? "1" : "0"));
                logParams += "isSpam=" + (isSpam.Value ? "1" : "0") + Environment.NewLine;
            }
            if (applied.HasValue)
            {
                restRequest.AddParameter("applied", (applied.Value ? "1" : "0"));
                logParams += "applied=" + (applied.Value ? "1" : "0") + Environment.NewLine;
            }

            if (string.IsNullOrEmpty(logParams))
            {
                Log("Warning: UpdateRequest called for requestId #" + requestId.ToString() + " with no changes.");
            }
            else
            {
                Request.ExecuteRequest(restRequest);
                Log(logMsg + logParams);
            }
        }

        private void UpdateRequest(int requestId, Post post, DateTime? applied = null)
        {
            if (post != null && RequestsByRequestID.ContainsKey(requestId))
            {
                RequestsByRequestID[requestId].RedditPostID = post.Id;
                RequestsByRequestID[requestId].ApprovedBy = post.Listing.ApprovedBy;
                RequestsByRequestID[requestId].IsSpam = post.Spam;
                RequestsByRequestID[requestId].Applied = applied;

                UpdateRequest(requestId, RequestsByRequestID[requestId]);
            }
        }

        private void AcceptPost(Post post)
        {
            post.SetFlair("APPLIED");
            post.Comment("Update request applied.").Submit();

            Log("Accepted post " + post.Id);
        }

        private void RejectPost(Post post, string reason = null)
        {
            post.SetFlair("REJECTED");

            Comment comment = post.Comment("Update request rejected.").Submit();
            if (!string.IsNullOrWhiteSpace(reason))
            {
                comment.Reply("Reason: " + reason);
            }

            Log("Rejected post " + post.Id);
        }

        private PublicationUpdateRequest GetReportData(SelfPost post)
        {
            PublicationUpdateRequest res = null;
            if (post != null && !string.IsNullOrWhiteSpace(post.SelfText))
            {
                // Report data will be JSON string enclosed in 4 spaces (Reddit markdown for code).  --Kris
                Match m = Regex.Match(post.SelfText, @"(?<=    )(.*?)(?=    )");
                if (m != null && m.Success)
                {
                    try
                    {
                        res = JsonConvert.DeserializeObject<PublicationUpdateRequest>(m.Value.Trim());
                    }
                    catch (Exception) { }
                }
            }

            return res;
        }

        private PublicationUpdateRequest GetReportData(Post post)
        {
            return (post.Listing.IsSelf ? GetReportData((SelfPost)post) : null);
        }

        private bool IsActiveReportPost(Post post, out PublicationUpdateRequest publicationUpdateRequest)
        {
            publicationUpdateRequest = null;
            if (IsCurrentReportPost(post))
            {
                // Query API for publication update request.  --Kris
                PublicationUpdateRequest report = GetReportData(post);
                if (report == null)
                {
                    return false;
                }

                string res = Request.ExecuteRequest(Request.Prepare("/berned/pubChangeRequest?requestId=" + report.RequestId));
                if (!string.IsNullOrWhiteSpace(res))
                {
                    publicationUpdateRequest = JsonConvert.DeserializeObject<PublicationUpdateRequest>(SanitizeReportJSON(res));
                    return (publicationUpdateRequest != null && !RequesterIPs.Contains(publicationUpdateRequest.IP));
                }
            }

            return false;
        }

        private PublicationUpdateRequest RefreshRequest(PublicationUpdateRequest publicationUpdateRequest, int retry = 30)
        {
            if (publicationUpdateRequest == null)
            {
                return null;
            }

            string res = Request.ExecuteRequest(Request.Prepare("/berned/pubChangeRequest?requestId=" + publicationUpdateRequest.RequestId));
            if (!string.IsNullOrWhiteSpace(res))
            {
                return JsonConvert.DeserializeObject<PublicationUpdateRequest>(SanitizeReportJSON(res));
            }
            else
            {
                if ((--retry).Equals(0))
                {
                    throw new ArgumentException("API refresh of PublicationUpdateRequest with RequestId " + publicationUpdateRequest.RequestId.ToString() + " failed!");
                }

                Thread.Sleep(30000);

                return RefreshRequest(publicationUpdateRequest, retry);
            }
        }

        private string SanitizeReportJSON(string json)
        {
            return json
                .Trim(new char[] { '[', ']' })
                .Replace("\\\"", "\"")
                .Replace("\"oldPublisherJson\":\"", "\"oldPublisherJson\":")
                .Replace("}\",\"newPublisherJson\":\"", "},\"newPublisherJson\":")
                .Replace("}\",\"redditPostId\"", "},\"redditPostId\"");
        }

        private bool IsCurrentReportPost(Post post)
        {
            return (post.Listing.IsSelf
                && post.Author.Equals(RedditClient.Account.Me.Name)
                && post.Title.StartsWith(POST_TITLE_PREFIX)
                && post.Created.AddMonths(1) > DateTime.Now
                && !post.Spam
                && !post.Removed
                && (string.IsNullOrEmpty(post.Listing.LinkFlairText) 
                    || (!post.Listing.LinkFlairText.Equals("APPLIED")
                        && !post.Listing.LinkFlairText.Equals("REJECTED"))));
        }

        /// <summary>
        /// Returns a version string based on the digit(s) after the final decimal point.
        /// If there are no decimal points, the original version string is returned unaltered.
        /// 
        /// Versioning Rules
        /// 
        /// Assuming the version format x.y.z.b, where x is the major version, y is the minor 
        /// version, z is the hotfix version, and b is the "branch version".  I gave it that name 
        /// because I set it based on the Git branch from which the app or library or whatever is 
        /// being built.
        /// 
        /// This function takes that last digit and replaces it with a more human-readable string.
        /// For example, version "1.0.0.2" becomes version "1.0.0+beta", indicating that this is a 
        /// beta release that came *after* version 1.0.0.  This also allows us the freedom to put 
        /// out an alpha or a beta without having to know yet what the next version number will 
        /// be; as in, will it be a hotfix or a major release?  This way, we don't have to care.
        /// Logically, this means that version "0.0.0.0" is the initial singularity with an empty 
        /// project, and version "0.0.0.2" ("0.0.0+beta") would be the first beta release.
        /// 
        /// Here are the different values for b and what they're used for:
        /// 
        /// 0 - This applies to anything released from the master branch, which means it should be 
        /// a stable (non-beta) release.  As such, the branch digit is replaced with nothing.  
        /// Version "1.0.0.0" becomes version "1.0.0".
        /// 
        /// 1 - This applies to anything released from the develop branch or any feature branches, 
        /// which means we should be dealing with an alpha release that may or may not be stable.  
        /// The branch digit is replaced with "+develop".
        /// Version "1.0.0.1" becomes version "1.0.0+develop".
        /// 
        /// 2+ - For any number 2 or greater, it indicates something released from a commit on the 
        /// Release branch, which means this should be a pre-release beta.  So in this case, the 
        /// branch digit is replaced with "+beta".  Furthermore, if the number is greater than 
        /// two, then we add an incremental value at the end of the string equal to the branch 
        /// version number minus one (b - 1).
        /// Version "1.0.0.2" becomes version "1.0.0+beta".
        /// Version "1.0.0.3" becomes version "1.0.0+beta2".
        /// Version "1.0.0.4" becomes version "1.0.0+beta3".
        /// Version "1.0.0.20" becomes version "1.0.0+beta19".
        /// And so on.
        /// 
        /// In Visual Studio, you can set the version numbers by editing the project properties.
        /// If expectedDecimals is not null, the version string will only be transformed if the 
        /// number of decimal points in the input string is equal to expectedDecimals.  Otherwise, 
        /// the original version string is returned unaltered.
        /// </summary>
        /// <param name="expectedDecimals">(Optional) Use the number of decimal points to validate</param>
        /// <returns>The resulting version string.</returns>
        public string GetVersion(int? expectedDecimals = 3)
        {
            string res = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if ((!expectedDecimals.HasValue || (res.Length - res.Replace(".", "").Length).Equals(expectedDecimals))
                && !string.IsNullOrWhiteSpace(res) && res.Contains(".") && res.LastIndexOf(".") < (res.Length - 1))
            {
                int branchVer = Convert.ToInt32(res.Substring(res.LastIndexOf(".") + 1));
                string branchVerStr = (branchVer > 0
                        ? (branchVer.Equals(1)
                            ? "develop" : "beta" + (branchVer.Equals(2)
                                ? "" : (branchVer - 1).ToString()))
                        : "");

                res = res.Substring(0, res.LastIndexOf(".")) + (!string.IsNullOrWhiteSpace(branchVerStr) ? "+" + branchVerStr : "");
            }

            return res;
        }

        /// <summary>
        /// Output the intro string to the log.
        /// </summary>
        public void Intro()
        {
            Log("*****************************************" + Environment.NewLine
                + " Bern-Ed Bot " + Environment.NewLine
                + " Version " + GetVersion() + NEWLINE

                + " Created by Kris Craig " + Environment.NewLine
                + "*****************************************" + NEWLINE, showDateTime: false);
        }

        /// <summary>
        /// Output a log message.
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="verboseOnly">If true, the log message will only be displayed if logging is set to verbose</param>
        public void Log(string message, bool verboseOnly = false, bool showDateTime = true)
        {
            if (!verboseOnly || LogVerbose)
            {
                Console.WriteLine((showDateTime ? "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " : "") + message);
            }
        }

        // Both data and score monitors will end up here.  --Kris
        public void C_PostUpdated(object sender, PostUpdateEventArgs e)
        {
            if (e.NewPost != null)
            {
                // First check to see if a moderator killed the post.  --Kris
                if (e.NewPost.Spam)
                {
                    // Update report in db to set isspam to true.  --Kris
                    UpdateRequest(RequestIdsByPostId[e.NewPost.Id], e.NewPost);

                    RejectPost(e.NewPost, "Post has been marked as SPAM by a moderator.");
                    UnmonitorPost(e.NewPost);
                }
                else if (!e.NewPost.Edited.Equals(default))
                {
                    RejectPost(e.NewPost, "Post was edited.");
                    UnmonitorPost(e.NewPost);
                }
                else if (e.NewPost.Removed
                    || (e.NewPost.Listing.LinkFlairText != null && e.NewPost.Listing.LinkFlairText.Equals("REJECTED")))
                {
                    RejectPost(e.NewPost, "Post has been rejected by a moderator.");
                    UnmonitorPost(e.NewPost);
                }
                else if (e.NewPost.Listing.LinkFlairText != null && e.NewPost.Listing.LinkFlairText.Equals("APPLIED"))
                {
                    UnmonitorPost(e.NewPost);
                }
                else if (e.NewPost.Listing.IsSelf)
                {
                    // Check to see if the post is ready and go from there.  --Kris
                    CheckPostIsReady((SelfPost)e.NewPost);
                }
            }
        }
    }
}
