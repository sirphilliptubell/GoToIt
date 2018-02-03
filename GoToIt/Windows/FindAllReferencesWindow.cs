using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.FindAllReferences;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoToIt.Windows
{
    /// <summary>
    /// Controls the "Find all references" window.
    /// </summary>
    [Export]
    public class FindAllReferencesWindow
    {
        [Import]
        internal SVsServiceProvider ServiceProvider;

        private IFindAllReferencesWindow window;
                
        /// <summary>
        /// Resets the "Find All References" window and loads the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        internal void Load(IProducerConsumerCollection<IEnumerable<SearchResult>> collection, CancellationToken cancellationToken) {
            var service = (IFindAllReferencesService)ServiceProvider.GetService(typeof(SVsFindAllReferences));
            //start a new search that doesn't actually search anything.
            window = service.StartSearch("");

            //the table that will take the search results and add them to the window
            var source = new SearchResultTableDataSource(collection, cancellationToken);

            //add the table as a source for the window
            window.Manager.AddSource(source);
        }
    }
}
