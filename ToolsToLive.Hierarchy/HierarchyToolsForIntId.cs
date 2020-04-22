using System.Collections.Generic;
using System.Linq;
using ToolsToLive.Hierarchy.Interfaces;

namespace ToolsToLive.Hierarchy
{
    public class HierarchyToolsForIntId<T> : IHierarchyTools<T, int, int?> where T : class, IHierarchyItem<T, int, int?>
    {
        private readonly HierarchyOptions _options;

        public HierarchyToolsForIntId(HierarchyOptions options)
        {
            _options = options;
        }

        ///<inheritdoc/>
        public List<T> ToHierarhyList(IEnumerable<T> source, int selectedId)
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
            // we do pass 'source' collection to the recursive methods. In order to avoid surprises, we create a new array for foreach.
            foreach (T item in source.Where(x => x.ParentId == null).ToArray())
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
            // we do pass 'allelements' collection to the recursive methods. In order to avoid surprises, we create a new array for foreach.
            foreach (T item in allelements.Where(x => x.ParentId == element.Id).ToArray()) //listing all children for the current item
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
        public T FindElement(IEnumerable<T> allElementsList, int Id)
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
        public List<T> FindChilds(IEnumerable<T> allElementsList, int hostId)
        {
            List<T> result = new List<T>();
            // we do pass 'allElementsList' collection to the recursive methods. In order to avoid surprises, we create a new array for foreach.
            foreach (T item in allElementsList.Where(x => x.ParentId == hostId).ToArray()) //Iterate over all descendants of the current element (for which Id == hostId)
            {
                result.AddRange(FindChilds(allElementsList, item.Id));
                result.Add(item);
            }

            return result;
        }
       
        ///<inheritdoc/>
        public List<T> FindChilds(IHierarchyItem<T, int, int?> host)
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
        public List<T> FindParents(IHierarchyItem<T, int, int?> element)
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
