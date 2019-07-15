using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace botTesting
{
    public class Welcome
    {
        [Key]
        public ulong userid { get; set; }
        public int age { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string desc { get; set; }
        public string plurals { get; set; }
        public string favfood { get; set; }
        public string favcolor { get; set; }
    }
}
