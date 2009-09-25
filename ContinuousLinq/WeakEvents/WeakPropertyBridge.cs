using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ContinuousLinq.WeakEvents
{
    public class WeakPropertyBridge
    {
        private INotifyPropertyChanged _source;
        private HybridDictionary _propertyNameToCallbacks;

        public bool HasActiveListeners
        {
            get { return _propertyNameToCallbacks.Count > 0; }
        }

        public WeakPropertyBridge(INotifyPropertyChanged source)
        {
            _source = source;
            _propertyNameToCallbacks = new HybridDictionary();

            source.PropertyChanged += OnPropertyChanged;

            INotifyPropertyChanging sourceAsINotifyPropertyChanging = source as INotifyPropertyChanging;
            if (sourceAsINotifyPropertyChanging != null)
            {
                sourceAsINotifyPropertyChanging.PropertyChanging += OnPropertyChanging;
            }
        }

        public WeakPropertyBridge(INotifyPropertyChanged rootSource, INotifyPropertyChanged source)
            : this(source)
        {
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var callbacksForProperty = LookupCallbacksForProperty(args.PropertyName);
            if (callbacksForProperty == null)
            {
                return;
            }

            //Note: this loop structure is repeated in other functions.  Didn't break out into
            //      a nice RemovingForEach for LinkedLists to avoid closure creation caused by
            //      capturing sender, and args.
            var currentNode = callbacksForProperty.First;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;

                IWeakEventCallback<PropertyChangedEventArgs> callback = (IWeakEventCallback<PropertyChangedEventArgs>)currentNode.Value;
                object listener = callback.ListenerReference.Target;

                if (!callback.Invoke(sender, args))
                {
                    callbacksForProperty.Remove(currentNode);
                }

                currentNode = nextNode;
            }

            CheckForUnsubscribe(callbacksForProperty, args.PropertyName);
        }

        private void CheckForUnsubscribe(LinkedList<IWeakCallback> callbacksForProperty, string propertyName)
        {
            if (callbacksForProperty.Count == 0)
            {
                _propertyNameToCallbacks.Remove(propertyName);
            }

            UnsubscribeIfNoMoreListeners();
        }

        private void UnsubscribeIfNoMoreListeners()
        {
            if (!this.HasActiveListeners)
            {
                WeakPropertyChangedEventManager.UnregisterSource(_source);
                _source.PropertyChanged -= OnPropertyChanged;

                INotifyPropertyChanging sourceAsINotifyPropertyChanging = _source as INotifyPropertyChanging;
                if (sourceAsINotifyPropertyChanging != null)
                {
                    sourceAsINotifyPropertyChanging.PropertyChanging -= OnPropertyChanging;
                }
            }
        }

        public void RemoveListener(
            object listener,
            string propertyName)
        {
            var callbacksForProperty = LookupCallbacksForProperty(propertyName);

            if (callbacksForProperty == null)
            {
                return;
            }

            var currentNode = callbacksForProperty.First;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;

                IWeakEventCallback<PropertyChangedEventArgs> callback = (IWeakEventCallback<PropertyChangedEventArgs>)currentNode.Value;
                object listenerForCallback = callback.ListenerReference.Target;

                if (listenerForCallback == null ||
                    listenerForCallback == listener)
                {
                    callbacksForProperty.Remove(currentNode);
                }

                currentNode = nextNode;
            }

            CheckForUnsubscribe(callbacksForProperty, propertyName);
        }

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs args)
        {
            var callbacksForProperty = LookupCallbacksForProperty(args.PropertyName);
            if (callbacksForProperty == null)
            {
                return;
            }

            var currentNode = callbacksForProperty.First;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;

                IWeakEventCallback<PropertyChangingEventArgs> callback = currentNode.Value as IWeakEventCallback<PropertyChangingEventArgs>;
                if (callback != null)
                {
                    object listener = callback.ListenerReference.Target;

                    if (!callback.Invoke(sender, args))
                    {
                        callbacksForProperty.Remove(currentNode);
                    }
                }
                currentNode = nextNode;
            }
        }

        public void AddListener<TListener>(
            string propertyName,
            TListener listener,
            Action<TListener, object, PropertyChangedEventArgs> propertyChangedCallback)
        {
            var callbacksForProperty = LookupOrCreateCallbacksForProperty(propertyName);
            var callback = new WeakEventCallback<TListener, PropertyChangedEventArgs>(listener, propertyChangedCallback);
            callbacksForProperty.AddLast(callback);
        }

        public void AddListener<TListener>(
            string propertyName,
            TListener listener,
            object rootSource,
            Action<TListener, object, object, PropertyChangingEventArgs> propertyChangingCallback,
            Action<TListener, object, object, PropertyChangedEventArgs> propertyChangedCallback)
        {
            var callbacksForProperty = LookupOrCreateCallbacksForProperty(propertyName);

            var callback = new WeakPropertyChangingCallback<TListener>(
                listener,
                rootSource,
                propertyChangingCallback,
                propertyChangedCallback);

            callbacksForProperty.AddLast(callback);
        }

        private LinkedList<IWeakCallback> LookupOrCreateCallbacksForProperty(string propertyName)
        {
            var callbacksForProperty = LookupCallbacksForProperty(propertyName);

            if (callbacksForProperty == null)
            {
                callbacksForProperty = new LinkedList<IWeakCallback>();
                _propertyNameToCallbacks.Add(propertyName, callbacksForProperty);
            }
            return callbacksForProperty;
        }

        private LinkedList<IWeakCallback> LookupCallbacksForProperty(string propertyName)
        {
            LinkedList<IWeakCallback> callbacksForProperty;

            callbacksForProperty = (LinkedList<IWeakCallback>)_propertyNameToCallbacks[propertyName];
            return callbacksForProperty;
        }
    }
}
