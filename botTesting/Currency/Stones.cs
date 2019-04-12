using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
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
            public async Task Me(IUser User = null)
            {
                if (User == null)
                {
                    await Context.Channel.SendMessageAsync($"You got ${Data.GetStones(Context.User.Id)}");
                    return;
                }
                else if (User.IsBot)
                {
                    await Context.Channel.SendMessageAsync("Bots don't have money retard");
                    return;
                }
                else if (User.Equals(Context.User))
                {
                    await Context.Channel.SendMessageAsync("Why not just use the ``!money`` command dumbass");
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"{User.Mention} got ${Data.GetStones(User.Id)}");
                }
            }
            [Command("give")]
            public async Task Give(IUser User= null, int Amount= 0)
            {
                if(Amount < 0)
                {
                    await Context.Channel.SendMessageAsync("Can't give negative money :rage: Use ``!money take ...``");
                    return;
                }
                if (User == null)
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
            [Command("take")]
            public async Task Take(IUser User = null, int Amount= 0)
            {

            }
            [Command("reset")]
            public async Task Reset(IUser User = null)
            {
                 if(User == null)
                {
                    await Context.Channel.SendMessageAsync("Specify a user to reset bruh");
                    return;
                }
                if (User.IsBot)
                {
                    await Context.Channel.SendMessageAsync("Stop including bots :neutral_face:");
                    return;
                }

                using(var DbContext = new SQLiteDBContext())
                {
                    int Num= DbContext.Stones.Where(x => x.UserId == User.Id).Count();
                    if (Num == 0)
                    {
                        await Context.Channel.SendMessageAsync($"{User.Mention} was already deleted");
                        return;
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("You got reset, so you lost all your money :cry:");
                        DbContext.Stones.RemoveRange(DbContext.Stones.Where(x => x.UserId == User.Id));
                        await DbContext.SaveChangesAsync();
                    }
                }
            }
        }

    }
}
