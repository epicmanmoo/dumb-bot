using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using System.Collections.Generic;
using FluentScheduler;

namespace botTesting
{
    class Program : ModuleBase<SocketCommandContext>
    {

        private DiscordSocketClient Client;
        private CommandService Commands;
        public static List<string> JoinMsgList = new List<string>();
        public static List<string> LeaveMsgList = new List<string>();
        public static string prefix = "!";
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

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
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

        //Maybe make it so that if a command is spelled incorrectly but is similar to a command
        //that exists then tell the user the right way to type the command?
        private async Task Commands_CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext Context, IResult result)
        {
            //Make better error messages so user knows what they did wrong
            if (result.Error.Equals(CommandError.ParseFailed))
            {
                SocketGuildUser User = Context.User as SocketGuildUser;
                if (User.GuildPermissions.Administrator)
                {
                    if (command.Value.Name.Equals("loop"))
                    {
                        await Context.Channel.SendMessageAsync("Go back and reread how to loop dumbass");
                        return;
                    }
                    if (command.Value.Name.Equals("setmsgprefix"))
                    {
                        await Context.Channel.SendMessageAsync("Enter a valid prefix!");
                        return;
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Not a mod retard");
                }
            }
            else if (result.Error.Equals(CommandError.UnknownCommand))
            {
                await Context.Channel.SendMessageAsync("That command does not exist");
            }
            else if (result.Error.Equals(CommandError.ObjectNotFound))
            {
                await Context.Channel.SendMessageAsync("That user does not exist");
            }
            else if (result.Error.Equals(CommandError.BadArgCount))
            {
                //error for this type
            }
            //etc errors...
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("with your feelings");
        }
        
        private async Task Disconnected()
        {

        }

        public async Task AnnounceJoinedUser(SocketGuildUser User)
        {
            //567602259102531594
            var channel = Client.GetChannel(565413968643096578) as SocketTextChannel;
            Random Random = new Random();
            int Rand = Random.Next(JoinMsgList.Count);
            await channel.SendMessageAsync($"{User.Mention} has joined! " + JoinMsgList[Rand]);
            using (var DbContext = new SQLiteDBContext())
            {
                DbContext.Add(new Stone
                {
                    UserId = User.Id,
                    Amount = 0,
                    Warnings = 0,
                    Item1 = 0,
                    Item2 = 0,
                    Item3 = 0,
                    Item4 = 0,
                    Item5 = 0,
                    Item6 = 0,
                    Item7 = 0,
                    Item8 = 0,
                    Item9 = 0,
                    Item10 = 0,

                });
                await DbContext.SaveChangesAsync();
            }
        }

        public async Task AnnounceLeavingUser(SocketGuildUser User)
        {
            //567604758106472448
            var Channel = Client.GetChannel(565413968643096578) as SocketTextChannel;
            Random Rand = new Random();
            int randIndex = Rand.Next(LeaveMsgList.Count);
            await Channel.SendMessageAsync($"{User} has left. " + LeaveMsgList[randIndex]);
            using (var DbContext = new SQLiteDBContext())
            {
                Stone Stone = DbContext.Stones.Where(x => x.UserId == User.Id).FirstOrDefault();
                DbContext.Remove(Stone);
                await DbContext.SaveChangesAsync();
            }
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);
            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.Username.Equals("myBot")) return;
            int ArgPos = 0;
            if (!(Message.HasStringPrefix(prefix, ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;
            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }

        }
    }
}
