﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Bot.Utilities
{
    public class DiscordSocketClientProvider : IDiscordSocketClientProvider
    {
        private readonly DiscordSocketClient _client;
        public DiscordSocketClientProvider()
        {
            _client = SetupClient();
        }
        public DiscordSocketClient ProvideDiscordSocketClient()
        {
            return _client;
        }
        private DiscordSocketClient SetupClient()
        {
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.AllUnprivileged
            };
            var client = new DiscordSocketClient(config);

            return client;
        }
    }
}
