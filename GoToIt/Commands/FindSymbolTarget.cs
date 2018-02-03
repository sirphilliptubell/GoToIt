using GoToIt.Constants;
using GoToIt.Extensions;
using GoToIt.Filters;
using GoToIt.Logging;
using GoToIt.Utilities;
using GoToIt.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace GoToIt.Commands
{
    /// <summary>
    /// Finds uses of Symbols.
    /// </summary>
    public class FindSymbolTarget
    {
        private readonly Func<SyntaxNode, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindSymbolTarget"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">predicate</exception>
        public FindSymbolTarget(Func<SyntaxNode, bool> predicate) {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Finds all references.
        /// </summary>
        /// <param name="symbol">The symbol to search all documents for.</param>
        /// <param name="solution">The solution to search in.</param>
        /// <param name="target">The target to add the found nodes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        internal void FindAllReferences(ISymbol symbol, Solution solution, IProducerConsumerCollection<IEnumerable<SearchResult>> target, CancellationToken cancellationToken) {
            Telemetry.WriteEvent($"{this.GetType()} started.");

            int matchCount = 0;
            int failCount = 0;

            try {
                Task.Run(() => {
                    var allTasks =
                        //get all the projects
                        solution
                        .Projects
                        .Select(async project => {
                            var compilation = await project.GetCompilationAsync(cancellationToken);

                            var semanticModels = compilation.GetSemanticModels();

                            //look through all the models
                            var findTasks = semanticModels
                                .Select(async sm => {
                                    var doc = solution.GetDocument(sm.SyntaxTree);

                                    //process each tree
                                    var searchResults = await SearchSemanticModel(doc.Name, symbol, project, sm, cancellationToken);

                                    var count = searchResults.Count;

                                    Interlocked.Add(ref matchCount, count);

                                    if (!target.TryAdd(searchResults)) {
                                        Interlocked.Increment(ref failCount);
                                    }
                                });

                            var result = Task.WhenAll(findTasks);
                            return result;
                        })
                        .Select(task => task.Unwrap())
                        .ToArray();

                    Task.WaitAll(allTasks);

                    Telemetry.WriteEvent($"{this.GetType()} completed and found {matchCount} instances. ({failCount} failures)");
                }, cancellationToken);
            }
            catch (AggregateException exs) {
                Telemetry.WriteException($"{this.GetType()} encountered exceptions.", exs.InnerExceptions.First());
            }
        }

        /// <summary>
        /// Searches the semantic model for matches.
        /// </summary>
        /// <param name="documentName">Name of the document.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="project">The project.</param>
        /// <param name="sm">The sm.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<IReadOnlyCollection<SearchResult>> SearchSemanticModel(string documentName, ISymbol symbol, Project project, SemanticModel sm, CancellationToken cancellationToken) {
            var found = (await sm.SyntaxTree.GetRootAsync(cancellationToken))
                .DescendantNodes()
                .Where(node
                    //must be the same symbol as the one we're searching for
                    => sm.GetSymbolInfo(node).Symbol == symbol
                    //must match the specified predicate
                    && _predicate(node)
                );

            var searchResults =
                found
                .Select(x => Convert(x, documentName, project.Name))
                .ToList()
                .AsReadOnly();

            return searchResults;
        }

        /// <summary>
        /// Converts the SyntaxNode into a search result.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="documentName">Name of the document.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        SearchResult Convert(SyntaxNode node, string documentName, string projectName) {
#if DEBUG
            Debug.WriteLine($"Item found: {node.GetLocation()} : {node.Parent?.Parent?.Parent}");
#endif 

            var position = node.GetLocation().GetLineSpan().StartLinePosition;

            var lines = node.GetLocation().SourceTree.GetText().Lines;

            var line = lines.Skip(position.Line).FirstOrDefault().ToString().Trim();

            return new SearchResult() {
                Line = position.Line,
                Column = position.Character,
                HasVerticalContent = false, //not sure what this is for
                Definition = node.ToString(),
                DetailsExpander = "Details Expander?",
                DocumentName = documentName,
                ProjectName = projectName,
                Text = line,
                TextInLines = node.Parent?.Parent?.ToString()
            };
        }
    }

}
