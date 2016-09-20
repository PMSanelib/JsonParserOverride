using System;
using System.IO;
using JsonParserOverride.Enumerations;
using JsonParserOverride.Extensions;
using JsonParserOverride.Models;
using Newtonsoft.Json;

namespace JsonParserOverride
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestPremitive();

            TestForDto<ContractDto>("1");
            TestEnum();

            Console.ReadLine();

        }

        private static void TestForDto<T>(string str) where T : DtoBase, new()
        {
            var json = File.ReadAllText($"JsonTestData/{typeof(T).Name}{str}.json");

            var request = JsonConvert.DeserializeObject<JsonRequestWrapper<T>>(json);
            Console.WriteLine($"Validation result for {json}");
            if (!request.ValidationResult.IsValid)
            {
                foreach (var o in request.ValidationResult.ValidationObjects)
                {
                    Console.WriteLine($"{o.Key}: {string.Join(",", o.Lines)}");
                }
            }
            Console.WriteLine(new string('=', 30));
        }

        private static void TestPremitive()
        {
            if (!PrimitiveTypes.Test(typeof(int)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(int?)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(DateTime)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(DateTime?)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(string)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(DateTimeOffset)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(TimeSpan)))
            {
                Console.WriteLine("Error");
            }

            if (!PrimitiveTypes.Test(typeof(Enum)))
            {
                Console.WriteLine("Error");
            }

            if (PrimitiveTypes.Test(typeof(DtoBase)))
            {
                Console.WriteLine("Error");
            }

        }

        private static void TestEnum()
        {
            object g;
            Console.WriteLine($"null: { ObjectMapperExtensions.TryMapEnum(null, typeof(Job?), out g) } : {g}");
            Console.WriteLine($"1: { ObjectMapperExtensions.TryMapEnum(1, typeof(Job?), out g) } : {g}");
            Console.WriteLine($"'1': { ObjectMapperExtensions.TryMapEnum("1", typeof(Job?), out g) } : {g}");
        }
    }
}
