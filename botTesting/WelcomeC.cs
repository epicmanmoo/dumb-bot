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
        //[Command("signuploc")]
        //
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
                    if(DbContext.welcomes.Where(x => x.userid == Context.User.Id).Count() > 0)
                    {
                        await ReplyAsync("You are already signed up! To update, use `!update <item> <value>`");
                        return;
                    }
                    await ReplyAsync($"You will begin signing up shortly `**{Context.User.Username}**`! Type `No` to any question you do not want to answer!");
                    await Task.Delay(3000);
                    await ReplyAsync("`How old are you?`");
                    var ageobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (ageobj != null)
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
                    await Task.Delay(800);
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
                    await Task.Delay(800);
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
                    await Task.Delay(800);
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
                    await Task.Delay(800);
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
                    await Task.Delay(800);
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
                    await Task.Delay(800);
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
                    await Task.Delay(800);
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
            using (var DbContext = new SQLiteDBContext())
            {
                if (DbContext.welcomes.Where(x => x.userid == User.Id).Count() > 0)
                {
                    await EmbedSender(User);
                }
                else
                {
                    await ReplyAsync("Either the user has not signed up or does not exist!");
                }
            }
        }
        //check for same as before?
        [Command("update")]
        public async Task UpdateIntro(string field, [Remainder] string value)
        {
            using (var DbContext = new SQLiteDBContext())
            {
                Welcome welcome = DbContext.welcomes.Where(x => x.userid == Context.User.Id).FirstOrDefault();
                switch (field)
                {
                    case "1":
                    case "age":
                        if (welcome.age.GetType() != typeof(string))
                        {
                            try
                            {
                                welcome.age = int.Parse(value);
                            }
                            catch(FormatException e)
                            {
                                await ReplyAsync("That is not a number!");
                            }
                        }
                        else
                        {
                            welcome.age = -2;
                        }
                        break;
                    case "2":    
                    case "description":
                        welcome.desc = value;
                        break;
                    case "3":   
                    case "favcolor":
                        welcome.favcolor = value;
                        break;
                    case "4":
                    case "favfood":
                        welcome.favfood = value;
                        break;
                    case "5":
                    case "location":
                        welcome.location = value;
                        break;
                    case "6":
                    case "name":
                        welcome.name = value;
                        break;
                    case "7":
                    case "plurals":
                        welcome.plurals = value;
                        break;
                }
                await Task.Delay(1000);
                await ReplyAsync("Updates made!");
                await DbContext.SaveChangesAsync();           
            }
        }
        [Command("delete")]
        public async Task Delete()
        {
            SocketGuildUser You = Context.User as SocketGuildUser;
            using (var DbContext = new SQLiteDBContext())
            {
                if(DbContext.welcomes.Where(x => x.userid == Context.User.Id).Count() > 0)
                {
                    await ReplyAsync("Are you sure you want to delete your intro? Yes or No");
                    var responseobj = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
                    if (responseobj.ToString().ToLower().Equals("yes"))
                    {
                        DbContext.Remove(DbContext.welcomes.Where(x => x.userid == Context.User.Id).FirstOrDefault());
                        await DbContext.SaveChangesAsync();
                    }
                    else if (responseobj.ToString().ToLower().Equals("no"))
                    {
                        await ReplyAsync("Nothing Deleted!");
                        return;
                    }
                    else
                    {
                        await ReplyAsync("Not a valid response!");
                        return;
                    }
                }
                else
                {
                    await ReplyAsync("Your intro doesn't exist yet, so you can't delete it!");
                    return;
                }
            }
        }

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
                if (c.IsKnownColor)
                {
                    embed.WithColor(c.R, c.G, c.B);
                }
                else
                {
                    embed.WithColor(Discord.Color.DarkGrey);
                }
                string agenum = welcome.age < 0 ? "Not Provided" : welcome.age.ToString();
                embed.WithDescription($"1. **Age**: {agenum}" + $"\n2. **Name/Nickname**: {welcome.name}\n3. **Location**: {welcome.location}\n4. **Descritpion**: {welcome.desc}\n" +
                    $"5. **Plurals**: {welcome.plurals}\n**6. Favorite Food**: {welcome.favfood}\n**7. Favorite Color**: {welcome.favcolor}");
                await ReplyAsync("", false, embed.Build());
            }
        }
    }
}
