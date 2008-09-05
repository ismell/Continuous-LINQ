using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Specialized;
using System.Windows;

namespace ContinuousLinq.Aggregates
{
    public abstract class ContinuousValue
    {
        protected bool RequiresRefresh { get; set; }

        protected static List<ContinuousValue> _dirtiedSincePause = new List<ContinuousValue>();

        private static int _globalPauseCount = 0;
        protected static bool IsGloballyPaused
        {
            get { return _globalPauseCount > 0 ; }
        }
       
        internal static void GlobalPause()
        {
            _globalPauseCount++;
        }

        internal static void GlobalResume()
        {
            _globalPauseCount--;
            if (_globalPauseCount == 0)
            {
                ResumeDirtyValues(); 
            }
        }

        private static void ResumeDirtyValues()
        {
            foreach (ContinuousValue continuousValue in _dirtiedSincePause)
            {
                continuousValue.Refresh();
                continuousValue.RequiresRefresh = false;
            }
            _dirtiedSincePause.Clear();
        }

        protected void MarkForResume()
        {
            if (!this.RequiresRefresh)
            {
                _dirtiedSincePause.Add(this);
                this.RequiresRefresh = true;
            }
        }

        internal abstract void Refresh();
    }

    public abstract class ContinuousValue<T> : ContinuousValue, INotifyPropertyChanged
    {
        private T _currentValue;
        public T CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, _currentValue))
                    return;

                _currentValue = value;

                OnPropertyChanged("CurrentValue");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    internal class ContinuousValue<TSource, TResult> : ContinuousValue<TResult> where TSource : INotifyPropertyChanged
    {
        internal IList<TSource> Source { get; set; }

        internal Func<TSource, TResult> Selector { get; set; }

        internal Func<IList<TSource>, Func<TSource, TResult>, TResult> AggregationOperation { get; set; }

        internal NotifyCollectionChangedMonitor<TSource> NotifyCollectionChangedMonitor { get; set; }

        internal ContinuousValue(
            IList<TSource> input, 
            Expression<Func<TSource, TResult>> selectorExpression,
            Func<IList<TSource>, Func<TSource,TResult>, TResult> aggregateOperation)
        {
            this.Source = input;

            this.AggregationOperation = aggregateOperation;

            PropertyAccessTree propertyAccessTree = null;
            
            if (selectorExpression != null)
            {
                this.Selector = selectorExpression.Compile();
                propertyAccessTree = ExpressionPropertyAnalyzer.Analyze(selectorExpression);
            }

            this.NotifyCollectionChangedMonitor = new NotifyCollectionChangedMonitor<TSource>(propertyAccessTree, input);
            
            this.NotifyCollectionChangedMonitor.CollectionChanged += OnCollectionChanged;
            this.NotifyCollectionChangedMonitor.ItemChanged += OnItemChanged;
            
            Refresh();
        }

        void OnItemChanged(INotifyPropertyChanged obj)
        {
            Refresh();
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;

            Refresh();
        }

        internal override void Refresh()
        {
            if (IsGloballyPaused)
            {
                MarkForResume();
                return;
            }

            this.CurrentValue = this.AggregationOperation(this.Source, this.Selector);
        }
    }
}
