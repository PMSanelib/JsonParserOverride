using System;
using System.Linq;
using System.Reflection;

namespace JsonParserOverride.Extensions
{
    internal static class PrimitiveTypes
    {
        public static readonly Type[] List;

        static PrimitiveTypes()
        {
            var types = new[]
            {
                typeof (Enum),
                typeof (string),
                typeof (char),
                typeof (Guid),

                typeof (bool),
                typeof (byte),
                typeof (short),
                typeof (int),
                typeof (long),
                typeof (float),
                typeof (double),
                typeof (decimal),

                typeof (sbyte),
                typeof (ushort),
                typeof (uint),
                typeof (ulong),

                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan)
            };


            var nullTypes = from t in types where t.GetTypeInfo().IsValueType select typeof(Nullable<>).MakeGenericType(t);

            List = types.Concat(nullTypes).ToArray();
        }

        public static bool Test(Type type)
        {
            if (List.Any(x => x.IsAssignableFrom(type))) return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.GetTypeInfo().IsEnum;
        }
    }
}