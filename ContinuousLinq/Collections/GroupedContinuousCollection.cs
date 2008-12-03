using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContinuousLinq.Collections
{
    public class GroupedContinuousCollection<TKey, TSource> : ContinuousCollection<TSource>, IGrouping<TKey, TSource>
    {
        private TKey _key;

        public GroupedContinuousCollection() : base() { }

        public GroupedContinuousCollection(TKey key)
            : base()
        {
            _key = key;
        }

        #region IGrouping<TKey,TSource> Members

        public TKey Key
        {
            get { return _key; }
        }

        #endregion
    }
}
