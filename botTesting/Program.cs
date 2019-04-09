using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;


namespace botTesting
{
    class Program
    {

        private DiscordSocketClient Client;
        private CommandService Commands;
        
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            string Token = "NTY1MDQ4OTY5MjA2NjkzODg4.XKwyZw.VXasdTjYji8qj96A0d7cJqrvN8M";
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
            await Task.Delay(-1);

        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("Playing with your feelings");
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);
            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;
            int ArgPos = 0;
            if (!(Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;
            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }

        }
    }
}
