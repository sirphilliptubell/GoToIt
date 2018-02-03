using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Extensions
{
    internal static class GeneralExtensions
    {
        /// <summary>
        /// Starting with this instance, traverses from each item to the next using the provided selector.
        /// Ends on the last non-null item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="selectNext">The get next.</param>
        /// <returns></returns>
        internal static IEnumerable<T> Traverse<T>(this T item, Func<T, T> selectNext)
            where T : class {

            while (item != null) {
                yield return item;
                item = selectNext(item);
            }            
        }
    }
}
