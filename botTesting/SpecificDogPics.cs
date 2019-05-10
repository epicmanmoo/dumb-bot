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
            [DataMember(IsRequired = true)]
            public string status { get; set; }
            [DataMember(IsRequired = false)]
            public string code { get; set; }
            [DataMember(IsRequired = true)]
            public List<string> message { get; set; }
        }
    }
}
