using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using Discord;
using Discord.Rest;

namespace botTesting
{
    public class ModCommands : ModuleBase<SocketCommandContext>
    {
        public static string nickNameOfBot = "Retard Bot"; //should be in database
        [Command("serverinvite")]
        public async Task ServerInvite(ulong GuildId)
        {
            SocketGuildUser CheckUser = Context.User as SocketGuildUser;
            if (!(CheckUser.GuildPermissions.Administrator))
            {
                await Context.Channel.SendMessageAsync("You are not a bot moderator dumbass :stuck_out_tongue:");
                return;
            }

            if (Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1)
            {
                await Context.Channel.SendMessageAsync("Can't send an invite to a server I'm not in :rage:");
                return;
            }

            if (!(Context.Guild.Id == GuildId))
            {

                SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
                var Invites = await Guild.GetInvitesAsync();
                if (Invites.Count() < 1)
                {
                    try
                    {
                        await Guild.TextChannels.First().CreateInviteAsync(86400);
                    }
                    catch (Exception Ex)
                    {
                        await Context.Channel.SendMessageAsync($"Creating an invite for guild {Guild.Name} went wrong with error: ``{Ex.Message}``");
                        return;
                    }
                }
                Invites = null;
                Invites = await Guild.GetInvitesAsync();
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor($"Invite for Server: {Guild.Name}", Guild.IconUrl);
                Embed.WithColor(40, 200, 150);
                Embed.WithFooter($"Owner: {Guild.Owner.Username}");
                Embed.WithCurrentTimestamp();
                foreach (var Current in Invites)
                {
                    Embed.AddField("Invite: ", $"[Click here to join!]({Current.Url})");
                }
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("Why would you create an invite for your server while on your own server");
            }
        }
        [Command("kick")]
        public async Task Kick(SocketGuildUser userAccount, [Remainder] string reason = "")
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            if (user.GuildPermissions.Administrator)
            {
                if (!(reason.Equals("")))
                {
                    try
                    {
                        await UserExtensions.SendMessageAsync(userAccount, $"Your dumbass got kicked :joy: Reason: {reason}");
                        await userAccount.KickAsync();
                    }
                    catch (Discord.Net.HttpException Ex)
                    {
                        Console.WriteLine(Ex.Message);
                    }
                    await Context.Channel.SendMessageAsync($"`{userAccount}` has been kicked. Reason: `{reason}`");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Include reason to kick :rolling_eyes:");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Admins can't be kicked dumbass");
            }
        }
        [Command("warn")]
        public async Task Warn(SocketGuildUser OtherUser, [Remainder] string reason = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {

                using (var DbContext = new SQLiteDBContext())
                {
                    if (DbContext.Stones.Where(x => x.UserId == OtherUser.Id).Count() < 1)
                    {
                        DbContext.Add(new Stone
                        {
                            UserId = OtherUser.Id,
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
                    if (!reason.Equals(""))
                    {
                        Stone WarningUpdate = DbContext.Stones.Where(x => x.UserId == OtherUser.Id).FirstOrDefault();
                        int CheckWarning = OtherUser.Roles.Count(x => x.Name == "muted");
                        if (CheckWarning == 1)
                        {
                            await Context.Channel.SendMessageAsync($"{OtherUser.Mention} is already muted");
                            return;
                        }
                        WarningUpdate.Warnings++;
                        if (WarningUpdate.Warnings == 5)
                        {
                            IRole Role = OtherUser.Guild.Roles.FirstOrDefault(x => x.Name == "muted");
                            await MuteRole();
                            await OtherUser.AddRoleAsync(Role);
                            WarningUpdate.Warnings = 0;
                            DbContext.Update(WarningUpdate);
                            await Context.Channel.SendMessageAsync($"{OtherUser.Mention} has been muted");
                            await DbContext.SaveChangesAsync();
                            return;
                        }
                        await Context.Channel.SendMessageAsync($"{OtherUser.Mention} has been warned. Warning: {reason}. Warning #{WarningUpdate.Warnings}");
                        DbContext.Update(WarningUpdate);
                        await DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Enter a reason to warn specified user");
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Not a mod retard");
            }
        }
        [Command("clearwarns")]
        public async Task ClearWarn(SocketGuildUser OtherUser)
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    Stone Warnings = DbContext.Stones.Where(x => x.UserId == OtherUser.Id).FirstOrDefault();
                    Warnings.Warnings = 0;
                    DbContext.Update(Warnings);
                    await DbContext.SaveChangesAsync();
                    await Context.Channel.SendMessageAsync($"Clearned warnings for {OtherUser.Mention}");
                }
            }
        }
        [Command("warns")]
        public async Task Warns(SocketGuildUser OtherUser)
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    Stone Warnings = DbContext.Stones.Where(x => x.UserId == OtherUser.Id).FirstOrDefault();
                    if (Warnings.Warnings == 1)
                    {
                        await Context.Channel.SendMessageAsync($"{OtherUser.Mention} has " + Warnings.Warnings + " Warning");
                        return;
                    }
                    await Context.Channel.SendMessageAsync($"{OtherUser.Mention} has " + Warnings.Warnings + " Warnings");
                }
            }
        }
        [Command("mute")]
        public async Task Mute(SocketGuildUser OtherUser)
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                IRole Role = OtherUser.Guild.Roles.FirstOrDefault(x => x.Name == "muted");
                if (!OtherUser.Roles.Contains(Role))
                {
                    await MuteRole();
                    await OtherUser.AddRoleAsync(Role);
                    try
                    {
                        await UserExtensions.SendMessageAsync(OtherUser, "You have been muted on " + Context.Guild.Name);
                    }
                    catch (Discord.Net.HttpException Ex)
                    {
                        Console.WriteLine(Ex.Message);
                    }
                    await Context.Channel.SendMessageAsync($"Muted {OtherUser.Mention}");
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"{OtherUser.Mention} is already muted");
                    return;
                }
            }
        }
        public async Task MuteRole()
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            IRole Role = User.Guild.Roles.FirstOrDefault(x => x.Name == "muted");
            if (!User.Guild.Roles.Contains(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted")))
            {
                await User.Guild.CreateRoleAsync("muted", new GuildPermissions());
                await User.Guild.GetTextChannel(565413968643096578).AddPermissionOverwriteAsync(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted"), new OverwritePermissions(sendMessages: PermValue.Deny));
                await Context.Channel.SendMessageAsync(":x: Repeat command");
                await UserExtensions.SendMessageAsync(User, "Do not remove the `muted` role created in the server");
                return;
            }
            IEnumerable<SocketRole> Roles = Context.Guild.Roles;
            SocketRole[] SortingArr = Roles.OrderByDescending(x => x.Position).ToArray();
            int IndexOfMuted = 0;
            SocketRole Temp = SortingArr[0];
            for (int i = 0; i < SortingArr.Length; i++)
            {
                if (SortingArr[i].ToString().Equals("muted"))
                {
                    IndexOfMuted = i;
                    break;
                }
            }
            for (int i = 0; i < SortingArr.Length; i++)
            {
                if (SortingArr[i].IsManaged)
                {
                    Temp = SortingArr[i];
                    SortingArr[i] = SortingArr[IndexOfMuted];
                    SortingArr[IndexOfMuted] = Temp;
                    break;
                }
            }
            await Role.ModifyAsync(x => x.Position = Temp.Position);
        }
        [Command("unmute")]
        public async Task UnMute(SocketGuildUser OtherUser)
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            IRole Role = OtherUser.Roles.FirstOrDefault(x => x.Name == "muted");
            if (User.GuildPermissions.Administrator)
            {
                if (OtherUser.Roles.Contains(Role))
                {
                    await OtherUser.RemoveRoleAsync(Role);
                    await Context.Channel.SendMessageAsync($"Unmuted {OtherUser.Mention}");
                    return;
                }
                await Context.Channel.SendMessageAsync("User is not muted");
            }
        }
        [Command("loop")]
        public async Task Loop(int Num = 0, [Remainder] string Input = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                if (!Input.Equals(""))
                {
                    for (int i = 0; i < Num; i++)
                    {
                        await Context.Channel.SendMessageAsync(Input);
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Include a phrase/number bruh");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Not a mod retard");
            }
        }
        [Command("ban")]//make it better, send user a message for why, etc.
        public async Task Ban(SocketGuildUser OtherUser)
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                await Context.Channel.SendMessageAsync($"{OtherUser} was banned");
                await OtherUser.BanAsync();
            }
        }
        //These commands below should be in a databse
        //------------------------------------------------
        [Command("addjoinmsg")]
        public async Task SetJoinMsg([Remainder] string msg = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                if (!msg.Equals(""))
                {
                    if (Program.JoinMsgList.Contains(msg))
                    {
                        await Context.Channel.SendMessageAsync("The join message list already has this message!");
                        return;
                    }
                    Program.JoinMsgList.Add(msg);
                    await Context.Channel.SendMessageAsync("``" + msg + "``" + " added to join message list");
                }
            }
        }
        [Command("clearjoinmsgs")]
        public async Task ClearJoinMsgs(string index = "0")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;

            if (User.GuildPermissions.Administrator)
            {
                if (Program.JoinMsgList.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("No messages to be removed!");
                    return;
                }
                if (index.Equals("all"))
                {
                    Program.JoinMsgList.Clear();
                    await Context.Channel.SendMessageAsync("All messages removed!");
                    return;
                }
                int parsedIndex = Int32.Parse(index);
                if (parsedIndex > Program.JoinMsgList.Count)
                {
                    await Context.Channel.SendMessageAsync("That index does not exist!");
                    return;
                }
                else if (parsedIndex > 0)
                {
                    await Context.Channel.SendMessageAsync("`" + Program.JoinMsgList[parsedIndex - 1] + "`" + " removed from join message list");
                    Program.JoinMsgList.RemoveAt(parsedIndex - 1);
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Enter a valid index or type ``all`` to remove all!");
                    return;
                }
            }
        }
        [Command("joinmsgs")]
        public async Task ShowJoinMsgs()
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("**Join Messages**");
                if (Program.JoinMsgList.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("No join messages in list");
                    return;
                }
                for (int i = 0; i < Program.JoinMsgList.Count; i++)
                {
                    Embed.WithColor(40, 200, 150);
                    Embed.AddField("Index " + (i + 1) + ":", "• " + Program.JoinMsgList[i]);
                }
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }
        [Command("editjoinmsgs")]
        public async Task EditJoinMsgs(int index = 0, [Remainder] string Msg = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                if (!Msg.Equals(""))
                {
                    if (index > 0)
                    {
                        Program.JoinMsgList[index - 1] = Msg;
                        await Context.Channel.SendMessageAsync("Updated join message at index " + index);
                    }
                }
            }
        }
        [Command("leavemsgs")]
        public async Task LeaveMsgs()
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithTitle("**Leave Messages**");
                if (Program.JoinMsgList.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("No leave messages in list");
                    return;
                }
                for (int i = 0; i < Program.LeaveMsgList.Count; i++)
                {
                    Embed.WithColor(40, 200, 150);
                    Embed.AddField("Index " + (i + 1) + ":", "• " + Program.LeaveMsgList[i]);
                }
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }
        [Command("addleavemsg")]
        public async Task AddLeaveMsg([Remainder] string msg = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                if (!msg.Equals(""))
                {
                    if (!Program.LeaveMsgList.Contains(msg))
                    {
                        Program.LeaveMsgList.Add(msg);
                        await Context.Channel.SendMessageAsync("`" + msg + "`" + "message added to leave message list");
                        return;
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Leave message already exists");
                        return;
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Enter a valid message");
                    return;
                }
            }
        }
        [Command("clearleavemsgs")]
        public async Task ClearLeaveMsgs(string index = "0")
        {

        }
        [Command("editleavemsgs")]
        public async Task UpdateLeaveMsgs(int index = 0, [Remainder] string msg = "")
        {

        }
        //Prefix default is '!'
        [Command("setmsgsprefix")]
        public async Task MsgPrefix(string prefix = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            SocketSelfUser Bot = Context.Client.CurrentUser as SocketSelfUser;
            if (User.GuildPermissions.Administrator)
            {
                if (!prefix.Equals(""))
                {
                    var guild = User.Guild;
                    var user = guild.GetUser(565048969206693888);
                    string nickname = user.Nickname;
                    Program.prefix = prefix;
                    await user.ModifyAsync(x => x.Nickname = "[" + Program.prefix + "] " + nickNameOfBot);
                    //await Bot.ModifyAsync(x => x.Username = "[" + Program.prefix + "] " + nameOfBot);
                    await Context.Channel.SendMessageAsync("Command prefix set to `" + prefix + "`");
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Enter a prefix!");
                }
            }
        }
        [Command("changebotnickname")]
        public async Task ChangeBotNickName([Remainder] String name = "")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                if (!name.Equals(""))
                {
                    var guild = User.Guild;
                    var user = guild.GetUser(565048969206693888);
                    string nickname = user.Nickname;
                    if (name.Equals(nickname))
                    {
                        await Context.Channel.SendMessageAsync("That is my current nickname!");
                        return;
                    }
                    nickNameOfBot = name;
                    await user.ModifyAsync(x => x.Nickname = "[" + Program.prefix + "] " + name);
                    await Context.Channel.SendMessageAsync($"My nickname has been changed to `{name}`!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Enter a nickname!");
                }
            }
        }
        //------------------------------------------------
    }
}
