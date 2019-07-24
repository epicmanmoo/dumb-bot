using System;
using System.Collections.Generic;
using System.Text;

namespace botTesting
{
    [Serializable]
    class Cats
    {
        [Serializable]
        public class RootObject
        {
            public string id { get; set; }
            public string url { get; set; }
            public string source_url { get; set; }
        }
    }
}
