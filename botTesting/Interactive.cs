using System;
using System.Collections;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace botTesting
{
    public class Interactive : InteractiveBase<SocketCommandContext>
    {
        [Command("next", RunMode = RunMode.Async)]
        public async Task CoolMessage()
        {
            await ReplyAsync("What is 2+2?");
            var response = await NextMessageAsync(timeout: TimeSpan.FromSeconds(5));
            if (response != null)
            {
                try
                {
                    int respint = int.Parse(response.ToString());
                    if (respint == 4)
                    {
                        await ReplyAsync("Good job!");
                    }
                    else
                    {
                        await ReplyAsync($"Incorrect :frowning:");
                    }
                }
                catch(Exception e)
                {
                    if (response.ToString()[0] != '!')
                    {
                        await ReplyAsync("That is not a number!");
                    }
                }
            }
            else
            {
                await ReplyAsync("You did not reply before the timeout");
            }
        }
        [Command("delete", RunMode = RunMode.Async)]
        public async Task DeleteMessage()
        {
            SocketGuild guild = Context.Guild as SocketGuild;
            var users = guild.Users.GetEnumerator();
            string[] ppl = new string[guild.Users.Count - 1];
            int index = 0;
            foreach (var user in guild.Users)
            {
                if (!user.IsBot)
                {
                    ppl[index] = user.Mention;
                    index++;
                }
            }
            Random rand = new Random();
            int nrand = rand.Next(ppl.Length);
            var message = await ReplyAndDeleteAsync("Test", timeout: TimeSpan.FromSeconds(10));
            await Task.Delay(5000);
            await message.ModifyAsync(x => x.Content = ppl[nrand]);
        }
        //[Command("paginator")]
        //public async Task Test_Paginator()
        //{
        //    var pages = new[] { "Page 1", "Page 2", "Page 3", "aaaaaa", "Page 5" };
        //    await PagedReplyAsync(pages);
        //}
    }
}
