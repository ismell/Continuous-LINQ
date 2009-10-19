using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ContinuousLinq.Collections
{
    public class ExceptReadOnlyContinuousCollection<TSource> : ReadOnlyTwoCollectionOperationContinuousCollection<TSource>
    {
        public ExceptReadOnlyContinuousCollection(IList<TSource> first, IList<TSource> second) : base(first, second)
        {
            this.Output = new List<TSource>();

            
            this.FirstItemLookup = new ReferenceCountTracker<TSource>();
            this.SecondItemLookup = new ReferenceCountTracker<TSource>(this.Second);

            this.AddItemsToFirst(this.First);
            
            this.NotifyCollectionChangedMonitorForFirst.Add += OnAddToFirst;
            this.NotifyCollectionChangedMonitorForFirst.Remove += OnRemoveFromFirst;
            this.NotifyCollectionChangedMonitorForFirst.Reset += OnResetFirst;
            this.NotifyCollectionChangedMonitorForFirst.Replace += OnReplaceOnFirst;

            this.NotifyCollectionChangedMonitorForSecond.Add += OnAddToSecond;
            this.NotifyCollectionChangedMonitorForSecond.Remove += OnRemoveFromSecond;
            this.NotifyCollectionChangedMonitorForSecond.Reset += OnResetSecond;
            this.NotifyCollectionChangedMonitorForSecond.Replace += OnReplaceOnSecond;
        }

        public List<TSource> Output { get; set; }

        private ReferenceCountTracker<TSource> FirstItemLookup { get; set; }
        private ReferenceCountTracker<TSource> SecondItemLookup { get; set; }

        public override TSource this[int index]
        {
            get { return this.Output[index]; }
            set { throw new AccessViolationException(); }
        }

        public override int Count
        {
            get { return this.Output.Count; }
        }

        private void AddItemsToFirstAndFireNotifyCollectionChanged(IEnumerable<TSource> newItems)
        {
            int indexOfAdd = this.Output.Count;
            List<TSource> itemsAdded = AddItemsToFirst(newItems);

            if (itemsAdded != null)
                FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsAdded, indexOfAdd));
        }

        private List<TSource> AddItemsToFirst(IEnumerable<TSource> items)
        {
            List<TSource> itemsAdded = null;

            foreach (TSource item in items)
            {
                if (this.FirstItemLookup.Add(item) && !this.SecondItemLookup.Contains(item))
                {
                    if (itemsAdded == null)
                    {
                        itemsAdded = new List<TSource>();
                    }

                    this.Output.Add(item);
                    itemsAdded.Add(item);
                }
            }
            
            return itemsAdded;
        }

        private void RemoveItemsFromSecond(IEnumerable<TSource> items)
        {
            List<TSource> itemsAdded = null;
            int indexOfAdd = this.Output.Count;

            foreach (TSource item in items)
            {
                if (this.SecondItemLookup.Remove(item) && this.FirstItemLookup.Contains(item))
                {
                    if (itemsAdded == null)
                    {
                        itemsAdded = new List<TSource>();
                    }

                    this.Output.Add(item);
                    itemsAdded.Add(item);
                }
            }

            if (itemsAdded != null)
                FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsAdded, indexOfAdd));
        }

        private void RemoveItemsFromFirst(IEnumerable<TSource> items)
        {
            foreach (TSource item in items)
            {
                if (this.FirstItemLookup.Remove(item) && !this.SecondItemLookup.Contains(item))
                {
                    RemoveFromOutputAndFireNotifyCollectionChanged(item);
                }
            }
        }

        private void AddItemsToSecond(IEnumerable<TSource> items)
        {
            foreach (TSource item in items)
            {
                if (this.SecondItemLookup.Add(item) && this.FirstItemLookup.Contains(item))
                {
                    RemoveFromOutputAndFireNotifyCollectionChanged(item);
                }
            }
        }

        private void RemoveFromOutputAndFireNotifyCollectionChanged(TSource item)
        {
            int indexOfRemoval = this.Output.IndexOf(item);
            this.Output.Remove(item);

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, indexOfRemoval));
        }

        #region First Event Handlers

        void OnAddToFirst(object sender, int index, IEnumerable<TSource> newItems)
        {
            AddItemsToFirstAndFireNotifyCollectionChanged(newItems);
        }

        void OnRemoveFromFirst(object sender, int index, IEnumerable<TSource> oldItems)
        {
            RemoveItemsFromFirst(oldItems);
        }

        void OnResetFirst(object sender)
        {
            this.Output.Clear();
            this.FirstItemLookup.Clear();

            AddItemsToFirst(this.First);

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnReplaceOnFirst(object sender, int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            RemoveItemsFromFirst(oldItems);
            AddItemsToFirstAndFireNotifyCollectionChanged(newItems);
        }

        #endregion

        #region Second Event Handlers

        void OnAddToSecond(object sender, int index, IEnumerable<TSource> newItems)
        {
            AddItemsToSecond(newItems);
        }

        void OnRemoveFromSecond(object sender, int index, IEnumerable<TSource> oldItems)
        {
            RemoveItemsFromSecond(oldItems);
        }

        void OnResetSecond(object sender)
        {
            this.Output.Clear();
            this.FirstItemLookup.Clear();
            this.SecondItemLookup.Clear();

            foreach (TSource item in this.Second)
            {
                this.SecondItemLookup.Add(item);
            }

            AddItemsToFirst(this.First);

            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void OnReplaceOnSecond(object sender, int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            RemoveItemsFromSecond(oldItems);
            AddItemsToSecond(newItems);
        }

        #endregion
    }
}