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
//    public class GroupJoinTest
//    {
//        private ObservableCollection<Person> _source0;
//        private ObservableCollection<Person> _source1;

//        [SetUp]
//        public void Setup()
//        {
//            _source0 = ClinqTestFactory.CreateTwoPersonSource();
//            _source1 = ClinqTestFactory.CreateTwoPersonSource();
//        }


//        private static void AssertAreEquivalent(IEnumerable<IEnumerable<Person>> matchingAgesStandardLinq, ReadOnlyContinuousCollection<ReadOnlyContinuousCollection<Person>> matchingAges)
//        {
//            Assert.AreEqual(matchingAgesStandardLinq.Count(), matchingAges.Count);

//            int i = 0;
//            foreach (var standardLinqGroup in matchingAgesStandardLinq)
//            {
//                CollectionAssert.AreEquivalent(standardLinqGroup, matchingAges[i]);
//                i++;
//            }
//        }

//        [Test]
//        public void SelectManyProjectingSubSequence_Always_SelectsAllValues()
//        {
//            ReadOnlyContinuousCollection<ReadOnlyContinuousCollection<Person>> matchingAges;
//            matchingAges = from person0 in _source0
//                           join person1 in _source1 on person0.Age equals person1.Age into g
//                           select g;

//            IEnumerable<IEnumerable<Person>> matchingAgesStandardLinq = from person0 in _source0.AsEnumerable()
//                                                                        join person1 in _source1 on person0.Age equals person1.Age into g
//                                                                        select g;

//            AssertAreEquivalent(matchingAgesStandardLinq, matchingAges);
//        }
//    }
//}
