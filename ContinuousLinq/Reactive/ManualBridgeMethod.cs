using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq.Reactive {

    public interface IPropertyBridge<T, TProperty> {
        void Subscribe(T listener, TProperty subject);
        void Unubscribe(T listener, TProperty subject);
    }

    public class ManualBridgeMethod<T> where T: ReactiveObject {

        private Action<T, object, PropertyChangedEventArgs> _ChangedCallback;
        private Action<T, object, PropertyChangingEventArgs> _ChangingCallback;

        public static ManualBridgeMethod<T> Create<TResult>(Expression<Func<T, TResult>> what) {
            var name = ExpressionPropertyAnalyzer.ExtractPropertyName(what);

            var item = new ManualBridgeMethod<T>();

            item._ChangedCallback = (me, sender, args) => me.OnPropertyChanged(name);
            item._ChangingCallback = (me, sender, args) => me.OnPropertyChanging(name);

            return item;
        }

        public IPropertyBridge<T, TProperty> With<TProperty>(Expression<Func<TProperty, object>> what) where TProperty : class, INotifyPropertyChanged, INotifyPropertyChanging {
            return new PropertyBridgeWithMethod<T, TProperty>(_ChangedCallback, _ChangingCallback, what);
        }


        private class PropertyBridgeWithMethod<K, TProperty> : IPropertyBridge<K, TProperty> where TProperty : class, INotifyPropertyChanged, INotifyPropertyChanging {
            private readonly IPropertyAccessTreeSubscriber<K> _PropertyChangeSubscriber;
            private readonly PropertyAccessTree _PropertyAccessTree;

            public PropertyBridgeWithMethod(Action<K, object, PropertyChangedEventArgs> changedCallback, Action<K, object, PropertyChangingEventArgs> changingCallback, Expression<Func<TProperty, object>> what) {
                _PropertyAccessTree = ExpressionPropertyAnalyzer.Analyze(what);

                if (_PropertyAccessTree.DoesEntireTreeSupportINotifyPropertyChanging) {
                    _PropertyChangeSubscriber = _PropertyAccessTree.CreateCallbackSubscription<K>(changingCallback, changedCallback);
                } else {
                    throw new Exception("The entire tree must support INotifyPropertyChanging");
                }
            }

            public void Subscribe(K listener, TProperty subject) {
                _PropertyChangeSubscriber.SubscribeToChanges(subject, listener);
            }

            public void Unubscribe(K listener, TProperty subject) {
                _PropertyChangeSubscriber.UnsubscribeFromChanges(subject, listener);
            }
        }
    }
}
