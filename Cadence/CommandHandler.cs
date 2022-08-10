using Discord.WebSocket;
using Discord.Commands;

namespace Cadence
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient DiscordClient;
        private readonly CommandService Commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            DiscordClient = client ?? throw new ArgumentNullException(nameof(client));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public async Task InstallCommandsAsync()
        {
            DiscordClient.MessageReceived += Handle_MessageReceived;

            await Commands.AddModulesAsync(assembly: System.Reflection.Assembly.GetEntryAssembly(), services: null);
        }

        private async Task Handle_MessageReceived(SocketMessage socketMessage)
        {
            SocketUserMessage? Message = socketMessage as SocketUserMessage;
            int ArgumentPosition = 0;

            if (Message == null)
            {
                return;
            }
            else if
                (
                    (!Message.HasCharPrefix('!', ref ArgumentPosition) ||
                    Message.HasMentionPrefix(DiscordClient.CurrentUser, ref ArgumentPosition)) ||
                    Message.Author.IsBot
                )
            {
                return;
            }

            SocketCommandContext Context = new SocketCommandContext(DiscordClient, Message);

            await Commands.ExecuteAsync(
                    context : Context
                    ,argPos: ArgumentPosition
                    ,services: null
                );
        }
    }
}
