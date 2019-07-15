using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
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
                    await ReplyAsync($"You will begin signing up shortly {Context.User.Username}! Type `No` to any question you do not want to answer!");
                    await Task.Delay(2500);
                    await ReplyAsync("How old are you?");
                    var ageobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(10));
                    if(ageobj != null)
                    {
                        if (ageobj.ToString().ToLower().StartsWith("no"))
                        {
                            welcome.age = -1;
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
                    await ReplyAsync("What is your name, or nickname");
                    var nameobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(15));
                    if (nameobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.name = "Not Provided";
                    }
                    else
                    {
                        welcome.name = nameobj.ToString();
                    }
                    await ReplyAsync("Where are you from?");
                    var locobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(15));
                    if (locobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.location = "Not Provided";
                    }
                    else
                    {
                        welcome.location = locobj.ToString();
                    }
                    await ReplyAsync("Describe yourself in 3 sentences or less!");
                    var descobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(300));
                    if (descobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.desc = "Not Provided";
                    }
                    else
                    {
                        welcome.desc = descobj.ToString();
                    }
                    await ReplyAsync("What are your plurals?");
                    var plurobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(60));
                    if (plurobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.plurals = "Not Provided";
                    }
                    else
                    {
                        welcome.plurals = plurobj.ToString();
                    }
                    await ReplyAsync("What is your favorite food?");
                    var foodobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (foodobj.ToString().ToLower().StartsWith("no"))
                    {
                        welcome.favfood = "Not Provided";
                    }
                    else
                    {
                        welcome.favfood = foodobj.ToString();
                    }
                    await ReplyAsync("Lastly, what is your favorite color?");
                    var colorobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(15));
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
                    EmbedBuilder embed = new EmbedBuilder();
                    embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                    embed.WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/a/af/Tux.png/220px-Tux.png");
                    embed.WithTitle("**User Details**");
                    embed.WithDescription($"**Age**: {welcome.age}\n**Name/Nickname**: {welcome.name}\n**Location**: {welcome.location}\n**Descritpion**: {welcome.desc}\n" +
                        $"**Plurals**: {welcome.plurals}\n**Favorite Food**: {welcome.favfood}\n**Favorite Color**: {welcome.favcolor}");
                    await ReplyAsync("", false, embed.Build());
                }
            }
        }
    }
}
