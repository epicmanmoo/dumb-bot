using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
//DONT NEED THIS, EDIT LATER!
namespace botTesting
{
    [Serializable]
    class SpecificDogPics
    {
        [Serializable]
        [DataContract]
        public class RootObject
        {
            public string status { get; set; }
            [DataMember(IsRequired = false)]
            public string code { get; set; }
            public List<string> message { get; set; }
        }
    }
}
