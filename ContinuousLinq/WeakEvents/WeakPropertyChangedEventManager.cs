using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ContinuousLinq.WeakEvents
{
    public static class WeakPropertyChangedEventManager
    {
        internal static WeakDictionary<INotifyPropertyChanged, WeakReference<WeakPropertyBridge>> SourceToBridgeTable { get; private set; }
        
        static WeakPropertyChangedEventManager()
        {
            SourceToBridgeTable = new WeakDictionary<INotifyPropertyChanged, WeakReference<WeakPropertyBridge>>();
        }

        public static void Register<TListener>(
            INotifyPropertyChanged source,
            string propertyName,
            TListener listener,
            Action<TListener, object, PropertyChangedEventArgs> forwardingAction)
        {
            WeakReference<WeakPropertyBridge> bridgeRef;
            WeakPropertyBridge bridge;

            if (!SourceToBridgeTable.TryGetValue(source, out bridgeRef))
            {
                AddNewPropertyBridge(source, out bridgeRef, out bridge);
            }
            else
            {
                bridge = bridgeRef.Target;

                //Can happen if the GC does it's magic 
                if (bridge == null)
                {
                    AddNewPropertyBridge(source, out bridgeRef, out bridge);
                }
            }
            
            bridge.AddListener(propertyName, listener, forwardingAction);
        }

        private static void AddNewPropertyBridge(
            INotifyPropertyChanged source,
            out WeakReference<WeakPropertyBridge> bridgeRef,
            out WeakPropertyBridge bridge)
        {
            bridge = new WeakPropertyBridge(source);
            bridgeRef = WeakReference<WeakPropertyBridge>.Create(bridge);
            SourceToBridgeTable[source] = bridgeRef;
        }

        private static void OnAllListenersForSourceUnsubscribed(INotifyPropertyChanged source)
        {
            UnregisterSource(source);
        }

        public static void Unregister(INotifyPropertyChanged source, string propertyName, object listener)
        {
            WeakReference<WeakPropertyBridge> bridgeRef;

            if (!SourceToBridgeTable.TryGetValue(source, out bridgeRef))
            {
                return;
            }

            WeakPropertyBridge bridge = bridgeRef.Target;
            if (bridge == null)
            {
                SourceToBridgeTable.Remove(source);
                return;
            }

            bridge.RemoveListener(listener, propertyName);
        }

        public static void UnregisterSource(INotifyPropertyChanged source)
        {
            SourceToBridgeTable.Remove(source);
        }

        public static void RemoveCollectedEntries()
        {
            SourceToBridgeTable.RemoveCollectedEntries();
        }
    }
}
