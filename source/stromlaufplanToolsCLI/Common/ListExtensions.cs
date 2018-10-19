using System.Collections.Generic;

namespace stromlaufplanToolsCLI.Common
{
    public static class ListExtensions
    {
        public static void RemoveRange<T>(this List<T> originList, IEnumerable<T> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
            {
                originList.Remove(item);
            }
        }
    }
}