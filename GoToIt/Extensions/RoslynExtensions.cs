using GoToIt.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.Linq;

namespace GoToIt.Extensions
{
    internal static class RoslynExtensions
    {
        /// <summary>
        /// Determines whether the specified node is searchable.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the specified node is searchable; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsSearchable(this SyntaxNode node)
            => SearchByMetadata.IsSearchable(node);

        /// <summary>
        /// Gets the Document of the SnapshotPoint.
        /// </summary>
        /// <param name="snapshotPoint">The textview.</param>
        /// <returns></returns>
        internal static Document GetDocument(this SnapshotPoint snapshotPoint)
            => snapshotPoint
            .Snapshot
            .GetOpenDocumentInCurrentContextWithChanges();

        /// <summary>
        /// Gets the semantic model.
        /// May return null.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        internal static SemanticModel GetSemanticModel(this Document document)
            => document
            .GetSemanticModelAsync()
            .Result;

        /// <summary>
        /// Gets the SyntaxNode under the cursor.
        /// </summary>
        /// <returns></returns>
        internal static SyntaxNode GetNodeUnderCursor(this ITextView textView) {
            var cursorPosition = textView
                .GetCursorPosition();

            //Generally we're interested in the parent of this token.
            //this token is likely an Identifier (instead of a Class/Struct/etc Declaration
            //or a curlybrace or something else not so interesting
            var token = cursorPosition
                .GetDocument()
                .GetSyntaxRootAsync().Result
                .FindToken(cursorPosition);
            
            return token.Parent;
        }

        internal static IEnumerable<SemanticModel> GetSemanticModels(this Compilation compilation)
            => compilation
            .SyntaxTrees
            .Select(t => compilation.GetSemanticModel(t))
            .ToList();
    }
}
