using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace botTesting
{
    public class NamingThings
    {
        [Key]
        public ulong GuildId { get; set; }
        public string IntroChannel { get; set; }
    }
}
