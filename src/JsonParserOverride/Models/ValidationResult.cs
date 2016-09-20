using System.Collections.Generic;
using System.Linq;
using JsonParserOverride.Extensions;

namespace JsonParserOverride.Models
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ValidationObjects = new List<ValidationObject>();
        }

        public List<ValidationObject> ValidationObjects { get; set; }

        public static object Lock = new object();

        public void AddError(string key, string value)
        {
            lock (Lock)
            {
                var obj = ValidationObjects.FirstOrDefault(x => x.Key == key);
                if (obj == null)
                {
                    obj = new ValidationObject { Key = key };
                    ValidationObjects.Add(obj);
                }
                obj.Lines.Add(value);
            }
        }

        public void MergeValidations(ValidationResult fromResult, string prefix)
        {
            foreach (var o in fromResult.ValidationObjects)
            {
                foreach (var line in o.Lines)
                {
                    AddError(prefix + "." + o.Key, line);
                }
            }
        }

        public bool IsValid => ValidationObjects.IsEmpty();
    }
}