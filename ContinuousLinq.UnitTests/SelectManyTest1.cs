//using System;
//using System.Linq;
//using NUnit.Framework;
//using System.Collections.ObjectModel;
//using System.Collections.Generic;
//using ContinuousLinq;
//using System.Collections.Specialized;

//namespace ContinuousLinq.UnitTests
//{
//    [TestFixture]
//    public class SelectManyTest
//    {
//        private ObservableCollection<Person> _source;
//        //private ObservableCollection<Person> _source1;

//        [SetUp]
//        public void Setup()
//        {
//            _source = ClinqTestFactory.CreateTwoPersonSource();

//            int parentNumber = 0;
//            foreach (var person in _source)
//            {
//                person.Parents = new ObservableCollection<Person>() 
//              {
//                  new Person("Parent: " + parentNumber, 40 + parentNumber++), 
//                  new Person("Parent: " + parentNumber, 40 + parentNumber++),
//              };
//            }

//            //_source1 = ClinqTestFactory.CreateTwoPersonSource();
//        }

//        private ReadOnlyContinuousCollection<int> QueryAllAgesOfParents()
//        {
//            //ReadOnlyContinuousCollection<int> agesOfAllParents = from person in _source
//            //                                                     from parent in person.Parents
//            //                                                     select parent.Age;
//            //return agesOfAllParents;
//            throw new NotImplementedException();
//        }

//        [Test]
//        public void SelectManyProjectingSubSequence_Always_SelectsAllValues()
//        {
//            ReadOnlyContinuousCollection<int> agesOfAllParents = QueryAllAgesOfParents();

//            IEnumerable<int> agesOfAllStandardLinq = from person in _source.AsEnumerable()
//                                                     from parent in person.Parents
//                                                     select parent.Age;

//            CollectionAssert.AreEquivalent(agesOfAllStandardLinq, agesOfAllParents);
//        }

//        [Test]
//        public void SelectManyProjectingSubSequence_AddNewItemToFirstSubList_NotifiesCollectionChanged()
//        {
//            ReadOnlyContinuousCollection<int> agesOfAllParents = QueryAllAgesOfParents();
//            Person newParent = new Person("NewParent", 123);

//            int callCount = 0;
//            agesOfAllParents.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(2, args.NewStartingIndex);
//                Assert.AreEqual(-1, args.OldStartingIndex);
//                CollectionAssert.Contains(args.NewItems, newParent);
//                Assert.AreEqual(1, args.NewItems.Count);
//            };

//            _source[0].Parents.Add(newParent);

//            Assert.AreEqual(newParent, _source[2]);
//            Assert.AreEqual(1, callCount);
//        }

//        [Test]
//        public void SelectManyProjectingSubSequence_AddNewItemToSecondSubList_NotifiesCollectionChanged()
//        {
//            ReadOnlyContinuousCollection<int> agesOfAllParents = QueryAllAgesOfParents();
//            Person newParent = new Person("NewParent", 123);

//            int callCount = 0;
//            agesOfAllParents.CollectionChanged += (sender, args) =>
//            {
//                callCount++;
//                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
//                Assert.AreEqual(3, args.NewStartingIndex);
//                Assert.AreEqual(-1, args.OldStartingIndex);
//                CollectionAssert.Contains(args.NewItems, newParent);
//                Assert.AreEqual(1, args.NewItems.Count);
//            };

//            _source[1].Parents.Insert(0, newParent);

//            Assert.AreEqual(newParent, _source[3]);
//            Assert.AreEqual(1, callCount);
//        }
//    }
//}