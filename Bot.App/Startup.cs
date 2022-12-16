using Bot.DataAccess;
using Bot.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using MultigamingBot.Bot;
using MultigamingBot.Configuration;
using Serilog;
using System;

namespace MultigamingBot
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddConsole();
            });
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("../logs/logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            services.AddOptions();
            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });
            services.AddSingleton<IBotCommands, BotCommands>()
                .AddTransient<IConfigurationReader, ConfigurationReader>()
                .AddTransient<IDBDataAccess, DBDataAccess>()
                .AddTransient<IHttpDataAccess, HttpDataAccess>()
                .AddTransient<IHttpClientProvider, HttpClientProvider>()
                .AddSingleton<IDiscordSocketClientProvider, DiscordSocketClientProvider>();
        }
    }

    class ConfigurationSection
    {
        public string Token { get; set; }
        public string Prefix { get; set; }

    }
}
