using System.Collections.Generic;
using System.Linq;
using ToolsToLive.Hierarchy.Interfaces;

namespace ToolsToLive.Hierarchy
{
    public static class HierarchyToolsForIntId
    {
        /// <summary>
        /// Converts a flat list to a hierarchy.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="source">Flat list of the elements.</param>
        /// <param name="selectedId">Current element (null if not used or no element selected)</param>
        /// <returns>Hierarchy list.</returns>
        public static List<T> ToHierarhyList<T>(this IEnumerable<T> source, int? selectedId) where T : class, IHierarchyItem<T, int, int?>
        {
            T selectedElement = null;
            if (selectedId.HasValue)
                selectedElement = source.FirstOrDefault(x => x.Id == selectedId);

            List<T> hList = source.ToHierarhyList();
            if (selectedElement != null)
            {
                selectedElement.IsSelected = true;
                foreach (var parent in FindParents(selectedElement))
                    parent.HasSelectedChild = true;
            }
            return hList;
        }

        /// <summary>
        /// Converts a flat list to a hierarchy.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="source">Flat list of the elements.</param>
        /// <returns>Hierarchy list.</returns>
        public static List<T> ToHierarhyList<T>(this IEnumerable<T> source) where T : IHierarchyItem<T, int, int?>
        {
            int level = 1;
            List<T> HierarchyList = new List<T>();
            //go over the list of top-level elements and add child elements to them, if any
            foreach (T item in source.Where(x => x.ParentId == null))
            {
                item.HierarhyLevel = level;
                if (source.FirstOrDefault(x => x.ParentId == item.Id) != null) //if there is at least one element whose current is indicated by the parent, then the current has children and they need to be added
                {
                    item.Childs = AddChilds(item, source, level + 1); //in this case, the old list of children is lost
                }
                HierarchyList.Add(item);
            }
            return HierarchyList;
        }

        private static List<T> AddChilds<T>(T element, IEnumerable<T> allelements, int level) where T : IHierarchyItem<T, int, int?>
        {
            List<T> ChildsList = new List<T>();
            foreach (T item in allelements.Where(x => x.ParentId == element.Id)) //listing all children for the current item
            {
                item.HierarhyLevel = level;
                if (allelements.FirstOrDefault(x => x.ParentId == element.Id) != null) //if there is at least one element whose current is indicated by the parent, then the current has children and they need to be added
                {
                    item.Childs = AddChilds(item, allelements, level + 1); //in this case, the old list of children is lost
                }
                item.Parent = element;
                ChildsList.Add(item);
            }
            return ChildsList;
        }

        /// <summary>
        /// A recursive method for finding an element by Id in a hierarchical collection (with a list of where to look for and Id of the element to be searched).
        /// </summary>
        /// <param name="allElementsList">The hierarchical list in which to search for an item.</param>
        /// <param name="Id">Item Id.</param>
        /// <returns>Element or null if element not found.</returns>
        public static T FindElement<T>(IEnumerable<T> allElementsList, int Id) where T : class, IHierarchyItem<T, int, int?>
        {
            foreach (T itemNodeData in allElementsList)
            {
                if (itemNodeData.Id == Id)
                    return itemNodeData;

                if (itemNodeData.Childs.Count > 0)
                {
                    T returnedNodeData = FindElement<T>(itemNodeData.Childs, Id);
                    if (returnedNodeData != null)
                        return returnedNodeData;
                }
            }
            return null;
        }

        /// <summary>
        /// Search for all descendants of an element with the specified hostId. Returns an unordered list of descendants without a hierarchical layout. "Owner of the offspring" is not included in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allElementsList">Complete, unordered list of items</param>
        /// <param name="hostId">Id of the item from the list whose descendants need to be found</param>
        /// <returns>List of element (empty list if elements not found).</returns>
        public static List<T> FindChilds<T>(IEnumerable<T> allElementsList, int hostId) where T : IHierarchyItem<T, int, int?>
        {
            List<T> result = new List<T>();
            foreach (T item in allElementsList.Where(x => x.ParentId == hostId)) //Iterate over all descendants of the current element (for which Id == hostId)
            {
                result.AddRange(FindChilds<T>(allElementsList, item.Id));
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Search for all descendants of the element specified in the parameter. Returns an unordered list of descendants without a hierarchical layout. "Owner of the offspring" is not included in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host">An element whose descendants need to be found</param>
        /// <returns>List of element (empty list if elements not found).</returns>
        public static List<T> FindChilds<T>(IHierarchyItem<T, int, int?> host) where T : IHierarchyItem<T, int, int?>
        {
            List<T> result = new List<T>();
            if (host.Childs != null)
            {
                foreach (T item in host.Childs) //Iterate over all descendants of the current element (for which Id == hostId)
                {
                    result.AddRange(FindChilds<T>(item));
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Listing Parent Elements for a Specified element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>List of element (empty list if elements not found).</returns>
        public static List<T> FindParents<T>(IHierarchyItem<T, int, int?> element) where T : IHierarchyItem<T, int, int?>
        {
            List<T> Parents = new List<T>();
            IHierarchyItem<T, int, int?> CurrentNode = element;
            while (CurrentNode.Parent != null)
            {
                Parents.Add(CurrentNode.Parent);
                CurrentNode = CurrentNode.Parent;
            }
            Parents.Reverse(); //So that the parent items in the list go in order - the first is the top level, the second is nested in it, and so on
            return Parents;
        }
    }
}
