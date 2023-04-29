using Bot.DataAccess;
using Bot.Utilities;
using Bot.DataAccess.DTO;
using Bot.DataObjects;
using Microsoft.Extensions.Logging;
using MultigamingBot.Configuration;
using Bot.DataObjects;
using System.Reflection.PortableExecutable;
using Bot.Constants;

namespace Bot.OSRSHiscores
{
    public class OSRSHiscoresHelper : IOSRSHiscoresHelper
    {
        private readonly string _hiscoreHost = "https://secure.runescape.com";
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

        public async Task<string> TryAddUserToOSRSUsers(string username)
        {
            var user = GetPlayerInWOM(username);
            switch (_dataAccess.ExactUserExistsInDatabase(user.Id, user.UserName, user.GameMode))
            {
                case -99:
                    return "User already exists, but has changed their name!";
                case 1:
                    return "User already exists!";
                case 0:
                    return await Task.FromResult(_dataAccess.AddUserToOSRSUsers(user.Id, user.UserName, user.GameMode)) == 1 ? "User successfully added!" : "Failed to add user to DB!";
                default:
                    return "Failed to fetch userdata from WOM!";
            }   
        }

        private OSRSUserBasic GetPlayerInWOM(string username)
        {
            try
            {
                WOMLookupDTO userData = new WOMLookupDTO();
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "x-api-key", _config.GetSection("WOMkey")},
                    { "userAgent", _config.GetSection("DiscordName")}
                };
                userData = _httpDataAccess.HttpClientGetJson<List<WOMLookupDTO>>(_womHost + "/players/search" + $"?username={username}", headers).FirstOrDefault();
                var basicData = new OSRSUserBasic();
                if (userData == null)
                    return basicData;
                else
                {
                    basicData.Id = userData.Id;
                    basicData.UserName = userData.Username;
                    basicData.GameMode = userData.Type;
                    return basicData;
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"GetPlayerInWOM failed. {e.Message}");
                return new OSRSUserBasic();
            }
        }
        
        //private int CheckUserNameChange(int userId, string username)
        //{

        //}
        //
        private void LookupHiscores(string username)
        {
            try
            {

                var gameModeMapper = new OSRSGameModes();
                var playerInfo = _dataAccess.GetOSRSBasicUserData(username);
                var userData = _httpDataAccess.HttpClientGetJson<OSRSHisccoreDTO>(_hiscoreHost + $"/m={gameModeMapper.GetGameMode(playerInfo.GameMode)}/index_lite.json" + $"?player={username}");
            }
            catch (Exception e)
            {
                _logger.LogError($"LookupHiscores Failed. {e.Message}");
            }

        }
    }
}
