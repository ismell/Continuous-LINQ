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
    internal class SelectReadOnlyContinuousCollection<TSource, TResult> : ReadOnlyAdapterContinuousCollection<TSource, TResult> 
    {
        internal Func<TSource, TResult> SelectorFunction { get; set; }
        
        internal Dictionary<TSource, TResult> CurrentValues { get; set; }

        internal ListIndexer<TSource> SourceIndex { get; set; }

        public SelectReadOnlyContinuousCollection(IList<TSource> list, Expression<Func<TSource,TResult>> selectorExpression)
            : base(list, ExpressionPropertyAnalyzer.Analyze(selectorExpression))
        {
            this.SelectorFunction = selectorExpression.Compile();

            this.CurrentValues = new Dictionary<TSource, TResult>(this.Source.Count);
            RecordCurrentValues(this.Source);

            this.SourceIndex = new ListIndexer<TSource>(this.Source);

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

            HashSet<int> currentIndices = this.SourceIndex[senderAsTSource];

            foreach (int index in currentIndices)
            {
                FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
            }
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
                if (!this.SourceIndex.Contains(item))
                {
                    this.CurrentValues.Remove(item);
                }
            }
        }

        void OnAdd(int index, IEnumerable<TSource> newItems)
        {
            this.SourceIndex.Add(index, newItems);
            RecordCurrentValues(newItems);
            IEnumerable<TResult> selectedItems = newItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, selectedItems.ToList(), index));
        }

        void OnRemove(int index, IEnumerable<TSource> oldItems)
        {
            this.SourceIndex.Remove(index, oldItems); 
            RemoveCurrentValues(oldItems);
            IEnumerable<TResult> selectedItems = oldItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, selectedItems.ToList(), index));
        }

        void OnReset()
        {
            this.SourceIndex.Reset();
            this.CurrentValues.Clear();
            RecordCurrentValues(this.Source);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnMove(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            this.SourceIndex.Move(oldStartingIndex, newStartingIndex);
            IEnumerable<TResult> newSelectedItems = newItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newSelectedItems.ToList(), newStartingIndex, oldStartingIndex));
        }
        
        void OnReplace(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            this.SourceIndex.Replace(newStartingIndex, oldItems, newItems);
            RemoveCurrentValues(oldItems);
            RecordCurrentValues(newItems);
            IEnumerable<TResult> newSelectedItems = newItems.Select(this.SelectorFunction);
            IEnumerable<TResult> oldSelectedItems = oldItems.Select(this.SelectorFunction);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newSelectedItems.ToList(), oldSelectedItems.ToList(), newStartingIndex));
        }
    }
}
