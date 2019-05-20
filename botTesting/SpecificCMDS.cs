using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace botTesting
{
    public class SpecificCMDS
    {
        [Key]
        public ulong GuildId { get; set; }
        public string Joinmsgs { get; set; }
        public string Leavemsgs { get; set; }
        public string MsgPrefix { get; set; }
        public string NameOfBot { get; set; }
    }
}
