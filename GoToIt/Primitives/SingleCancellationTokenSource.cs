using System;
using System.Threading;

namespace GoToIt.Primitives
{
    /// <summary>
    /// Provides CancellationTokens while ensuring each time a new one is retrieved, the previous ones are cancelled.
    /// </summary>
    internal class SingleCancellationTokenSource : IDisposable
    {
        private readonly object _syncRoot = new object();
        private CancellationTokenSource _source = new CancellationTokenSource();

        public void Dispose() {
            lock (_syncRoot) {
                if (_source != null) {
                    _source.Cancel();
                    _source.Dispose();
                    _source = null;
                }
            }
        }

        /// <summary>
        /// Cancels the existing token and gets a new one.
        /// </summary>
        /// <returns></returns>
        public CancellationToken Next() {
            lock (_syncRoot) {
                if (_source == null) {
                    throw new ObjectDisposedException(nameof(SingleCancellationTokenSource));
                }
                _source.Cancel();
                _source.Dispose();
                _source = new CancellationTokenSource();
                return _source.Token;
            }
        }
    }
}
