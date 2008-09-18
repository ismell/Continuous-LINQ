using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class SortTest
    {
        private ObservableCollection<Person> _source;

        private void ResetAges()
        {
            for (int x = 0; x < _source.Count; x++)
            {
                _source[x].Age = x * 10;
            }
        }

        private void ResetAgesForThenBy()
        {
            ResetAges();
            _source[1].Age = 20; // same as _source[2].age
            _source[2].Age = 20;
            _source[1].Name = "Zoolander";
            _source[2].Name = "Alfonse";
        }

        [SetUp]
        public void SetUp()
        {
            _source = ClinqTestFactory.CreateSixPersonSource();
        }

        [Test]
        public void Sort_CountRemainsTheSame()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age
                select person;

            Assert.AreEqual(_source.Count, output.Count);
        }

        [Test]
        public void Sort_SortedListRemainsSorted()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age
                select person;

            // source list is already pre-sorted by age, verify that it
            // stayed that way.
            for (int x = 0; x < _source.Count; x++)
                Assert.AreEqual(_source[x].Age, output[x].Age);
        }

        [Test]
        public void Sort_HeadBecomesTail()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age
                select person;

            _source[0].Age = 99; //move the first to the last.

            Assert.AreEqual(_source[0].Age, output[output.Count - 1].Age);
        }

        [Test]
        public void Sort_TailBecomesHead()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age
                select person;

            _source[_source.Count - 1].Age = -1;

            Assert.AreEqual(_source[_source.Count - 1].Age, output[0].Age);

        }

        [Test]
        public void Sort_Descending_CountRemainsTheSame()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age descending
                select person;

            Assert.AreEqual(_source.Count, output.Count);
        }

        [Test]
        public void Sort_Descending_HeadBecomesTail()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age descending
                select person;

            _source[0].Age = -1; // reversed direction, set to -1 should put it last.

            Assert.AreEqual(_source[0].Age, output[output.Count - 1].Age);
        }

        [Test]
        public void Sort_Descending_TailBecomesHead()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age descending
                select person;

            _source[_source.Count - 1].Age = 99; // reversed direction, set to 99 should put it first.
            Assert.AreEqual(_source[_source.Count - 1].Age, output[0].Age);
        }

        [Test]
        public void Sort_Descending_SortedItemsAreInProperIndexes()
        {
            ResetAges();
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age descending
                select person;
            
            int outputIdx = output.Count - 1;

            for (int sourceIdx = 0; sourceIdx < _source.Count; sourceIdx++)
            {
                Assert.AreEqual(_source[sourceIdx].Age, output[outputIdx].Age);
                outputIdx--;
            }
        }

        [Test]
        public void Sort_ThenBy_CountRemainsTheSame()
        {
            ResetAgesForThenBy();

            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age, person.Name
                select person;

            Assert.AreEqual(_source.Count, output.Count);
        }

        [Test]
        public void Sort_ThenBy_SortedItemsAreInProperIndexes()
        {
            ResetAgesForThenBy();
           
            ReadOnlyContinuousCollection<Person> output =
                from person in _source
                orderby person.Age ascending, person.Name ascending
                select person;

            Assert.AreEqual(output[0].Age, 0);
            Assert.AreEqual(output[1].Age, 20);
            Assert.AreEqual(output[2].Age, 20);
            // output IDX 1 should be alfonse
            // output IDX 2 should be zoolander
            /*Assert.AreEqual(output[1].Age, output[2].Age); // should both be 20
            Assert.AreEqual(output[1].Name.ToLower(), "alfonse");
            Assert.AreEqual(output[2].Name.ToLower(), "zoolander"); */
        }

        [Test]
        public void Test()
        {

            ResetAgesForThenBy();
           
            IEnumerable<Person> list = _source;

            var output = from person in list
                         orderby person.Age, person.Name
                         select person;

            Assert.AreEqual(_source.Count, output.Count());
        }

        [Test]
        public void Sort_ThenBy_HeadBecomesTail()
        {
        }

        [Test]
        public void Sort_ThenBy_TailBecomesHead()
        {
        }

        [Test]
        public void Sort_ThenBy_Descending_CountRemainsTheSame()
        {
        }

        [Test]
        public void Sort_ThenBy_Descending_SortedItemsAreInProperIndexes()
        {
        }

        [Test]
        public void Sort_ThenBy_Descending_HeadBecomesTail()
        {
        }

        [Test]
        public void Sort_ThenBy_Descending_TailBecomesHead()
        {
        }
    }
}
