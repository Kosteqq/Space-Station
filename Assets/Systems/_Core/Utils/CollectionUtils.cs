using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SpaceStation.Utils
{
    public static class CollectionUtils
    {
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> p_collection)
        {
            return p_collection.Count == 0;
        }

        public static bool IsLast<T>(this ICollection<T> p_collection, T p_element)
        {
            return p_collection.Last().Equals(p_element);
        }
    }
}