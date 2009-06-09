using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq.Reactive
{
    public class DependsOnMethod<T> where T : class, INotifyPropertyChanged
    {
        #region Constructor

        internal DependsOnMethod(Action<T> callback)
        {
            this.AccessTrees = new List<PropertyAccessTree>();

            this.Callback = callback;
        }

        #endregion

        #region Properties

        private Action<T> Callback { get; set; }

        private List<PropertyAccessTree> AccessTrees { get; set; }

        #endregion

        #region Methods

        public DependsOnMethod<T> OnChanged<TResult>(Expression<Func<T, TResult>> propertyAccessor)
        {
            PropertyAccessTree propertyAccessTree = ExpressionPropertyAnalyzer.Analyze(propertyAccessor);
            this.AccessTrees.Add(propertyAccessTree);

            return this;
        }

        internal void CreateSubscriptions(INotifyPropertyChanged subject, List<SubscriptionTree> listToAppendTo)
        {
            foreach (var accessTree in this.AccessTrees)
            {
                var subscriptionTree = accessTree.CreateSubscriptionTree(subject);

                listToAppendTo.Add(subscriptionTree);

                subscriptionTree.PropertyChanged += obj => this.Callback((T)subject);
            }
        }

        #endregion
    }
}
