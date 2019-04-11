using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace botTesting
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        public async Task Hello()
        {
            await Context.Channel.SendMessageAsync("What do you want?");
        }
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("``This is my Bot, here are a list of available commands:\n!help\n!hello\n!embed <phrase>\n!fuck <user>\n!loop <phrase> <numoftimes>``");
        }
        [Command("fuck")]
        public async Task<RuntimeResult> Fuck([Remainder] IGuildUser OtherUser)
        {
            if (Context.User.Equals(OtherUser))
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " has fucked themselves");
            }
            else if (OtherUser.Username.Equals("myBot"))
            {
                await Context.Channel.SendMessageAsync("No, fuck **_you_**");
            }
            else if (!OtherUser.Equals(Context.User))
            {
                await Context.Channel.SendMessageAsync(Context.User.Mention + " says fuck you to " + OtherUser.Mention);
            }
            return Errors.FromError("User Not Found");
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
        [Command("loop")]
        public async Task Loop(string Input = "None", int Num= 0)
        {
            if (!Input.Equals("None")){
                for (int i = 0; i < Num; i++)
                {
                    await Context.Channel.SendMessageAsync(Input);
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Please include a phrase/number!");
            }
        }
        [Command("F")]
        public async Task F()
        {
            await Context.Channel.SendMessageAsync("Press F for Respects :cry:");
        }
    }
}
