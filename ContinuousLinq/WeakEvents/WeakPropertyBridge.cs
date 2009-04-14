using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ContinuousLinq.WeakEvents
{
    public class WeakPropertyBridge
    {
        private INotifyPropertyChanged _source;
        public HybridDictionary Callbacks { get; set; }

        public bool HasActiveListeners 
        {
            get { return this.Callbacks.Count > 0; } 
        }

        public WeakPropertyBridge(INotifyPropertyChanged source)
        {
            _source = source;
            this.Callbacks = new HybridDictionary();

            source.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            LinkedList<WeakPropertyChangedCallback> callbacksForProperty;

            callbacksForProperty = (LinkedList<WeakPropertyChangedCallback>)this.Callbacks[args.PropertyName];
            //if (!this.Callbacks.TryGetValue(args.PropertyName, out callbacksForProperty))
            if(callbacksForProperty == null)
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

                WeakPropertyChangedCallback callback = currentNode.Value;
                object listener = callback.ListenerReference.Target;
                if (listener != null)
                {
                    callback.Callback(listener, sender, args);
                }
                else
                {
                    callbacksForProperty.Remove(currentNode);
                }

                currentNode = nextNode;
            }

            CheckForUnsubscribe(callbacksForProperty, args.PropertyName);
        }

        private void CheckForUnsubscribe(LinkedList<WeakPropertyChangedCallback> callbacksForProperty, string propertyName)
        {
            if (callbacksForProperty.Count == 0)
            {
                this.Callbacks.Remove(propertyName);
            }

            UnsubscribeIfNoMoreListeners();
        }

        private void UnsubscribeIfNoMoreListeners()
        {
            if (!this.HasActiveListeners)
            {
                WeakPropertyChangedEventManager.UnregisterSource(_source);
                _source.PropertyChanged -= OnPropertyChanged;
            }
        }

        public void RemoveListener(
            object listener,
            string propertyName)
        {
            LinkedList<WeakPropertyChangedCallback> callbacksForProperty;

            callbacksForProperty = (LinkedList<WeakPropertyChangedCallback>)this.Callbacks[propertyName];

            //if (!this.Callbacks.TryGetValue(propertyName, out callbacksForProperty))
            if (callbacksForProperty == null)
            {
                return;
            }

            var currentNode = callbacksForProperty.First;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;

                WeakPropertyChangedCallback callback = currentNode.Value;
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

        public void AddListener<TListener>(
            string propertyName,
            TListener listener,
            Action<TListener, object, PropertyChangedEventArgs> forwardingAction)
        {
            LinkedList<WeakPropertyChangedCallback> callbacks;

            callbacks = (LinkedList<WeakPropertyChangedCallback>)this.Callbacks[propertyName];

            //if (!this.Callbacks.TryGetValue(propertyName, out callbacks))
            if (callbacks == null)
            {
                callbacks = new LinkedList<WeakPropertyChangedCallback>();
                this.Callbacks.Add(propertyName, callbacks);
            }

            Action<object, object, PropertyChangedEventArgs> callback = (listenerForForward, sender, args) =>
                forwardingAction((TListener)listenerForForward, sender, args);

            callbacks.AddLast(new WeakPropertyChangedCallback(listener, callback));
        }
    }
}
