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
    internal class PropertyAccessNode : PropertyAccessTreeNode
    {
        public PropertyInfo Property { get; private set; }

        public PropertyAccessNode(PropertyInfo property)
        {
            this.Property = property;
        }

        public override bool IsRedundantVersion(PropertyAccessTreeNode other)
        {
            var otherAsPropertyNode = other as PropertyAccessNode;
            if (otherAsPropertyNode == null)
                return false;

            return other != this && otherAsPropertyNode.Property == this.Property;
        }

        public override SubscriptionNode CreateSubscription(INotifyPropertyChanged parameter)
        {
            var subscriptionNode = new SubscriptionNode()
            {
                AccessNode = this,
            };

            SubscribeToChildren(subscriptionNode, parameter);

            return subscriptionNode;
        }

        public override string ToString()
        {
            return string.Format("Node: {0}", this.Property.ToString());
        }
    }
}
