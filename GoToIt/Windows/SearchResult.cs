namespace GoToIt.Windows
{
    /// <summary>
    /// A search result to add to the Find All References Window.
    /// </summary>
    internal class SearchResult
    {
        public string DocumentName { get; set; }
        public string ProjectName { get; set; }
        public string Definition { get; set; }
        public bool HasVerticalContent { get; set; }
        public string DetailsExpander { get; set; }
        public string TextInLines { get; set; }
        public string Text { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }
}
