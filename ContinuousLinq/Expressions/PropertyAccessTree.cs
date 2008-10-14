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

        public SubscriptionTree CreateSubscriptionTree(INotifyPropertyChanged parameter)
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

        public string GetParameterPropertyAccessString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (PropertyAccessTreeNode child in this.Children)
            {
                var childAsParameterNode = child as ParameterNode;
                
                if (childAsParameterNode == null)
                    continue;
                
                GetParameterPropertyAccessString(stringBuilder, childAsParameterNode);
            }

            return stringBuilder.ToString();
        }

        private void GetParameterPropertyAccessString(StringBuilder stringBuilder, PropertyAccessTreeNode currentNode)
        {
            foreach (PropertyAccessTreeNode child in currentNode.Children)
            {
                var childAsPropertyAccessNode = child as PropertyAccessNode;

                if (childAsPropertyAccessNode == null)
                    continue;

                if (child.Children.Count > 1)
                {
                    throw new Exception("This property access tree has multiple branches.  Use only with singl branch lambdas like (foo=> foo.Bar) NOT (foo => foo.Bar || foo.Ninja");
                }

                stringBuilder.AppendFormat(
                    stringBuilder.Length == 0 ? "{0}" : ".{0}",
                    childAsPropertyAccessNode.Property.Name);

                GetParameterPropertyAccessString(stringBuilder, childAsPropertyAccessNode);
            }
        }
    }
}