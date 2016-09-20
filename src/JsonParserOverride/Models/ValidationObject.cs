using System.Collections.Generic;

namespace JsonParserOverride.Models
{
    public class ValidationObject
    {
        public ValidationObject()
        {
            Lines = new List<string>();
        }

        public string Key { get; set; }
        public List<string> Lines { get; set; }
    }
}