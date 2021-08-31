using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;

namespace exmo_trader_bot_console.Services.SettingsService
{
    public class SettingsService: ISettingsService
    {
        private const string SettingsPath = "settings.json";

        public Settings GetSettings()
        {
            var settingsJson = ReadSettingsJson();
            return JsonSerializer.Deserialize<Settings>(settingsJson,
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private string ReadSettingsJson()
        {
            using (StreamReader r = new StreamReader(SettingsPath))
            {
                string json = r.ReadToEnd();
                return json;
            }
        }
    }
}
