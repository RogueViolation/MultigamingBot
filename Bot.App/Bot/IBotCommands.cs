using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultigamingBot.Bot
{
    public interface IBotCommands
    {
        Task RedeemCodeAutomaticAsync(SocketMessage msg);

        Task HandleRedeemCommandAsync(SocketSlashCommand command);

        Task HandleListRoleCommandAsync(SocketSlashCommand command);

        Task SlashCommandHandlerAsync(SocketSlashCommand command);

        Task MessageReceivedAsync(SocketMessage msg);

        Task ClientReadyAsync();

        Task LogAsync(LogMessage msg);
    }
}
