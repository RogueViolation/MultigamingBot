using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultigamingBot.DTO;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Bot.DataAccess;
using Bot.Utilities;
using System.Net.Http;
using MultigamingBot.Configuration;
using System.Threading;
using Bot.OSRSHiscores;

namespace MultigamingBot.Bot
{
    public class BotCommands : IBotCommands
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<BotCommands> _logger;
        private readonly IDBDataAccess _dataAccess;
        private readonly IConfigurationReader _config;
        private readonly IHttpDataAccess _httpDataAccess;
        private readonly IOSRSHiscoresHelper _osrsHiscoresHelper;
        private readonly EmbedBuilder embedBuilder;
        public BotCommands(ILogger<BotCommands> logger, IDBDataAccess dataAccess, IDiscordSocketClientProvider clientProvider, IConfigurationReader config, IHttpDataAccess httpDataAccess, IOSRSHiscoresHelper osrsHiscoresHelper)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _config = config;
            _client = clientProvider.ProvideDiscordSocketClient();
            _httpDataAccess = httpDataAccess;
            _osrsHiscoresHelper = osrsHiscoresHelper;
            embedBuilder = new EmbedBuilder();
        }
        public async Task HandleListRoleCommandAsync(SocketSlashCommand command)
        {
            // We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            // We remove the everyone role and select the mention of each role.
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embedBuiler.Build());
        }

        public async Task HandleRedeemCommandAsync(SocketSlashCommand command)
        {
            var embedBuilder = new EmbedBuilder();
            var guildUser = command.User;
            if (!await _dataAccess.CodeExists(command.Data.Options.First().Value.ToString()))
            {
                var data = await Task.Run(() => DoCodeRedeemRequest(command.Data.Options.First().Value.ToString()));

                switch (data.state)
                {
                    case "success":
                        embedBuilder
                            .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                            .WithTitle("NRZ Code Redeem")
                            .AddField(data.state.ToUpper(), data.message)
                            .WithColor(Color.Green)
                            .WithCurrentTimestamp();
                        break;
                    case "error":
                        embedBuilder
                            .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                            .WithTitle("NRZ Code Redeem failed")
                            .AddField(data.state.ToUpper(), data.message)
                            .WithColor(Color.Red)
                            .WithCurrentTimestamp();
                        break;
                    default:
                        embedBuilder
                            .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                            .WithTitle("NRZ Code Redeem fallback")
                            .AddField("Command fallback", $"Something went wrong while redeeming the code.")
                            .WithColor(Color.DarkRed)
                            .WithFooter("Probably HTTP Request failed...")
                            .WithCurrentTimestamp();

                        break;
                }

                // Now, Let's respond with the embed.
                ProcessCodeMessage(data.message, data.state, command.Data.Options.First().Value.ToString(), guildUser.Id);
                await command.RespondAsync(embed: embedBuilder.Build());
            }
        }

        public async Task RedeemCodeAutomaticAsync(SocketMessage msg)
        {
            try
            {
                var embedBuilder = new EmbedBuilder();
                var guildUser = msg.Author;
                var codeRedeemable = msg.Content.Substring(msg.Content.LastIndexOf("Copy this code ") + "Copy this code ".Length, 19);

                await Task.Delay(1000); //Add delay because when code is published it is not still active
                if (!await _dataAccess.CodeExists(codeRedeemable))
                {
                    var data = await Task.Run(() => DoCodeRedeemRequest(codeRedeemable));

                    switch (data.state)
                    {
                        case "ok":
                            embedBuilder
                                .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                                .WithTitle("NRZ Code Redeem")
                                .AddField(data.state.ToUpper(), Regex.Replace(data.message, "<.*?>", String.Empty))
                                .WithColor(Color.Green)
                                .WithCurrentTimestamp();
                            break;
                        case "error":
                            embedBuilder
                                .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                                .WithTitle("NRZ Code Redeem failed")
                                .AddField(data.state.ToUpper(), data.message)
                                .WithColor(Color.Red)
                                .WithCurrentTimestamp();
                            break;
                        default:
                            embedBuilder
                                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                                .WithTitle("NRZ Code Redeem fallback")
                                .AddField("Command fallback", $"Something went wrong while redeeming the code.")
                                .WithColor(Color.DarkRed)
                                .WithFooter("Probably HTTP Request failed...")
                                .WithCurrentTimestamp();

                            break;
                    }

                    await _client.GetGuild(443484727236624384).GetTextChannel(751866189306921111).SendMessageAsync(embed: embedBuilder.Build());
                    ProcessCodeMessage(data.message, data.state, codeRedeemable, guildUser.Id);
                }
                else
                {
                    embedBuilder
                        .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                        .WithTitle("NRZ Code Redeem fallback")
                        .AddField("Command fallback", "Code already exists in DB!")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp();

                    await _client.GetGuild(443484727236624384).GetTextChannel(751866189306921111).SendMessageAsync(embed: embedBuilder.Build());
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Automatic code redemption failed {e.Message}");
            }

            await Task.CompletedTask;
        }
        private async Task ProcessCodeMessage(string message, string status, string code, ulong author)
        {
            if (status == "ok")
            {
                string[] dataFields = new string[3]; //Avoid bad things
                dataFields = Regex.Match(message, @"\[(.*?)\]").Groups[1].Value.Split('/');
                await _dataAccess.ExecuteCodeRedeemedProcedure(code, dataFields[0] ?? "null", dataFields[1] ?? "null", dataFields[2] ?? "null", true, DateTime.Now.ToString(), author);
            }
            else //Check if code was already attempted
            {
                await _dataAccess.ExecuteCodeRedeemedProcedure(code, "null", "null", "null", false, DateTime.Now.ToString(), author);
            }

            await Task.CompletedTask;
        }

        public Task LogAsync(LogMessage msg)
        {
            _logger.LogInformation(msg.Message);

            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(SocketMessage msg)
        {
            if ((msg.Author.Id == 174623298146009089 || msg.Author.Username.Contains("vehicles-drop")) && msg.Content.Contains("Copy this code"))
            {
                await RedeemCodeAutomaticAsync(msg);
            }
            _logger.LogInformation($"Received message.");
            await Task.CompletedTask;
        }

        public async Task SlashCommandHandlerAsync(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "list-roles":
                    await HandleListRoleCommandAsync(command);
                    break;
                case "nrz-redeem":
                    await HandleRedeemCommandAsync(command);
                    break;
                case "nrz-codes":

                    break;
                case "osrs":
                    await HandleAddUserCommand(command);
                    break;
            }
        }

        public async Task ClientReadyAsync()
        {
            ulong guildId = 443484727236624384;
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            var guild = _client.GetGuild(guildId);

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var guildCommand = new SlashCommandBuilder()
            .WithName("list-roles")
            .WithDescription("Lists all roles of a user.")
            .AddOption("user", ApplicationCommandOptionType.User, "The users whose roles you want to be listed", isRequired: true);
            await guild.CreateApplicationCommandAsync(guildCommand.Build());

            guildCommand = new SlashCommandBuilder()
            .WithName("nrz-redeem")
            .WithDescription("Redeem NIGHTRIDERZ WORLD code.")
            .AddOption("code", ApplicationCommandOptionType.String, "Enter the code to be redeemed", isRequired: true);
            await guild.CreateApplicationCommandAsync(guildCommand.Build());

            guildCommand = new SlashCommandBuilder()
            .WithName("osrs")
            .WithDescription("TEST COMMAND DO NOT INTO PRODUCTION")
            .AddOption("name", ApplicationCommandOptionType.String, "name", isRequired: true);
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }

        private async Task<NRZResponse> DoCodeRedeemRequest(string codeRedeemable)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    {"Sec-Ch-Ua", "\"(Not(A:Brand\"; v = \"8\", \"Chromium\"; v = \"99\""},
                    {"Easharpptr-P", _config.GetSection("Easharpptr-P") },
                    {"Sec-Ch-Ua-Mobile", "?0"},
                    {"Easharpptr-U", _config.GetSection("Easharpptr-U")},
                    {"Sec-Ch-Ua-Platform", "Windows"},
                    {"Sec-Fetch-Site", "same-site"},
                    {"Sec-Fetch-Mode", "cors"},
                    {"Sec-Fetch-Dest", "empty"},
                };

            return _httpDataAccess.HttpClientPostJson<NRZResponse>("https://api.nightriderz.world/gateway.php", $"{{\"serviceName\":\"account\",\"methodName\":\"redeemcode\",\"parameters\":[\"{codeRedeemable}\"]}}", headers);
        }

        private async Task HandleAddUserCommand(SocketSlashCommand command)
        {
            var embedBuilder = new EmbedBuilder();
            var returnCode = await _osrsHiscoresHelper.TryAddUserToOSRSUsers(command.Data.Options.First().Value.ToString());

            switch (returnCode)
            {
                case 0:
                    embedBuilder
                        .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                        .WithTitle("Add user to OSRS Player DB")
                        .AddField("Insert failed!", "Add player failed!")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp();
                    await command.RespondAsync(embed: embedBuilder.Build());
                    break;
                case -99:
                    embedBuilder
                        .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                        .WithTitle("Add user to OSRS Player DB")
                        .AddField("Insert failed!", "User already exists!")
                        .WithColor(Color.Gold)
                        .WithCurrentTimestamp();
                    await command.RespondAsync(embed: embedBuilder.Build());
                    break;
                default:
                    embedBuilder
                        .WithAuthor(_client.CurrentUser.ToString(), _client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                        .WithTitle("Add user to OSRS Player DB")
                        .AddField("Successfully added!", "User was successfully added!")
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp();
                    await command.RespondAsync(embed: embedBuilder.Build());
                    break;
            }
        }



        public async Task PeriodicFooAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                //await Exit();
                await Task.Delay(interval, cancellationToken);
            }
        }
    }
}
