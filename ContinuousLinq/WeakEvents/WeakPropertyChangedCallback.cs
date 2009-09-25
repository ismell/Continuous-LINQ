using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ContinuousLinq.WeakEvents
{
    public interface IWeakCallback
    {
        WeakReference ListenerReference { get; }
    }

    public interface IWeakEventCallback<TArgs> : IWeakCallback
    {
        bool Invoke(object sender, TArgs args);
    }

    public class WeakPropertyChangingCallback<TListener> : 
        IWeakEventCallback<PropertyChangingEventArgs>,
        IWeakEventCallback<PropertyChangedEventArgs> 
    {
        private WeakReference _rootSource;
        public WeakReference ListenerReference { get; private set; }

        private Action<TListener, object, object, PropertyChangingEventArgs> _propertyChangingCallback;
        private Action<TListener, object, object, PropertyChangedEventArgs> _propertyChangedCallback;

        public WeakPropertyChangingCallback(
            object listener,
            object rootSource,
            Action<TListener, object, object, PropertyChangingEventArgs> propertyChangingCallback,
            Action<TListener, object, object, PropertyChangedEventArgs> propertyChangedCallback)
        {
            _rootSource = new WeakReference(rootSource);
            ListenerReference = new WeakReference(listener);
            _propertyChangingCallback = propertyChangingCallback;
            _propertyChangedCallback = propertyChangedCallback;
        }

        public bool Invoke(object sender, PropertyChangingEventArgs args)
        {
            TListener listenerForCallback = (TListener)ListenerReference.Target;
            object rootSource = _rootSource.Target;
            if (listenerForCallback != null && rootSource != null)
            {
                _propertyChangingCallback(listenerForCallback, sender, rootSource, args);
                return true;
            }
            return false;
        }

        public bool Invoke(object sender, PropertyChangedEventArgs args)
        {
            TListener listenerForCallback = (TListener)ListenerReference.Target;
            if (listenerForCallback != null)
            {
                _propertyChangedCallback(listenerForCallback, sender, _rootSource.Target, args);
                return true;
            }
            return false;
        }
    }


    public class WeakEventCallback<TListener, TArgs> : IWeakEventCallback<TArgs>
    {
        public WeakReference ListenerReference { get; private set; }

        private Action<TListener, object, TArgs> _callback;

        public WeakEventCallback(
            object listener,
            Action<TListener, object, TArgs> callback)
        {
            ListenerReference = new WeakReference(listener);
            _callback = callback;
        }

        public bool Invoke(object sender, TArgs args)
        {
            TListener listenerForCallback = (TListener)ListenerReference.Target;
            if (listenerForCallback != null)
            {
                _callback(listenerForCallback, sender, args);
                return true;
            }
            return false;
        }
    }

    public interface IWeakEventCallback : IWeakCallback
    {
        bool Invoke(object sender);
    }

    public class WeakEventCallback<TListener> : IWeakEventCallback
    {
        public WeakReference ListenerReference { get; private set; }

        private Action<TListener, object> _callback;

        public WeakEventCallback(
            object listener,
            Action<TListener, object> callback)
        {
            ListenerReference = new WeakReference(listener);
            _callback = callback;
        }

        public bool Invoke(object sender)
        {
            TListener listenerForCallback = (TListener)ListenerReference.Target;
            if (listenerForCallback != null)
            {
                _callback(listenerForCallback, sender);
                return true;
            }

            return false;
        }
    }
}
