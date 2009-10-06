using System;
using System.Collections.Generic;

namespace ContinuousLinq
{
    ////public interface ISkipListNode
    ////{
    ////    ISkipListNode Above { get; set; }
    ////    ISkipListNode Next { get; set; }
    ////    ISkipListNode Previous { get; set; }
    ////}

    //public class SkipListNode<TKey, TValue>
    //{
    //    public TKey Key { get; set; }
    //    public TValue Value { get; set; }

    //    public SkipListNode<TKey, TValue> Above { get; set; }
    //    public SkipListNode<TKey, TValue> Next { get; set; }
    //    public SkipListNode<TKey, TValue> Previous { get; set; }
    //    public SkipListNode<TKey, TValue> Below { get; set; }

    //    public bool IsLeaf
    //    {
    //        get { return this.Below == null; }
    //    }

    //    public SkipListNode<TKey, TValue> BottomMost
    //    {
    //        get
    //        {
    //            SkipListNode<TKey, TValue> current = this;
    //            while (current.Below != null)
    //            {
    //                current = current.Below;
    //            }
    //            return current;
    //        }
    //    }

    //    public SkipListNode(TKey key, TValue value)
    //    {
    //        this.Value = value;
    //        this.Key = key;
    //    }
    //}

    ////public class MiddleSkipListNode : ISkipListNode
    ////{
    ////    public ISkipListNode Above { get; set; }
    ////    public ISkipListNode Below { get; set; }
    ////    public ISkipListNode Next { get; set; }
    ////    public ISkipListNode Previous { get; set; }

    ////    public MiddleSkipListNode()
    ////    {
    
    ////    }
    ////}

    //public class SkipList<TKey, TValue>
    //{
    //    private Random _rand;
    //    SkipListNode<TKey, TValue> _topLeft;

    //    Comparer<TKey> _comparer;

    //    public SkipList()
    //    {
    //        _rand = new Random();
    //        _comparer = Comparer<TKey>.Default;
    //    }

    //    private TValue GetValue(TKey key)
    //    {
    //        if (_topLeft == null)
    //        {
    //            throw new KeyNotFoundException();
    //        }
    //        SkipListNode<TKey, TValue> closestNode;
    //        if (!TryFindNodeForKey(key, _topLeft, out closestNode))
    //        {
    //            throw new KeyNotFoundException();
    //        }
            
    //        return closestNode.Value;
    //    }

    //    private bool TryFindNodeForKey(TKey key, SkipListNode<TKey, TValue> node, out SkipListNode<TKey, TValue> closestNode)
    //    {
    //        SkipListNode<TKey, TValue> currentNode = node;
    //        closestNode = null;
    //        while (true)
    //        {
    //            SkipListNode<TKey, TValue> closestNodeAtThisLevel;
    //            if (TrySearchForward(key, node, out closestNodeAtThisLevel))
    //            {
    //                closestNode = closestNodeAtThisLevel;
    //                return true;
    //            }

    //            if (closestNodeAtThisLevel == null || closestNodeAtThisLevel.Below == null)
    //            {
    //                break;
    //            }

    //            closestNodeAtThisLevel = closestNodeAtThisLevel.Below;
    //        }

    //        return false;
    //    }

    //    private bool TrySearchForward(TKey key, SkipListNode<TKey, TValue> node, out SkipListNode<TKey, TValue> foundNode)
    //    {
    //        SkipListNode<TKey, TValue> currentNode = node;
    //        foundNode = currentNode;
    //        while (currentNode != null)
    //        {
    //            int resultOfComparison = _comparer.Compare(key, currentNode.Key);
    //            if (resultOfComparison == 0)
    //            {
    //                foundNode = currentNode.BottomMost;
    //                return true;
    //            }
    //            else if (resultOfComparison > 0)
    //            {
    //                return false;
    //            }

    //            foundNode = currentNode;
    //            currentNode = currentNode.Next;
    //        }

    //        return false;
    //    }

    //    public void Add(TKey index, TValue value)
    //    {
    //        SkipListNode<TKey, TValue> newLeaf = new SkipListNode<TKey, TValue>(index, value);

    //        if (_topLeft == null)
    //        {
    //            _topLeft = newLeaf;
    //            return;
    //        }

    //        SkipListNode<TKey, TValue> currentNode = _topLeft;

    //        if (_topLeft is SkipListNode<TKey, TValue>)
    //        {
    //            if (ShouldPromoteToNextLevel())
    //            {

    //            }
    //        }
    //    }

    //    private bool ShouldPromoteToNextLevel()
    //    {
    //        return (_rand.Next() & 1) == 0;
    //    }
    //}
}
