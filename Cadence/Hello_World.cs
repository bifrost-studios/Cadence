using System.Reflection;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using NLog;
using NLog.Extensions.Logging;

namespace Cadence
{
    public class HelloWorld
    {
        private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly Logger Logger = LogManager.GetLogger(AppName + ".log");
        public static Task Main() => new HelloWorld().MainAsync();

        public async Task MainAsync()
        {

            IConfiguration Config = new ConfigurationBuilder()
            .AddJsonFile("C:\\Config\\Cadence.json")
            .AddJsonFile("C:\\Config\\NLog.json")
            .Build();

            Config.GetSection("NLog:targets:MainLogFile:fileName").Value += Config["LogFile"];
            Config.GetSection("NLog:targets:MainLogFile:archiveFileName").Value += Config["ArchiveLogFile"];
            Config.GetSection("NLog:rules:logging:minLevel").Value += Config["LogLevel"];

            LogManager.Configuration = new NLogLoggingConfiguration(Config.GetSection("NLog"));

            try
            {
                var Commands = new CommandService();

                var DiscordClient = new DiscordSocketClient();
                DiscordClient.Log += Log;

                var Token = File.ReadAllText(Config["Token"]).Trim();

                await DiscordClient.LoginAsync(TokenType.Bot, Token);
                await DiscordClient.StartAsync();

                var Handler = new CommandHandler(DiscordClient, Commands);

                await Handler.InstallCommandsAsync();

                await Task.Delay(-1);

            }catch (Exception E)
            {
                await Log(new LogMessage(LogSeverity.Error, E.Source, E.Message, E));
            }
        }



        private Task Log(LogMessage message)
        {
            Logger.Error(message.ToString());
            return Task.CompletedTask;
        }
    }
}
