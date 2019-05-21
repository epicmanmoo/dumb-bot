using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
//I made this before I realized there was support for finding random images for a specific breed 
//or sub breed in the API. :/ oh well
namespace botTesting
{
    [Serializable]
    class SpecificDogPics
    {
        [Serializable]
        public class RootObject
        {
            public string status { get; set; }
            public List<string> message { get; set; }
        }
    }
}
