using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
using Discord;

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
    }
}
