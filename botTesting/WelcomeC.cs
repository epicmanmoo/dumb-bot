using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace botTesting
{
    public class WelcomeC : InteractiveBase<SocketCommandContext>
    {
        public async Task Register(ulong id)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                if (DbContext.welcomes.Where(x => x.userid == id).Count() < 1)
                {
                    DbContext.Add(new Welcome
                    {
                        userid = id,
                        age = -1,
                        name = "",
                        location = "",
                        desc = "",
                        plurals = "",
                        favfood = "",
                        favcolor = ""
                    });
                    await DbContext.SaveChangesAsync();
                }
            }
        }
        //todo: check if already signed up
        [Command("signup", RunMode = RunMode.Async)]
        public async Task SignUp()
        {
            SocketChannel signup = Context.Guild.Channels.Where(x => x.Name.Equals("signup")).FirstOrDefault() as SocketChannel;
            if (Context.Channel.Id == signup.Id)
            {
                await Register(Context.User.Id);
                using (var DbContext = new SQLiteDBContext())
                {
                    Welcome welcome = DbContext.welcomes.Where(x => x.userid == Context.User.Id).FirstOrDefault();
                    await ReplyAsync($"You will begin signing up shortly **{Context.User.Username}**! Type `No` to any question you do not want to answer!");
                    await Task.Delay(2500);
                    await ReplyAsync("`How old are you?`");
                    var ageobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if(ageobj != null)
                    {
                        if (ageobj.ToString().ToLower().StartsWith("no"))
                        {
                            welcome.age = -2;
                        }
                        else
                        {
                            try
                            {
                                welcome.age = int.Parse(ageobj.ToString());
                            }
                            catch (FormatException e)
                            {
                                await ReplyAsync("That is not a number!");
                                return;
                            }
                        }
                    }
                    await ReplyAsync("`What is your name, or nickname`");
                    var nameobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (nameobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.name = "Not Provided";
                    }
                    else
                    {
                        welcome.name = nameobj.ToString();
                    }
                    await ReplyAsync("`Where are you from?`");
                    var locobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (locobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.location = "Not Provided";
                    }
                    else
                    {
                        welcome.location = locobj.ToString();
                    }
                    await ReplyAsync("`Describe yourself in a few sentences!`");
                    var descobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(300));
                    if (descobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.desc = "Not Provided";
                    }
                    else
                    {
                        welcome.desc = descobj.ToString();
                    }
                    await ReplyAsync("`What are your plurals?`");
                    var plurobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(60));
                    if (plurobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.plurals = "Not Provided";
                    }
                    else
                    {
                        welcome.plurals = plurobj.ToString();
                    }
                    await ReplyAsync("`What is your favorite food?`");
                    var foodobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (foodobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.favfood = "Not Provided";
                    }
                    else
                    {
                        welcome.favfood = foodobj.ToString();
                    }
                    await ReplyAsync("`Lastly, what is your favorite color?`");
                    var colorobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (colorobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.favcolor = "Not Provided";
                    }
                    else
                    {
                        welcome.favcolor = colorobj.ToString();
                    }
                    await Task.Delay(600);
                    await ReplyAsync("Your information is being prepared!");
                    await DbContext.SaveChangesAsync();
                    await Task.Delay(2000);
                    await EmbedSender(Context.User as SocketGuildUser);
                }
            }
        }
        [Command("view")]
        public async Task View(SocketGuildUser User)
        {
            if (User.IsBot)
            {
                await ReplyAsync("Bots don't need to be signed up!");
                return;
            }
            using (var DbContext = new SQLiteDBContext())
            {
                int hasUser = DbContext.welcomes.Where(x => x.userid == User.Id).Count();
                if (hasUser > 0)
                {
                    await EmbedSender(User);
                }
                else
                {
                    await ReplyAsync("Either the user has not signed up or does not exist!");
                }
            }
        }
        //[Command("update")]
        //[Command("delete")]

        public async Task EmbedSender(SocketGuildUser User)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                Welcome welcome = DbContext.welcomes.Where(x => x.userid == User.Id).FirstOrDefault();
                EmbedBuilder embed = new EmbedBuilder();
                System.Drawing.Color c = System.Drawing.Color.FromName(welcome.favcolor);
                embed.WithAuthor(User.Username, User.GetAvatarUrl());
                embed.WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/a/af/Tux.png/220px-Tux.png");
                embed.WithTitle("**User Details**");
                embed.WithColor(c.R, c.G, c.B);
                string agenum = welcome.age < 0 ? "Not Provided" : welcome.age.ToString();
                embed.WithDescription($"**Age** {agenum}" + $"\n**Name/Nickname**: {welcome.name}\n**Location**: {welcome.location}\n**Descritpion**: {welcome.desc}\n" +
                    $"**Plurals**: {welcome.plurals}\n**Favorite Food**: {welcome.favfood}\n**Favorite Color**: {welcome.favcolor}");
                await ReplyAsync("", false, embed.Build());
            }
        }
    }
}
