using GoToIt.Logging;
using System;

namespace GoToIt.Utilities
{
    /// <summary>
    /// Handles navigating to documents in the text editor.
    /// </summary>
    /// <seealso cref="GoToIt.Utilities.IDocumentNavigationService" />
    internal class DocumentNavigationService : IDocumentNavigationService
    {
        private readonly EnvDTE80.DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentNavigationService"/> class.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <exception cref="ArgumentNullException">dte</exception>
        public DocumentNavigationService(EnvDTE80.DTE2 dte) {
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        /// <summary>
        /// Tries to navigate the editor to the specified location.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public bool TryNavigateTo(EnvDTE80.DTE2 dte, string filePath, int lineNumber, int offset) {
            Telemetry.WriteEvent($"Trying to Navigate to {lineNumber}:{offset} @ {filePath}");
            try {
                //bring the window forward
                _dte.MainWindow.Activate();

                EnvDTE.Window w = _dte.ItemOperations.OpenFile(filePath, EnvDTE.Constants.vsViewKindTextView);

                ((EnvDTE.TextSelection)_dte.ActiveDocument.Selection).MoveToLineAndOffset(lineNumber, offset);

                return true;
            }
            catch (Exception ex) {
                Telemetry.WriteException("DocumentNavigationService.TryNavigateTo", ex);
                return false;
            }
        }
    }
}
