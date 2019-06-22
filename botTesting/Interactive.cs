using System;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;

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
            else
            {
                await ReplyAsync("You did not reply before the timeout");
            }
        }
        [Command("delete", RunMode = RunMode.Async)]
        public async Task DeleteMessage()
        {
            var message = await ReplyAndDeleteAsync("Ok", timeout: TimeSpan.FromSeconds(10));
            await Task.Delay(5000);
            await message.ModifyAsync(x => x.Content = "Meghan you suck");
        }
        //[Command("paginator")]
        //public async Task Test_Paginator()
        //{
        //    var pages = new[] { "Page 1", "Page 2", "Page 3", "aaaaaa", "Page 5" };
        //    await PagedReplyAsync(pages);
        //}
    }
}
