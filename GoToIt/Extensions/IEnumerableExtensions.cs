using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Extensions
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Tries to get the index of the first item that matches the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal static bool TryFirstIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate, out int index) {
            var i = 0;
            foreach (var item in collection) {
                if (predicate(item)) {
                    index = i;
                    return true;
                }
                i++;
            }

            index = -1;
            return false;
        }
    }
}
