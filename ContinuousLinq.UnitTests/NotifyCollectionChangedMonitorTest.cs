using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class NotifyCollectionChangedMonitorTest
    {
        private NotifyCollectionChangedMonitor<Person> _target;
        private ObservableCollection<Person> _source;

        private PropertyAccessTree _propertyAccessTree;
        
        [SetUp]
        public void Setup()
        {
            _source = ClinqTestFactory.CreateTwoPersonSource();

            _propertyAccessTree = new PropertyAccessTree();
            ParameterNode parameterNode = new ParameterNode();
            _propertyAccessTree.Children.Add(parameterNode);

            var agePropertyAccessNode = new PropertyAccessNode(typeof(Person).GetProperty("Age"));
            parameterNode.Children.Add(agePropertyAccessNode);

            _target = new NotifyCollectionChangedMonitor<Person>(_propertyAccessTree, _source);
        }

        [Test]
        public void AddToSource_SingleItem_FiresAdd()
        {
            int callCount = 0;
            _target.Add += (index, items) =>
            {
                callCount++;
                Assert.AreEqual(2, index);
                Assert.IsNotNull(items);
            };

            Person newPerson = new Person("New", 100);
            _source.Add(newPerson);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void RemoveFromSource_SingleItem_FiresRemove()
        {
            int callCount = 0;
            _target.Remove += (index, items) =>
            {
                callCount++;
                Assert.AreEqual(1, index);
                Assert.IsNotNull(items);
            };

            _source.RemoveAt(1);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ReplaceInSource_SingleItem_FiresReplace()
        {
            int callCount = 0;
            _target.Replace += (oldIndex, oldItems, newIndex, newItems) =>
            {
                callCount++;
                Assert.AreEqual(0, oldIndex);
                Assert.IsNotNull(oldItems);
                Assert.AreEqual(1, oldItems.Count());
                
                Assert.AreEqual(0, newIndex);
                Assert.IsNotNull(newItems);
                Assert.AreEqual(1, newItems.Count());
            };

            _source[0] = new Person();
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void MoveInSource_SingleItem_FiresMove()
        {
            int callCount = 0;
            _target.Move += (oldIndex, oldItems, newIndex, newItems) =>
            {
                callCount++;
                Assert.AreEqual(0, oldIndex);
                Assert.IsNotNull(oldItems);
                Assert.AreEqual(1, oldItems.Count());

                Assert.AreEqual(1, newIndex);
                Assert.IsNotNull(newItems);
                Assert.AreEqual(1, newItems.Count());
            };

            _source.Move(0, 1);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void InsertInSource_SingleItem_FiresReplace()
        {
            int callCount = 0;
            _target.Add += (index, items) =>
            {
                callCount++;
            };

            _source.Insert(0, new Person());
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ClearSource_SingleItem_FiresReset()
        {
            int callCount = 0; 
            _target.Reset += () =>
            {
                callCount++;
            };

            _source.Clear();
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void SetPropertyOnItemInSourceCollection_PropertyDifferent_FiresItemChangedEvent()
        {
            int callCount = 0;
            _target.ItemChanged += (item) =>
            {
                callCount++;
                Assert.AreSame(_source[0], item);
            };

            _source[0].Age = 1000;
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void RemoveFromSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0]; 
            
            _target.ItemChanged += (item) => Assert.Fail();

            _source.Remove(person);
            person.Age = 1000;
        }

        [Test]
        public void ReplaceItemInSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            _target.ItemChanged += (item) => Assert.Fail();

            _source[0] = new Person();
            person.Age = 1000;
        }

        [Test]
        public void ReplaceItemInSourceAndChangePropertyOnNewItem_SingleItem_ItemChangedFired()
        {
            int callCount = 0;
            _target.ItemChanged += (item) => callCount++;

            Person newPerson = new Person();
            
            _source[0] = newPerson;
            newPerson.Age = 1000;

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void MoveItemInSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            int callCount = 0;
            _target.ItemChanged += (item) => callCount++;

            _source.Move(0, 1);
            person.Age = 1000;
            
            Assert.AreEqual(1, callCount);
        }

        [Test]
        //[Ignore("Need to modify own PropertyChangedEventManager to track subscriptions")]
        public void ClearSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            _target.ItemChanged += (item) => Assert.Fail();

            _source.Clear();
            person.Age = 1000;
        }
    }
}
