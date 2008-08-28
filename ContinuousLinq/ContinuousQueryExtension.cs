using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq
{
    public static class ContinuousQueryExtension
    {
        #region Select

        public static ReadOnlyContinuousCollection<TResult> Select<TSource, TResult>(
            this ObservableCollection<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return new SelectReadOnlyContinuousCollection<TSource, TResult>(source, selector);
        }

        public static ReadOnlyContinuousCollection<TResult> Select<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return new SelectReadOnlyContinuousCollection<TSource, TResult>(source, selector);
        }

        public static ReadOnlyContinuousCollection<TResult> Select<TSource, TResult>(
            this ReadOnlyContinuousCollection<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return new SelectReadOnlyContinuousCollection<TSource, TResult>(source, selector);
        }
        
        #endregion

        #region Where

        public static ReadOnlyContinuousCollection<TSource> Where<TSource>(
            this ObservableCollection<TSource> source, Expression<Func<TSource, bool>> filter)
            where TSource : INotifyPropertyChanged
        {
            return new FilteringReadOnlyContinuousCollection<TSource>(source, filter);
        }

        public static ReadOnlyContinuousCollection<TSource> Where<TSource>(
            this ReadOnlyObservableCollection<TSource> source, Expression<Func<TSource, bool>> filter)
            where TSource : INotifyPropertyChanged
        {
            return new FilteringReadOnlyContinuousCollection<TSource>(source, filter);
        }

        public static ReadOnlyContinuousCollection<TSource> Where<TSource>(
            this ReadOnlyContinuousCollection<TSource> source, Expression<Func<TSource, bool>> filter)
            where TSource : INotifyPropertyChanged
        {
            return new FilteringReadOnlyContinuousCollection<TSource>(source, filter);
        }
        
        #endregion

        #region AsReadOnly

        public static ReadOnlyContinuousCollection<TSource> AsReadOnly<TSource>(
            this ObservableCollection<TSource> source)
        {
            return new PassThroughReadOnlyContinuousCollection<TSource>(source); 
        }

        public static ReadOnlyContinuousCollection<TSource> AsReadOnly<TSource>(
            this ReadOnlyObservableCollection<TSource> source)
        {
            return new PassThroughReadOnlyContinuousCollection<TSource>(source);
        }

        #endregion

    }
}
