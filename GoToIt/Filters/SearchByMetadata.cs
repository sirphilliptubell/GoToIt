using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Filters
{
    class SearchBySymbol
    {
        internal void SearchDocument(Document doc, ISymbol symbol) {
            var references = SymbolFinder
                .FindReferencesAsync(symbol, doc.Project.Solution)
                .Result;

            //references.First().Locations.First().Location.SourceTree.GetRoot().FindNode();
        }
    }

    class SearchByMetadata
    {
        private readonly Dictionary<Type, IWhatFilter> _filters = new Dictionary<Type, IWhatFilter>();
        public bool UseAndOperator { get; set; }

        public SearchByMetadata() {

        }

        internal void AddOrUpdate(IWhatFilter filter) {
            var type = filter.GetType();
            _filters[type] = filter;
        }
        
        internal void SearchDocument(Document doc) {
            var infos =
                from Project in doc.Project.Solution.Projects
                let Compilation = Project.GetCompilationAsync().Result
                from Name in Compilation.Assembly.TypeNames
                let symbols = Compilation.GetSymbolsWithName(n => n == Name)
                    .Where(x => x is INamedTypeSymbol && x is ITypeSymbol)
                from symbol in symbols
                let NamedTypeSymbol = (INamedTypeSymbol)symbol
                let TypeSymbol = (ITypeSymbol)symbol
                where CombineFilters()(TypeSymbol, NamedTypeSymbol)
                select new {
                    Project,
                    Compilation,
                    Name,
                    TypeSymbol,
                    NamedTypeSymbol
                };

            //SymbolFinder.FindReferencesAsync()


        }

        private NameFilter GetNameFilter()
            => _filters.TryGetValue(typeof(NameFilter), out var result)
            ? (NameFilter)result
            : new NameFilter(null);

        private Func<ITypeSymbol, INamedTypeSymbol, bool> CombineFilters() {
            if (UseAndOperator) {
                return (n, t) => _filters.Values.All(x => x.Accepts(n, t));
            } else {
                return (n, t) => _filters.Values.Any(x => x.Accepts(n, t));
            }
        }

        /// <summary>
        /// Determines whether the specified node is searchable.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the specified node is searchable; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsSearchable(SyntaxNode node)
            => node is ClassDeclarationSyntax
            || node is StructDeclarationSyntax
            || node is InterfaceDeclarationSyntax
            || node is DelegateDeclarationSyntax
            || node is MethodDeclarationSyntax
            || node is EnumDeclarationSyntax
            || node is GenericNameSyntax;
    }
}
