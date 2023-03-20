using Bot.DataAccess;
using Bot.DataAccess.DTO;
using Microsoft.Extensions.Logging;
using MultigamingBot.Configuration;

namespace Bot.OSRSHiscores
{
    public class OSRSHiscoresHelper : IOSRSHiscoresHelper
    {
        private readonly string _hiscoreHost = "secure.runescape.com";
        private readonly string _hiscorePath = $"/m=hiscore_oldschool{0}/index_lite.json";
        private readonly string _womHost = "https://api.wiseoldman.net/v2";
        private readonly ILogger<OSRSHiscoresHelper> _logger;
        private readonly IDBDataAccess _dataAccess;
        private readonly IHttpDataAccess _httpDataAccess;
        private readonly IConfigurationReader _config;
        public OSRSHiscoresHelper(ILogger<OSRSHiscoresHelper> logger, IDBDataAccess dataAccess, IHttpDataAccess httpDataAccess, IConfigurationReader config)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _httpDataAccess = httpDataAccess;
            _config = config;
        }

        public void LookupHiscores(string username)
        {

        }

        public int AddUserToOSRSUsers(int id, string username, string gamemode)
        {
            _logger.LogInformation("rows affected "+_dataAccess.AddUserToOSRSUsers(id,username,gamemode).ToString());
            return 0;
        }

        public string CheckPlayerInWOM(string username)
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "x-api-key", _config.GetSection("WOMkey")},
                { "userAgent", _config.GetSection("DiscordName")}
            };
                var Uri = new UriBuilder(_womHost);
                Uri.Path = "/players/search";
                var asd = _httpDataAccess.HttpClientGetJson<List<WOMLookupDTO>>(_womHost + "/players/search" + $"?username={username}", headers);
                _logger.LogInformation(asd[0].DisplayName);
                AddUserToOSRSUsers(asd[0].Id, asd[0].DisplayName, asd[0].Type);
                return "";
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }
            return "";
        }
    }
}
