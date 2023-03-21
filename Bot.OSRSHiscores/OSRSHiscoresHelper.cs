using Bot.DataAccess;
using Bot.Utilities;
using Bot.DataAccess.DTO;
using Bot.OSRSHiscores.DataObjects;
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

        public async Task<int> TryAddUserToOSRSUsers(string username)
        {
            if (_dataAccess.UserExistsInDatabase(username))
            {
                return -99;
            }
            var user = GetPlayerInWOM(username);
            if (user == null) return -1;
            return _dataAccess.AddUserToOSRSUsers(user.Id, user.UserName, user.GameMode);
        }

        private OSRSUserBasic? GetPlayerInWOM(string username)
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "x-api-key", _config.GetSection("WOMkey")},
                    { "userAgent", _config.GetSection("DiscordName")}
                };
                var userData = _httpDataAccess.HttpClientGetJson<List<WOMLookupDTO>>(_womHost + "/players/search" + $"?username={username}", headers);
                return new OSRSUserBasic {
                    Id = userData.FirstOrDefault().Id,
                    UserName = userData.FirstOrDefault().DisplayName,
                    GameMode = userData.FirstOrDefault().Type
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation($"GetPlayerInWOM failed. {e.Message}");
            }
            return null;
        }

        private void LookupHiscores(string username)
        {

        }
    }
}
