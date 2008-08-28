using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace ContinuousLinq.Aggregates
{
    public class PausedAggregation : IDisposable
    {
        private bool _wasDisposed;

        public PausedAggregation()
        {
            ContinuousValue.GlobalPause();
        }

        ~PausedAggregation()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!_wasDisposed)
            {
                ContinuousValue.GlobalResume();
                _wasDisposed = true;
            }
        }
        #endregion
    }
}
