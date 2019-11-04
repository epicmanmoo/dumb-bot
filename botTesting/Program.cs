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
using System.IO;

namespace botTesting
{
    class Program : ModuleBase<SocketCommandContext>
    {
        public readonly string weirdString = "sexsexsexseeeeeeeeeeexxxxxxxxxxxxxxxxxxxxxx66666969696wetsfsfscxvvc";

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
            Client.GuildMemberUpdated += GuildMemberUpdated;
            string[] lines = File.ReadAllLines(@"M:\token.txt");
            string token = lines[0];
            await Client.LoginAsync(TokenType.Bot, token);
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
            if (result.Error.Equals(CommandError.ObjectNotFound))
            {
                if (command.Value.Name.Equals("give"))
                {
                    await Context.Channel.SendMessageAsync("User does not exist.");
                    return;
                }
                if (command.Value.Name.Equals("view"))
                {
                    await Context.Channel.SendMessageAsync("Either the user has not signed up or does not exist!");
                    return;
                }
            }
            if (result.Error.Equals(CommandError.BadArgCount))
            {
                if (command.Value.Name.Equals("buydogs"))
                {
                    await Context.Channel.SendMessageAsync("Please provide the number of dogs you want to buy");
                    return;
                }
                if (command.Value.Name.Equals("lyrics"))
                {
                    await Context.Channel.SendMessageAsync("Format is `!lyrics <author> <song>`. Surround authors with quotes if the name is longer than one word!");
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
            await Client.SetGameAsync("[!]help");
        }

        public async Task AnnounceJoinedUser(SocketGuildUser User)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                SocketGuild guild = Context.Guild as SocketGuild;
                await CreateGuildInTable(guild.Id);
                SpecificCMDS joinmsg = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                string[] sjoinmsg = joinmsg.Joinmsgs.Split(weirdString);
                Random rand = new Random();
                int index = rand.Next(sjoinmsg.Length);
                await Context.Channel.SendMessageAsync($"{User.Username} has joined! {sjoinmsg[index]}");
            }
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
                SocketGuild guild = Context.Guild as SocketGuild;
                await CreateGuildInTable(guild.Id);
                SpecificCMDS leavemsg = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                string[] sleavemsg = leavemsg.Joinmsgs.Split(weirdString);
                Random rand = new Random();
                int index = rand.Next(sleavemsg.Length);
                await Context.Channel.SendMessageAsync($"{User.Nickname ?? User.Username} has left! {sleavemsg[index]}");
            }
        }
        public async Task CreateGuildInTable(ulong GuildId)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                if (DbContext.Spclcmds.Where(x => x.GuildId == GuildId).Count() < 1)
                {
                    DbContext.Add(new SpecificCMDS
                    {
                        GuildId = GuildId,
                        Joinmsgs = "",
                        Leavemsgs = "",
                        MsgPrefix = "!",
                        NameOfBot = "Bot"
                    });
                    await DbContext.SaveChangesAsync();
                }
                return;
            }
        }
        //change in DB
        public async Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            Console.WriteLine(before.Nickname + " " + after.Nickname);
            await before.Guild.GetTextChannel(597009241919979521).SendMessageAsync(before.Nickname + " " + after.Nickname);
        }
        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);
            string prefix = "";
            using (var DbContext = new SQLiteDBContext())
            {
                SocketGuild guild = Context.Guild as SocketGuild;
                await CreateGuildInTable(guild.Id);
                SpecificCMDS spref = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                prefix = spref.MsgPrefix;
            }
            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.Username.Equals(Client.CurrentUser.Username)) return;
            int ArgPos = 0;
            if (!(Message.HasStringPrefix(prefix, ref ArgPos))) return;
            var Result = await Commands.ExecuteAsync(Context, ArgPos, services);
            await Client.SetGameAsync("[" + prefix + "]help");
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Command] Something went wrong Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
            }
        }
    }
}
