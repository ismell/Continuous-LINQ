using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace ContinuousLinq
{
    public static class ExpressionPropertyAnalyzer
    {
        #region Methods

        public static PropertyAccessTree Analyze<T, TResult>(Expression<Func<T, TResult>> expression, Predicate<Type> typeFilter)
        {
            if (!typeFilter(typeof(T)))
            {
                return null;
            }

            PropertyAccessTree tree = BuildUnoptimizedTree(expression.Body, typeFilter);
            RemoveRedundantNodesFromTree(tree.Children);
            ApplyTypeFilter(tree.Children, typeFilter);
            return tree;
        }

        private static void ApplyTypeFilter(List<PropertyAccessTreeNode> children, Predicate<Type> typeFilter)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                var propertyAccessNode = children[i] as PropertyAccessNode;
                if (propertyAccessNode != null)
                {
                    if (propertyAccessNode.Children.Count > 0 && !typeFilter(propertyAccessNode.Property.PropertyType))
                    {
                        propertyAccessNode.Children.Clear();
                    }
                }
                ApplyTypeFilter(children[i].Children, typeFilter);
            }
        }

        public static PropertyAccessTree Analyze<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return Analyze(expression, DoesTypeImplementINotifyPropertyChanged);
        }

        private static void RemoveRedundantNodesFromTree(IList<PropertyAccessTreeNode> nodes)
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

        private static PropertyAccessTree BuildUnoptimizedTree(Expression expression, Predicate<Type> typeFilter)
        {
            PropertyAccessTree tree = new PropertyAccessTree();

            var currentNodeBranch = new Stack<PropertyAccessTreeNode>();
            BuildBranches(expression, tree, currentNodeBranch, typeFilter);

            return tree;
        }

        private static bool DoesTypeImplementINotifyPropertyChanged(Type type)
        {
            return type.GetInterface(typeof(INotifyPropertyChanged).Name) != null;
        }

        private static void BuildBranches(Expression expression, PropertyAccessTree tree, Stack<PropertyAccessTreeNode> currentNodeBranch, Predicate<Type> typeFilter)
        {
            BinaryExpression binaryExpression = expression as BinaryExpression;

            if (binaryExpression != null)
            {
                BuildBranches(binaryExpression.Left, tree, currentNodeBranch, typeFilter);
                BuildBranches(binaryExpression.Right, tree, currentNodeBranch, typeFilter);
                return;
            }

            UnaryExpression unaryExpression = expression as UnaryExpression;

            if (unaryExpression != null)
            {
                BuildBranches(unaryExpression.Operand, tree, currentNodeBranch, typeFilter);
                return;
            }

            MethodCallExpression methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression != null)
            {
                foreach (Expression argument in methodCallExpression.Arguments)
                {
                    BuildBranches(argument, tree, currentNodeBranch, typeFilter);
                }
                return;
            }

            ConditionalExpression conditionalExpression = expression as ConditionalExpression;

            if (conditionalExpression != null)
            {
                BuildBranches(conditionalExpression.Test, tree, currentNodeBranch, typeFilter);
                BuildBranches(conditionalExpression.IfTrue, tree, currentNodeBranch, typeFilter);
                BuildBranches(conditionalExpression.IfFalse, tree, currentNodeBranch, typeFilter);
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
                        PropertyAccessNode node = new PropertyAccessNode(property);
                        currentNodeBranch.Push(node);

                        BuildBranches(memberExpression.Expression, tree, currentNodeBranch, typeFilter);
                    }
                    else if (fieldInfo != null)
                    {
                        if (typeFilter(fieldInfo.FieldType))
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
                        BuildBranches(memberExpression.Expression, tree, currentNodeBranch, typeFilter);
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
                        if (typeFilter(constantExpression.Type) &&
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

        private static void AddBranch(PropertyAccessTree tree, Stack<PropertyAccessTreeNode> currentNodeBranch)
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

        #endregion
    }
}


