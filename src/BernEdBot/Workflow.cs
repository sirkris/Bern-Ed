using BernEdBot.Structures;
using Reddit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BernEdBot
{
    public class Workflow
    {
        public bool Active { get; set; }
        public RedditClient RedditClient
        {
            get
            {
                if (redditClient == null)
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

        public Workflow() { }

        public void MainLoop()
        {
            Active = true;
            while (Active)
            {
                // TODO
            }
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
    }
}
