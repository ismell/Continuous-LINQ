using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Collections.Specialized;
using ContinuousLinq.Expressions;

namespace ContinuousLinq.Collections
{
    public class SelectManyReadOnlyContinuousCollection<TSource, TResult> : ReadOnlyAdapterContinuousCollection<TSource, TResult> 
    {
        internal Func<TSource, IEnumerable<TResult>> SelectorFunction { get; set; }        
        internal Dictionary<TSource, IEnumerable<TResult>> CurrentValues { get; set; }

        public SelectManyReadOnlyContinuousCollection(IList<TSource> list,
            Expression<Func<TSource, IEnumerable<TResult>>> manySelector) :
            base(list, ExpressionPropertyAnalyzer.Analyze(manySelector))
        {
            this.SelectorFunction = manySelector.CachedCompile();
            this.CurrentValues = new Dictionary<TSource, IEnumerable<TResult>>(this.Source.Count);
            RecordCurrentValues(this.Source);

            this.NotifyCollectionChangedMonitor.Add += OnAdd;
            this.NotifyCollectionChangedMonitor.Remove += OnRemove;
            this.NotifyCollectionChangedMonitor.Reset += OnReset;
            this.NotifyCollectionChangedMonitor.Move += OnMove;
            this.NotifyCollectionChangedMonitor.Replace += OnReplace;
            this.NotifyCollectionChangedMonitor.ItemChanged += OnItemChanged;
        }

        public override TResult this[int index]
        {
            /// Needs to return the value at the normalized index
            /// relative to the _output_. e.g. if we have 5 source items,
            /// we can still receive a request for item at index 32.
            get
            {
                int tempIndex = 0;
                foreach (IEnumerable<TResult> resultItems in this.CurrentValues.Values)
                {
                    if (index <= resultItems.Count() + tempIndex)
                    {
                        return resultItems.ElementAt(index - tempIndex);
                    }
                    tempIndex += resultItems.Count();
                }
                return default(TResult);
            }
            set
            {
                throw new AccessViolationException();
            }
        }

        public override int Count
        {
            get 
            {
                return this.CurrentValues.Sum(v => v.Value.Count());
            }
        }

        #region Private Utilities
        private void RecordCurrentValues(IEnumerable<TSource> items)
        {
            this.CurrentValues.Clear();
            foreach (TSource item in items)
            {
                this.CurrentValues[item] = this.SelectorFunction(item);
            }
        }

        private int TrueIndexOf(TSource targetItem)
        {
            int realIndex = 0;
            foreach (TSource currentItem in this.CurrentValues.Keys)
            {
                if (EqualityComparer<TSource>.Default.Equals(currentItem, targetItem))
                    return realIndex;

                realIndex += this.CurrentValues[currentItem].Count();
            }
            return realIndex;
        }
        #endregion

        #region Collection Change Handlers

        void OnItemChanged(INotifyPropertyChanged sender)
        {
            TSource senderAsTSource = (TSource)sender;

            IEnumerable<TResult> oldItems = this.CurrentValues[senderAsTSource];
            // Cannot infer whether items list resulting from previous version of source item
            // is the same as list resulting from modified item, so have to re-run selector func
            // every time it changes - thankfully smart notify keeps us from being notified of
            // irrelevant prop changes.
            IEnumerable<TResult> resultItems = this.SelectorFunction(senderAsTSource);

            this.CurrentValues.Remove(senderAsTSource);
            this.CurrentValues.Add(senderAsTSource, resultItems);

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                resultItems, oldItems));

        }

        void OnAdd(int index, IEnumerable<TSource> newItems)
        {
            foreach (TSource newItem in newItems)
            {
                IEnumerable<TResult> itemsToAdd = this.SelectorFunction(newItem);
                this.CurrentValues.Add(newItem, itemsToAdd);
                FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                itemsToAdd, TrueIndexOf(newItem)));
            }                        
        }

        void OnRemove(int index, IEnumerable<TSource> oldItems)
        {
            foreach (TSource oldItem in oldItems)
            {
                IEnumerable<TResult> itemsToRemove = this.CurrentValues[oldItem];
                this.CurrentValues.Remove(oldItem);
                FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    itemsToRemove));
            }
        }

        void OnReset()
        {
            /// TODO
            RecordCurrentValues(this.Source);
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnMove(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            // could possibly be more efficient, but this is very straightforward.
            OnRemove(oldStartingIndex, oldItems);
            OnAdd(newStartingIndex, newItems);
        }

        void OnReplace(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            OnMove(oldStartingIndex, oldItems, newStartingIndex, newItems);
        }
        #endregion
    }
}
