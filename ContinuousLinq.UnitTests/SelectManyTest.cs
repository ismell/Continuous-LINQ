//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using System.Collections.ObjectModel;
//using ContinuousLinq;
//using System.Collections.Specialized;

//namespace ContinuousLinq.UnitTests
//{

//    [TestFixture]
//    public class SelectManyReadOnlyContinuousCollectionTest
//    {
//        private ReadOnlyContinuousCollection<Person> _target;
//        private ObservableCollection<ObservableCollection<Person>> _parents;

//        ObservableCollection<Person> _source;

//        [SetUp]
//        public void Setup()
//        {
//            _source = ClinqTestFactory.CreateTwoPersonSourceWithParents();

//            _parents = new ObservableCollection<ObservableCollection<Person>>()
//            {
//                _source[0].Parents,
//                _source[1].Parents
//            };

//            _target = _source.SelectMany(src => src.Parents);
//        }

//        [Test]
//        public void IndexerGet_ItemsInSource_ItemsMatchSelection()
//        {
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("JimParent0", _target[2].Name);
//            Assert.AreEqual("JimParent1", _target[3].Name);
//        }

//        [Test]
//        public void Count_ItemsInSource_IsTotalOfAllSubcollections()
//        {
//            Assert.AreEqual(4, _target.Count);
//        }

//        [Test]
//        public void AddItemToSource_FirstSublist_FireCollectionChangedEvent()
//        {
//            Person newPerson = new Person() { Name = "NewPerson", Age = 5 };
//            int callCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(3, args.NewStartingIndex);
//                Assert.AreEqual(newPerson, args.NewItems[0]);
//            };

//            _parents[0].Add(newPerson);
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(5, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("NewPerson", _target[2].Name);
//            Assert.AreEqual("JimParent0", _target[3].Name);
//            Assert.AreEqual("JimParent1", _target[4].Name);
//        }

//        [Test]
//        public void AddItemToSource_SecondSublist_FireCollectionChangedEvent()
//        {
//            Person newPerson = new Person() { Name = "NewPerson", Age = 5 };
//            int callCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(5, args.NewStartingIndex);
//                Assert.AreEqual(newPerson, args.NewItems[0]);
//            };

//            _parents[1].Add(newPerson);
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(5, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("JimParent0", _target[2].Name);
//            Assert.AreEqual("JimParent1", _target[3].Name);
//            Assert.AreEqual("NewPerson", _target[4].Name);
//        }

//        [Test]
//        public void RemoveItemFromSource_FirstSublist_FireCollectionChangedEvent()
//        {
//            Person personToRemove = _parents[0][1];
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(1, args.OldStartingIndex);
//                Assert.AreEqual(personToRemove, args.OldItems[0]);
//            };

//            _parents[0].Remove(personToRemove);
//            Assert.AreEqual(1, callCount);
//            Assert.AreEqual(3, _target.Count);
//        }

//        [Test]
//        public void RemoveItemFromSource_SecondSublist_FireCollectionChangedEvent()
//        {
//            Person personToRemove = _parents[1][1];
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(4, args.OldStartingIndex);
//                Assert.AreEqual(personToRemove, args.OldItems[0]);
//            };

//            _parents[1].Remove(personToRemove);
//            Assert.AreEqual(1, callCount);
//            Assert.AreEqual(3, _target.Count);
//        }

//        [Test]
//        public void MoveItemInSource_InFirstSublist_FireCollectionChangedEvent()
//        {
//            Person personToMove = _parents[0][0];
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Move, args.Action);
//                Assert.AreEqual(0, args.OldStartingIndex);
//                Assert.AreEqual(1, args.NewStartingIndex);
//                Assert.AreEqual(personToMove, args.OldItems[0]);
//                Assert.AreEqual(personToMove, args.NewItems[0]);
//            };

//            _parents[0].Move(0, 1);
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(4, _target.Count);
//            Assert.AreEqual("BobParent1", _target[0].Name);
//            Assert.AreEqual("BobParent0", _target[1].Name);
//            Assert.AreEqual("JimParent0", _target[2].Name);
//            Assert.AreEqual("JimParent1", _target[3].Name);
//        }

//        [Test]
//        public void MoveItemInSource_InSecond_FireCollectionChangedEvent()
//        {
//            Person personToMove = _parents[1][0];
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Move, args.Action);
//                Assert.AreEqual(2, args.OldStartingIndex);
//                Assert.AreEqual(3, args.NewStartingIndex);
//                Assert.AreEqual(personToMove, args.OldItems[0]);
//                Assert.AreEqual(personToMove, args.NewItems[0]);
//            };

//            _parents[1].Move(0, 1);
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(4, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("JimParent1", _target[2].Name);
//            Assert.AreEqual("JimParent0", _target[3].Name);
//        }

//        [Test]
//        public void ReplaceItemInSource_FirstCollection_FireCollectionChangedEvent()
//        {
//            Person replacement = new Person("Replacement", 7);
//            Person oldValue = _parents[0][1];

//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Replace, args.Action);
//                Assert.AreEqual(1, args.OldStartingIndex);
//                Assert.AreEqual(oldValue, args.OldItems[0]);
//                Assert.AreEqual(replacement, args.NewItems[0]);
//            };

//            _parents[0][1] = replacement;

//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(4, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("Replacement", _target[1].Name);
//            Assert.AreEqual("JimParent0", _target[2].Name);
//            Assert.AreEqual("JimParent1", _target[3].Name);
//        }

//        [Test]
//        public void ReplaceItemInSource_SecondCollection_FireCollectionChangedEvent()
//        {
//            Person replacement = new Person("Replacement", 7);
//            Person oldValue = _parents[1][0];

//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Replace, args.Action);
//                Assert.AreEqual(2, args.OldStartingIndex);
//                Assert.AreEqual(oldValue, args.OldItems[0]);
//                Assert.AreEqual(replacement, args.NewItems[0]);
//            };

//            _parents[1][0] = replacement;

//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(4, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("Replacement", _target[2].Name);
//            Assert.AreEqual("JimParent1", _target[3].Name);

//        }

//        [Test]
//        public void ResetSource_FirstSublist_FireCollectionChangedEvent()
//        {
//            List<Person> oldValues = new List<Person>(_parents[0]);
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(0, args.OldStartingIndex);
//                CollectionAssert.AreEquivalent(oldValues, args.OldItems);
//            };

//            _parents[0].Clear();
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(2, _target.Count);
//        }

//        [Test]
//        public void ResetSource_SecondSublist_FireCollectionChangedEvent()
//        {
//            List<Person> oldValues = new List<Person>(_parents[1]);
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(2, args.OldStartingIndex);
//                CollectionAssert.AreEquivalent(oldValues, args.OldItems);
//            };

//            _parents[1].Clear();
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(2, _target.Count);
//        }

//        [Test]
//        public void AddNewSublistToSource_Always_FireCollectionChangedEvent()
//        {
//            Person newPerson = new Person("Ninja", 23);
//            ClinqTestFactory.InitializeParents(newPerson);

//            int callCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(2, args.NewStartingIndex);
//                CollectionAssert.AreEquivalent(newPerson.Parents, args.NewItems);
//            };

//            _source.Insert(1, newPerson);
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(6, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("NinjaParent0", _target[2].Name);
//            Assert.AreEqual("NinjaParent1", _target[3].Name);
//            Assert.AreEqual("JimParent0", _target[4].Name);
//            Assert.AreEqual("JimParent1", _target[5].Name);
//        }

//        [Test]
//        public void AddNewSublistToSource_SublistIsNull_DoesNotFireCollectionChangedEvent()
//        {
//            Person newPerson = new Person("Ninja", 23);

//            int callCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//            };

//            _source.Insert(0, newPerson);
//            Assert.AreEqual(0, callCount);

//            Assert.AreEqual(4, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//            Assert.AreEqual("JimParent0", _target[2].Name);
//            Assert.AreEqual("JimParent1", _target[3].Name);
//        }

//        [Test]
//        public void RemoveSublistFromSource_Always_FireCollectionChangedEvent()
//        {
//            int callCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(2, args.OldStartingIndex);
//                CollectionAssert.AreEquivalent(_parents[1], args.OldItems);
//            };

//            _source.RemoveAt(1);

//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(2, _target.Count);
//            Assert.AreEqual("BobParent0", _target[0].Name);
//            Assert.AreEqual("BobParent1", _target[1].Name);
//        }

//        [Test]
//        public void ReplaceItemInSource_Always_FireCollectionChangedEvent()
//        {
//            Person replacement = new Person("Ninja", 23);
//            ClinqTestFactory.InitializeParents(replacement);

//            int removeCallCount = 0;
//            int addCallCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                if (args.Action != NotifyCollectionChangedAction.Remove)
//                    return;

//                Assert.AreEqual(0, addCallCount);
//                removeCallCount++;

//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(2, args.OldStartingIndex);
//                CollectionAssert.AreEquivalent(_parents[1], args.OldItems);

//                Assert.AreEqual(2, _target.Count);
//                Assert.AreEqual("BobParent0", _target[0].Name);
//                Assert.AreEqual("BobParent1", _target[1].Name);
//            };

//            _target.CollectionChanged += (sender, args) =>
//            {
//                if (args.Action != NotifyCollectionChangedAction.Add)
//                    return;

//                Assert.AreEqual(1, removeCallCount);
//                addCallCount++;

//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(2, args.NewStartingIndex);
//                CollectionAssert.AreEquivalent(replacement.Parents, args.NewItems);

//                Assert.AreEqual(4, _target.Count);
//                Assert.AreEqual("BobParent0", _target[0].Name);
//                Assert.AreEqual("BobParent1", _target[1].Name);
//                Assert.AreEqual("NinjaParent0", _target[1].Name);
//                Assert.AreEqual("NinjaParent1", _target[2].Name);
//            };

//            _source[1] = replacement;

//            Assert.AreEqual(1, removeCallCount);
//            Assert.AreEqual(1, addCallCount);
//        }

//        [Test]
//        public void ReplaceExistingSublist_Always_FireCollectionChangedEvent()
//        {
//            var replacementParents = new ObservableCollection<Person>()
//            {
//                new Person("ReplacementParent0", 42),
//                new Person("ReplacementParent0", 43),
//            };

//            int removeCallCount = 0;
//            int addCallCount = 0;

//            _target.CollectionChanged += (sender, args) =>
//            {
//                if (args.Action != NotifyCollectionChangedAction.Remove)
//                    return;

//                Assert.AreEqual(0, addCallCount);
//                removeCallCount++;

//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(2, args.OldStartingIndex);
//                CollectionAssert.AreEquivalent(_parents[0], args.OldItems);

//                Assert.AreEqual(2, _target.Count);
//                Assert.AreEqual("JimParent0", _target[0].Name);
//                Assert.AreEqual("JimParent1", _target[1].Name);
//            };

//            _target.CollectionChanged += (sender, args) =>
//            {
//                if (args.Action != NotifyCollectionChangedAction.Add)
//                    return;

//                Assert.AreEqual(1, removeCallCount);
//                addCallCount++;

//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(2, args.NewStartingIndex);
//                CollectionAssert.AreEquivalent(replacementParents, args.NewItems);

//                Assert.AreEqual(4, _target.Count);
//                Assert.AreEqual("ReplacementParent0", _target[0].Name);
//                Assert.AreEqual("ReplacementParent0", _target[1].Name);
//                Assert.AreEqual("JimParent0", _target[1].Name);
//                Assert.AreEqual("JimParent1", _target[2].Name);
//            };

//            _source[0].Parents = replacementParents;

//            Assert.AreEqual(1, removeCallCount);
//            Assert.AreEqual(1, addCallCount);
//        }

//        [Test]
//        public void ReplaceExistingSublist_SublistIsNull_FireCollectionChangedEvent()
//        {
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;

//                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
//                Assert.AreEqual(0, args.OldStartingIndex);
//                CollectionAssert.AreEquivalent(_parents[1], args.OldItems);
//            };

//            _source[0].Parents = null;

//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(2, _target.Count);
//            Assert.AreEqual("JimParent0", _target[0].Name);
//            Assert.AreEqual("JimParent1", _target[1].Name);
//        }

//        [Test]
//        public void ResetSource_Always_FireCollectionChangedEvent()
//        {
//            int callCount = 0;
//            _target.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Reset, args.Action);
//            };

//            _source.Clear();
//            Assert.AreEqual(1, callCount);

//            Assert.AreEqual(0, _target.Count);
//        }
//    }
//}
