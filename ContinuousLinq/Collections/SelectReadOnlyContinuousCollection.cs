using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq
{
    public class SelectReadOnlyContinuousCollection<TSource, TResult> : ReadOnlyAdapterContinuousCollection<TSource, TResult> 
    {
        internal Func<TSource, TResult> SelectorFunction { get; set; }
        internal Dictionary<TSource, TResult> CurrentValues { get; set; }

        internal Dictionary<TSource, int> CurrentIndices { get; set; }

        public SelectReadOnlyContinuousCollection(IList<TSource> list, Expression<Func<TSource,TResult>> selectorExpression)
            : base(list, ExpressionPropertyAnalyzer.Analyze(selectorExpression))
        {
            this.SelectorFunction = selectorExpression.Compile();

            this.CurrentValues = new Dictionary<TSource, TResult>(this.Source.Count);
            RecordCurrentValues(this.Source);

            this.CurrentIndices = new Dictionary<TSource, int>(this.Source.Count);
            RecordIndicesOfItemsInSource();

            this.NotifyCollectionChangedMonitor.Add += OnAdd;
            this.NotifyCollectionChangedMonitor.Remove += OnRemove;
            this.NotifyCollectionChangedMonitor.Reset += OnReset;
            this.NotifyCollectionChangedMonitor.Move += OnMove;
            this.NotifyCollectionChangedMonitor.Replace += OnReplace;
            this.NotifyCollectionChangedMonitor.ItemChanged += OnItemChanged;
        }

        void OnItemChanged(INotifyPropertyChanged sender)
        {
            TSource senderAsTSource = (TSource)sender;
            
            TResult oldValue = this.CurrentValues[senderAsTSource];
            TResult newValue = this.SelectorFunction(senderAsTSource);
            
            if (EqualityComparer<TResult>.Default.Equals(oldValue, newValue))
                return;

            this.CurrentValues[senderAsTSource] = newValue;

            int index = this.CurrentIndices[senderAsTSource];
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
        }

        public override int Count
        {
            get { return this.Source.Count; }
        }

        public override TResult this[int index]
        {
            get { return this.CurrentValues[this.Source[index]]; }
            set { throw new AccessViolationException(); }
        }

        private void RecordIndicesOfItemsInSource()
        {
            for (int i = 0; i < this.Source.Count; i++)
            {
                this.CurrentIndices[this.Source[i]] = i;
            }
        }

        private void RecordCurrentValues(IEnumerable<TSource> items)
        {
            foreach (TSource item in items)
            {
                this.CurrentValues[item] = this.SelectorFunction(item);
            }
        }

        private void RemoveCurrentValues(IEnumerable<TSource> items)
        {
            foreach (TSource item in items)
            {
                this.CurrentValues.Remove(item);
            }
        }

        private void InsertIntoIndexTable(int index, IEnumerable<TSource> newItems)
        {
            foreach (TSource item in newItems)
            {
                this.CurrentIndices[item] = index++;
            }

            for (; index < this.Source.Count; index++)
            {
                this.CurrentIndices[this.Source[index]] = index;
            }
        }

        private void RemoveFromIndexTable(int index, IEnumerable<TSource> oldItems)
        {
            foreach (TSource item in oldItems)
            {
                this.CurrentIndices.Remove(item);
            }

            for (; index < this.Source.Count; index++)
            {
                this.CurrentIndices[this.Source[index]] = index;
            }
        }

        private void ReplaceInIndexTable(int index, IEnumerable<TSource> oldItems, IEnumerable<TSource> newItems)
        {
            foreach (TSource item in oldItems)
            {
                this.CurrentIndices.Remove(item);
            }

            foreach (TSource item in newItems)
            {
                this.CurrentIndices[item] = index++;
            }
        }

        private void MoveInIndexTable(int oldStartingIndex, int newStartingIndex)
        {
            int firstIndexToStartUpdating = Math.Min(oldStartingIndex, newStartingIndex);
            int lastIndexToUpdate = Math.Max(oldStartingIndex, newStartingIndex);

            for (int i = firstIndexToStartUpdating; i <= lastIndexToUpdate; i++)
            {
                this.CurrentIndices[this.Source[i]] = i;
            }
        }

        void OnAdd(int index, IEnumerable<TSource> newItems)
        {
            RecordCurrentValues(newItems);
            InsertIntoIndexTable(index, newItems);
            IEnumerable<TResult> selectedItems = newItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, selectedItems.ToList(), index));
        }

        void OnRemove(int index, IEnumerable<TSource> oldItems)
        {
            RemoveCurrentValues(oldItems);
            RemoveFromIndexTable(index, oldItems);
            IEnumerable<TResult> selectedItems = oldItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, selectedItems.ToList(), index));
        }

        void OnReset()
        {
            this.CurrentIndices.Clear();
            this.CurrentValues.Clear();
            RecordCurrentValues(this.Source);
            RecordIndicesOfItemsInSource();
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnMove(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            MoveInIndexTable(oldStartingIndex, newStartingIndex);
            IEnumerable<TResult> newSelectedItems = newItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newSelectedItems.ToList(), newStartingIndex, oldStartingIndex));
        }
        
        void OnReplace(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            ReplaceInIndexTable(newStartingIndex, oldItems, newItems);
            RemoveCurrentValues(oldItems);
            RecordCurrentValues(newItems);
            IEnumerable<TResult> newSelectedItems = newItems.Select(this.SelectorFunction);
            IEnumerable<TResult> oldSelectedItems = oldItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newSelectedItems.ToList(), oldSelectedItems.ToList(), newStartingIndex));
        }
    }
}
