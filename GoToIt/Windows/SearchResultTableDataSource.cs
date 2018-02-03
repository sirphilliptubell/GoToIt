using GoToIt.Primitives;
using Microsoft.VisualStudio.Shell.TableManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace GoToIt.Windows
{
    /// <summary>
    /// Provides Search Result entries.
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.Shell.TableManager.ITableDataSource" />
    internal class SearchResultTableDataSource : ITableDataSource
    {
        private readonly HashSet<ITableDataSink> _sinks = new HashSet<ITableDataSink>();
        private readonly string _identifier = Guid.NewGuid().ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultTableDataSource"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public SearchResultTableDataSource(IProducerConsumerCollection<IEnumerable<SearchResult>> collection, CancellationToken cancellationToken) {
            var blocking = new BlockingCollection<IEnumerable<SearchResult>>(collection);

            Task.Run(() => {
                try {
                    foreach (var item in blocking.GetConsumingEnumerable(cancellationToken)) {
                        var entries = item
                            .Select(x => new TableEntry(x))
                            .ToList()
                            .AsReadOnly();

                        foreach (var sink in _sinks) {
                            sink.AddEntries(entries);
                        }
                    }
                }
                catch (OperationCanceledException) {
                }
            });            
        }

        /// <summary>
        /// Identifier that describes the type of entries provided by this source (e.g. <see cref="F:Microsoft.VisualStudio.Shell.TableManager.StandardTableDataSources.CommentTableDataSource" />)
        /// </summary>
        /// <remarks>
        /// <para>Different sources can have the same identifier (e.g. there could be multiple sources of <see cref="F:Microsoft.VisualStudio.Shell.TableManager.StandardTableDataSources.ErrorTableDataSource" />).</para>
        /// <para>This identifier cannot change over the lifetime of the <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableDataSource" />.</para>
        /// </remarks>
        public string SourceTypeIdentifier => StandardTableDataSources.ErrorTableDataSource;

        /// <summary>
        /// Unique identifier of this data source.
        /// </summary>
        /// <remarks>
        /// This identifier cannot change over the lifetime of the <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableDataSource" />.
        /// </remarks>
        public string Identifier => _identifier;

        /// <summary>
        /// Localized name to identify the source in any UI displayed to the user. Can be null.
        /// </summary>
        public string DisplayName => "GoToIt Results";

        /// <summary>
        /// Subscribe to <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableEntry" />s created by this data source.
        /// </summary>
        /// <param name="sink">Contains methods called when the entries provided by the source change.</param>
        /// <returns>
        /// A key that controls the lifetime of the subscription. The <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableDataSource" /> must continue to provide updates until either the key is disposed
        /// or the source is removed from the table (which will, as a side-effect, cause the key to be disposed of).
        /// </returns>
        /// <remarks>
        /// <para>If, when the call is made to subscribe to a <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableDataSource" />, the source already has entries, then the source needs to add these entries using <paramref name="sink" />. It can make the call to add the entries
        /// before returning from the <see cref="M:Microsoft.VisualStudio.Shell.TableManager.ITableDataSource.Subscribe(Microsoft.VisualStudio.Shell.TableManager.ITableDataSink)" /> call.</para>
        /// <para>A <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableDataSource" /> can have multiple, simultaneous subscribers (and each subscriber will have its own <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableDataSink" />.</para>
        /// </remarks>
        public IDisposable Subscribe(ITableDataSink sink) {
            _sinks.Add(sink);

            return EmptyDisposable.Instance;
        }
    }


}
