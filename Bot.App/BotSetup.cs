using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Discord;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using Discord.Commands;
using Discord.Net;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using MultigamingBot.DTO;
using MultigamingBot.Bot;
using MultigamingBot.Configuration;
using Bot.DBDataAccess;
using Bot.Utilities;

namespace MultigamingBot
{
    public class BotSetup
    {
        private readonly ILogger<BotSetup> _logger;
        private DiscordSocketClient _client;
        private readonly IBotCommands _command;
        private readonly IConfigurationReader _configuration;

        public BotSetup(ILogger<BotSetup> logger, IBotCommands command, IConfigurationReader configuration, IDiscordSocketClientProvider clientProvider)
        {
            _logger = logger;
            _command = command;
            _configuration = configuration;
            _client = clientProvider.ProvideClient();
        }

        public async void StartBot()
        {
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            await _client.SetGameAsync("for codes", null, ActivityType.Watching);

            _client.Log += _command.LogAsync;
            _client.MessageReceived += _command.MessageReceivedAsync;
            _client.Ready += _command.ClientReadyAsync;
            _client.SlashCommandExecuted += _command.SlashCommandHandlerAsync;

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = _configuration.GetSection("token");

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);

        }
    }
}
