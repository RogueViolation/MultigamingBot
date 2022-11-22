using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Utilities
{
    public interface IDiscordSocketClientProvider
    {
        DiscordSocketClient ProvideClient();
    }
}
