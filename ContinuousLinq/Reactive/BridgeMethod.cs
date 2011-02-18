using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace ContinuousLinq.Reactive
{
    public class BridgeMethod<T> : ReactiveMethod<T> where T : ReactiveObject {

        public static BridgeMethod<T> Create<TResult>(Expression<Func<T, TResult>> propertyAccessor) {
            var name = ExpressionPropertyAnalyzer.ExtractPropertyName(propertyAccessor);

            Action<T> changedCallback = me => me.OnPropertyChanged(name);
            Action<T> changingCallback = me => me.OnPropertyChanging(name);

            var method = new BridgeMethod<T>(changingCallback, changedCallback);

            return method;
        }

        private BridgeMethod(Action<T> onPropertyChanging, Action<T> onPropertyChanged)
            : base(onPropertyChanging, onPropertyChanged) {

            }

        public BridgeMethod<T> With<TResult>(Expression<Func<T, TResult>> propertyAccessor) {
            Register(propertyAccessor, FireOn.PropertyChanging | FireOn.PropertyChanged);
            return this;
        }
    }
}
