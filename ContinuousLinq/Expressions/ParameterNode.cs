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
        private Type _type;
        public override Type Type
        {
            get { return _type; }
        }

        public override bool IsRedundantVersion(PropertyAccessTreeNode other)
        {
            return other != this && other is ParameterNode;
        }

        public ParameterNode(Type type)
        {
            _type = type;
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
