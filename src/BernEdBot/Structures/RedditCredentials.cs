using Newtonsoft.Json;
using System;
using System.IO;

namespace BernEdBot.Structures
{
    [Serializable]
    public class RedditCredentials
    {
        public string AppId
        {
            get
            {
                return "G56wUjR2l3HKGQ";
            }
            private set { }
        }

        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        private readonly string ConfigJSON;
        private string CredentialsFilename
        {
            get
            {
                return "RedditCredentials.json";
            }
            set { }
        }

        public RedditCredentials(string username = null, string refreshToken = null, string accessToken = null, bool autoLoad = true)
        {
            Username = username;
            AccessToken = accessToken;
            RefreshToken = refreshToken;

            if (autoLoad)
            {
                string path = Path.Combine(Environment.CurrentDirectory, CredentialsFilename);
                if (!File.Exists(path))
                {
                    // If we can't find the resource file, just re-create it.  --Kris
                    File.WriteAllText(path, JsonConvert.SerializeObject(new RedditCredentials(autoLoad: false)));
                }

                try
                {
                    ConfigJSON = File.ReadAllText(path);
                }
                catch (Exception) { }

                if (string.IsNullOrWhiteSpace(ConfigJSON))
                {
                    throw new Exception("Please populate RedditCredentials.json before proceeding.");
                }
                else
                {
                    RedditCredentials redditCredentials = Load();

                    Username = redditCredentials.Username;
                    AccessToken = redditCredentials.AccessToken;
                    RefreshToken = redditCredentials.RefreshToken;

                    if (string.IsNullOrWhiteSpace(redditCredentials.RefreshToken))
                    {
                        Save();
                        throw new Exception("Please add credentials to RedditCredentials.json before proceeding.");
                    }
                }
            }
        }

        public void Clear()
        {
            Username = null;
            AccessToken = null;
            RefreshToken = null;
        }

        private RedditCredentials Load()
        {
            return JsonConvert.DeserializeObject<RedditCredentials>(ConfigJSON);
        }

        public void Save()
        {
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, CredentialsFilename), JsonConvert.SerializeObject(this));
        }
    }
}
