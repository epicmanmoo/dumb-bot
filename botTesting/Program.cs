using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Collections.Generic;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Discord.Addons.Interactive;
using DiscordRPC;
using DiscordRPC.Logging;

namespace botTesting
{
    class Program : ModuleBase<SocketCommandContext>
    {

        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider services;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                MessageCacheSize = 100000000,
                AlwaysDownloadUsers = true
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            services = new ServiceCollection()
            .AddSingleton(Client)
            .AddSingleton<InteractiveService>()
            .BuildServiceProvider();
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

            Client.MessageReceived += Client_MessageReceived;
            Client.Ready += Client_Ready;
            Commands.CommandExecuted += Commands_CommandExecutedAsync;
            Client.UserJoined += AnnounceJoinedUser;
            Client.Log += Client_Log;
            Client.UserLeft += AnnounceLeavingUser;
            string Token = "NTY1MDQ4OTY5MjA2NjkzODg4.XK432A.z3Bcq5ZOsN9L_vErrmGW8hFryA8";
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private async Task Commands_CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext Context, IResult result)
        {
            SocketGuildUser Admin = Context.User as SocketGuildUser;
            bool isAdmin = Admin.GuildPermissions.Administrator;
            if (result.Error.Equals(CommandError.ParseFailed))
            {
                if (command.Value.Name.Equals("loop"))
                {
                    if (isAdmin)
                    {
                        await Context.Channel.SendMessageAsync("Use `!loop <amount> <input>`");
                        return;
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("You are not a mod!");
                        return;
                    }
                }
                if (command.Value.Name.Equals("give"))
                {
                    await Context.Channel.SendMessageAsync("Use `!give <user> <money>`");
                    return;
                }
            }
            if (result.Error.Equals(CommandError.UnknownCommand))
            {
                await Context.Channel.SendMessageAsync("That command does not exist");
            }
            if (result.Error.Equals(CommandError.ObjectNotFound))
            {
                if (command.Value.Name.Equals("give"))
                {
                    await Context.Channel.SendMessageAsync("User does not exist.");
                }
            }
            if (result.Error.Equals(CommandError.BadArgCount))
            {
                if (command.Value.Name.Equals("buydogs"))
                {
                    await Context.Channel.SendMessageAsync("Please provide the number of dogs you want to buy");
                    return;
                }
            }
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("!help");
        }

        public async Task AnnounceJoinedUser(SocketGuildUser User)
        {
            await Context.Channel.SendMessageAsync($"{User.Nickname ?? User.Username} has joined");
        }

        public async Task AnnounceLeavingUser(SocketGuildUser User)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                if (DbContext.Stones.Where(x => x.UserId == User.Id).Count() == 1)
                {
                    Stone Stone = DbContext.Stones.Where(x => x.UserId == User.Id).FirstOrDefault();
                    DbContext.Remove(Stone);
                    await DbContext.SaveChangesAsync();
                }
            }
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);
            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.Username.Equals("Retard Bot")) return;
            int ArgPos = 0;
            if (!(Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;
            var Result = await Commands.ExecuteAsync(Context, ArgPos, services);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }
        }
    }
}
