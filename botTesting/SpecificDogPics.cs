using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
namespace botTesting
{
    [Serializable]
    class SpecificDogPics
    {
        [Serializable]
        public class RootObject
        {
            public string status { get; set; }
            [DataMember(IsRequired = false)]
            public string code { get; set; }
            public List<string> message { get; set; }
        }
    }
}
