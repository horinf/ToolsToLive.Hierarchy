using System.Collections.Generic;

namespace ToolsToLive.Hierarchy.Interfaces
{
    /// <summary>
    /// An element in hierarchy, that can contain children - the same type elements.
    /// </summary>
    /// <typeparam name="T">Type of the element.</typeparam>
    /// <typeparam name="TId">Type of the identifier field (e.g. string or int).</typeparam>
    /// <typeparam name="TParentId">Type of the parent identifier (should be nullable and comparable to Id) (e.g. string, int?)TParentId.</typeparam>
    public interface IHierarchyItem<T, TId, TParentId>
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        TId Id { get; }

        /// <summary>
        /// Parent Id.
        /// </summary>
        TParentId ParentId { get; }

        /// <summary>
        /// Parent.
        /// </summary>
        T Parent { get; set; }

        /// <summary>
        /// Children of the element.
        /// </summary>
        List<T> Childs { get; set; }

        /// <summary>
        /// Level in hierarchy (1 - is the top level)
        /// </summary>
        int HierarhyLevel { get; set; }

        /// <summary>
        /// Is it the element, that marked as selected (or current).
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Indicates if element has (contain) child that marked as selected.
        /// Can be used to expand tree branch.
        /// </summary>
        bool HasSelectedChild { get; set; }
    }
}
