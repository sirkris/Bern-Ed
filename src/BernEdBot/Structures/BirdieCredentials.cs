using Newtonsoft.Json;
using System;
using System.IO;

namespace BernEdBot.Structures
{
    [Serializable]
    public class BirdieCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

        private readonly string ConfigJSON;
        private string CredentialsFilename
        {
            get
            {
                return "BirdieCredentials.json";
            }
            set { }
        }

        public BirdieCredentials(string username = null, string password = null, bool autoLoad = true)
        {
            Username = username;
            Password = password;

            if (autoLoad)
            {
                string path = Path.Combine(Environment.CurrentDirectory, CredentialsFilename);
                if (!File.Exists(path))
                {
                    // If we can't find the resource file, just re-create it.  --Kris
                    File.WriteAllText(path, JsonConvert.SerializeObject(new BirdieCredentials(autoLoad: false)));
                }

                try
                {
                    ConfigJSON = File.ReadAllText(path);
                }
                catch (Exception) { }

                if (string.IsNullOrWhiteSpace(ConfigJSON))
                {
                    throw new Exception("Please populate BirdieCredentials.json before proceeding.");
                }
                else
                {
                    BirdieCredentials birdieCredentials = Load();

                    Username = birdieCredentials.Username;
                    Password = birdieCredentials.Password;

                    if (string.IsNullOrWhiteSpace(birdieCredentials.Username))
                    {
                        Save();
                        throw new Exception("Please add credentials to BirdieCredentials.json before proceeding.");
                    }
                }
            }
        }

        public void Clear()
        {
            Username = null;
            Password = null;
        }

        private BirdieCredentials Load()
        {
            return JsonConvert.DeserializeObject<BirdieCredentials>(ConfigJSON);
        }

        public void Save()
        {
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, CredentialsFilename), JsonConvert.SerializeObject(this));
        }
    }
}
