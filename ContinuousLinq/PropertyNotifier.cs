using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ContinuousLinq {
    public delegate void PropertyChangedEventHandler<T>(T sender, PropertyChangedEventArgs e);

    public class PropertyNotifier<T> where T : INotifyPropertyChanged {
        #region Data Members

        private IPropertyAccessTreeSubscriber<PropertyNotifier<T>> _PropertyAccessTree;
        private Dictionary<T, SubscriptionTree> _Subscriptions;
        public PropertyAccessTree PropertyAccessTree { get; private set; }

        private bool EntireTreeSupportINotifyPropertyChanging { get; set; }

        #endregion

        public event Action<PropertyNotifier<T>, T, PropertyChangedEventArgs> ItemChanged;
        public event Action<PropertyNotifier<T>, T, PropertyChangingEventArgs> ItemChanging;

        #region Constructors

        public static PropertyNotifier<T> Create<TResult>(Expression<Func<T, TResult>> expression) {
            return new PropertyNotifier<T>(ExpressionPropertyAnalyzer.Analyze(expression));
        }

        public PropertyNotifier(PropertyAccessTree tree)
            : base() {
            if (tree == null)
                throw new ArgumentNullException("tree");

            PropertyAccessTree = tree;
            EntireTreeSupportINotifyPropertyChanging = PropertyAccessTree.DoesEntireTreeSupportINotifyPropertyChanging;
        }
        #endregion

        #region Methods

        internal void SubscribeToChanges(T subject) {
            if (EntireTreeSupportINotifyPropertyChanging) {
                if (_PropertyAccessTree == null) {
                    _PropertyAccessTree = PropertyAccessTree.CreateCallbackSubscription<PropertyNotifier<T>>(OnItemChanging, OnItemChanged);
                }
                _PropertyAccessTree.SubscribeToChanges(subject, this);
            } else {
                if (_Subscriptions == null) {
                    _Subscriptions = new Dictionary<T, SubscriptionTree>();
                }

                SubscriptionTree tree = PropertyAccessTree.CreateSubscriptionTree(subject);
                tree.PropertyChanged += OnAnyPropertyChangeInSubscriptionTree;
                _Subscriptions.Add(subject, tree);
            }
        }

        internal void UnsubscribeFromChanges(T subject) {
            if (EntireTreeSupportINotifyPropertyChanging) {
                if (_PropertyAccessTree == null)
                    return;
                _PropertyAccessTree.UnsubscribeFromChanges(subject, this);
            } else {
                if (_Subscriptions == null)
                    return;

                _Subscriptions[subject].PropertyChanged -= OnAnyPropertyChangeInSubscriptionTree;
                _Subscriptions.Remove(subject);
            }
        }
        
        #endregion

        #region Event Handlers

        private void OnAnyPropertyChangeInSubscriptionTree(SubscriptionTree sender) {
            OnItemChanged(this, sender.Parameter, null);
        }

        private static void OnItemChanging(PropertyNotifier<T> tree, object itemThatIsChanging, PropertyChangingEventArgs args) {
            var itemChanging = tree.ItemChanging;

            if (itemChanging != null)
                itemChanging(tree, (T)itemThatIsChanging, args);
        }

        private static void OnItemChanged(PropertyNotifier<T> tree, object itemThatChanged, PropertyChangedEventArgs args) {
            var itemChanged = tree.ItemChanged;
            
            if (itemChanged != null)
                itemChanged(tree, (T)itemThatChanged, args);
        }

        #endregion
    }
}
