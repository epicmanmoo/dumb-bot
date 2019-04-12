using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace botTesting
{
    public class Stone
    {
        [Key]
        public ulong UserId { get; set; }
        public int Amount { get; set; }
    }
}
