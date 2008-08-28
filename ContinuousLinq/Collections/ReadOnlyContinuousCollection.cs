using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace ContinuousLinq
{
    public abstract class ReadOnlyContinuousCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IList<T>, IList
    {
        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        protected void FireCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }

            if (args.Action != NotifyCollectionChangedAction.Replace)
            {
                OnPropertyChanged("Count");
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new AccessViolationException();
        }

        public void RemoveAt(int index)
        {
            throw new AccessViolationException();
        }

        public abstract T this[int index] { get; set; }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new AccessViolationException();
        }

        public void Clear()
        {
            throw new AccessViolationException();
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int currentArrayIndex = arrayIndex;
            for (int i = 0; i < this.Count; i++, arrayIndex++)
            {
                array[arrayIndex] = this[i];
            }
        }

        public abstract int Count
        {
            get;
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new AccessViolationException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new AccessViolationException();
        }

        private static bool CanUseGenericVersionOfMethod(object value)
        {
            return (value is T) || (typeof(T).IsValueType && value != null);
        }

        public bool Contains(object value)
        {
            if (CanUseGenericVersionOfMethod(value))
            {
                return Contains((T)value);
            }

            return false;
        }

        public int IndexOf(object value)
        {
            if (CanUseGenericVersionOfMethod(value))
            {
                return IndexOf((T)value);
            }
            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new AccessViolationException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            throw new AccessViolationException();
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList<T>)this)[index];
            }
            set
            {
                throw new AccessViolationException();
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int arrayIndex)
        {
            for (int i = 0; i < this.Count; i++, arrayIndex++)
            {
                array.SetValue(this[i], arrayIndex);
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

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
}