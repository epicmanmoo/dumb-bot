using System;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace botTesting
{
    public class Interactive : InteractiveBase<SocketCommandContext>
    {
        [Command("next", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
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
        [Command("delete")]
        public async Task<RuntimeResult> Test_DeleteAfterAsync()
        {
            await ReplyAndDeleteAsync("this message will delete in 10 seconds", timeout: TimeSpan.FromSeconds(10));
            return Ok();
        }
        [Command("paginator")]
        public async Task Test_Paginator()
        {
            var pages = new[] { "Page 1", "Page 2", "Page 3", "aaaaaa", "Page 5" };
            await PagedReplyAsync(pages);
        }
    }
}
