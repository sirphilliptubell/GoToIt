using System.ComponentModel.Composition;

namespace GoToIt.Utilities
{
    /// <summary>
    /// Currently this service is not publicly available.
    /// See http://source.roslyn.io/#Microsoft.CodeAnalysis.Features/Navigation/IDocumentNavigationService.cs,cb483617a243f389,references
    /// GitHub request to make public: https://github.com/dotnet/roslyn/issues/7328
    /// </summary>
    public interface IDocumentNavigationService
    {
        /// <summary>
        /// Tries to navigate the editor to the specified location.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        bool TryNavigateTo(EnvDTE80.DTE2 dte, string filePath, int lineNumber, int offset);
    }
}
