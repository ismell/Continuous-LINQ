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
            var name = ExtractPropertyName(propertyAccessor);

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

        private static string ExtractPropertyName<TResult>(Expression<Func<T, TResult>> propertyAccessor) {
            var lambda = propertyAccessor as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression) {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            } else {
                memberExpression = lambda.Body as MemberExpression;
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            var name = propertyInfo.Name;

            return name;
        }
    }
}
