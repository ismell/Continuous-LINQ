using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Collections.Specialized;
using ContinuousLinq.Expressions;
using ContinuousLinq;

namespace ContinuousLinq.Collections
{
    //public class GroupJoinReadOnlyContinuousCollection<TOuter, TInner, TKey, TResult> : ReadOnlyContinuousCollection<TResult>
    //{
    //    private IList<TInner> _inner;
    //    private IList<TOuter> _outer; 

    //    private Func<TInner, TKey> _innerKeySelector;
    //    private Func<TOuter, TKey> _outerKeySelector;
        
    //    private NotifyCollectionChangedMonitor<TOuter> _outerNotifyCollectionChangedMonitor;
    //    private NotifyCollectionChangedMonitor<TInner> _innerNotifyCollectionChangedMonitor;

    //    private Dictionary<TKey, TOuter> _outerToKeyLookup;

    //    private Dictionary<TOuter, ContinuousCollection<TInner>> _outputToMatchingInnerTable;
    //    private Dictionary<TOuter, ReadOnlyContinuousCollection<TInner>> _outputToMatchingInnerReadOnlyTable;

    //    private Func<TOuter, ReadOnlyContinuousCollection<TInner>, TResult> _resultSelector;

    //    private ContinuousCollection<TResult> _output;

    //    public override int Count
    //    {
    //        get { return _output.Count; }
    //    }

    //    public override TResult this[int index]
    //    {
    //        get
    //        {
    //            return _output[index];
    //        }
    //        set { throw new InvalidOperationException(); }
    //    }

    //    public GroupJoinReadOnlyContinuousCollection(
    //        IList<TOuter> outer,
    //        IList<TInner> inner,
    //        Expression<Func<TOuter, TKey>> outerKeySelector,
    //        Expression<Func<TInner, TKey>> innerKeySelector,
    //        Expression<Func<TOuter, ReadOnlyContinuousCollection<TInner>, TResult>> resultSelector)
    //    {
    //        _outer = outer;
    //        _inner = inner;

    //        _outerKeySelector = outerKeySelector.CachedCompile();
    //        _innerKeySelector = innerKeySelector.CachedCompile();
    //        _resultSelector = resultSelector.CachedCompile();

    //        RebuildAll();

    //        _outerNotifyCollectionChangedMonitor = new NotifyCollectionChangedMonitor<TOuter>(ExpressionPropertyAnalyzer.Analyze(outerKeySelector), outer);
    //        _outerNotifyCollectionChangedMonitor.Add += OnOuterAdd;
    //        _outerNotifyCollectionChangedMonitor.Remove += OnOuterRemove;
    //        _outerNotifyCollectionChangedMonitor.Reset += OnOuterReset;
    //        _outerNotifyCollectionChangedMonitor.Move += OnOuterMove;
    //        _outerNotifyCollectionChangedMonitor.Replace += OnOuterItemReplace;
    //        _outerNotifyCollectionChangedMonitor.ItemChanged += OnOuterItemChanged;

    //        _innerNotifyCollectionChangedMonitor = new NotifyCollectionChangedMonitor<TInner>(ExpressionPropertyAnalyzer.Analyze(innerKeySelector), inner);
    //        _innerNotifyCollectionChangedMonitor.Add += OnInnerAdd;
    //        _innerNotifyCollectionChangedMonitor.Remove += OnInnerRemove;
    //        _innerNotifyCollectionChangedMonitor.Reset += OnInnerReset;
    //        _innerNotifyCollectionChangedMonitor.Move += OnInnerMove;
    //        _innerNotifyCollectionChangedMonitor.Replace += OnInnerItemReplace;
    //        _innerNotifyCollectionChangedMonitor.ItemChanged += OnInnerItemChanged;
    //    }

    //    private void RebuildAll()
    //    {
    //        Dictionary<TOuter, ContinuousCollection<TInner>> outerToInnerAssociation = new Dictionary<TOuter,ContinuousCollection<TInner>>();
    //        Dictionary<TKey, HashSet<TOuter>> outerIndex = new Dictionary<TKey, HashSet<TOuter>>();

    //        foreach (var outerItem in _outer)
    //        {
    //            TKey outerKey = _outerKeySelector(outerItem);
    //            HashSet<TOuter> outersMatchingKey;
    //            if (!outerIndex.TryGetValue(outerKey, out outersMatchingKey))
    //            {
    //                outersMatchingKey = new HashSet<TOuter>();
    //                outerIndex.Add(outerKey, outersMatchingKey);
    //            }

    //            outersMatchingKey.Add(outerItem);

    //            outerToInnerAssociation[outerItem] = new ContinuousCollection<TInner>();
    //        }

    //        foreach (var innerItem in _inner)
    //        {
    //            TKey innerItemKey = _innerKeySelector(innerItem);

    //            HashSet<TOuter> outersMatchingKey;
    //            if (outerIndex.TryGetValue(innerItemKey, out outersMatchingKey))
    //            {
    //                foreach (var outerMatchingKey in outersMatchingKey)
    //                {
    //                    outerToInnerAssociation[outerMatchingKey].Add(innerItem);
    //                }
    //            }
    //        }
    //    }

    //    void OnOuterItemChanged(INotifyPropertyChanged sender)
    //    {
    //        TOuter senderAsTSource = (TOuter)sender;
    //    }

    //    void OnOuterAdd(int index, IEnumerable<TOuter> newItems)
    //    {
    //    }

    //    void OnOuterRemove(int index, IEnumerable<TOuter> oldItems)
    //    {
    //    }

    //    void OnOuterReset()
    //    {
    //    }

    //    void OnOuterMove(int oldStartingIndex, IEnumerable<TOuter> oldItems, int newStartingIndex, IEnumerable<TOuter> newItems)
    //    {
    //    }

    //    void OnOuterItemReplace(int oldStartingIndex, IEnumerable<TOuter> oldItems, int newStartingIndex, IEnumerable<TOuter> newItems)
    //    {
    //    }


    //    void OnInnerItemChanged(INotifyPropertyChanged sender)
    //    {
    //        TInner senderAsTSource = (TInner)sender;
    //    }

    //    void OnInnerAdd(int index, IEnumerable<TInner> newItems)
    //    {
    //    }

    //    void OnInnerRemove(int index, IEnumerable<TInner> oldItems)
    //    {
    //    }

    //    void OnInnerReset()
    //    {
    //    }

    //    void OnInnerMove(int oldStartingIndex, IEnumerable<TInner> oldItems, int newStartingIndex, IEnumerable<TInner> newItems)
    //    {
    //    }

    //    void OnInnerItemReplace(int oldStartingIndex, IEnumerable<TInner> oldItems, int newStartingIndex, IEnumerable<TInner> newItems)
    //    {
    //    }
    //}
}
