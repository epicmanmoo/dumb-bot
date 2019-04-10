using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace botTesting.Currency
{
    public class Stones : ModuleBase<SocketCommandContext>
    {
        [Group("money")]
        public class StonesGroup : ModuleBase<SocketCommandContext>
        {
            [Command("")]
            public async Task Me()
            {

            }
            [Command("give")]
            public async Task Give(IUser User= null, int Amount= 0)
            {
                if(User == null)
                {
                    await Context.Channel.SendMessageAsync("Who tf should I give money to?");
                    return;
                }
                if (User.Username.Equals("myBot"))
                {
                    await Context.Channel.SendMessageAsync("Tf? What do you want me to do with money?");
                    return;
                }
                if (User.IsBot && !User.Username.Equals("myBot"))
                {
                    await Context.Channel.SendMessageAsync("Stop trying to give money to robots :rage:");
                    return;
                }
                if (User == Context.User)
                {
                    await Context.Channel.SendMessageAsync("**TF??** You can't give money to yourself :angry:");
                    return;
                }
                if (Amount == 0)
                {
                    await Context.Channel.SendMessageAsync("How much should I give bruh?");
                    return;
                }
                //SocketGuildUser CheckUser = Context.User as SocketGuildUser;
                //if (!CheckUser.GuildPermissions.Administrator)
                //{
                //    await Context.Channel.SendMessageAsync("You aren't admin bruh.");
                //    return;
                //}
               
                await Context.Channel.SendMessageAsync($"{User.Mention} got ${Amount} from {Context.User.Mention}");
                            
                await Data.SaveStones(User.Id, Amount);
                

            }
        }

    }
}
