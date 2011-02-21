using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class CollectionMonitorTest
    {
        private CollectionMonitor<Person> _target;
        private ObservableCollection<Person> _source;
        
        [SetUp]
        public void Setup()
        {
            _source = ClinqTestFactory.CreateTwoPersonSource();

            _target = new CollectionMonitor<Person>(_source);
        }

        [Test]
        public void AddToSource_SingleItem_FiresAdd()
        {
            int callCount = 0;
            _target.Add += (sender, index, items) =>
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
            _target.Remove += (sender, index, items) =>
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
            _target.Replace += (sender, oldItems, newIndex, newItems) =>
            {
                callCount++;
                Assert.IsNotNull(oldItems);
                Assert.AreEqual(1, oldItems.Count());
                
                Assert.AreEqual(0, newIndex);
                Assert.IsNotNull(newItems);
                Assert.AreEqual(1, newItems.Count());
            };

            _source[0] = new Person();
            Assert.AreEqual(1, callCount);
        }

        #if !SILVERLIGHT
        [Test]
        public void MoveInSource_SingleItem_FiresMove()
        {
            int callCount = 0;
            _target.Move += (sender, oldIndex, oldItems, newIndex, newItems) =>
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
        #endif

        [Test]
        public void InsertInSource_SingleItem_FiresReplace()
        {
            int callCount = 0;
            _target.Add += (sender, index, items) =>
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
            _target.Reset += (sender) =>
            {
                callCount++;
            };

            _source.Clear();
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void SetPropertyOnItemInSourceCollection_PropertyDifferent_FiresItemChangedEvent()
        {
            int callCount = 0, callCountChanging = 0;
            _target.AddProperty(p => p.Age,
                (item, args) => {
                    callCountChanging++;
                    Assert.AreSame(_source[0], item);
                },
                (item, args) => {
                    callCount++;
                    Assert.AreSame(_source[0], item);
                }
            );

            _source[0].Age = 1000;
            Assert.AreEqual(1, callCount);
            
#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountChanging);
#endif
        }

        [Test]
        public void RemoveFromSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            _target.AddProperty(p => p.Age,
                (item, args) => Assert.Fail(),
                (item, args) => Assert.Fail()
            );


            _source.Remove(person);
            person.Age = 1000;
        }

        [Test]
        public void ReplaceItemInSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            _target.AddProperty(p => p.Age,
                (item, args) => Assert.Fail(),
                (item, args) => Assert.Fail()
            );

            _source[0] = new Person();
            person.Age = 1000;
        }

        [Test]
        public void ReplaceItemInSourceAndChangePropertyOnNewItem_SingleItem_ItemChangedFired()
        {
            int callCount = 0, callCountChanging = 0;
            Person newPerson = new Person();

            _target.AddProperty(p => p.Age,
                (item, args) => {
                    callCountChanging++;
                    Assert.AreSame(newPerson, item);
                },
                (item, args) => {
                    callCount++;
                    Assert.AreSame(newPerson, item);
                }
            );
            
            _source[0] = newPerson;
            newPerson.Age = 1000;

            Assert.AreEqual(1, callCount);
#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountChanging);
#endif
        }

#if !SILVERLIGHT
        [Test]
        public void MoveItemInSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            int callCount = 0, callCountChanging = 0;
            _target.AddProperty(p => p.Age,
                (item, args) => callCountChanging++,
                (item, args) => callCount++
            );

            _source.Move(0, 1);
            person.Age = 1000;
            
            Assert.AreEqual(1, callCount);
#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountChanging);
#endif
        }
#endif

        [Test]
        public void ClearSourceAndChangePropertyOnItem_SingleItem_ItemChangedNotFired()
        {
            Person person = _source[0];

            _target.AddProperty(p => p.Age,
                (item, args) => Assert.Fail(),
                (item, args) => Assert.Fail()
            );

            _source.Clear();
            person.Age = 1000;
        }

        [Test]
        public void DuplicateInSource_ChangePropertyOnItem_OnlyNotifiedOnce()
        {
            Person person = _source[0];
            
            int callCount = 0, callCountChanging = 0;
            _target.AddProperty(p => p.Age,
                (item, args) => callCountChanging++,
                (item, args) => callCount++
            );

            _source.Add(person);

            person.Age++;

            Assert.AreEqual(1, callCount);
#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountChanging);
#endif
        }

        [Test]
        public void DuplicateInSource_AddAndThenRemove_OnlyNotifiedOnce()
        {
            Person person = _source[0];

            int callCount = 0, callCountChanging = 0;
            _target.AddProperty(p => p.Age,
                (item, args) => callCountChanging++,
                (item, args) => callCount++
            );

            _source.Add(person);
            _source.Remove(person);

            person.Age++;

            Assert.AreEqual(1, callCount);
#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountChanging);
#endif
        }

        [Test]
        public void DuplicateInSource_AddAndThenRemoveBoth_NotNotified()
        {
            Person person = _source[0];

            int callCount = 0;
            _target.AddProperty(p => p.Age,
                (item, args) => callCount++,
                (item, args) => callCount++
            );

            _source.Add(person);
            _source.Remove(person);
            _source.Remove(person);

            person.Age++;

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void AddAndRemovePropertyTree_FireMultipleEvents() {
            Person person = _source[0];
            int callCountAge = 0, callCountAgeChanging = 0;
            _target.AddProperty(p => p.Age,
                (item, args) => callCountAgeChanging++,
                (item, args) => callCountAge++
            );

            int callCountName = 0, callCountNameChanging = 0;

            var tree = _target.AddProperty(p => p.Name);
            tree.ItemChanged += (sender, item, args) => callCountName++;
            tree.ItemChanging += (sender, item, args) => callCountNameChanging++;

            person.Age++;
            person.Name = "Joe";

            Assert.AreEqual(1, callCountAge);
            Assert.AreEqual(1, callCountName);

#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountAgeChanging);
            Assert.AreEqual(1, callCountNameChanging);
#endif
            _target.RemoveProperty(tree);

            person.Name = "Smith";

            Assert.AreEqual(1, callCountAge);
            Assert.AreEqual(1, callCountName);

#if USE_NOTIFYING_VERSION
            Assert.AreEqual(1, callCountAgeChanging);
            Assert.AreEqual(1, callCountNameChanging);
#endif
        }
    }
}
