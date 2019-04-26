using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using Discord;
using System.Threading;

namespace botTesting
{
    public class ModCommands : ModuleBase<SocketCommandContext>
    {
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
                await Context.Channel.SendMessageAsync("Why **_THE FUCK_** would you create an invite for your server while on your **OWN** server");
            }
        }
        [Command("kick")]
        public async Task Kick(SocketGuildUser userAccount, string reason = "")
        {
            var user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Owner");
            if (!(userAccount.Roles.Contains(role)))
            {
                if (user.Roles.Contains(role))
                {
                    if (!(reason.Equals("")))
                    {
                        try
                        {
                            await Discord.UserExtensions.SendMessageAsync(userAccount, $"Your dumbass got kicked :joy: Reason: {reason}");
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
                    await Context.Channel.SendMessageAsync("Not a mod spedboi");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Admins can't be kicked dumbass");
            }
        }
        [Command("warn")]
        public async Task Warn(IGuildUser OtherUser, [Remainder] string reason ="")
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            if (User.GuildPermissions.Administrator)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    if(DbContext.Stones.Where(x => x.UserId == OtherUser.Id).Count() < 1)
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
                        WarningUpdate.Warnings++;
                        if(WarningUpdate.Warnings == 5)
                        {
                            IRole Role = OtherUser.Guild.Roles.FirstOrDefault(x => x.Name == "muted");
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
        [Command("clearwarn")]
        public async Task ClearWarn(SocketGuildUser User)
        {
            //clears warnings from database
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
                    if (!User.Guild.Roles.Contains(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted")))
                    {
                        await User.Guild.CreateRoleAsync("muted", new GuildPermissions());
                        await User.Guild.GetTextChannel(565413968643096578).AddPermissionOverwriteAsync(User.Guild.Roles.FirstOrDefault(x => x.Name == "muted"), new OverwritePermissions(sendMessages: PermValue.Deny));
                        await Context.Channel.SendMessageAsync(":x: Repeat command");
                        await UserExtensions.SendMessageAsync(User, "Do not remove the `muted` role created in the server");
                        return;
                    }
                    await OtherUser.AddRoleAsync(Role);
                    try
                    {
                        await Discord.UserExtensions.SendMessageAsync(OtherUser, "You have been muted on " + Context.Guild.Name);
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
        [Command("loop")]
        public async Task Loop(string Input = "", int Num = 0)
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
    }
}
