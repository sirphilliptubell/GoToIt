using GoToIt.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Filters
{
    static class FilterBuilder
    {
        internal static IEnumerable<IWhatFilter> Create(Document doc, SyntaxNode node) {
            var model = doc.GetSemanticModel();
            var symbol = model.GetDeclaredSymbol(node);
            var symbolInfo = model.GetSymbolInfo(node);

            var asType = symbol as ITypeSymbol;
            var asNamedType = symbol as INamedTypeSymbol;

            var result = new List<IWhatFilter>();

            result.Add(TypeFilter.From(asType));

            return result
                .Where(x => x.Enabled)
                .ToList();
        }
    }
}
