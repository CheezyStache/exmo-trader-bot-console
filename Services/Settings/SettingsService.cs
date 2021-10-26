using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.Settings
{
    public class SettingsService<T>: ISettingsService<T> where T: class
    {
        private readonly string _path;
        private T _settings;

        public SettingsService(string path)
        {
            _path = path;
        }

        public T GetSettings()
        {
            if (_settings != null)
                return _settings;

            var settingsJson = ReadSettingsJson();
            _settings = JsonSerializer.Deserialize<T>(settingsJson,
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return _settings;
        }

        private string ReadSettingsJson()
        {
            using (StreamReader r = new StreamReader(_path))
            {
                string json = r.ReadToEnd();
                return json;
            }
        }
    }
}
