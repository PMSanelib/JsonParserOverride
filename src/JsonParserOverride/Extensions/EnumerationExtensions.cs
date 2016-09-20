using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonParserOverride.Extensions
{
    public static class EnumerationExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null) return true;
            return !source.Any();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
        {
            return !IsEmpty(source);
        }

        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            items.ForEach(action);
        }

    }
}