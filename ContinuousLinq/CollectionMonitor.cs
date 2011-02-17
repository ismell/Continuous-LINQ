using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Linq.Expressions;

using ContinuousLinq.WeakEvents;

namespace ContinuousLinq {
    public class CollectionMonitor<T> : INotifyCollectionChanged where T : INotifyPropertyChanged {
        
        #region Data Members

        protected readonly IList<T> _input;

        public ReadOnlyCollection<PropertyNotifier<T>> Properties { get; private set; }
        public List<PropertyNotifier<T>> _Properties;

        public ReferenceCountTracker<T> ReferenceCountTracker { get; private set; }

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event Action<object, int, IEnumerable<T>> Add;
        public event Action<object, int, IEnumerable<T>> Remove;
        public event Action<object, IEnumerable<T>, int, IEnumerable<T>> Replace;
#if !SILVERLIGHT
        public event Action<object, int, IEnumerable<T>, int, IEnumerable<T>> Move;
#endif
        public event Action<object> Reset;

        #endregion

        #region Properties

        public bool IsMonitoringChildProperties {
            get {
                if (_Properties.Count == 0)
                    return false;

                return _Properties.Any(t => t.PropertyAccessTree.IsMonitoringChildProperties);
            }
        }

        #endregion

        #region Constructors

        public CollectionMonitor(IList<T> input) {
            if (input == null)
                throw new ArgumentNullException("input");

            INotifyCollectionChanged inputAsINotifyCollectionChanged = input as INotifyCollectionChanged;

            if (inputAsINotifyCollectionChanged == null)
                throw new ArgumentException("Collections must implement INotifyCollectionChanged.");

            _input = input;

            _Properties = new List<PropertyNotifier<T>>();
            Properties = new ReadOnlyCollection<PropertyNotifier<T>>(_Properties);

            ReferenceCountTracker = new ReferenceCountTracker<T>();

            WeakNotifyCollectionChangedEventHandler.Register(
                inputAsINotifyCollectionChanged,
                this,
                (me, sender, args) => me.OnCollectionChanged(args));

            // Add Items to Reference Counter
            foreach (var item in input)
                ReferenceCountTracker.Add(item);
        }

        #endregion

        #region Public Methods

        #region Add Property

        /// <summary>
        /// Adds a property to monitor for change events.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">The expression that points to the property</param>
        /// <returns>Property Notifier used to register change events on</returns>
        public PropertyNotifier<T> AddProperty<TResult>(Expression<Func<T, TResult>> expression) {
            var tree = PropertyNotifier<T>.Create(expression);

            _Properties.Add(tree);

            SetupTreeSubscription(tree);

            return tree;
        }

        /// <summary>
        /// Adds a property to monitor for changed events
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">The expression that points to the property to monitor</param>
        /// <param name="itemChanging">Callback for Item Changing</param>
        /// <param name="itemChanged">Callback for Item Changed</param>
        /// <returns>Property Notifier for the specific property</returns>
        public PropertyNotifier<T> AddProperty<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<T, PropertyChangingEventArgs> itemChanging,
            Action<T, PropertyChangedEventArgs> itemChanged) {

            var tree = AddProperty(expression);

            if (itemChanging != null)
                tree.ItemChanging += (s, o, e) => itemChanging(o, e);

            if (itemChanged != null)
                tree.ItemChanged  += (s, o, e) => itemChanged(o, e);

            return tree;
        }

        #endregion

        #region Remove Property

        /// <summary>
        /// Stops listening for change events on the specified property
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public bool RemoveProperty(PropertyNotifier<T> tree) {
            var removed = _Properties.Remove(tree);
            if (removed)
                RemoveTreeSubscription(tree);

            return removed;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Subscription Methods

        private void SetupTreeSubscription(PropertyNotifier<T> tree) {
            foreach (var item in ReferenceCountTracker.Items) {
                tree.SubscribeToChanges(item);
            }
        }

        private void SubscribeItems(IEnumerable<T> items) {
            foreach (var item in items)
                SubscribeItem(item);
        }

        private void SubscribeItem(T item) {
            bool wasFirstTimeAddedToCollection = this.ReferenceCountTracker.Add(item);
            if (!wasFirstTimeAddedToCollection)
                return;

            foreach (var tree in _Properties)
                tree.SubscribeToChanges(item);
        }

        #endregion

        #region Unsubscribe Methods

        private void RemoveTreeSubscription(PropertyNotifier<T> tree) {
            foreach (var item in ReferenceCountTracker.Items) {
                tree.UnsubscribeFromChanges(item);
            }
        }

        private void UnsubscribeItems(IEnumerable<T> items) {
            foreach (var item in items)
                UnsubscribeItem(item);
        }

        private void UnsubscribeItem(T item) {
            bool wasLastInstanceOfItemRemovedFromCollection = this.ReferenceCountTracker.Remove(item);
            if (!wasLastInstanceOfItemRemovedFromCollection)
                return;

            foreach (var tree in _Properties)
                tree.UnsubscribeFromChanges(item);
        }


        #endregion

        #region Clear Subscription

        private void ClearSubscriptions() {
            foreach (var item in ReferenceCountTracker.Items) {

                foreach (var tree in _Properties) {
                    tree.UnsubscribeFromChanges(item);
                }
            }
            this.ReferenceCountTracker.Clear();
        }

        #endregion

        #endregion

        #region Event Handlers

        private void OnCollectionChanged(EventArgs e) {
            NotifyCollectionChangedEventArgs collectionArgs = (NotifyCollectionChangedEventArgs)e;
            switch (collectionArgs.Action) {
                case NotifyCollectionChangedAction.Remove:
                    try {
                        Remove.Raise(this, collectionArgs.OldStartingIndex, collectionArgs.OldItems.Cast<T>());
                        CollectionChanged.Raise(this, collectionArgs);
                    } finally {
                        // We unsubscribe the items after we raise the events
                        // so any processing that needs to happen on the items happens
                        UnsubscribeItems(collectionArgs.OldItems.Cast<T>());
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    SubscribeItems(collectionArgs.NewItems.Cast<T>());

                    Add.Raise(this, collectionArgs.NewStartingIndex, collectionArgs.NewItems.Cast<T>());
                    CollectionChanged.Raise(this, collectionArgs);

                    break;
                case NotifyCollectionChangedAction.Replace:
                    SubscribeItems(collectionArgs.NewItems.Cast<T>());
                    try {
                        Replace.Raise(
                            this,
                            collectionArgs.OldItems.Cast<T>(),
                            collectionArgs.NewStartingIndex,
                            collectionArgs.NewItems.Cast<T>());
                        CollectionChanged.Raise(this, collectionArgs);
                    } finally {
                        UnsubscribeItems(collectionArgs.OldItems.Cast<T>());
                    }
                    break;
#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    Move.Raise(
                        this,
                        collectionArgs.OldStartingIndex,
                        collectionArgs.OldItems.Cast<T>(),
                        collectionArgs.NewStartingIndex,
                        collectionArgs.NewItems.Cast<T>());
                    CollectionChanged.Raise(this, collectionArgs);
                    break;
#endif
                case NotifyCollectionChangedAction.Reset:
                    try {
                        Reset.Raise(this);
                        CollectionChanged.Raise(this, collectionArgs);
                    } finally {
                        ClearSubscriptions();
                    }

                    break;
            }
        }

        #endregion
    }
}
