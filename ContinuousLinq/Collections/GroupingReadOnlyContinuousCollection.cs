using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ContinuousLinq.Collections
{
    public class GroupingReadOnlyContinuousCollection<TKey,TSource> : ReadOnlyAdapterContinuousCollection<TSource, GroupedContinuousCollection<TKey, TSource>> where TKey : IEquatable<TKey>
    {

        internal ContinuousCollection<GroupedContinuousCollection<TKey, TSource>> Output { get; set; }
        internal Func<TSource, TKey> KeySelector { get; set; }

        public GroupingReadOnlyContinuousCollection(IList<TSource> list,
            Expression<Func<TSource, TKey>> keySelectorExpression) : base(list, ExpressionPropertyAnalyzer.Analyze(keySelectorExpression))
        {
            this.KeySelector = keySelectorExpression.Compile();
            this.Output = new ContinuousCollection<GroupedContinuousCollection<TKey, TSource>>();

            AddNewItems(this.Source);

            this.NotifyCollectionChangedMonitor.Add += OnAdd;
            this.NotifyCollectionChangedMonitor.Remove += OnRemove;
            this.NotifyCollectionChangedMonitor.Reset += OnReset;
            this.NotifyCollectionChangedMonitor.Replace += OnReplace;
            this.NotifyCollectionChangedMonitor.ItemChanged += OnItemChanged;

            this.Output.CollectionChanged += OnOutputCollectionChanged;
           
        }

        private void OnOutputCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            FireCollectionChanged(args);
        }

        private GroupedContinuousCollection<TKey, TSource> GetCollectionForKey(TKey key)
        {
            GroupedContinuousCollection<TKey, TSource> col =
                this.Output.FirstOrDefault(item => item.Key.Equals(key));
            if (col == null)
            {
                col = new GroupedContinuousCollection<TKey, TSource>(key);
                this.Output.Add(col);
            }
            return col;
        }

        private void AddNewItems(IEnumerable<TSource> source)
        {
            foreach (TSource item in source)
            {
                GroupedContinuousCollection<TKey, TSource> col = GetCollectionForKey(this.KeySelector(item));
                col.Add(item);
            }            
        }

        private void RemoveOldItems(IEnumerable<TSource> oldItems)
        {
            foreach (TSource item in oldItems)
            {
                TKey key = this.KeySelector(item);
                GroupedContinuousCollection<TKey, TSource> col = GetCollectionForKey(key);
                if (col.Contains(item))
                    col.Remove(item);
                if (col.Count == 0)
                    this.Output.Remove(col);
            }
        }

        private void Regroup(TSource modifiedItem)
        {
            TKey key = this.KeySelector(modifiedItem);
            GroupedContinuousCollection<TKey, TSource> targetCol =
                this.Output.FirstOrDefault(col => col.Key.Equals(key));
            GroupedContinuousCollection<TKey, TSource> currentCol =
                this.Output.FirstOrDefault(col => col.Contains(modifiedItem));

            if (targetCol == null)
            {
                currentCol.Remove(modifiedItem);
                GroupedContinuousCollection<TKey, TSource> newCol = new GroupedContinuousCollection<TKey, TSource>(key);
                newCol.Add(modifiedItem);
                this.Output.Add(newCol);
            }
            else if (!targetCol.Contains(modifiedItem))
            {
                currentCol.Remove(modifiedItem);
                targetCol.Add(modifiedItem);
            }
            // ELSE - item belongs in its current collection - do nothing.
        }

        #region Source Changed Event Handlers

        void OnAdd(int index, IEnumerable<TSource> newItems)
        {
            AddNewItems(newItems);
        }

        void OnItemChanged(INotifyPropertyChanged sender)
        {
            TSource senderAsSource = (TSource)sender;
            Regroup(senderAsSource);
        }

        void OnRemove(int index, IEnumerable<TSource> oldItems)
        {
            RemoveOldItems(oldItems);
        }

        void OnReset()
        {
            this.Output.Clear();            
        }

        void OnReplace(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            RemoveOldItems(oldItems);
            AddNewItems(newItems);
        }

        #endregion

        #region Overrides
        public override int Count
        {
            get { return Output.Count; }
        }

        public override GroupedContinuousCollection<TKey, TSource> this[int index]
        {
            get
            {
                return this.Output[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        

        public IEnumerable<TKey> Keys
        {
            get
            {
                var q = from GroupedContinuousCollection<TKey, TSource> col in this.Output
                        select col.Key;
                foreach (TKey key in q)
                {
                    yield return key;
                }
            }
        }
    }
}
