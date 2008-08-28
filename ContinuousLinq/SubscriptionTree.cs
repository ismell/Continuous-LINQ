using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using System.Collections;

namespace ContinuousLinq
{
    internal class SubscriptionTree
    {
        public INotifyPropertyChanged Parameter { get; set; }

        public List<SubscriptionNode> Children { get; set; }

        public event Action<SubscriptionTree> PropertyChanged;

        public SubscriptionTree(INotifyPropertyChanged parameter, List<SubscriptionNode> children)
        {
            this.Parameter = parameter;
            this.Children = children;

            SubscribeToEntireTree(this.Children);
        }

        private void SubscribeToEntireTree(List<SubscriptionNode> nodes)
        {
            foreach (SubscriptionNode child in nodes)
            {
                child.PropertyChanged += OnNodeInTreePropertyChanged;
                if (child.Children != null)
                {
                    SubscribeToEntireTree(child.Children);
                }
            }
        }

        void OnNodeInTreePropertyChanged()
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this);
        }
    }
}
