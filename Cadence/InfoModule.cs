using Discord.Commands;

public class InfoModule : ModuleBase<SocketCommandContext>
{
    [Command("Hello")]
    [Summary("Replies with \"World!\"")]
    public Task HelloAsync() => ReplyAsync("World!");
}