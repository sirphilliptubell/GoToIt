using GoToIt.Logging;
using Microsoft.VisualStudio.Shell.TableManager;
using System;

namespace GoToIt.Windows
{
    /// <summary>
    /// Represents an entry in a SearchResultTableDataSource.
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.Shell.TableManager.ITableEntry" />
    internal class TableEntry : ITableEntry
    {
        private readonly string _identifier = Guid.NewGuid().ToString();
        private readonly SearchResult _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableEntry"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <exception cref="ArgumentNullException">entry</exception>
        public TableEntry(SearchResult entry) {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        /// <summary>
        /// Returns an object that uniquely identifies the entry.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Two entries are considered equivalent if their <see cref="P:Microsoft.VisualStudio.Shell.TableManager.ITableEntry.Identity" /> are equal using <see cref="M:System.Object.Equals(System.Object,System.Object)" />.
        /// </para>
        /// <para>
        /// This property (and the related properties in <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableEntriesSnapshot" /> are used to persist various attributes like selection state
        /// when an <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableEntry" /> is replaced with a new <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableEntry" />. Entries that replace an existing entry will have their
        /// attributes set based on the attributes of the replaced entry.
        /// </para>
        /// <para>
        /// When <see cref="M:Microsoft.VisualStudio.Shell.TableManager.ITableDataSink.ReplaceEntries(System.Collections.Generic.IReadOnlyList{Microsoft.VisualStudio.Shell.TableManager.ITableEntry},System.Collections.Generic.IReadOnlyList{Microsoft.VisualStudio.Shell.TableManager.ITableEntry})" /> is called, every entry in the
        /// list of old entries is checked to see if it has state and there is a corresponding entry among the added entries. If there is,
        /// then the two entries are considered equivalent and the old entry's attributes are copied to the new entry.
        /// </para>
        /// <para>
        /// When a <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableEntriesSnapshotFactory" /> replaces its snapshot with a new version, the entry state is transfered over exactly as if <see cref="M:Microsoft.VisualStudio.Shell.TableManager.ITableDataSink.ReplaceSnapshot(Microsoft.VisualStudio.Shell.TableManager.ITableEntriesSnapshot,Microsoft.VisualStudio.Shell.TableManager.ITableEntriesSnapshot)" /> had
        /// been called on the factory's old and new snapshots.
        /// </para>
        /// </remarks>
        public object Identity => _identifier;

        /// <summary>
        /// Can the data associated with the specified column be set?
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// <remarks>
        /// <para>This method returning true is not a guarantee that <see cref="M:Microsoft.VisualStudio.Shell.TableManager.ITableEntry.TrySetValue(System.String,System.Object)" /> will work for <paramref name="keyName" />.</para>
        /// <para>This method is normally used so that the UI displaying this <see cref="T:Microsoft.VisualStudio.Shell.TableManager.ITableEntry" /> can indicate whether or not the value can be set.</para>
        /// </remarks>
        public bool CanSetValue(string keyName) => false;

        /// <summary>
        /// Set the data associated with the specified column (if this entry has data associated with <paramref name="keyName" />).
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="content"></param>
        /// <returns>
        /// true if the value was changed.
        /// </returns>
        public bool TrySetValue(string keyName, object content) => false;

        /// <summary>
        /// Get the data associated with the specified column (if this entry has data associated with <paramref name="keyName" />).
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="content"></param>
        /// <returns>
        /// true if the entry has data associated with the column.
        /// </returns>
        /// <remarks>
        ///   <paramref name="keyName" />s are compared using a case-sensitive comparison.
        /// </remarks>
        public bool TryGetValue(string keyName, out object content) {
            content = null;

            switch (keyName) {
                case StandardTableKeyNames.DocumentName:
                    content = _entry.DocumentName;
                    break;

                case StandardTableKeyNames.ProjectName:
                case "projectnames":
                    content = _entry.ProjectName;
                    break;

                case "definition":
                    content = _entry.Definition;
                    break;

                case StandardTableKeyNames.HasVerticalContent:
                    content = _entry.HasVerticalContent;
                    break;

                case StandardTableKeyNames.DetailsExpander:
                    content = _entry.DetailsExpander;
                    break;

                case "textinlines":
                    content = _entry.TextInLines;
                    break;

                case StandardTableKeyNames.Text:
                    content = _entry.Text;
                    break;

                case StandardTableKeyNames.Line:
                    content = _entry.Line;
                    break;

                case StandardTableKeyNames.Column:
                    content = _entry.Column;
                    break;

                case "helpkeyword":
                case "helplink":
                    content = null;
                    break;

                default:
                    Telemetry.WriteEvent($"{nameof(TableEntry)}.{nameof(TryGetValue)}({keyName}) was not recognized.");
                    break;
            }

            return content != null;
        }
    }
}
