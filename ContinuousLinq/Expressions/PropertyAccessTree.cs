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
    public class PropertyAccessTree
    {
        internal List<PropertyAccessTreeNode> Children { get; set; }

        public PropertyAccessTree()
        {
            this.Children = new List<PropertyAccessTreeNode>();
        }

        internal SubscriptionTree CreateSubscriptionTree(INotifyPropertyChanged parameter)
        {
            List<SubscriptionNode> subscribers = new List<SubscriptionNode>(this.Children.Count);
            foreach (PropertyAccessTreeNode child in this.Children)
            {
                if (child.Children.Count > 0)
                {
                    var subscriptionNode = child.CreateSubscription(parameter);
                    subscribers.Add(subscriptionNode);
                }
            }

            var subscriptionTree = new SubscriptionTree(parameter, subscribers);
            return subscriptionTree;
        }
    }
}