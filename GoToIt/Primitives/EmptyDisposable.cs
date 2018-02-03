using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Primitives
{
    /// <summary>
    /// An IDisposable that does nothing.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal class EmptyDisposable : IDisposable
    {
        public static readonly EmptyDisposable Instance = new EmptyDisposable();
        public void Dispose() { }
    }
}
