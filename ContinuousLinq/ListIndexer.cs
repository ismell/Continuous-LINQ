using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Collections.Specialized;
using System.Collections;

namespace ContinuousLinq
{
    public class ListIndexer<TSource>
    {
        private IList<TSource> Source { get; set; }

        private Dictionary<TSource, HashSet<int>> CurrentIndices { get; set; }

        public HashSet<int> this[TSource item]
        {
            get { return this.CurrentIndices[item]; }
        }

        public ListIndexer(IList<TSource> source)
        {
            this.Source = source;
            this.CurrentIndices = new Dictionary<TSource, HashSet<int>>(this.Source.Count);
            RecordIndicesOfItemsInSource();
        }

        public bool Contains(TSource item)
        {
            return this.CurrentIndices.ContainsKey(item);
        }

        private void RecordIndicesOfItemsInSource()
        {
            for (int i = 0; i < this.Source.Count; i++)
            {
                RecordIndexOfItem(i);
            }
        }

        private void RecordIndexOfItem(int i)
        {
            TSource itemInSource = this.Source[i];

            HashSet<int> currentIndices = GetIndices(itemInSource);
            currentIndices.Add(i);
        }

        private HashSet<int> GetIndices(TSource itemInSource)
        {
            HashSet<int> currentIndices;
            if (!this.CurrentIndices.TryGetValue(itemInSource, out currentIndices))
            {
                currentIndices = new HashSet<int>();
                this.CurrentIndices[itemInSource] = currentIndices;
            }
            return currentIndices;
        }

        public void Add(int index, IEnumerable<TSource> newItems)
        {
            int numberOfNewItems = 0;
            foreach (TSource item in newItems)
            {
                var indices = GetIndices(item);
                indices.Add(index++);
                numberOfNewItems++;
            }

            int previousIndexOfCurrentItem = index - numberOfNewItems;
            ReindexRestOfList(index, previousIndexOfCurrentItem);
        }

        private void ReindexRestOfList(int index, int previousIndexOfCurrentItem)
        {
            for (; index < this.Source.Count; index++, previousIndexOfCurrentItem++)
            {
                UpdateIndex(index, previousIndexOfCurrentItem);
            }
        }

        private void UpdateIndex(int newIndex, int previousIndex)
        {
            HashSet<int> indices = this.CurrentIndices[this.Source[newIndex]];
            if (previousIndex >= this.Source.Count ||
                !EqualityComparer<TSource>.Default.Equals(this.Source[newIndex], this.Source[previousIndex]))
            {
                indices.Remove(previousIndex);
            }
            indices.Add(newIndex);
        }

        public void Remove(int index, IEnumerable<TSource> oldItems)
        {
            int numberOfOldItems = 0;
            int oldIndex = index;
            foreach (TSource item in oldItems)
            {
                RemoveFromIndexTable(item, oldIndex++);
                numberOfOldItems++;
            }

            int previousIndexOfCurrentItem = index + numberOfOldItems;
            ReindexRestOfList(index, previousIndexOfCurrentItem);
        }

        public void Reset()
        {
            this.CurrentIndices.Clear();
            RecordIndicesOfItemsInSource(); 
        }

        private void RemoveFromIndexTable(TSource item, int indexToRemove)
        {
            var indices = GetIndices(item);
            if (indices.Count <= 1)
            {
                this.CurrentIndices.Remove(item);
            }
            else
            {
                indices.Remove(indexToRemove);
            }
        }

        public void Replace(int index, IEnumerable<TSource> oldItems, IEnumerable<TSource> newItems)
        {
            int oldIndex = index;
            foreach (TSource item in oldItems)
            {
                RemoveFromIndexTable(item, oldIndex++);
            }

            foreach (TSource item in newItems)
            {
                var indices = GetIndices(item);
                indices.Add(index++);
            }
        }

        public void Move(int oldStartingIndex, int newStartingIndex)
        {
            UpdateIndex(newStartingIndex, oldStartingIndex);
            int firstIndexToStartUpdating = Math.Min(oldStartingIndex, newStartingIndex);
            int lastIndexToUpdate = Math.Max(oldStartingIndex, newStartingIndex);

            int incrementerToFindOldIndex = Math.Sign(newStartingIndex - oldStartingIndex);
            for (int i = firstIndexToStartUpdating; i <= lastIndexToUpdate; i++)
            {
                if (i == newStartingIndex)
                    continue;

                UpdateIndex(i, i + incrementerToFindOldIndex);
            }
        }

    }
}
