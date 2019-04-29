using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;

namespace botTesting
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        public async Task Hello()
        {
            Random Rand = new Random();
            string[] HelloTexts = {"What do you want?", "Who are you",
                                  "I don't care about you", "Bye",
                                  "Don't bother me"};
            int RandNum = Rand.Next(HelloTexts.Length);
            await Context.Channel.SendMessageAsync(HelloTexts[RandNum]);
        }
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("```Here are a list of available commands:\n!help\n!hello\n!embed <phrase>\n!fuck <user>\n!loop <phrase> <numoftimes> (surround phrases longer than one word in quotes)\n!money\n!money <user>\n!money give <user> <amount>\n!money take <user> <amount>\n!store\n!money work\n!money reset <user>\n!money inventory\n!avatar\n!avatar <user>```");
        }
        [Command("fuck")]
        public async Task Fuck([Remainder] IGuildUser OtherUser)
        {
            if (Context.User.Equals(OtherUser))
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " has fucked themselves");
            }
            else if (OtherUser.Username.Equals("myBot"))
            {
                await Context.Channel.SendMessageAsync("No, fuck **_you_**");
            }
            else
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " says fuck you to " + OtherUser.Mention);
            }
         }

        [Command("embed")]
        public async Task Embed([Remainder] string Input = "")
        {
            if (!Input.Equals(""))
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor("Embed", Context.User.GetAvatarUrl());
                Embed.WithColor(40, 200, 150);
                Embed.WithFooter(Context.User.Username);
                Embed.WithCurrentTimestamp();
                //Embed.WithDescription("");
                Embed.AddField("User input:", Input);
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("Please include a phrase!");
            }
        }
        [Command("F")]
        public async Task F()
        {
            Random Ran = new Random();
            string[] DiffTexts = {"Press F for Respects :cry:", "Drop an F :(",
                                 "You better press F before dying", "You get one life, F away",
                                 "F for your future", "F means Failure, which you already are"};
            int Rand = Ran.Next(DiffTexts.Length);
            await Context.Channel.SendMessageAsync(DiffTexts[Rand]);
        }
        [Command("store")]//more shop items!!
        public async Task Store()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Shop");
            Embed.WithColor(40, 200, 150);
            Embed.AddField("Item 1", "1. Dog $100");
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
        [Command("caps")]
        public async Task Caps([Remainder] string Phrase = "")
        {
            if (!Phrase.Equals(""))
            {
                await Context.Channel.SendMessageAsync(Phrase.ToUpper());
            }
            else
            {
                await Context.Channel.SendMessageAsync("Include a message to be capitalized");
            }
        }
        [Command("avatar")]
        public async Task Avatar(IUser User = null)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(255, 0, 238);
            if (User != null)
            {
                if(User.GetAvatarUrl() == null)
                {
                    await Context.Channel.SendMessageAsync("No PFP for this user");
                    return;
                }
                Embed.WithImageUrl(User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
            else
            {
                Embed.WithImageUrl(Context.User.GetAvatarUrl());
                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            }
        }
    }
}
