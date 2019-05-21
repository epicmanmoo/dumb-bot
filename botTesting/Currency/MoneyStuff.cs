using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace botTesting.Currency
{

    public class MoneyStuff : ModuleBase<SocketCommandContext>
    {
        public static List<DateTimeOffset> workTimer = new List<DateTimeOffset>();
        public static List<SocketGuildUser> workTarget = new List<SocketGuildUser>();

        public static List<DateTimeOffset> robTimer = new List<DateTimeOffset>();
        public static List<SocketGuildUser> robTarget = new List<SocketGuildUser>();
        public class StonesGroup : ModuleBase<SocketCommandContext>
        {
            public async Task CreateUserInTable(IUser User = null)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    if (User != null)
                    {
                        if (DbContext.Stones.Where(x => x.UserId == User.Id).Count() < 1)
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
                        return;
                    }
                    return;
                }
            }
            [Command("money")]
            public async Task Money(SocketGuildUser User = null)
            {
                SocketGuildUser Yourself = Context.User as SocketGuildUser;
                await CreateUserInTable(User);
                await CreateUserInTable(Yourself);
                using (var DbContext = new SQLiteDBContext())
                {
                    if (User == null)
                    {
                        Stone You = DbContext.Stones.Where(x => x.UserId == Yourself.Id).FirstOrDefault();
                        if (You.Amount == 0)
                        {
                            await Context.Channel.SendMessageAsync("You are broke, literally.");
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You have ${You.Amount}");
                            return;
                        }
                    }
                    else if (User.IsBot)
                    {
                        await Context.Channel.SendMessageAsync("Bots don't have money");
                        return;
                    }
                    else
                    {
                        Stone Other = DbContext.Stones.Where(x => x.UserId == User.Id).FirstOrDefault();
                        await Context.Channel.SendMessageAsync($"{User.Nickname ?? User.Username} has ${Other.Amount}");
                    }
                }
            }
            [Command("give")]
            public async Task Give(string User, int Amount = 0)
            {
                if (User.IndexOf('@') == -1 || User.Replace("<", "").Replace(">", "").Length != User.Length - 2)
                {
                    await Context.Channel.SendMessageAsync("User does not exist!");
                    return;
                }

                string idStr = User.Replace("<", "").Replace(">", "").Replace("@", "");
                IUser xo;
                try
                {
                    ulong id = ulong.Parse(idStr);
                    xo = await Context.Channel.GetUserAsync(id);
                    Console.WriteLine(xo);
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("Can't send money to a bot!");
                    throw new Exception("Error");
                }
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuildUser oUser = xo as SocketGuildUser;
                    SocketGuildUser You = Context.User as SocketGuildUser;
                    Stone sMoney = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    Stone tMoney = DbContext.Stones.Where(x => x.UserId == oUser.Id).FirstOrDefault();
                    await CreateUserInTable(oUser);
                    await CreateUserInTable(You);
                    int Money = sMoney.Amount;
                    if (!(Money < Amount))
                    {
                        if (Amount < 0)
                        {
                            await Context.Channel.SendMessageAsync("Can't give negative money");
                            return;
                        }
                        if (oUser == null)
                        {
                            await Context.Channel.SendMessageAsync("Who should I give money to?");
                            return;
                        }
                        if (oUser == Context.User)
                        {
                            await Context.Channel.SendMessageAsync("You can't give money to yourself :wink:");
                            return;
                        }
                        if (Amount == 0)
                        {
                            await Context.Channel.SendMessageAsync("How much should I give?");
                            return;
                        }
                        await Context.Channel.SendMessageAsync($"You gave ${Amount} to {oUser.Username ?? oUser.Nickname}");
                        sMoney.Amount -= Amount;
                        tMoney.Amount += Amount;
                        await DbContext.SaveChangesAsync();
                        return;
                    }
                    await Context.Channel.SendMessageAsync("You do not enough money to send");
                }
            }
            [Command("rob")]
            public async Task Take(SocketGuildUser User = null)
            {
                if (User != null)
                {
                    if (User.Id == Context.User.Id)
                    {
                        await Context.Channel.SendMessageAsync("You can't rob from yourself, that'd be a loophole :wink:");
                        return;
                    }
                    if (User.IsBot)
                    {
                        await Context.Channel.SendMessageAsync("Bots don't have money");
                        return;
                    }
                    if (robTarget.Contains(Context.User as SocketGuildUser))
                    {
                        if (robTimer[robTarget.IndexOf(Context.Message.Author as SocketGuildUser)].AddSeconds(30) >= DateTimeOffset.Now)
                        {
                            int secondsLeft = (int)(robTimer[robTarget.IndexOf(Context.Message.Author as SocketGuildUser)].AddSeconds(30) - DateTimeOffset.Now).TotalSeconds;
                            EmbedBuilder Embed = new EmbedBuilder();
                            Embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                            Embed.WithColor(40, 200, 150);
                            Embed.WithDescription($":x: Wait {secondsLeft} more seconds before robbing");
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            return;
                        }
                        else
                        {
                            robTimer[robTarget.IndexOf(Context.Message.Author as SocketGuildUser)] = DateTimeOffset.Now;
                            await RobMethod(User);
                        }
                    }
                    else
                    {
                        robTarget.Add(Context.User as SocketGuildUser);
                        robTimer.Add(DateTimeOffset.Now);
                        await RobMethod(User);
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Specify a user to rob from!");
                }
            }
            public async Task RobMethod(SocketGuildUser User)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuildUser You = Context.User as SocketGuildUser;
                    await CreateUserInTable(User);
                    await CreateUserInTable(You);
                    Stone UserMoneyS = DbContext.Stones.Where(x => x.UserId == User.Id).FirstOrDefault();
                    Stone UserMoneyM = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    if (UserMoneyS.Amount < 300)
                    {
                        await Context.Channel.SendMessageAsync("You can't make them go broke, let them live :/");
                        return;
                    }
                    if (UserMoneyM.Amount < 300)
                    {
                        await Context.Channel.SendMessageAsync("You don't have enough money to rob yet");
                        return;
                    }
                    Random RandStealSuccess = new Random();
                    Random RandStealMoney = new Random();
                    Random RandFailedMoney = new Random();
                    int rss = RandStealSuccess.Next(0, 100);
                    int rsm = RandStealMoney.Next(100, UserMoneyS.Amount - 100);
                    int rfm = RandFailedMoney.Next(100, UserMoneyM.Amount - 100);
                    if (rss % 2 == 0)
                    {
                        if (rsm != 0)
                        {
                            UserMoneyS.Amount -= rsm;
                            UserMoneyM.Amount += rsm;
                            await Context.Channel.SendMessageAsync($"You stole ${rsm} from {User.Username ?? User.Nickname}!");
                            await DbContext.SaveChangesAsync();
                            return;
                        }
                    }
                    else if (rss % 2 == 1)
                    {
                        UserMoneyM.Amount -= rsm;
                        await Context.Channel.SendMessageAsync($"You were caught trying to steal money! You lost ${rfm}");
                        await DbContext.SaveChangesAsync();
                        return;
                    }
                }
            }
            //[Command("reset")]
            //public async Task Reset(IUser User = null)
            //{
            //    SocketGuildUser ChkUser = Context.User as SocketGuildUser;
            //    if (ChkUser.GuildPermissions.Administrator)
            //    {
            //        if (User == null)
            //        {
            //            await Context.Channel.SendMessageAsync("Specify a user to reset bruh");
            //            return;
            //        }
            //        if (User.IsBot)
            //        {
            //            await Context.Channel.SendMessageAsync("Stop including bots :neutral_face:");
            //            return;
            //        }

            //        using (var DbContext = new SQLiteDBContext())
            //        {
            //            int Num = DbContext.Stones.Where(x => x.UserId == User.Id).Count();
            //            if (Num == 0)
            //            {
            //                await Context.Channel.SendMessageAsync($"{User.Mention} was already deleted");
            //                return;
            //            }
            //            else
            //            {
            //                await Context.Channel.SendMessageAsync("You got reset, so you lost all your money :cry:");
            //                DbContext.Stones.RemoveRange(DbContext.Stones.Where(x => x.UserId == User.Id));
            //                await DbContext.SaveChangesAsync();
            //            }
            //        }
            //    }
            //    await Context.Channel.SendMessageAsync("Not a mod retard");
            //}
            [Command("buydogs")]
            public async Task BuyDogs(int amount)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuildUser You = Context.User as SocketGuildUser;
                    await CreateUserInTable(You);
                    Stone Money = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    if (Money.Amount >= 100 * amount)
                    {
                        Money.Amount -= 100 * amount;
                        Money.Item1 += amount;
                        await DbContext.SaveChangesAsync();
                        if (amount == 1)
                        {
                            await Context.Channel.SendMessageAsync($"You bought a dog!");
                            return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You bought {amount} dogs!");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("You don't have enough money to buy dogs :cry:");
                    }
                }
            }
            //9 more methods for store!! ^
            [Command("work")]
            public async Task Work()
            {
                if (workTarget.Contains(Context.User as SocketGuildUser))
                {
                    if (workTimer[workTarget.IndexOf(Context.Message.Author as SocketGuildUser)].AddSeconds(30) >= DateTimeOffset.Now)
                    {
                        int secondsLeft = (int)(workTimer[workTarget.IndexOf(Context.Message.Author as SocketGuildUser)].AddSeconds(30) - DateTimeOffset.Now).TotalSeconds;
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                        Embed.WithColor(40, 200, 150);
                        Embed.WithDescription($":x: Wait {secondsLeft} more seconds before working");
                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        return;
                    }
                    else
                    {
                        workTimer[workTarget.IndexOf(Context.Message.Author as SocketGuildUser)] = DateTimeOffset.Now;
                        await WorkMethod();
                    }
                }
                else
                {
                    workTarget.Add(Context.User as SocketGuildUser);
                    workTimer.Add(DateTimeOffset.Now);
                    await WorkMethod();
                }

            }
            public async Task WorkMethod()
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuildUser You = Context.User as SocketGuildUser;
                    await CreateUserInTable(You);
                    Random Rand = new Random();
                    string[] Jobs = {"You worked at a factory", "You worked at a hotel",
                                "You worked as a chef", "You worked at a graveyard",
                                "You did chores because no women were there to help", "You got away **just** in time from a bank robbery",
                                "Before the owners could find out, you robbed their mansion", "You became a sex slave for a day, pleasing several people",
                                "You were on your way to work but got hit by a car and get compensated", "You get hired to murder"}; //add more jobs
                    int Job = Rand.Next(Jobs.Length);
                    int Cash = Rand.Next(401) + 100;
                    Stone GiveCash = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    GiveCash.Amount += Cash;
                    await Context.Channel.SendMessageAsync(Jobs[Job] + ", earning $" + Cash);
                    DbContext.Update(GiveCash);
                    await DbContext.SaveChangesAsync();
                }
            }
            [Command("inventory")]
            public async Task Inventory(SocketGuildUser User = null)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    SocketGuildUser You = Context.User as SocketGuildUser;
                    await CreateUserInTable(You);
                    Stone Inv = DbContext.Stones.Where(x => x.UserId == Context.User.Id).FirstOrDefault();
                    if (Inv.Item1 > 0)
                    {
                        EmbedBuilder Embed = new EmbedBuilder();
                        Embed.WithAuthor("Items");
                        Embed.WithColor(40, 200, 150);
                        if (Inv.Item1 == 1)
                        {
                            Embed.AddField("Dog:", Inv.Item1);
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                            return;
                        }
                        else
                        {
                            Embed.AddField("Dogs:", Inv.Item1);
                            await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        }
                    }
                }
            }

        }

    }
}
