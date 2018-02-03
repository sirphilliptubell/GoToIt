using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace GoToIt.Extensions
{
    internal static class VisualStudioExtensions
    {
        /// <summary>
        /// Sets the visiblity of a command.
        /// </summary>
        /// <param name="oleCommands">The OLE commands.</param>
        /// <param name="commandIndex">Index of the command.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        internal static void SetVisible(this OLECMD[] oleCommands, int commandIndex, bool visible)
            => oleCommands[commandIndex].cmdf
            = visible
            ? (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED
            : 0;

        /// <summary>
        /// Gets the snapshot of the position of the cursor in the TextView.
        /// </summary>
        /// <param name="textview">The textview.</param>
        /// <returns></returns>
        internal static SnapshotPoint GetCursorPosition(this ITextView textview)
            => textview.Caret.Position.BufferPosition;

    }
}
