using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ContinuousLinq.Collections;
using NUnit.Framework;
using System.Linq;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class ExceptReadOnlyContinuousCollectionTest
    {
        private ExceptReadOnlyContinuousCollection<Person> _target;
        private ObservableCollection<Person> _first;
        private ObservableCollection<Person> _second;

        private Person _person1;
        private Person _person2;

        [SetUp]
        public void Setup()
        {
            _first = ClinqTestFactory.CreateTwoPersonSource();
            _second = new ObservableCollection<Person>();
            
            _target = new ExceptReadOnlyContinuousCollection<Person>(_first, _second);

            _person1 = _first[0];
            _person2 = _first[1];
        }

        [Test]
        public void Construct_BothListHaveSameElements_OutputIsEmpty()
        {
            _second = new ObservableCollection<Person>(_first);
            _target = new ExceptReadOnlyContinuousCollection<Person>(_first, _second);

            Assert.AreEqual(0, _target.Count);
        }

        [Test]
        public void Construct_SecondListIsEmpty_OutputIsFirstList()
        {
            Assert.AreEqual(2, _target.Count);
        }

        [Test]
        public void AddItemToFirst_IsNotInSecond_WillBeAddedToOutput()
        {
            var newPerson = new Person { Name = "Frank" };
            _first.Add(newPerson);

            Assert.AreEqual(3, _target.Count);
            Assert.IsTrue(_target.Contains(newPerson));
        }

        [Test]
        public void RemoveItemFromFirst_IsNotInSecond_WillBeRemovedFromOutput()
        {
            var newPerson = new Person { Name = "Frank" };
            _first.Add(newPerson);

            _first.Remove(newPerson);
            Assert.AreEqual(2, _target.Count);
            Assert.IsFalse(_target.Contains(newPerson));
        }

        [Test]
        public void AddItemToSecond_IsInFirst_WillBeRemovedFromOutput()
        {
            Assert.AreEqual(2, _target.Count);
            Assert.IsTrue(_target.Contains(_person1));

            _second.Add(_person1);

            Assert.AreEqual(1, _target.Count);
            Assert.IsFalse(_target.Contains(_person1));
        }

        [Test]
        public void RemoveItemFromSecond_IsInFirst_WillBeAddedToOutput()
        {
            _second.Add(_person1);

            _second.Remove(_person1);

            Assert.AreEqual(2, _target.Count);
            Assert.IsTrue(_target.Contains(_person1));
        }

        [Test]
        public void AddItemToFirst_ItemIsDuplicateAndItemNotInSecond_ShouldAppearInOutputOnlyOnce()
        {
            _first.Add(_person1);

            Assert.AreEqual(2, _target.Count);
        }

        [Test]
        public void RemoveItemFromFirst_ItemIsDuplicateAndItemNotInSecond_ShouldAppearInOutputOnlyOnce()
        {
            _first.Add(_person1);
            
            _first.Remove(_person1);

            Assert.AreEqual(2, _target.Count);
        }

        [Test]
        public void AddItemToSecond_ItemIsDuplicateAndItemIsInFirst_ShouldNotAppearInOutput()
        {
            _second.Add(_person1);
            _second.Add(_person1);

            Assert.AreEqual(1, _target.Count);
            Assert.IsFalse(_target.Contains(_person1));
        }

        [Test]
        public void RemoveItemFromSecond_ItemIsDuplicateAndItemIsInFirst_ShowsOutputOnlyWhenAllDuplicatesRemoved()
        {
            _second.Add(_person1);
            _second.Add(_person1);

            _second.Remove(_person1);

            Assert.AreEqual(1, _target.Count);
            Assert.IsFalse(_target.Contains(_person1));

            _second.Remove(_person1);

            Assert.AreEqual(2, _target.Count);
            Assert.IsTrue(_target.Contains(_person1));
        }

        [Test]
        public void RemoveItemFromSecond_IsInFirstTwice_ShouldAppearInOutputOnlyOnce()
        {
            _first.Add(_person1);
            
            _second.Add(_person1);
            _second.Remove(_person1);

            Assert.AreEqual(2, _target.Count);
        }

        [Test]
        public void ReplaceItemInFirst_NewItemIsInSecond_NeitherAppearsInOutput()
        {
            _second.Add(_person2);

            _first[0] = _person2;

            Assert.AreEqual(0, _target.Count);
        }

        [Test]
        public void ReplaceItemInFirst_NewItemIsNotInSecond_ShouldShowOnlySecond()
        {
            _first[0] = _person2;

            Assert.AreEqual(1, _target.Count);
            Assert.IsTrue(_target.Contains(_person2));
            Assert.IsFalse(_target.Contains(_person1));
        }

        [Test]
        public void ReplaceItemInFirst_RemovedItemWasDuplicateAndNewItemIsNotInSecond_RemovedAndNewItemAppearInOutput()
        {
            _first[1] = _person1;

            _first[1] = _person2;

            Assert.AreEqual(2, _target.Count);
            Assert.IsTrue(_target.Contains(_person1));
            Assert.IsTrue(_target.Contains(_person2));
        }

        [Test]
        public void ReplaceItemInFirst_RemovedItemWasDuplicateAndNewItemIsInSecond_ShouldOnlyShowRemovedItem()
        {
            _second.Add(_person2);
            _first[1] = _person1;

            _first[1] = _person2;

            Assert.AreEqual(1, _target.Count);
            Assert.IsTrue(_target.Contains(_person1));
            Assert.IsFalse(_target.Contains(_person2));
        }

        [Test]
        public void ReplaceItemInSecond_OldItemIsInFirstAndNewItemIsInFirst_OldItemIsInOuputAndNewItemIsNot()
        {
            _second.Add(_person2);

            _second[0] = _person1;

            Assert.AreEqual(1, _target.Count);
            Assert.IsTrue(_target.Contains(_person2));
            Assert.IsFalse(_target.Contains(_person1));
        }

        [Test]
        public void ReplaceItemOnSecond_OldItemWasDuplicateAndNewItemIsInFirst_NeitherIsInOutput()
        {
            _second.Add(_person2);
            _second.Add(_person2);

            _second[1] = _person1;

            Assert.AreEqual(0, _target.Count);
        }

        [Test]
        public void ReplaceItemOnSecond_NeitherOldOrNewIsInFirst_BothAreInOutput()
        {
            _second.Add(new Person());

            _second[0] = new Person();

            Assert.AreEqual(2, _target.Count);
            Assert.IsTrue(_target.Contains(_person1));
            Assert.IsTrue(_target.Contains(_person2));
        }

        [Test]
        public void ResetFirst_Always_LeavesOutputIntact()
        {
            var people = ClinqTestFactory.CreateSixPersonSource();

            var first = new TestContinuousCollection<Person>(people.ToList());
            var second = new TestContinuousCollection<Person> {people[0]};
            _target = new ExceptReadOnlyContinuousCollection<Person>(first, second);

            first.FireReset();

            Assert.AreEqual(5, _target.Count);
            CollectionAssert.AreEquivalent(people.Skip(1), _target.Output);
        }

        [Test]
        public void ResetSecond_Always_LeavesOutputIntact()
        {
            var people = ClinqTestFactory.CreateSixPersonSource();

            var first = new TestContinuousCollection<Person>(people.ToList());
            var second = new TestContinuousCollection<Person> { people[0] };
            _target = new ExceptReadOnlyContinuousCollection<Person>(first, second);

            second.FireReset();

            Assert.AreEqual(5, _target.Count);
            CollectionAssert.AreEquivalent(people.Skip(1), _target.Output);
        }

        [Test]
        public void ResetOnFirst_Always_RaisesNotifyCollectionChangedWithResetAction()
        {
            var people = ClinqTestFactory.CreateSixPersonSource();

            var first = new TestContinuousCollection<Person>(people.ToList());
            var second = new TestContinuousCollection<Person> { people[0] };
            _target = new ExceptReadOnlyContinuousCollection<Person>(first, second);

            var eventArgsList = new List<NotifyCollectionChangedEventArgs>();
            _target.CollectionChanged += (sender, e) => eventArgsList.Add(e);

            first.FireReset();

            Assert.AreEqual(1, eventArgsList.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
        }

        [Test]
        public void ResetOnSecond_Always_RaisesNotifyCollectionChangedWithResetAction()
        {
            var people = ClinqTestFactory.CreateSixPersonSource();

            var first = new TestContinuousCollection<Person>(people.ToList());
            var second = new TestContinuousCollection<Person> { people[0] };
            _target = new ExceptReadOnlyContinuousCollection<Person>(first, second);

            var eventArgsList = new List<NotifyCollectionChangedEventArgs>();
            _target.CollectionChanged += (sender, e) => eventArgsList.Add(e);

            second.FireReset();

            Assert.AreEqual(1, eventArgsList.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
        }

        [Test]
        public void ClearOnFirst_Always_MakesOutputEmpty()
        {
            _first.Clear();

            Assert.AreEqual(0, _target.Count);
        }

        [Test]
        public void ClearOnSecond_Always_MakesOutputEqualToFirst()
        {
            _second.Add(_person1);
            _second.Add(_person2);

            _second.Clear();

            CollectionAssert.AreEquivalent(_first, _target.Output);
        }

        [Test]
        public void ClearOnFirst_Always_RaisesNotifyCollectionChangedWithResetAction()
        {
            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _first.Clear();

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                                                  eventArgsList[0]);
        }

        [Test]
        public void ClearOnSecond_Always_RaisesNotifyCollectionChangedWithResetAction()
        {
            _second.Add(_person1);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _second.Clear();

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                                                eventArgsList[0]);
        }

        [Test]
        public void AddToFirst_Always_RaisesNotifyCollectionChangedWithAddActionAndCorrectValues()
        {
            var person = new Person();

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _first.Add(person);

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[]{person}, 2),
                                                eventArgsList[0]);
        }

        [Test]
        public void AddRangeToFirst_Always_RaisesNotifyCollectionChangedWithAddActionAndCorrectValues()
        {
            var continuousFirstCollection = new ContinuousCollection<Person>(_first.ToList());
            _target = new ExceptReadOnlyContinuousCollection<Person>(continuousFirstCollection, _second);

            var people = new List<Person> { new Person(), new Person() };

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            continuousFirstCollection.AddRange(people);

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, people, 2),
                                                eventArgsList[0]);
        }

        [Test]
        public void RemoveFromFirst_Always_RaisesNotifyCollectionChangedWithRemoveActionAndCorrectValues()
        {
            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _first.Remove(_person1);

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { _person1 }, 0),
                                                eventArgsList[0]);
        }

        [Test]
        public void RemoveRangeFromFirst_Always_RaisesNotifyCollectionChangedWithRemoveActionAndCorrectValuesForEachItemRemoved()
        {
            var continuousFirstCollection = new ContinuousCollection<Person>(_first.ToList());
            _target = new ExceptReadOnlyContinuousCollection<Person>(continuousFirstCollection, _second);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            continuousFirstCollection.RemoveRange(0, 2);

            Assert.AreEqual(2, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { _person1 }, 0),
                                                 eventArgsList[0]);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { _person2 }, 0),
                                                 eventArgsList[1]);
        }

        [Test]
        public void ReplaceOnFirst_Always_RaisesNotifyCollectionChangedWithAddAndRemoveActionsAndCorrectValues()
        {
            var person = new Person();

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _first[0] = person;

            Assert.AreEqual(2, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { _person1 }, 0),
                                                  eventArgsList[0]);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { person }, 1),
                                                  eventArgsList[1]);
        }

        [Test]
        public void ReplaceRangeOnFirst_Always_RaisesNotifyCollectionChangedWithRemoveActionsForEachItemReplacedAndOneAdd()
        {
            var continuousFirstCollection = new ContinuousCollection<Person>(_first.ToList());
            _target = new ExceptReadOnlyContinuousCollection<Person>(continuousFirstCollection, _second);

            var people = new List<Person> { new Person(), new Person() };

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            continuousFirstCollection.ReplaceRange(0, people);

            Assert.AreEqual(3, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[]{_person1}, 0),
                                                  eventArgsList[0]);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { _person2 }, 0),
                                                  eventArgsList[1]);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, people, 0),
                                                  eventArgsList[2]);
        }

        [Test]
        public void AddToSecond_Always_RaisesNotifyCollectionChangedWithRemoveActionAndCorrectValues()
        {
            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _second.Add(_person1);

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[]{_person1}, 0),
                                                  eventArgsList[0]);
        }

        [Test]
        public void AddRangeToSecond_Always_RaisesNotifyCollectionChangedWithRemoveActionAndCorrectValuesForEachItemRemoved()
        {
            var continuousSecondCollection = new ContinuousCollection<Person>();
            _target = new ExceptReadOnlyContinuousCollection<Person>(_first, continuousSecondCollection);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            continuousSecondCollection.AddRange(_first);

            Assert.AreEqual(2, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,new[]{_person1}, 0),
                                                  eventArgsList[0]);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { _person2 }, 0),
                                                  eventArgsList[1]);
        }

        [Test]
        public void RemoveFromSecond_Always_RaisesNotifyCollectionChangedWithAddActionAndCorrectValues()
        {
            _second.Add(_person1);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _second.Remove(_person1);

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[]{_person1}, 1),
                                                  eventArgsList[0]);
        }

        [Test]
        public void RemoveRangeFromFirst_Always_RaisesNotifyCollectionChangedWithAddActionAndCorrectValues2()
        {
            var continuousSecondCollection = new ContinuousCollection<Person>(_first.ToList());
            _target = new ExceptReadOnlyContinuousCollection<Person>(_first, continuousSecondCollection);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            continuousSecondCollection.RemoveRange(0, 2);

            Assert.AreEqual(1, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _first, 0), 
                                                  eventArgsList[0]);
        }

        [Test]
        public void ReplaceOnSecond_Always_RaisesNotifyCollectionChangedWithAddAndRemoveActionsAndCorrectValues()
        {
            _second.Add(_person1);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            _second[0] = _person2;

            Assert.AreEqual(2, eventArgsList.Count);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[]{_person1}, 1), 
                                                  eventArgsList[0]);

            AssertCollectionChangedEventArgsEqual(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] {_person2}, 0), 
                                                  eventArgsList[1]);
        }

        [Test]
        public void ReplaceRangeOnSecond_FirstContainsNewItems_RaisesNotifyCollectionChangedWithOneAddAndRemoveActionsForEachItemReplacedRemoved2()
        {
            var people = new List<Person> { new Person(), new Person() };

            _first = new ObservableCollection<Person> { _person1, _person2, people[0], people[1] };
            var continuousSecondCollection = new ContinuousCollection<Person> { _person1, _person2 };

            _target = new ExceptReadOnlyContinuousCollection<Person>(_first, continuousSecondCollection);

            var eventArgsList = GetCollectionChangedEventArgsList(_target);

            continuousSecondCollection.ReplaceRange(0, people);

            Assert.AreEqual(3, eventArgsList.Count);

            var expectedEventArg1 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { _person1, _person2 }, 2);
            AssertCollectionChangedEventArgsEqual(expectedEventArg1, eventArgsList[0]);

            var expectedEventArg2 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] {people[0]}, 0);
            AssertCollectionChangedEventArgsEqual(expectedEventArg2, eventArgsList[1]);

            var expectedEventArg3 = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { people[1] }, 0);
            AssertCollectionChangedEventArgsEqual(expectedEventArg3, eventArgsList[2]);
        }

        private static List<NotifyCollectionChangedEventArgs> GetCollectionChangedEventArgsList(INotifyCollectionChanged target)
        {
            var eventArgsList = new List<NotifyCollectionChangedEventArgs>();
            target.CollectionChanged += (sender, e) => eventArgsList.Add(e);
            return eventArgsList;
        }

        private static void AssertCollectionChangedEventArgsEqual(NotifyCollectionChangedEventArgs expectedEventArgs, NotifyCollectionChangedEventArgs actualEventArgs)
        {
            Assert.AreEqual(expectedEventArgs.Action, actualEventArgs.Action);
            
            if (actualEventArgs.NewItems == null)
                Assert.IsNull(expectedEventArgs.NewItems);
            else
                CollectionAssert.AreEquivalent(expectedEventArgs.NewItems, actualEventArgs.NewItems);

            Assert.AreEqual(expectedEventArgs.NewStartingIndex, actualEventArgs.NewStartingIndex);

            if (actualEventArgs.OldItems == null)
                Assert.IsNull(expectedEventArgs.OldItems);
            else
                CollectionAssert.AreEquivalent(expectedEventArgs.OldItems, actualEventArgs.OldItems);

            Assert.AreEqual(expectedEventArgs.OldStartingIndex, actualEventArgs.OldStartingIndex);
        }
    }

    public class TestContinuousCollection<T> : ContinuousCollection<T>
    {
        public TestContinuousCollection()
        {
        }

        public TestContinuousCollection(List<T> list) : base(list)
        {
        }

        public void FireReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}