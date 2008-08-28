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
    internal class ParameterNode : PropertyAccessTreeNode
    {
        public override bool IsRedundantVersion(PropertyAccessTreeNode other)
        {
            return other != this && other is ParameterNode;
        }

        public override SubscriptionNode CreateSubscription(INotifyPropertyChanged parameter)
        {
            var subscriptionNode = new SubscriptionNode() 
            { 
                AccessNode = this 
            };

            SubscribeToChildren(subscriptionNode, parameter);
            subscriptionNode.Subject = parameter;

            return subscriptionNode;
        }
    }
}
