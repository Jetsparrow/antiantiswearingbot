using System;
using System.Collections.Generic;

namespace AntiAntiSwearingBot
{
    public static class IListExtensions
    {
        public static void Move<T>(this IList<T> list, int from, int to)
        {
            if (from < 0 || from > list.Count)
                throw new ArgumentOutOfRangeException("from");
            if (to < 0 || to > list.Count)
                throw new ArgumentOutOfRangeException("to");
            if (from == to)
                return;

            var item = list[from];
            list.RemoveAt(from);
            if (to > from) --to; // the actual index could have shifted due to the removal
            list.Insert(to, item);
        }
    }
}
