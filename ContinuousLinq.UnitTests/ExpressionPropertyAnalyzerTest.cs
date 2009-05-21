using System;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class ExpressionPropertyAnalyzerTest : INotifyPropertyChanged
    {
        private PropertyInfo _ageProperty;
        private PropertyInfo _nameProperty;
        private PropertyInfo _brotherProperty;

        #region For Unit Testing 'this'

        public int TestProperty { get; set; }

        public Func<string, string> StringPassThrough { get; set; }

        #endregion

        [SetUp]
        public void Setup()
        {
            _ageProperty = typeof(Person).GetProperty("Age");
            _nameProperty = typeof(Person).GetProperty("Name");
            _brotherProperty = typeof(Person).GetProperty("Brother");
            this.StringPassThrough = str => str;
        }

        [Test]
        public void Analyze_ExpressionAccessesOnlyParameter_TreeHasOnlyParameter()
        {
            Expression<Func<Person, Person>> expression = p => p;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);
            
            Assert.AreEqual(1, tree.Children.Count);
            Assert.IsInstanceOfType(typeof(ParameterNode), tree.Children[0]);
        }

        [Test]
        public void Analyze_ExpressionReturnsOneLevelProperty_TreeHasOneParameterBranchWithOneProperty()
        {
            Expression<Func<Person, int>> expression = p => p.Age;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessNode propertyNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_ageProperty, propertyNode.Property);
            Assert.AreEqual(0, propertyNode.Children.Count);
        }

        [Test]
        public void Analyze_ExpressionUsesMethodCallOnOneLevelProperty_TreeHasOneParameterBranchWithOneProperty()
        {
            Expression<Func<Person, bool>> expression = p => string.IsNullOrEmpty(p.Name);

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessNode propertyNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_nameProperty, propertyNode.Property);
            Assert.AreEqual(0, propertyNode.Children.Count);
        }



        [Test]
        public void Analyze_ExpressionReturnsTwoLevelProperty_TreeHasOneParameterBranchWithTwoProperties()
        {
            Expression<Func<Person, int>> expression = p => p.Brother.Age;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessNode brotherNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_brotherProperty, brotherNode.Property);
            Assert.AreEqual(1, brotherNode.Children.Count);

            PropertyAccessNode ageNode = (PropertyAccessNode)brotherNode.Children[0];
            Assert.AreEqual(_ageProperty, ageNode.Property);
            Assert.AreEqual(0, ageNode.Children.Count);
        }

        [Test]
        public void Analyze_ConditionalExpressionReferencingTwoProperties_TreeContainsOnePropertyBranchWithBothProperties()
        {
            Expression<Func<Person, int>> expression = p => string.IsNullOrEmpty(p.Name) ? p.Age : 0;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.AreEqual(2, parameterNode.Children.Count);

            PropertyAccessNode nameNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_nameProperty, nameNode.Property);
            Assert.AreEqual(0, nameNode.Children.Count);

            PropertyAccessNode ageNode = (PropertyAccessNode)parameterNode.Children[1];
            Assert.AreEqual(_ageProperty, ageNode.Property);
            Assert.AreEqual(0, ageNode.Children.Count);
        }


        [Test]
        public void Analyze_ConditionalExpressionBothOneLevelAndTwoLevelProperties_TreeCorrect()
        {
            Expression<Func<Person, int>> expression = p => p.Age > 3 ? p.Age : p.Brother.Age;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.AreEqual(2, parameterNode.Children.Count);

            PropertyAccessNode firstLevelAgeNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_ageProperty, firstLevelAgeNode.Property);
            Assert.AreEqual(0, firstLevelAgeNode.Children.Count);

            PropertyAccessNode brotherNode = (PropertyAccessNode)parameterNode.Children[1];
            Assert.AreEqual(_brotherProperty, brotherNode.Property);
            Assert.AreEqual(1, brotherNode.Children.Count);

            PropertyAccessNode secondLevelAgeNode = (PropertyAccessNode)brotherNode.Children[0];
            Assert.AreEqual(_ageProperty, secondLevelAgeNode.Property);
            Assert.AreEqual(0, secondLevelAgeNode.Children.Count);
        }


        [Test]
        public void Analyze_ExpressionIncludesConstantImplementingINotifyPropertyChanged_TreeContainsTwoBranches()
        {
            Person localPersonAppearingAsConstantInExpression = new Person();
            Expression<Func<Person, int>> expression = p => p.Age + localPersonAppearingAsConstantInExpression.Age;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(2, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessNode parameterAgeNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_ageProperty, parameterAgeNode.Property);
            Assert.AreEqual(0, parameterAgeNode.Children.Count);

            PropertyAccessTreeNode constantNode = tree.Children[1];
            Assert.IsInstanceOfType(typeof(ConstantNode), constantNode);
            Assert.AreEqual(localPersonAppearingAsConstantInExpression, ((ConstantNode)constantNode).Value);
            Assert.AreEqual(1, constantNode.Children.Count);

            PropertyAccessNode constantAgeNode = (PropertyAccessNode)constantNode.Children[0];
            Assert.AreEqual(_ageProperty, constantAgeNode.Property);
            Assert.AreEqual(0, constantAgeNode.Children.Count);
        }


        [Test]
        public void Analyze_ExpressionContainsThis_TreeContainsOneBranch()
        {
            Expression<Func<Person, int>> expression = p => this.TestProperty;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);

            PropertyAccessTreeNode constantNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ConstantNode), constantNode);
            Assert.AreEqual(this, ((ConstantNode)constantNode).Value);
            Assert.AreEqual(1, constantNode.Children.Count);

            PropertyAccessNode testPropertyNode = (PropertyAccessNode)constantNode.Children[0];
            Assert.AreEqual(GetType().GetProperty("TestProperty"), testPropertyNode.Property);
            Assert.AreEqual(0, testPropertyNode.Children.Count);
        }

        [Test]
        public void Analyze_TwoLevelPropertyWhereSubPropertyIsNotINotifyPropertyChanged_TreeContainsFirstProperty()
        {
            Expression<Func<Person, bool>> expression = p => p.Name.Length == 0;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessNode nameNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_nameProperty, nameNode.Property);
            Assert.AreEqual(0, nameNode.Children.Count);
        }

        [Test]
        public void Analyze_ExpressionContainsNothingWithINotifyPropertyChanged_ReturnsNull()
        {
            Expression<Func<int, bool>> expression = i => i == 0;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.IsNull(tree);
        }

        [Test]
        public void Analyze_SpecialTypeFilterWithExpressionThatPassesFilter_ReturnsPropertyAccessTree()
        {
            Expression<Func<string, bool>> expression = str => str.Length == 0;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression, type => type == typeof(string));

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessNode lengthNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(typeof(string).GetProperty("Length"), lengthNode.Property);
            Assert.AreEqual(0, lengthNode.Children.Count);
        }

        [Test]
        public void Analyze_SpecialTypeFilterWithExpressionThatFailsFilter_ReturnsPropertyAccessTree()
        {
            Expression<Func<string, bool>> expression = str => str.Length == 0;

            PropertyAccessTree tree = ExpressionPropertyAnalyzer.Analyze(expression, type => type == typeof(int));

            Assert.IsNull(tree);
        }

        [Test]
        public void Analyze_ExpressionContainsNewOperatorWithMemberAssignmentInitialization_ReturnsPropertyAccessTree()
        {
            Expression<Func<Person, Person>> expression = person => new Person() { Name = person.Name, Age = person.Age};

            var tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(2, parameterNode.Children.Count);

            PropertyAccessNode nameNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_nameProperty, nameNode.Property);
            Assert.AreEqual(0, nameNode.Children.Count);

            PropertyAccessNode ageNode = (PropertyAccessNode)parameterNode.Children[1];
            Assert.AreEqual(_ageProperty, ageNode.Property);
            Assert.AreEqual(0, ageNode.Children.Count);
        }

        [Test]
        public void Analyze_ExpressionContainsNewOperatorWithConstructorArguments_ReturnsPropertyAccessTree()
        {
            Expression<Func<Person, Person>> expression = person => new Person(person.Name, person.Age);

            var tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(2, parameterNode.Children.Count);

            PropertyAccessNode nameNode = (PropertyAccessNode)parameterNode.Children[0];
            Assert.AreEqual(_nameProperty, nameNode.Property);
            Assert.AreEqual(0, nameNode.Children.Count);

            PropertyAccessNode ageNode = (PropertyAccessNode)parameterNode.Children[1];
            Assert.AreEqual(_ageProperty, ageNode.Property);
            Assert.AreEqual(0, ageNode.Children.Count);
        }

        [Test]
        public void Analyze_ExpressionContainsDelegate_ReturnsPropertyAccessTree()
        {
            Func<Person, string> del = person => person.Name;

            Expression<Func<Person, string>> expression = person => del(person);

            var tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(1, tree.Children.Count);
            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(0, parameterNode.Children.Count);
        }

        [Test]
        public void Analyze_ExpressionContainsDelegateAndPropertyAccess_ReturnsPropertyAccessTree()
        {
            Expression<Func<Person, string>> expression = person => this.StringPassThrough(person.Name);

            var tree = ExpressionPropertyAnalyzer.Analyze(expression);

            Assert.AreEqual(2, tree.Children.Count);

            PropertyAccessTreeNode parameterNode = tree.Children[0];
            Assert.IsInstanceOfType(typeof(ParameterNode), parameterNode);
            Assert.AreEqual(1, parameterNode.Children.Count);

            PropertyAccessTreeNode constantNode = tree.Children[1];
            Assert.IsInstanceOfType(typeof(ConstantNode), constantNode);
            Assert.AreEqual(1, constantNode.Children.Count);
        }

        //[Test]
        //public void Test()
        //{
        //    Expression<Func<Person, string>> nameAccessorZero = p => p.Name;
        //    Expression<Func<Person, string>> nameAccessorOne = p => p.Name;
        //    Assert.AreEqual(nameAccessorZero, nameAccessorOne);
        //    //Assert.AreSame(typeof(Person).GetProperty("Name").Name, typeof(Person).GetProperty("Name").Name);
        //    //Assert.AreSame("Name", typeof(Person).GetProperty("Name").Name);
        //}

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion        
    }
}
