﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace ContinuousLinq.Reactive
{
    public interface IReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
    }

    public abstract class ReactiveObject : IReactiveObject {
        #region Data Members
#if !SILVERLIGHT
        [NonSerialized]
#endif
        private bool _SuppressPropertyChanged;

#if !SILVERLIGHT
        [NonSerialized]
#endif
        private Dispatcher _Dispatcher;

        #endregion

        #region Properties

        private static Dictionary<Type, IDependsOn> DependsOn { get; set; }

        private List<SubscriptionTree> _subscriptionTrees;

        [XmlIgnore]
        public bool SuppressPropertyChanged {
            get { return _SuppressPropertyChanged; }
            set { _SuppressPropertyChanged = value; }
        }

        [XmlIgnore]
        public Dispatcher Dispatcher {
            get { return _Dispatcher; }
            set { _Dispatcher = value; }
        }

        #endregion

        #region Events & Delegates

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
        
        #endregion

        #region Constructors

        static ReactiveObject()
        {
            DependsOn = new Dictionary<Type, IDependsOn>();
        }

        protected ReactiveObject()
        {
#if !SILVERLIGHT
            this.Dispatcher = Dispatcher.CurrentDispatcher;
#endif
            Type type = this.GetType();

            CreateSubscriptionsStartingBaseFirst(type);
        }

        #endregion

        #region Methods

        private void CreateSubscriptionsStartingBaseFirst(Type type)
        {
            if (type == typeof(ReactiveObject))
                return;

            CreateSubscriptionsStartingBaseFirst(type.BaseType);

            IDependsOn dependsOn;
            if (DependsOn.TryGetValue(type, out dependsOn))
            {
                dependsOn.CreateSubscriptions(this, ref _subscriptionTrees);
            }
        }

        protected static DependsOn<T> Register<T>() where T : ReactiveObject
        {
            Type type = typeof(T);

            if (DependsOn.ContainsKey(type))
                throw new InvalidOperationException("Type has already been registered for: " + typeof(T));

            var dependsOn = new DependsOn<T>();

            DependsOn[type] = dependsOn;

            return dependsOn;
        }

        [DebuggerNonUserCode]
        protected internal void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        [DebuggerNonUserCode]
        protected internal void OnPropertyChanging(string propertyName)
        {
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        [DebuggerNonUserCode]
        protected internal void OnPropertyChanged(PropertyChangedEventArgs args) {
            if (this.SuppressPropertyChanged)
                return;

            PropertyChanged.Raise(this, args);
        }

        [DebuggerNonUserCode]
        protected internal void OnPropertyChanging(PropertyChangingEventArgs args) {
            if (this.SuppressPropertyChanged)
                return;

            PropertyChanging.Raise(this, args);
        }

        #endregion
    }
}