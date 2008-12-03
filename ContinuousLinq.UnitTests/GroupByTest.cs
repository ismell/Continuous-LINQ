using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ContinuousLinq;
using ContinuousLinq.Collections;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class GroupByTest
    {
        private ObservableCollection<Person> _source;

        [SetUp]
        public void Setup()
        {
            _source = ClinqTestFactory.CreateGroupablePersonSource();
        }

        [Test]
        public void GroupBy_DotSyntax_CountGroups()
        {
            GroupingReadOnlyContinuousCollection<int, Person> liveGroup =
                _source.GroupBy(p => p.Age);

            Assert.AreEqual(10, liveGroup.Count);
            Assert.AreEqual(6, liveGroup[5].Count);

        }

        [Test]
        public void GroupBy_CountGroups()
        {
            GroupingReadOnlyContinuousCollection<int, Person> liveGroup =
                from p in _source
                group p by p.Age;

            Assert.AreEqual(10, liveGroup.Count);
            Assert.AreEqual(6, liveGroup[5].Count);

        }

        [Test]
        public void GroupBy_CountGroups_AfterChange()
        {
            GroupingReadOnlyContinuousCollection<int, Person> liveGroup =
                from p in _source
                group p by p.Age;

            // now change one of the ages to create an 11th group.
            // change in the source, not liveGroup.

            _source[0].Age = 999;

            Assert.AreEqual(11, liveGroup.Count);
            Assert.AreEqual(1, liveGroup[liveGroup.Count - 1].Count);
        }
    }
}
