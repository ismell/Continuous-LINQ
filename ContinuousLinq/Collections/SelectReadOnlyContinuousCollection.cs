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

            if (this.NotifyCollectionChangedMonitor.IsMonitoringChildProperties)
            {
                this.SourceIndex = new ListIndexer<TSource>(this.Source);
            }

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
                if (!this.NotifyCollectionChangedMonitor.ReferenceCountTracker.Contains(item))
                {
                    this.CurrentValues.Remove(item);
                }
            }
        }

        void OnAdd(int index, IEnumerable<TSource> newItems)
        {
            if (this.NotifyCollectionChangedMonitor.IsMonitoringChildProperties)
            {
                this.SourceIndex.Add(index, newItems);
            }

            RecordCurrentValues(newItems);
            List<TResult> selectedItems = GetCurrentValues(newItems);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, selectedItems, index));
        }

        void OnRemove(int index, IEnumerable<TSource> oldItems)
        {
            if (this.NotifyCollectionChangedMonitor.IsMonitoringChildProperties)
            {
                this.SourceIndex.Remove(index, oldItems);
            }
            List<TResult> oldValues = GetCurrentValues(oldItems);
            RemoveCurrentValues(oldItems);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldValues, index));
        }

        private List<TResult> GetCurrentValues(IEnumerable<TSource> items)
        {
            List<TResult> oldValues = new List<TResult>();
            foreach (TSource item in items)
            {
                oldValues.Add(this.CurrentValues[item]);
            }
            return oldValues;
        }
        
        void OnReset()
        {
            if (this.NotifyCollectionChangedMonitor.IsMonitoringChildProperties)
            {
                this.SourceIndex.Reset();
            }
            this.CurrentValues.Clear();
            RecordCurrentValues(this.Source);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnMove(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            if (this.NotifyCollectionChangedMonitor.IsMonitoringChildProperties)
            {
                this.SourceIndex.Move(oldStartingIndex, newStartingIndex);
            }
            List<TResult> newSelectedItems = GetCurrentValues(newItems);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newSelectedItems, newStartingIndex, oldStartingIndex));
        }
        
        void OnReplace(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            if (this.NotifyCollectionChangedMonitor.IsMonitoringChildProperties)
            {
                this.SourceIndex.Replace(newStartingIndex, oldItems, newItems);
            }
            List<TResult> oldValues = GetCurrentValues(oldItems);
            RemoveCurrentValues(oldItems);
            RecordCurrentValues(newItems);
            List<TResult> newSelectedItems = GetCurrentValues(newItems);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newSelectedItems, oldValues, newStartingIndex));
        }
    }
}
