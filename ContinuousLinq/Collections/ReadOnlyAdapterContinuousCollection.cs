using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ContinuousLinq
{
    public abstract class ReadOnlyAdapterContinuousCollection<TSource, TResult> : ReadOnlyContinuousCollection<TResult>
    {
        protected IList<TSource> Source { get; set; }

        internal NotifyCollectionChangedMonitor<TSource> NotifyCollectionChangedMonitor { get; set; }

        internal ReadOnlyAdapterContinuousCollection(IList<TSource> list, PropertyAccessTree propertyAccessTree)
        {
            INotifyCollectionChanged listAsINotifyCollectionChanged = list as INotifyCollectionChanged;
            this.Source = list;
            this.NotifyCollectionChangedMonitor = new NotifyCollectionChangedMonitor<TSource>(propertyAccessTree, list);
        }

        internal ReadOnlyAdapterContinuousCollection(IList<TSource> list)
            :this(list, null)
        {
        }

    }
}
