using Bot.DBDataAccess;
using Bot.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultigamingBot.Bot;
using MultigamingBot.Configuration;

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
            services.AddOptions();
            services.AddSingleton<IBotCommands, BotCommands>()
                .AddTransient<IConfigurationReader, ConfigurationReader>()
                .AddTransient<IDBDataAccess, DBDataAccess>()
                .AddSingleton<IDiscordSocketClientProvider, DiscordSocketClientProvider>();
        }
    }

    class ConfigurationSection
    {
        public string Token { get; set; }
        public string Prefix { get; set; }

    }
}
