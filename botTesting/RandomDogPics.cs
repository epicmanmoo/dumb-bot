using System;
using System.Collections.Generic;
using System.Text;

namespace botTesting
{   
    [Serializable]
    class RandomDogPics
    {
        [Serializable]
        public class RootObject
        {
            public string status { get; set; }
            public string message { get; set; }
        }
    }
}
