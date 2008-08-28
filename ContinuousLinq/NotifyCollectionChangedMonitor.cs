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
    internal class NotifyCollectionChangedMonitor<T> : IWeakEventListener, INotifyCollectionChanged //where T : INotifyPropertyChanged
    {
        protected readonly IList<T> _input;

        public PropertyAccessTree PropertyAccessTree { get; set; }

        public Dictionary<T, SubscriptionTree> Subscriptions { get; set; }

        public event Action<int, IEnumerable<T>> Add;
        public event Action<int, IEnumerable<T>> Remove;
        public event Action<int, IEnumerable<T>, int, IEnumerable<T>> Replace;
        public event Action<int, IEnumerable<T>, int, IEnumerable<T>> Move;
        public event Action Reset;

        public event Action<INotifyPropertyChanged> ItemChanged;

        #region IWeakEventListener Members

        public NotifyCollectionChangedMonitor(PropertyAccessTree propertyAccessTree, IList<T> input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            INotifyCollectionChanged inputAsINotifyCollectionChanged = input as INotifyCollectionChanged;

            if (inputAsINotifyCollectionChanged == null)
                throw new ArgumentException("Collections must implement INotifyCollectionChanged to work with CLINQ");

            _input = input;

            this.PropertyAccessTree = propertyAccessTree;
            this.Subscriptions = new Dictionary<T, SubscriptionTree>();
            
            SubscribeToItems(_input);

            CollectionChangedEventManager.AddListener(inputAsINotifyCollectionChanged, this);
        }

        private void SubscribeToItems(IEnumerable<T> items)
        {
            if (this.PropertyAccessTree == null)
                return; 
            
            foreach (T item in items)
            {
                SubscribeToItem(item);
            }
        }

        private void SubscribeToItem(T item)
        {
            SubscriptionTree subscriptionTree = this.PropertyAccessTree.CreateSubscriptionTree((INotifyPropertyChanged)item);
            subscriptionTree.PropertyChanged += OnAnyPropertyChangeInSubscriptionTree;
            this.Subscriptions.Add(item, subscriptionTree);
        }

        void OnAnyPropertyChangeInSubscriptionTree(SubscriptionTree sender)
        {
            if (this.ItemChanged == null)
                return;
            
            this.ItemChanged(sender.Parameter);
        }

        private void UnsubscribeFromItems(IEnumerable<T> items)
        {
            if (this.PropertyAccessTree == null)
                return;

            foreach (T item in items)
            {
                UnsubscribeFromItem(item);
            }
        }

        private void UnsubscribeFromItem(T item)
        {
            this.Subscriptions[item].PropertyChanged -= OnAnyPropertyChangeInSubscriptionTree;
            this.Subscriptions.Remove(item);
        }

        private void ClearSubscriptions()
        {
            if (this.PropertyAccessTree == null)
                return; 

            foreach (SubscriptionTree subscriptionTree in this.Subscriptions.Values)
            {
                subscriptionTree.PropertyChanged -= OnAnyPropertyChangeInSubscriptionTree;
            }

            this.Subscriptions.Clear();
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(CollectionChangedEventManager))
            {
                ReceiveCollectionChangedEvent(e);
            }

            return true;
        }

        private void SubscribeToNewItems(IList items)
        {
            if (this.PropertyAccessTree == null)
                return; 

            IEnumerable<T> newItems = items.Cast<T>();
            SubscribeToItems(newItems);
        }

        private void UnsubscribeFromOldItems(IList items)
        {
            if (this.PropertyAccessTree == null)
                return; 

            IEnumerable<T> oldItems = items.Cast<T>();
            UnsubscribeFromItems(oldItems);
        }

        private void ReceiveCollectionChangedEvent(EventArgs e)
        {
            NotifyCollectionChangedEventArgs collectionArgs = (NotifyCollectionChangedEventArgs)e;
            switch (collectionArgs.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    UnsubscribeFromOldItems(collectionArgs.OldItems);
                    if (Remove != null)
                    {
                        Remove(collectionArgs.OldStartingIndex, collectionArgs.OldItems.Cast<T>());
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    SubscribeToNewItems(collectionArgs.NewItems);
                    if (Add != null)
                    {
                        Add(collectionArgs.NewStartingIndex, collectionArgs.NewItems.Cast<T>());
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    UnsubscribeFromOldItems(collectionArgs.OldItems);
                    SubscribeToNewItems(collectionArgs.NewItems);
                    if (Replace != null)
                    {
                        Replace(collectionArgs.OldStartingIndex,
                            collectionArgs.OldItems.Cast<T>(),
                            collectionArgs.NewStartingIndex,
                            collectionArgs.NewItems.Cast<T>());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (Move != null)
                    {
                        Move(collectionArgs.OldStartingIndex,
                            collectionArgs.OldItems.Cast<T>(),
                            collectionArgs.NewStartingIndex,
                            collectionArgs.NewItems.Cast<T>());
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearSubscriptions();
                    SubscribeToItems(_input);
                    if (Reset != null)
                    {
                        Reset();
                    }
                    break;
            }
            if (CollectionChanged != null)
            {
                CollectionChanged(this, collectionArgs);
            }
        }
        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
