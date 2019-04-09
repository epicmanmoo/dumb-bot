using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace botTesting
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("``This is my Bot, here are a list of available commands:\n!help\n!hello``");
        }
    }
}
