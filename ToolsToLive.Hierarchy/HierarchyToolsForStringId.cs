﻿using System.Collections.Generic;
using System.Linq;
using ToolsToLive.Hierarchy.Interfaces;

namespace ToolsToLive.Hierarchy
{
    public class HierarchyToolsForStringId<T> : IHierarchyTools<T, string, string> where T : class, IHierarchyItem<T, string, string>
    {
        private readonly HierarchyOptions _options;

        public HierarchyToolsForStringId(HierarchyOptions options)
        {
            _options = options;
        }

        ///<inheritdoc/>
        public List<T> ToHierarhyList(IEnumerable<T> source, string selectedId)
        {
            T selectedElement = source.FirstOrDefault(x => x.Id == selectedId);

            List<T> hList = ToHierarhyList(source);
            if (selectedElement != null)
            {
                selectedElement.IsSelected = true;
                foreach (var parent in FindParents(selectedElement))
                {
                    parent.HasSelectedChild = true;
                }
            }
            return hList;
        }

        ///<inheritdoc/>
        public List<T> ToHierarhyList(IEnumerable<T> source)
        {
            int level = 1;
            List<T> HierarchyList = new List<T>();
            //go over the list of top-level elements and add child elements to them, if any
            foreach (T item in source.Where(x => x.ParentId == null))
            {
                item.HierarhyLevel = level;
                item.Childs = AddChilds(item, source, level + 1); //in this case, the old list of children is lost
                HierarchyList.Add(item);
            }
            return HierarchyList;
        }

        private List<T> AddChilds(T element, IEnumerable<T> allelements, int level)
        {
            List<T> ChildsList = new List<T>();
            foreach (T item in allelements.Where(x => x.ParentId == element.Id)) //listing all children for the current item
            {
                item.HierarhyLevel = level;
                item.Childs = AddChilds(item, allelements, level + 1); //in this case, the old list of children is lost
                if (_options.SetParents)
                {
                    item.Parent = element;
                }
                ChildsList.Add(item);
            }
            return ChildsList;
        }

        ///<inheritdoc/>
        public T FindElement(IEnumerable<T> allElementsList, string Id)
        {
            foreach (T itemNodeData in allElementsList)
            {
                if (itemNodeData.Id == Id)
                {
                    return itemNodeData;
                }

                if (itemNodeData.Childs.Count > 0)
                {
                    T returnedNodeData = FindElement(itemNodeData.Childs, Id);
                    if (returnedNodeData != null)
                    {
                        return returnedNodeData;
                    }
                }
            }
            return null;
        }

        ///<inheritdoc/>
        public List<T> FindChilds(IEnumerable<T> allElementsList, string hostId)
        {
            List<T> result = new List<T>();
            foreach (T item in allElementsList.Where(x => x.ParentId == hostId)) //Iterate over all descendants of the current element (for which Id == hostId)
            {
                result.AddRange(FindChilds(allElementsList, item.Id));
                result.Add(item);
            }

            return result;
        }

        ///<inheritdoc/>
        public List<T> FindChilds(IHierarchyItem<T, string, string> host)
        {
            List<T> result = new List<T>();
            if (host.Childs != null)
            {
                foreach (T item in host.Childs) //Iterate over all descendants of the current element (for which Id == hostId)
                {
                    result.AddRange(FindChilds(item));
                    result.Add(item);
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public List<T> FindParents(IHierarchyItem<T, string, string> element)
        {
            List<T> Parents = new List<T>();
            IHierarchyItem<T, string, string> CurrentNode = element;
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
