using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                    if (Data.GetStones(Context.User.Id) == 0)
                    {
                        await Context.Channel.SendMessageAsync("Lmao, you got nothing. Broke af :joy:");
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync($"You have ${Data.GetStones(Context.User.Id)}");
                        return;
                    }
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
                    await Context.Channel.SendMessageAsync("Can't give negative money :rage: Use ``!money take ...`` if you want to take");
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

                await Context.Channel.SendMessageAsync($"{User.Mention} got ${Amount} from {Context.User.Mention}");
                            
                await Data.SaveStones(User.Id, Amount);
               
            }
            [Command("take")]
            public async Task Take(IUser User = null, int Amount= 0)
            {
                if (Amount < 0)
                {
                    await Context.Channel.SendMessageAsync("Use a positive value :rage: Use ``!money give ...`` if you want to give");
                    return;
                }
                if (User == null)
                {
                    await Context.Channel.SendMessageAsync("Who tf should I take money from?");
                    return;
                }
                if (User.Username.Equals("myBot"))
                {
                    await Context.Channel.SendMessageAsync("Tf? What are you taking away when I have nothing?");
                    return;
                }
                if (User.IsBot && !User.Username.Equals("myBot"))
                {
                    await Context.Channel.SendMessageAsync("Stop trying to take money from robots :rage: (they don't even have any in the first place :cry:)");
                    return;
                }
                if (User == Context.User)
                {
                    await Context.Channel.SendMessageAsync("**TF??** You can't take away money from yourself :angry: (unless you're retarded)");
                    return;
                }
                if (Amount == 0)
                {
                    await Context.Channel.SendMessageAsync("How much should I take bruh?");
                    return;
                }

                await Context.Channel.SendMessageAsync($"{User.Mention} got ${Amount} from {Context.User.Mention}");

                await Data.SaveStones(User.Id, Amount);
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
            [Command("buyDogs")]
            public async Task BuyDogs()
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    Stone Money = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    if(Money.Amount >= 100)
                    {
                        Money.Amount -= 100;
                        DbContext.Update(Money);
                        await DbContext.SaveChangesAsync();
                        await Context.Channel.SendMessageAsync("Bought a dog");
                        await Data.BuyDogs(Context.User.Id);
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("You don't have enough money peasant");
                    }
                }
            }
            [Command("work")]
            public async Task Work()
            {                  
                using (var DbContext = new SQLiteDBContext()) {
                    Random Rand = new Random();
                    string[] Jobs = {"You worked at a factory", "You worked at a hotel",
                                "You worked as a chef", "You worked at a graveyard"}; //add more jobs
                    int Job = Rand.Next(Jobs.Length);
                    int Cash = Rand.Next(201) + 300;
                    Stone GiveCash = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    GiveCash.Amount += Cash;
                    await Context.Channel.SendMessageAsync(Jobs[Job]);    
                    DbContext.Update(GiveCash);
                    await DbContext.SaveChangesAsync();
                    await Context.Channel.SendMessageAsync("You earned $" + Cash);
                }   
            }
            [Command("inventory")]
            public async Task Inventory()
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    Stone Inv= DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    if (Inv.Item1 > 0)
                    {
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Items");
                        Embed.WithColor(40, 200, 150);
                        Embed.AddField("Dogs:", Inv.Item1);
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    }
                }
            }
            
        }

    }
}
