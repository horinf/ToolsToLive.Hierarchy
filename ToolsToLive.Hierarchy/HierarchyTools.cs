using System.Collections.Generic;
using System.Linq;
using ToolsToLive.Hierarchy.Interfaces;

namespace ToolsToLive.Hierarchy
{
    public static class HierarchyTools
    {
        /// <summary>
        /// Converts a flat list to a hierarchy.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="source">Flat list of the elements.</param>
        /// <param name="selectedId">Current element (null if not used or no element selected)</param>
        /// <returns>Hierarchy list.</returns>
        public static List<T> ToHierarhyList<T>(this IEnumerable<T> source, string selectedId) where T : class, IHierarchyItem<T>
        {
            T selectedElement = null;
            if (!string.IsNullOrWhiteSpace(selectedId))
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
        public static List<T> ToHierarhyList<T>(this IEnumerable<T> source) where T : IHierarchyItem<T>
        {
            int level = 1;
            List<T> HierarchyList = new List<T>();
            //пробегаемся по списку элементов верхнего уровня и добавляем в них дочерние элементы при их наличии
            foreach (T item in source.Where(x => x.ParentId == null))
            {
                item.HierarhyLevel = level;
                if (source.FirstOrDefault(x => x.ParentId == item.Id) != null) //если есть хоть один элемент, у которого родителем указан текущий, значит у текущего есть дети и их нужно добавить
                {
                    item.Childs = AddChilds(item, source, level + 1); //При этом старый список детей теряется
                }
                HierarchyList.Add(item);
            }
            return HierarchyList;
        }

        private static List<T> AddChilds<T>(T element, IEnumerable<T> allelements, int level) where T : IHierarchyItem<T>
        {
            List<T> ChildsList = new List<T>();
            foreach (T item in allelements.Where(x => x.ParentId == element.Id)) //перечисление всех дочерних элементов для текущего элемента
            {
                item.HierarhyLevel = level;
                if (allelements.FirstOrDefault(x => x.ParentId == element.Id) != null) //если есть хоть один элемент, у которого родителем указан текущий, значит у текущего есть дети и их нужно добавить
                {
                    item.Childs = AddChilds(item, allelements, level + 1); //При этом старый список детей теряется
                }
                item.Parent = element;
                ChildsList.Add(item);
            }
            return ChildsList;
        }

        /// <summary>
        /// Рекурсивный метод поиска элемента по Id в иерархической коллекции (c с указанием списка где искать и Id искомого элемента).
        /// </summary>
        /// <param name="allElementsList">Иерархический список, где нужно искать элемент</param>
        /// <param name="Id">Id искомого элемента</param>
        /// <returns>Element or null if element not found.</returns>
        public static T FindElement<T>(IEnumerable<T> allElementsList, string Id) where T : class, IHierarchyItem<T>
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
        /// Поиск всех потомков элемента с указанным hostId. Возвращает неупорядоченный список потомков без иерархической раскладки. "Владелец потомства" в список не включается.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allElementsList">Полный, не упорядоченный список элементов</param>
        /// <param name="hostId">Id элемента из списка, чьих потомков нужно найти</param>
        /// <returns>List of element (empty list if elements not found).</returns>
        public static List<T> FindChilds<T>(IEnumerable<T> allElementsList, string hostId) where T : IHierarchyItem<T>
        {
            List<T> result = new List<T>();
            foreach (T item in allElementsList.Where(x => x.ParentId == hostId)) //Перебор всех потомков текущего элемента (у которого Id==hostId)
            {
                result.AddRange(FindChilds<T>(allElementsList, item.Id));
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Поиск всех потомков указанного в параметре элемента. Возвращает неупорядоченный список потомков без иерархической раскладки. "Владелец потомства" в список не включается.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host">Элемент, чьих потомков нужно найти</param>
        /// <returns>List of element (empty list if elements not found).</returns>
        public static List<T> FindChilds<T>(IHierarchyItem<T> host) where T : IHierarchyItem<T>
        {
            List<T> result = new List<T>();
            foreach (T item in host.Childs) //Перебор всех потомков текущего элемента (у которого Id==hostId)
            {
                result.AddRange(FindChilds<T>(item));
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Составление списка родительских элементов для указанного
        /// </summary>
        /// <param name="element"></param>
        /// <returns>List of element (empty list if elements not found).</returns>
        public static List<T> FindParents<T>(IHierarchyItem<T> element) where T : IHierarchyItem<T>
        {
            List<T> Parents = new List<T>();
            IHierarchyItem<T> CurrentNode = element;
            while (CurrentNode.Parent != null)
            {
                Parents.Add(CurrentNode.Parent);
                CurrentNode = CurrentNode.Parent;
            }
            Parents.Reverse(); //Что бы родительские элементы в списке шли по порядку -- первый верхнего уровня, второй вложен в него и так далее
            return Parents;
        }
    }
}
