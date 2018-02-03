using GoToIt.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Filters
{
    interface IWhereFilter
    {
        SyntaxNode GetMatchOrDefault(SyntaxNode node);
    }

    /// <summary>
    /// Finds cases where the SyntaxNode is a Generic Type Argument.
    /// </summary>
    public class WhereGenericArgumentFilter
    {
        public static WhereGenericArgumentFilter Instance { get; }

        static WhereGenericArgumentFilter() {
            Instance = new WhereGenericArgumentFilter();
        }

        public SyntaxNode GetMatchOrDefault(SyntaxNode node)
            => node
            .Traverse(x => x.Parent)
            .TakeWhile(x => !(x is CompilationUnitSyntax))
            .FirstOrDefault(x => x.Parent is TypeArgumentListSyntax);

        public bool IsMatch(SyntaxNode node)
            => node
            .Traverse(x => x.Parent)
            .TakeWhile(x => !(x is CompilationUnitSyntax))
            .Where(x => x.Parent is TypeArgumentListSyntax)
            .Any();
    }

    /// <summary>
    /// Finds cases where the SyntaxNode is a Generic Type Argument.
    /// </summary>
    public class WhereMethodArgumentFilter {
        public static WhereMethodArgumentFilter Instance { get; }

        static WhereMethodArgumentFilter() {
            Instance = new WhereMethodArgumentFilter();
        }

        public SyntaxNode GetMatchOrDefault(SyntaxNode node)
            => node
            .Traverse(x => x.Parent)
            .TakeWhile(x => !(x is CompilationUnitSyntax))
            .FirstOrDefault(x => x.Parent is ArgumentListSyntax);

        /// <summary>
        /// Determines whether the specified node is a match.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the specified node is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(SyntaxNode node)
            => node
            .Traverse(x => x.Parent)
            .TakeWhile(x => !(x is CompilationUnitSyntax))
            .Where(x
                => x.Parent is ParameterSyntax
                || IsArgumentInFuncOrAction(x)
            )
            .Any();

        /// <summary>
        /// Determines whether a SyntaxNode is an argument in a Func or Action.
        /// </summary>
        /// <param name="node">The node.</param>
        private static bool IsArgumentInFuncOrAction(SyntaxNode node) {
            //must be an generic type argument
            var parent = node?.Parent;
            if (!(parent is TypeArgumentListSyntax)) return false;

            //Func and Action are both GenericNames
            var gParent = parent?.Parent as GenericNameSyntax;
            if (gParent == null) return false;

            var gParentIdentifier = gParent.Identifier.Text;

            //can be any argument in Action
            var isAction = gParentIdentifier == "Action";
            if (isAction) return true;

            //must not be last argument in Func
            var isFunc = gParentIdentifier == "Func";
            if (isFunc) {
                var isLastSibling = parent
                    .DescendantNodes(x => x == parent) //only descend into siblings
                    .Last() == node;
                return !isLastSibling;
            } else { //not func or action
                return false;
            }
        }
    }
}
