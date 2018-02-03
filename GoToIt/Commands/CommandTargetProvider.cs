using GoToIt.Utilities;
using GoToIt.Windows;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace GoToIt.Commands
{
    [Name("GoToIt.TextViewCreationListener")]
    [Export(typeof(IVsTextViewCreationListener))]    
    [ContentType(ContentTypes.CSharp)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class CommandTargetProvider : IVsTextViewCreationListener
    {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterFactoryService { get; set; }
        
        internal IDocumentNavigationService DocumentNavigationService { get; }

        [Import]
        internal FindAllReferencesWindow Window { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTargetProvider"/> class.
        /// </summary>
        public CommandTargetProvider() {
            var dte2 = (EnvDTE80.DTE2)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            DocumentNavigationService = new DocumentNavigationService(dte2);
        }

        /// <summary>
        /// Called when a <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView" /> adapter has been created and initialized.
        /// </summary>
        /// <param name="textViewAdapter">The newly created and initialized text view
        /// adapter.</param>
        public void VsTextViewCreated(IVsTextView textViewAdapter) {
            var textView = AdapterFactoryService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            textView
                .Properties
                .GetOrCreateSingletonProperty(() => new CommandTarget(textViewAdapter, textView, DocumentNavigationService, Window));
        }
    }
}
