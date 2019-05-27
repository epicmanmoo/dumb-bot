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
                        NameOfBot = "Retard Bot"
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
            if (!User.Guild.Roles.Contains(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted")))
            {
                await User.Guild.CreateRoleAsync("muted", new GuildPermissions());
                await User.Guild.GetTextChannel(565413968643096578).AddPermissionOverwriteAsync(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted"), new OverwritePermissions(sendMessages: PermValue.Deny));
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
  
        //USING DB FOR BELOW-------------------------
        [Command("addjoinmsg")]
        public async Task SetJoinMsg([Remainder] string msg = "")
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            if (You.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuild guild = Context.Guild as SocketGuild;
                    string weirdString = "sexsexsexseeeeeeeeeeexxxxxxxxxxxxxxxxxxxxxx66666969696wetsfsfscxvvc";
                    await CreateGuildInTable(guild.Id);
                    SpecificCMDS scmds = DbContext.Spclcmds.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                    if (!msg.Equals(""))
                    {
                        scmds.Joinmsgs += msg + weirdString;
                        await Context.Channel.SendMessageAsync($"Join message, `{msg}`, stored");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Enter a join message to store!");
                    }
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
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
                    string weirdString = "sexsexsexseeeeeeeeeeexxxxxxxxxxxxxxxxxxxxxx66666969696wetsfsfscxvvc";
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
                        }
                        string thisjoinmsg = parsejoinmsgs[iIndex] + weirdString;
                        scmds.Joinmsgs = scmds.Joinmsgs.Replace(thisjoinmsg, "");
                        await Context.Channel.SendMessageAsync($"Join message, `{thisjoinmsg.Substring(0, thisjoinmsg.Length - weirdString.Length)}` deleted");
                    }
                    await DbContext.SaveChangesAsync();
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You're not a mod!");
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
                        string weirdString = "sexsexsexseeeeeeeeeeexxxxxxxxxxxxxxxxxxxxxx66666969696wetsfsfscxvvc";
                        string[] parsejoinmsgs = scmds.Joinmsgs.Split(weirdString);
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithTitle("**List of Join Messages**");
                        embed.WithColor(Color.DarkMagenta);
                        for(int i= 0; i < parsejoinmsgs.Length - 1; i++)
                        {
                            embed.AddField($"Index {i+1}: ", parsejoinmsgs[i], true);
                            Console.WriteLine(parsejoinmsgs.Length);
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
        }
        [Command("editjoinmsgs")]
        public async Task EditJoinMsgs(int index = 0, [Remainder] string Msg = "")
        {
            using (var DbContext = new SQLiteDBContext())
            {

            }
        }
        [Command("addleavemsg")]
        public async Task AddLeaveMsg([Remainder] string msg = "")
        {
            using (var DbContext = new SQLiteDBContext())
            {

            }
        }
        [Command("clearleavemsgs")]
        public async Task ClearLeaveMsgs(string index = "0")
        {

        }
        [Command("leavemsgs")]
        public async Task LeaveMsgs()
        {
            using (var DbContext = new SQLiteDBContext())
            {

            }
        }
        [Command("editleavemsgs")]
        public async Task UpdateLeaveMsgs(int index = 0, [Remainder] string msg = "")
        {
            using (var DbContext = new SQLiteDBContext())
            {

            }
        }
        [Command("setmsgsprefix")]
        public async Task MsgPrefix(string prefix = "")
        {
            using (var DbContext = new SQLiteDBContext())
            {

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
                    var Bot = guild.GetUser(565048969206693888);
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
            }
        }
        //-----------------------------------------------
    }
}
