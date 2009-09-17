using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ContinuousLinq.Collections
{
    public class ConcatReadOnlyContinuousCollection<TSource> : ReadOnlyTwoCollectionOperationContinuousCollection<TSource>
    {
        public ConcatReadOnlyContinuousCollection(IList<TSource> first, IList<TSource> second)
            : base(first, second)
        {
            this.NotifyCollectionChangedMonitorForFirst.Add += OnAddToFirst;
            this.NotifyCollectionChangedMonitorForFirst.Remove += OnRemoveFromFirst;
            this.NotifyCollectionChangedMonitorForFirst.Reset += OnResetFirst;
            this.NotifyCollectionChangedMonitorForFirst.Replace += OnReplaceOnFirst;

            this.NotifyCollectionChangedMonitorForSecond.Add += OnAddToSecond;
            this.NotifyCollectionChangedMonitorForSecond.Remove += OnRemoveFromSecond;
            this.NotifyCollectionChangedMonitorForSecond.Reset += OnResetSecond;
            this.NotifyCollectionChangedMonitorForSecond.Replace += OnReplaceOnSecond;
        }

        public override TSource this[int index]
        {
            get { return index < this.First.Count ? this.First[index] : this.Second[index - this.First.Count]; }
            set { throw new AccessViolationException(); }
        }

        public override int Count
        {
            get { return this.First.Count + this.Second.Count; }
        }

        #region First Event Handlers

        void OnAddToFirst(int index, IEnumerable<TSource> newItems)
        {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems.ToList(), index));
        }

        void OnRemoveFromFirst(int index, IEnumerable<TSource> oldItems)
        {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems.ToList(), index));
        }

        void OnResetFirst()
        {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnReplaceOnFirst(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems.ToList(), oldItems.ToList(), oldStartingIndex));
        }

        #endregion

        #region Second Event Handlers

        void OnAddToSecond(int index, IEnumerable<TSource> newItems)
        {
            int adjustedIndex = this.First.Count + index;

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems.ToList(), adjustedIndex));
        }

        void OnRemoveFromSecond(int index, IEnumerable<TSource> oldItems)
        {
            int adjustedIndex = this.First.Count + index;

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems.ToList(), adjustedIndex));
        }

        void OnResetSecond()
        {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnReplaceOnSecond(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            int adjustedIndex = this.First.Count + oldStartingIndex;

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems.ToList(),
                                                                       oldItems.ToList(), adjustedIndex));
        }

        #endregion
    }
}