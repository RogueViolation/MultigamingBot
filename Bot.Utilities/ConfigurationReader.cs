using System.Configuration;

namespace MultigamingBot.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        public string GetSection(string section)
        {
            var reader = new AppSettingsReader();
            return reader.GetValue(section, typeof(string)).ToString() ?? "";
        }
    }
}
