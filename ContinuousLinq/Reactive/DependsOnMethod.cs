using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq.Reactive
{
    public class DependsOnMethod<T> : ReactiveMethod<T> where T : class, INotifyPropertyChanged
    {
        #region Constructor

        internal DependsOnMethod(Action<T> callback) : base(callback, callback)
        {
        }

        #endregion

        #region Methods

        public DependsOnMethod<T> OnChanged<TResult>(Expression<Func<T, TResult>> propertyAccessor)
        {
            Register(propertyAccessor, FireOn.PropertyChanged);
            return this;
        }

        public DependsOnMethod<T> OnChanging<TResult>(Expression<Func<T, TResult>> propertyAccessor) {
            Register(propertyAccessor, FireOn.PropertyChanging);
            return this;
        }

        #endregion
    }
}
