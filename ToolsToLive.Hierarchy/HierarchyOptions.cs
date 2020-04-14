namespace ToolsToLive.Hierarchy
{
    public class HierarchyOptions
    {
        /// <summary>
        /// Indicates if during hierarchy creating parents should be set to their children as a link (all parents will contain children in "Childs" list property and each children will contain link to parent in "Parent" property)
        /// Be careful, if true - during JSON serialization an endless loop will be detected and most likely exception will be thrown.
        /// But if false - method to finde parents won't work.
        /// False by default.
        /// </summary>
        public bool SetParents { get; set; }
    }
}
