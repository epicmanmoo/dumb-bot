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
            await Context.Channel.SendMessageAsync("``This is my Bot, here are a list of available commands:\n!help\n!hello\n!embed <phrase>\n!fuck <user>``");
        }
        [Command("fuck")]
        public async Task<RuntimeResult> Fuck([Remainder] IGuildUser OtherUser)
        {
            await Context.Channel.SendMessageAsync(Context.User.Mention + " says fuck you to " + OtherUser.Mention);
            return Errors.FromError("User Not Found");
        }
  
        [Command("embed")]
        public async Task Embed([Remainder] string Input= "None")
        {
            if (!Input.Equals("None"))
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
    }
}
