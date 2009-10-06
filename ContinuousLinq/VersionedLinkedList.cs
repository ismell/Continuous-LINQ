using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousLinq
{
    internal interface IVersionedLinkedListNode<T>
    {
        T Value { get; set; }
    }

    internal class VersionedLinkedList<T> 
    {
        public ulong Version { get; private set; }
        internal VersionedLinkedListNode<T> First { get; set; }
        internal VersionedLinkedListNode<T> Last { get; set; }
        private int _recursionCount;
        private ulong _topMostVersion;
        private Stack<VersionedLinkedListNode<T>> _toBeRemoved;

        public bool IsEmpty
        {
            get { return this.First == null; }
        }

        public VersionedLinkedList()
        {
            _toBeRemoved = new Stack<VersionedLinkedListNode<T>>();
        }

        public void AddLast(T value)
        {
            this.Version++;

            VersionedLinkedListNode<T> nodeToAdd = new VersionedLinkedListNode<T>(value, this.Version);

            if (this.Last == null)
            {
                this.Last = nodeToAdd;
                this.First = nodeToAdd;
                return;
            }

            VersionedLinkedListNode<T> previousLast = this.Last;
            previousLast.Next = nodeToAdd;
            nodeToAdd.Previous = previousLast;

            this.Last = nodeToAdd;
        }

        public IVersionedLinkedListNode<T> StartIterating(ulong version)
        {
            if (_recursionCount == 0)
            {
                _topMostVersion = this.Version;
            }

            _recursionCount++;

            var firstNodeLessThanVersion = ScanListLookingForNextNodeMatchingVersion(version, this.First);
            return firstNodeLessThanVersion;
        }

        public IVersionedLinkedListNode<T> GetNext(ulong version, IVersionedLinkedListNode<T> currentNode)
        {
            VersionedLinkedListNode<T> node = (VersionedLinkedListNode<T>)currentNode;
            node = node.Next;
            return ScanListLookingForNextNodeMatchingVersion(version, node);
        }

        private static IVersionedLinkedListNode<T> ScanListLookingForNextNodeMatchingVersion(ulong version, VersionedLinkedListNode<T> currentNode)
        {
            while (currentNode != null && currentNode.Version > version)
            {
                currentNode = currentNode.Next;
            }

            return currentNode;
        }

        public void StopIterating()
        {
            _recursionCount--;

            if (_recursionCount == 0)
            {
                while (_toBeRemoved.Count > 0)
                {
                    var nodeToRemove = _toBeRemoved.Pop();
                    Remove(nodeToRemove);
                }
            }
        }

        public void MarkForRemove(IVersionedLinkedListNode<T> nodeToRemove)
        {
            VersionedLinkedListNode<T> node = (VersionedLinkedListNode<T>)nodeToRemove;
            if (_recursionCount == 0)
            {
                Remove(node);
            }
            else
            {
                node.MarkForDelete();
                _toBeRemoved.Push(node);
            }
        }

        private void Remove(VersionedLinkedListNode<T> nodeToRemove)
        {
            if (_recursionCount != 0)
                throw new InvalidOperationException("You may not do a remove when iterating.  Use MarkForRemove instead.");

            var previousNode = nodeToRemove.Previous;
            var nextNode = nodeToRemove.Next;

            if (previousNode != null)
            {
                previousNode.Next = nextNode;
            }

            if (nextNode != null)
            {
                nextNode.Previous = previousNode;
            }

            if (nodeToRemove == this.First)
            {
                this.First = nextNode;
            }

            if (nodeToRemove == this.Last)
            {
                this.Last = previousNode;
            }

            nodeToRemove.Next = null;
            nodeToRemove.Previous = null;
        }

        internal class VersionedLinkedListNode<T> : IVersionedLinkedListNode<T>
        {
            public ulong Version { get; private set; }

            public T Value { get; set; }

            public VersionedLinkedListNode<T> Next { get; set; }
            public VersionedLinkedListNode<T> Previous { get; set; }

            public bool IsMarkedForDelete
            {
                get { return this.Version == ulong.MaxValue; }
            }

            public void MarkForDelete()
            {
                this.Version = ulong.MaxValue;
            }

            public VersionedLinkedListNode(T value, ulong version)
            {
                this.Value = value;
                this.Version = version;
            }
        }
    }
}
