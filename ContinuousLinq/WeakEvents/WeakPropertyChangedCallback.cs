using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ContinuousLinq.WeakEvents
{
    public struct WeakPropertyChangedCallback
    {
        public WeakReference ListenerReference { get; private set; }

        public Action<object, object, PropertyChangedEventArgs> Callback { get; private set; }

        public WeakPropertyChangedCallback(
            object listener,
            Action<object, object, PropertyChangedEventArgs> callback)
            : this()
        {
            ListenerReference = new WeakReference(listener);
            Callback = callback;
        }
    }
}
