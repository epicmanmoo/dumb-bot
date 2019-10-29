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
        public readonly string weirdString = "sexsexsexseeeeeeeeeeexxxxxxxxxxxxxxxxxxxxxx66666969696wetsfsfscxvvc";
        public async Task CreateUserInTable(SocketGuildUser OtherUser = null)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                if (OtherUser != null)
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
                    return;
                }
                return;
            }
        }
        public async Task CreateGuildInTable(ulong GuildId)
        {
            using(var DbContext = new SQLiteDBContext())
            {
                if(DbContext.Spclcmds.Where(x => x.GuildId == GuildId).Count() < 1)
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
        [Command("serverinvite")]
        public async Task ServerInvite(ulong GuildId)
        {
            SocketGuildUser CheckUser = Context.User as SocketGuildUser;
            if (!(CheckUser.GuildPermissions.Administrator))
            {
                await Context.Channel.SendMessageAsync("You are not a mod!");
                return;
            }

            if (Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1)
            {
                await Context.Channel.SendMessageAsync("Can't send an invite to a server I'm not in!");
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
                await Context.Channel.SendMessageAsync("Why would you create an invite for your server while on your own server?");
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
                    await CreateUserInTable(User);                  
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
                await Context.Channel.SendMessageAsync("Not a mod");
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
                    await CreateUserInTable(User);
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
                    await CreateUserInTable(User);
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
            if (!User.Guild.Roles.Contains(Role))
            {
                await User.Guild.CreateRoleAsync("muted", new GuildPermissions());
                await User.Guild.GetTextChannel(Context.Channel.Id).AddPermissionOverwriteAsync(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted"), new OverwritePermissions(sendMessages: PermValue.Deny));
                await Context.Channel.SendMessageAsync(":x: Repeat command (one time thing)");
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
        //make this command better!
        [Command("ban")]
        public async Task Ban(SocketGuildUser OtherUser)
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                await Context.Channel.SendMessageAsync($"{OtherUser} was banned");
                await OtherUser.BanAsync();
            }
        }
        //check for duplicates on this and leave msgs!
        [Command("addjoinmsg")]
        public async Task SetJoinMsg([Remainder] string msg = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS scmds = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if (!msg.Equals(""))
                    {
                        scmds.Joinmsgs += msg + weirdString;
                        await Context.Channel.SendMessageAsync($"Join message, `{msg}`, stored");
                        await DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Enter a join message to store!");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("clearjoinmsgs")]
        public async Task ClearJoinMsgs(string index = "0")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS scmds = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if (scmds.Joinmsgs.Length == 0)
                    {
                        await Context.Channel.SendMessageAsync("No join messages in list!");
                        return;
                    }
                    string[] parsejoinmsgs = scmds.Joinmsgs.Split(weirdString);
                    if (index.Equals("all"))
                    {
                        scmds.Joinmsgs = scmds.Joinmsgs.Replace(scmds.Joinmsgs, "");
                        await Context.Channel.SendMessageAsync("All join messages deleted");
                    }
                    else
                    {
                        int iIndex = int.Parse(index) - 1;
                        if (iIndex < 0 || iIndex > parsejoinmsgs.Length)
                        {
                            await Context.Channel.SendMessageAsync("Invalid index");
                            return;
                        }
                        string thisjoinmsg = parsejoinmsgs[iIndex];
                        scmds.Joinmsgs = scmds.Joinmsgs.Replace(thisjoinmsg + weirdString, "");
                        await Context.Channel.SendMessageAsync($"Join message, `{thisjoinmsg}` deleted");
                    }
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("joinmsgs")]
        public async Task ShowJoinMsgs()
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS scmds = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if(scmds.Joinmsgs.Length > 0)
                    {
                        string[] parsejoinmsgs = scmds.Joinmsgs.Split(weirdString);
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithTitle("**List of Join Messages**");
                        embed.WithColor(Color.DarkMagenta);
                        for(int i= 0; i < parsejoinmsgs.Length - 1; i++)
                        {
                            embed.AddField($"Index {i+1}: ", parsejoinmsgs[i], true);
                            if (i+2 != parsejoinmsgs.Length)
                            {
                                embed.AddField("\u200b", "\u200b");
                            }
                        }
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("No join messages in list!");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("editjoinmsgs")]
        public async Task EditJoinMsgs(int index = 0, [Remainder] string msg = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS edit = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if(edit.Joinmsgs.Length > 0)
                    {
                        string[] psmsgs = edit.Joinmsgs.Split(weirdString);
                        if(index >=1 && index < psmsgs.Length)
                        {
                            if (!msg.Equals(""))
                            {
                               string msgtoedit = psmsgs[index-1] + weirdString;
                               edit.Joinmsgs = edit.Joinmsgs.Replace(msgtoedit, msg + weirdString);
                               await DbContext.SaveChangesAsync();
                               await Context.Channel.SendMessageAsync($"Join msg `{msgtoedit.Substring(0, msgtoedit.Length - weirdString.Length)}` changed to `{msg}`");
                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Message cannot be empty!");
                                return;
                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Invalid index");
                            return;
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("No join messages to edit!");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("addleavemsg")]
        public async Task AddLeaveMsg([Remainder] string msg = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    if (!msg.Equals(""))
                    {
                        SpecificCMDS lmsg = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                        lmsg.Leavemsgs += msg + weirdString;
                        await Context.Channel.SendMessageAsync($"Leave msg, `{msg}`, stored");
                        await DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Message cannot be empty!");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("clearleavemsgs")]
        public async Task ClearLeaveMsgs(string index = "0")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                SocketGuild guild = Context.Guild as SocketGuild;
                await CreateGuildInTable(guild.Id);
                using (var DbContext = new SQLiteDBContext())
                {
                    SpecificCMDS lmsgs = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if (lmsgs.Leavemsgs.Length > 0)
                    {
                        if (index.Equals("all"))
                        {
                            lmsgs.Leavemsgs = lmsgs.Leavemsgs.Replace(lmsgs.Leavemsgs, "");
                            await Context.Channel.SendMessageAsync("All leave messages removed!");
                        }
                        else
                        {
                            int iindex = int.Parse(index) - 1;
                            string[] slmsgs = lmsgs.Joinmsgs.Split(weirdString);
                            if (iindex > 0 || (iindex < slmsgs.Length))
                            {
                                string msgtodel = slmsgs[iindex];
                                lmsgs.Leavemsgs.Replace(msgtodel + weirdString, "");
                                await Context.Channel.SendMessageAsync($"Leave message, {msgtodel}, deleted!");                                
                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Invalid index");
                                return;
                            }                        
                        }
                        await DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Leave message list empty.");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("leavemsgs")]
        public async Task LeaveMsgs()
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS msgs = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if(msgs.Leavemsgs.Length > 0)
                    {
                        string[] splitlmsgs = msgs.Leavemsgs.Split(weirdString);
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithTitle("**List of Leave Messages**");
                        embed.WithColor(Color.DarkMagenta);
                        for (int i = 0; i < splitlmsgs.Length - 1; i++)
                        {
                            embed.AddField($"Index {i + 1}: ", splitlmsgs[i], true);
                            if (i + 2 != splitlmsgs.Length)
                            {
                                embed.AddField("\u200b", "\u200b");
                            }
                        }   
                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("No leave messages in list!");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("editleavemsgs")]
        public async Task EditLeaveMsgs(int index = 0, [Remainder] string msg = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS msgs = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    string[] slmsgs = msgs.Leavemsgs.Split(weirdString);
                    if (index >= 1 && index < slmsgs.Length)
                    {
                        if (!msg.Equals(""))
                        {
                            string msgtoedit = slmsgs[index - 1];
                            msgs.Leavemsgs.Replace(msgtoedit + weirdString, msg);
                            await Context.Channel.SendMessageAsync($"Leave msg, {msgtoedit}, changed to {msg}.");   
                            await DbContext.SaveChangesAsync();
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Leave message cannot be blank!");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Invalid index");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("setmsgprefix")]
        public async Task MsgPrefix(string prefix = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    DiscordSocketClient cli = Context.Client as DiscordSocketClient;
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS msgPrefix = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if(prefix.Contains("\\s"))
                    {
                        await Context.Channel.SendMessageAsync("Prefix cannot contain spaces!");
                        return;
                    }
                    if (!prefix.Equals(""))
                    {
                        msgPrefix.MsgPrefix = prefix;
                        await DbContext.SaveChangesAsync();
                        await cli.SetGameAsync("[" + prefix + "]help");
                        await Context.Channel.SendMessageAsync($"Message prefix has been set to `{prefix}`");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Message prefix cannot be empty!");
                        return;
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("changebotnickname")]
        public async Task ChangeBotNickName([Remainder] String name = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    var Bot = guild.GetUser(guild.CurrentUser.Id);
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS scmds = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if (!name.Equals(""))
                    {
                        scmds.NameOfBot = name;
                        await Bot.ModifyAsync(x => x.Nickname = scmds.NameOfBot);
                        await Context.Channel.SendMessageAsync($"My name has been changed to `{name}`");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Enter a valid nickname for the bot!");
                    }
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("userstates")]
        public async Task UserStates()
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                SocketGuild guild = Context.Guild as SocketGuild;
                int offline = 0, online = 0, idle = 0, dnd = 0;
                foreach (var user in guild.Users)
                {
                    if (!user.IsBot)
                    {
                        switch (user.Status)
                        {
                            case UserStatus.Offline:
                                offline++;
                                break;
                            case UserStatus.Online:
                                online++;
                                break;
                            case UserStatus.Idle:
                                idle++;
                                break;
                            case UserStatus.DoNotDisturb:
                                dnd++;
                                break;
                        }
                    }
                }
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle("**User States**");
                embed.AddField("Offline Count", offline);
                embed.AddField("Online Count", online);
                embed.AddField("Idle Count", idle);
                embed.AddField("Do Not Disturb Count", dnd);
                embed.WithColor(Color.DarkBlue);
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
                return;
            }
        }
        [Command("purge")]
        public async Task Purge(int num = 0)
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                if (!(num <= 0) && (num < 100))
                {
                    IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(num+1).FlattenAsync();
                    int count = messages.Count();
                    if(count == 0)
                    {
                        await Context.Channel.SendMessageAsync("No messages to delete!");
                        return;
                    }
                    await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                    IUserMessage msg;
                    if(num == 1)
                    {
                        msg = await ReplyAsync($"Deleted 1 message!");
                    }
                    else
                    {
                        if (num > count)
                        {
                            msg = await ReplyAsync($"Deleted all messages in this channel!");
                        }
                        else
                        {
                            msg = await ReplyAsync($"Deleted {num} messages!");
                        }
                    }
                    await Task.Delay(3000);
                    await msg.DeleteAsync();
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Enter a valid number of messages to delete (max is 99)");
                    return;
                }
            }
        }
    }
}   