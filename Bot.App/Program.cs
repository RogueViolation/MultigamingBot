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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MultigamingBot.Bot;
using Microsoft.Extensions.Configuration;
using System.Threading;
using MultigamingBot.Configuration;
using Bot.DBDataAccess;
using Bot.Utilities;

namespace MultigamingBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("Hello world!");

            logger.LogDebug("Logger is working!");

            RunFlow(serviceProvider.GetService<ILogger<BotSetup>>(), serviceProvider.GetService<IBotCommands>(), serviceProvider.GetService<IConfigurationReader>(), serviceProvider.GetService<IDiscordSocketClientProvider>());
            Thread.Sleep(-1);
        }
        public static void RunFlow(ILogger<BotSetup> logger, IBotCommands commands, IConfigurationReader configuration, IDiscordSocketClientProvider clientProvider) => new BotSetup(logger, commands, configuration, clientProvider).StartBot();
    }
}
