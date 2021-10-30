using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YAB.Plugins.Injectables.Options
{
    public abstract class Options<T> : IOptions
        where T : Options<T>, new()
    {
        [JsonIgnore]
        protected string decryptPassword = null;

        [JsonIgnore]
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions { IncludeFields = true };

        [JsonIgnore]
        public static string Filename { get => typeof(T).Name.ToLower().Trim().Replace(".", "") + "EncryptedSettings.encjson"; }

        public void Load(string password)
        {
            if (File.Exists(Filename))
            {
                // TODO decrypt file
                var loadedSettings = JsonSerializer.Deserialize<T>(File.ReadAllText(Filename), serializerOptions);

                loadedSettings.decryptPassword = password;

                foreach (var prop in this.GetType().GetProperties())
                {
                    prop.SetValue(this, prop.GetValue(loadedSettings));
                }
            }
            else
            {
                throw new FileNotFoundException($"Could not load file for {this.GetType().Name}. Did you register this option yet?");
            }
        }

        public void Save(string password)
        {
            // TODO encrypt file
            File.WriteAllText(Filename, JsonSerializer.Serialize((T)this));
        }
    }
}