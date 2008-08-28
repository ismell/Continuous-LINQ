using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace ContinuousLinq
{
    public static class ExpressionPropertyAnalyzer
    {
        public static PropertyAccessTree Analyze<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            if (!DoesTypeImplementINotifyPropertyChanged(typeof(T)))
            {
                return null;
            }

            List<PropertyAccessNode> propertyNodes = new List<PropertyAccessNode>();

            PropertyAccessTree tree = BuildUnoptimizedTree(expression.Body);
            RemoveRedundantNodesFromTree(tree.Children);

            return tree;
        }

        private static void RemoveRedundantNodesFromTree(List<PropertyAccessTreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = nodes.Count - 1; j > i; j--)
                {
                    if (nodes[i].IsRedundantVersion(nodes[j]))
                    {
                    	nodes[i].Children.AddRange(nodes[j].Children);
                        nodes.RemoveAt(j);
                    }
                }
                RemoveRedundantNodesFromTree(nodes[i].Children);
            }
        }

        private static PropertyAccessTree BuildUnoptimizedTree(Expression expression)
        {
            PropertyAccessTree tree = new PropertyAccessTree();

            var currentNodeBranch = new Stack<PropertyAccessTreeNode>();
            BuildBranches(expression, tree, currentNodeBranch);

            return tree;
        }

        private static bool DoesTypeImplementINotifyPropertyChanged(Type type)
        {
            return type.GetInterface(typeof(INotifyPropertyChanged).Name) != null;
        }

        private static void BuildBranches(Expression expression, PropertyAccessTree tree, Stack<PropertyAccessTreeNode> currentNodeBranch)
        {
            BinaryExpression binaryExpression = expression as BinaryExpression;

            if (binaryExpression != null)
            {
                BuildBranches(binaryExpression.Left, tree, currentNodeBranch);
                BuildBranches(binaryExpression.Right, tree, currentNodeBranch);
                return;
            }

            UnaryExpression unaryExpression = expression as UnaryExpression;

            if (unaryExpression != null)
            {
                BuildBranches(unaryExpression.Operand, tree, currentNodeBranch);
                return;
            }

            MethodCallExpression methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression != null)
            {
                foreach (Expression argument in methodCallExpression.Arguments)
                {
                    BuildBranches(argument, tree, currentNodeBranch);
                }
                return;
            }

            ConditionalExpression conditionalExpression = expression as ConditionalExpression;

            if (conditionalExpression != null)
            {
                BuildBranches(conditionalExpression.Test, tree, currentNodeBranch);
                BuildBranches(conditionalExpression.IfTrue, tree, currentNodeBranch);
                BuildBranches(conditionalExpression.IfFalse, tree, currentNodeBranch);

                return;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    MemberExpression memberExpression = (MemberExpression)expression;

                    PropertyInfo property = memberExpression.Member as PropertyInfo;
                    FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
                    if (property != null)
                    {
                        if (DoesTypeImplementINotifyPropertyChanged(property.DeclaringType))
                        {
                            PropertyAccessNode node = new PropertyAccessNode(property);
                            currentNodeBranch.Push(node);
                        }
                        BuildBranches(memberExpression.Expression, tree, currentNodeBranch);
                    }
                    else if (fieldInfo != null)
                    {
                        if (DoesTypeImplementINotifyPropertyChanged(fieldInfo.FieldType))
                        {                        
                            ConstantExpression constantExpression = (ConstantExpression)memberExpression.Expression;
                            if (constantExpression.Value != null)
                            {
                                object value = fieldInfo.GetValue(constantExpression.Value);
                                ConstantNode constantNode = new ConstantNode((INotifyPropertyChanged)value);
                                currentNodeBranch.Push(constantNode);
                                AddBranch(tree, currentNodeBranch);
                            }
                        }
                        else
                        {
                            currentNodeBranch.Clear();
                        }
                    }
                    else
                    {
                        BuildBranches(memberExpression.Expression, tree, currentNodeBranch);
                    }

                    break;

                case ExpressionType.Parameter:
                    ParameterNode parameterNode = new ParameterNode();
                    currentNodeBranch.Push(parameterNode);
                    AddBranch(tree, currentNodeBranch);
                    break;

                case ExpressionType.Constant:
                    {
                        ConstantExpression constantExpression = (ConstantExpression)expression;
                        if (DoesTypeImplementINotifyPropertyChanged(constantExpression.Type) &&
                            constantExpression.Value != null)
                        {
                            ConstantNode constantNode = new ConstantNode((INotifyPropertyChanged)constantExpression.Value);
                            currentNodeBranch.Push(constantNode);
                            AddBranch(tree, currentNodeBranch);
                        }
                        else
                        {
                            currentNodeBranch.Clear();
                        }
                    }
                    break;

                default:
                    throw new InvalidProgramException("Unsupported expression type");
            }
        }

        internal static void AddBranch(PropertyAccessTree tree, Stack<PropertyAccessTreeNode> currentNodeBranch)
        {
            if (currentNodeBranch.Count == 0)
                return;

            PropertyAccessTreeNode currentNode = currentNodeBranch.Pop();
            tree.Children.Add(currentNode);

            while (currentNodeBranch.Count != 0)
            {
                PropertyAccessTreeNode nextNode = currentNodeBranch.Pop();
                currentNode.Children.Add(nextNode);
                currentNode = nextNode;
            }
        }

    }
}


