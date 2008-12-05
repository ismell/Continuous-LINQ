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
    public class FilteringReadOnlyContinuousCollection<TSource> : ReadOnlyAdapterContinuousCollection<TSource, TSource>
    {
        internal ContinuousCollection<TSource> Output { get; set; }

        internal ReferenceCountTracker<TSource> SourceCountTracker { get; set; }

        internal HashSet<TSource> ItemLookup { get; set; }

        internal Func<TSource, bool> FilterFunction { get; set; }

        public FilteringReadOnlyContinuousCollection(IList<TSource> list, Expression<Func<TSource, bool>> expression)
            : base(list, ExpressionPropertyAnalyzer.Analyze(expression))
        {
            this.FilterFunction = expression.Compile();

            this.Output = new ContinuousCollection<TSource>();
            this.ItemLookup = new HashSet<TSource>();
            this.SourceCountTracker = new ReferenceCountTracker<TSource>();
            
            this.Output.CollectionChanged += RefireCollectionChangedFromOutput;

            AddNewItems(this.Source);

            this.NotifyCollectionChangedMonitor.Add += OnAdd;
            this.NotifyCollectionChangedMonitor.Remove += OnRemove;
            this.NotifyCollectionChangedMonitor.Reset += OnReset;
            this.NotifyCollectionChangedMonitor.Replace += OnReplace;
            this.NotifyCollectionChangedMonitor.ItemChanged += OnItemChanged;
        }

        void RefireCollectionChangedFromOutput(object sender, NotifyCollectionChangedEventArgs args)
        {
            FireCollectionChanged(args);
        }

        void OnItemChanged(INotifyPropertyChanged sender)
        {
            TSource senderAsTSource = (TSource)sender;

            Filter(senderAsTSource);
        }

        private void Filter(TSource item)
        {
            if (this.FilterFunction(item))
            {
                if (!this.ItemLookup.Contains(item))
                {
                    int numberOfInstancesInSource = this.SourceCountTracker[item];
                    for (int i = 0; i < numberOfInstancesInSource; i++)
                    {
                        this.Output.Add(item);
                    }
                    this.ItemLookup.Add(item);
                }
            }
            else
            {
                int numberOfInstancesInSource = this.SourceCountTracker[item];
                for (int i = 0; i < numberOfInstancesInSource; i++)
                {
                    this.Output.Remove(item);
                }
                this.ItemLookup.Remove(item);
            }
        }

        public override int Count
        {
            get { return this.Output.Count; }
        }

        public override TSource this[int index]
        {
            get { return this.Output[index]; }
            set { throw new AccessViolationException(); }
        }

        void OnAdd(int index, IEnumerable<TSource> newItems)
        {
            AddNewItems(newItems);
        }

        private void AddNewItems(IEnumerable<TSource> newItems)
        {
            foreach (TSource item in newItems)
            {
                this.SourceCountTracker.Add(item);
                if (this.FilterFunction(item))
                {
                    this.Output.Add(item);
                    this.ItemLookup.Add(item);
                }
            }
        }

        private void RemoveOldItems(IEnumerable<TSource> oldItems)
        {
            foreach (TSource item in oldItems)
            {
                if (this.SourceCountTracker.Remove(item))
                {
                    this.ItemLookup.Remove(item);
                }
                this.Output.Remove(item);
            }
        }

        void OnRemove(int index, IEnumerable<TSource> oldItems)
        {
            RemoveOldItems(oldItems);
        }

        void OnReset()
        {
            this.Output.Clear();
            this.ItemLookup.Clear();
            this.SourceCountTracker.Clear();
        }

        void OnReplace(int oldStartingIndex, IEnumerable<TSource> oldItems, int newStartingIndex, IEnumerable<TSource> newItems)
        {
            RemoveOldItems(oldItems);
            AddNewItems(newItems);
        }
    }
}
