using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq.Reactive {

    public abstract class ReactiveMethod<T> where T : class, INotifyPropertyChanged
    {
        [Flags]
        protected enum FireOn {
            None,
            PropertyChanged,
            PropertyChanging
        }

        private readonly Action<T> _changingCallback;
        private readonly Action<T> _changedCallback;
        private List<PropertyAccessTree> _accessTreesWithoutNotifyPropertyChanging;
        private List<IPropertyAccessTreeSubscriber<ReactiveMethod<T>>> _propertyChangeSubscribers;

        #region Constructor

        internal ReactiveMethod(Action<T> changingCallback, Action<T> changedCallback) {
            _changingCallback = changingCallback;
            _changedCallback = changedCallback;
        }

        #endregion

        #region Methods

        protected void Register<TResult>(Expression<Func<T, TResult>> propertyAccessor, FireOn fireOn)
        {
            PropertyAccessTree propertyAccessTree = ExpressionPropertyAnalyzer.Analyze(propertyAccessor);
            if (propertyAccessTree.DoesEntireTreeSupportINotifyPropertyChanging)
            {
                if (_propertyChangeSubscribers == null)
                {
                    _propertyChangeSubscribers = new List<IPropertyAccessTreeSubscriber<ReactiveMethod<T>>>();
                }

                Action<ReactiveMethod<T>, object, PropertyChangingEventArgs> onPropertyChanging = OnAnyPropertyInSubscriptionChanging;
                Action<ReactiveMethod<T>, object, PropertyChangedEventArgs> onPropertyChanged = OnAnyPropertyInSubscriptionChanged;

                if ((fireOn & FireOn.PropertyChanging) != FireOn.PropertyChanging) {
                    onPropertyChanging = null;
                }

                if ((fireOn & FireOn.PropertyChanged) != FireOn.PropertyChanged) {
                    onPropertyChanged = null;
                }

                var subscriber = propertyAccessTree.CreateCallbackSubscription<ReactiveMethod<T>>(
                    onPropertyChanging, onPropertyChanged
                );

                _propertyChangeSubscribers.Add(subscriber);
            } 
            else if ((fireOn & FireOn.PropertyChanged) == FireOn.PropertyChanged)
            {
                if (_accessTreesWithoutNotifyPropertyChanging == null)
                {
                	_accessTreesWithoutNotifyPropertyChanging = new List<PropertyAccessTree>();
                }
                _accessTreesWithoutNotifyPropertyChanging.Add(propertyAccessTree);
            }
        }

        private static void OnAnyPropertyInSubscriptionChanging(ReactiveMethod<T> me, object objectThatChanged, PropertyChangingEventArgs args) {

            if (me._changingCallback != null) {
                me._changingCallback((T)objectThatChanged);
            }
        }

        private static void OnAnyPropertyInSubscriptionChanged(ReactiveMethod<T> me, object objectThatChanged, PropertyChangedEventArgs args) {
            if (me._changedCallback != null) {
                me._changedCallback((T)objectThatChanged);
            }
        }

        internal void CreateSubscriptions(INotifyPropertyChanged subject, ref List<SubscriptionTree> listToAppendTo)
        {
            if (_propertyChangeSubscribers != null)
            {
                for (int i = 0; i < _propertyChangeSubscribers.Count; i++)
                {
                    var propertyChangeSubscriber = _propertyChangeSubscribers[i];
                    propertyChangeSubscriber.SubscribeToChanges(subject, this);
                }
            }

            if(_accessTreesWithoutNotifyPropertyChanging != null)
            {
                if (listToAppendTo == null)
                {
                    listToAppendTo = new List<SubscriptionTree>();
                }

                // Don't make a closure on "this"
                var callback = _changedCallback;

                for (int i = 0; i < _accessTreesWithoutNotifyPropertyChanging.Count; i++)
                {
                    var accessTree = _accessTreesWithoutNotifyPropertyChanging[i];

                    var subscriptionTree = accessTree.CreateSubscriptionTree(subject);

                    listToAppendTo.Add(subscriptionTree);

                    subscriptionTree.PropertyChanged += obj => callback((T)obj.Parameter);
                }
            }
        }

        #endregion
    }
}
