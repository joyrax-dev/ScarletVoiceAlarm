using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace ScarletVoiceAlarm.Server
{
    #region Types
    internal class Config
    {
        [JsonPropertyName("Listen")]
        public Listen Listen { get; set; }

        [JsonPropertyName("KeyBindings")]
        public KeyBindings[] KeyBindings { get; set; }

        [JsonPropertyName("Locations")]
        public Locations[] Locations { get; set; }

        public Config(Listen listen, KeyBindings[] keyBindings, Locations[] locations) 
        {
            Listen = listen;
            KeyBindings = keyBindings;
            Locations = locations;
        }
    }

    internal class Listen
    {
        [JsonPropertyName("Host")]
        public string Host { get; set; }

        [JsonPropertyName("Port")]
        public int Port { get; set; }

        public Listen(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }

    internal class KeyBindings
    {
        [JsonPropertyName("Key")]
        public string Key { get; set; }

        [JsonPropertyName("Locations")]
        public string[] Locations { get; set; }

        [JsonIgnore]
        public Key SystemKey { get; set; }

        public KeyBindings(string key, string[] locations)
        {
            Key = key;
            Locations = locations;

            try
            {
                Enum.TryParse(Key, out Key systemKey);
                SystemKey = systemKey;
            }
            catch
            {
                Console.WriteLine("[Error] Bad key value: " + Key);
            }
        }
    }

    internal class Locations
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Host")]
        public string Host { get; set; }

        [JsonPropertyName("Port")]
        public int Port { get; set; }

        [JsonIgnore]
        public IPEndPoint EndPoint { get; set; }

        public Locations(string name, string host, int port)
        {
            Name = name;
            Host = host;
            Port = port;
            EndPoint = new IPEndPoint(IPAddress.Parse(Host), Port);
        }
    }
    #endregion

    internal static class Configuration
    {
        public static Config? Config { get; set; }

        public static void Load()
        {
            using (FileStream fs = new FileStream("config.json", FileMode.OpenOrCreate))
            {
                Config? config = JsonSerializer.Deserialize<Config>(fs);

                Config = config;
            }
        }
    }
}
