using System;
using System.Collections.Generic;
using System.Text;

namespace botTesting
{
    [Serializable]
    class Lyrics
    {
        [Serializable]
        public class RootObject
        {
            public string lyrics { get; set; }
        }
    }
}
