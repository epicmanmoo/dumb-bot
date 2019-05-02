using System;
using System.Collections.Generic;
using System.Text;

namespace botTesting
{
    [Serializable]
    class YandexTranslate
    {
        [Serializable]
        public class RootObject
        {
            public int code { get; set; }
            public string lang { get; set; }
            public List<string> text { get; set; }
        }
    }
}
