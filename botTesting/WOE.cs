using System;

namespace botTesting
{
    [Serializable]
    public class WOE
    {
        [Serializable]
        public class RootObject
        {
            public string title { get; set; }
            public string location_type { get; set; }
            public long woeid { get; set; }
            public string latt_long { get; set; }
        }
    }
}