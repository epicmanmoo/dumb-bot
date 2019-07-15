using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace botTesting
{
    public class WelcomeC : InteractiveBase<SocketCommandContext>
    {
        [Command("signup")]
        public async Task SignUp()
        {
            SocketChannel signup = Context.Guild.Channels.Where(x => x.Name.Equals("signup")).FirstOrDefault() as SocketChannel;
            if (Context.Channel.Id == signup.Id)
            {
                using (var DbContext = new SQLiteDBContext())
                {
                    Welcome welcome = DbContext.welcomes.Where(x => x.userid == Context.User.Id).FirstOrDefault();
                }
            }
        }
    }
}
