using System.Collections.Generic;

namespace ToolsToLive.Hierarchy.Interfaces
{
    public interface IHierarchyTools<T, TId, TParentId> where T : class, IHierarchyItem<T, TId, TParentId>
    {
        /// <summary>
        /// Converts a flat list to a hierarchy.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="source">Flat list of the elements.</param>
        /// <param name="selectedId">Current element (null if not used or no element selected).</param>
        /// <returns>Hierarchy list.</returns>
        List<T> ToHierarhyList(IEnumerable<T> source, TId selectedId);

        /// <summary>
        /// Converts a flat list to a hierarchy.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="source">Flat list of the elements.</param>
        /// <returns>Hierarchy list.</returns>
        List<T> ToHierarhyList(IEnumerable<T> source);

        /// <summary>
        /// A recursive method for finding an element by Id in a hierarchical collection (with a list of where to look for and Id of the element to be searched).
        /// </summary>
        /// <param name="allElementsList">The hierarchical list in which to search for an item.</param>
        /// <param name="Id">Item Id.</param>
        /// <returns>Element or null if element not found.</returns>
        T FindElement(IEnumerable<T> allElementsList, TId Id);

        /// <summary>
        /// Search for all descendants of an element with the specified hostId. Returns an unordered list of descendants without a hierarchical layout. "Owner of the offspring" is not included in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allElementsList">Complete, unordered list of items.</param>
        /// <param name="hostId">Id of the item from the list whose descendants need to be found.</param>
        /// <returns>List of element (empty list if elements not found).</returns>
        List<T> FindChilds(IEnumerable<T> allElementsList, TId hostId);

        /// <summary>
        /// Search for all descendants of the element specified in the parameter. Returns an unordered list of descendants without a hierarchical layout. "Owner of the offspring" is not included in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host">An element whose descendants need to be found.</param>
        /// <returns>List of element (empty list if elements not found).</returns>
        List<T> FindChilds(IHierarchyItem<T, TId, TParentId> host);

        /// <summary>
        /// Listing Parent Elements for a Specified element.
        /// <see cref="HierarchyOptions.SetParents"/> should be set to true, otherwise this method won't work.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>List of element (empty list if elements not found).</returns>
        List<T> FindParents(IHierarchyItem<T, TId, TParentId> element);
    }
}
