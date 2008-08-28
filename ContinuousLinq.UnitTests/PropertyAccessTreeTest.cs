using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows;
using System.Reflection;

namespace ContinuousLinq.UnitTests
{

    [TestFixture]
    public class PropertyAccessTreeTest
    {
        private PropertyAccessTree _target;
        private Person _person;

        private PropertyInfo _ageProperty;
        private PropertyAccessNode _agePropertyAccessNode;
        
        private PropertyInfo _brotherProperty; 
        private PropertyAccessNode _brotherPropertyAccessNode;
       
        [SetUp]
        public void Setup()
        {
            _target = new PropertyAccessTree();
            _person = new Person();
            _ageProperty = typeof(Person).GetProperty("Age");
            _brotherProperty = typeof(Person).GetProperty("Brother");
        }

        private void InitializeTargetJustAgeAccess()
        {
            ParameterNode parameterNode = new ParameterNode();
            _target.Children.Add(parameterNode);

            _agePropertyAccessNode = new PropertyAccessNode(_ageProperty);
            parameterNode.Children.Add(_agePropertyAccessNode);
        }

        private void InitializeTargetBrothersAgeAccess()
        {
            ParameterNode parameterNode = new ParameterNode();
            _target.Children.Add(parameterNode);

            _brotherPropertyAccessNode = new PropertyAccessNode(_brotherProperty);
            parameterNode.Children.Add(_brotherPropertyAccessNode); 
            
            _agePropertyAccessNode = new PropertyAccessNode(_ageProperty);
            _brotherPropertyAccessNode.Children.Add(_agePropertyAccessNode);
        }

        [Test]
        public void CreateSubscriptionTree_SimplePropertyAccess_TreeHasAllItems()
        {
            InitializeTargetJustAgeAccess();

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);

            Assert.AreEqual(1, subscriptionTree.Children.Count);
            SubscriptionNode root = subscriptionTree.Children[0];
            Assert.AreEqual(_person, root.Subject);
            Assert.IsNull(root.Children);
        }

        [Test]
        public void CreateSubscriptionTree_TwoLevelPropertyAccess_TreeHasAllItems()
        {
            InitializeTargetBrothersAgeAccess();

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);

            Assert.AreEqual(1, subscriptionTree.Children.Count);
            
            SubscriptionNode root = subscriptionTree.Children[0];
            Assert.AreEqual(_person, root.Subject);
            Assert.AreEqual(1, root.Children.Count);

            SubscriptionNode brotherSubscriptionNode = root.Children[0];
            Assert.AreEqual(_brotherPropertyAccessNode, brotherSubscriptionNode.AccessNode);
            Assert.IsNull(brotherSubscriptionNode.Children);
        }

        [Test]
        public void CreateSubscriptionTreeChangePropertyOnParameter_PropertyBeingMonitored_PropertyChanges()
        {
            InitializeTargetJustAgeAccess();

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);
            int callCount = 0;
            subscriptionTree.PropertyChanged += (sender) => callCount++;

            _person.Age = 12321321;
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CreateSubscriptionTreeChangePropertyOnParameter_PropertyNotBeingMonitored_PropertyChanges()
        {
            InitializeTargetJustAgeAccess();

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);
            int callCount = 0;
            subscriptionTree.PropertyChanged += (sender) => callCount++;

            _person.Name = "Foooadf";
            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void CreateSubscriptionTreeChangePropertyOnParameter_TwoLevelPropertyBeingMonitored_PropertyChanges()
        {
            InitializeTargetBrothersAgeAccess();

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);
            int callCount = 0;
            subscriptionTree.PropertyChanged += (sender) => callCount++;

            _person.Brother = new Person();
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CreateSubscriptionTreeChangeSecondLevelPropertyOnParameter_TwoLevelPropertyBeingMonitored_PropertyChanges()
        {
            InitializeTargetBrothersAgeAccess();

            _person.Brother = new Person();

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);
            int callCount = 0;
            subscriptionTree.PropertyChanged += (sender) => callCount++;

            _person.Brother.Age++;

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CreateSubscriptionTreeChangeSecondLevelPropertyOnParameterAndThenModifyOldProperty_TwoLevelPropertyBeingMonitored_PropertyDoesNotChange()
        {
            InitializeTargetBrothersAgeAccess();

            Person brother = new Person();
            _person.Brother = brother;

            SubscriptionTree subscriptionTree = _target.CreateSubscriptionTree(_person);
            int callCount = 0;
            subscriptionTree.PropertyChanged += (sender) => callCount++;

            _person.Brother = null;

            brother.Age = 1000022;

            Assert.AreEqual(1, callCount);
        }
    }
}
