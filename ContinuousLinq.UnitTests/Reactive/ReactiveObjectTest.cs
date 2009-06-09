using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ContinuousLinq.Reactive;
using System.ComponentModel;

namespace ContinuousLinq.UnitTests.Reactive
{
    [TestFixture]
    public class ReactiveObjectTest
    {
        private ReactivePerson _target;

        [SetUp]
        public void Initialize()
        {
            _target = new ReactivePerson();
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void TestPersonsAreUnique()
        {
            Person p1 = new Person();
            Person p2 = new Person();

            Assert.AreNotEqual(p1, p2);
        }

        [Test]
        public void OnPropertyChangedRaisesPropertyChangedEvent()
        {
            bool eventFired = false;
            string propertyFired = "";

            _target.PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e)
                {
                    eventFired = true;
                    propertyFired = e.PropertyName;
                };

            _target.Age++;
            Assert.IsTrue(eventFired);
            Assert.AreEqual("Age", propertyFired);
        }

        [Test]
        public void DependsOn_AgeSetToNewValue_OnAgeChangedCalledOnce()
        {
            _target.Age = 1000;
            Assert.AreEqual(1, _target.OnAgeChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherSetToNewValue_OnBrotherChangedCalledOnce()
        {
            _target.Brother = new Person();
            Assert.AreEqual(1, _target.OnBrotherChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherSetToNewValue_OnBrotherAgeChangedCalledOnce()
        {
            _target.Brother = new Person();
            Assert.AreEqual(1, _target.OnBrotherAgeChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherAgeSetToNewValue_OnBrotherAgeChangedCalledTwice()
        {
            _target.Brother = new Person();
            _target.Brother.Age = 10;
            Assert.AreEqual(2, _target.OnBrotherAgeChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherAgeSetToNewValue_OnBrotherChangedCalledOnce()
        {
            _target.Brother = new Person();
            _target.Brother.Age = 10;
            Assert.AreEqual(1, _target.OnBrotherChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherSetToNewValueThenBackToNull_OnBrotherChangedCalledTwice()
        {
            _target.Brother = new Person();
            _target.Brother = null;
            Assert.AreEqual(2, _target.OnBrotherChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherSetToNewValueThenBackToNull_OnBrotherAgeChangedCalledTwice()
        {
            _target.Brother = new Person();
            _target.Brother = null;
            Assert.AreEqual(2, _target.OnBrotherAgeChangedCalledCount);
        }

        [Test]
        public void DependsOn_BrotherSetToNewValueThenBackToNullAndAgeChangedAfterward_DoesNotTriggerAnyDependsOnMethods()
        {
            Person brother = new Person();
            _target.Brother = brother;
            _target.Brother = null;
            brother.Age = 10;
            Assert.AreEqual(2, _target.OnBrotherChangedCalledCount);
            Assert.AreEqual(2, _target.OnBrotherAgeChangedCalledCount);
        }

        //[Test]
        ////[ExpectedException(typeof(InvalidProgramException))]
        //public void ConstructDerivedClass_DerivedClassMethodSignatureMissingCorrectArguments_ThrowsException()
        //{
        //    try
        //    {
        //        ClassWithIncorrectDependsOn classWithIncorrectDependsOn = new ClassWithIncorrectDependsOn();
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}

        private class ReactivePerson : ReactiveObject
        {
            private int _age;
            private string _name;

            public string Name
            {
                get { return _name; }
                set
                {
                    if (value == _name)
                        return;

                    _name = value;
                    OnPropertyChanged("Name");
                }
            }

            public int Age
            {
                get { return _age; }
                set
                {
                    if (value == _age)
                        return;

                    _age = value;
                    OnPropertyChanged("Age");
                }
            }

            static ReactivePerson()
            {
                var dependsOn = Register<ReactivePerson>();

                dependsOn.Call(obj => obj.OnAgeChanged())
                    .OnChanged(obj => obj.Age);

                dependsOn.Call(obj => obj.OnBrotherChanged())
                    .OnChanged(obj => obj.Brother);

                dependsOn.Call(obj => obj.OnBrotherAgeChanged())
                    .OnChanged(obj => obj.Brother.Age);
            }

            private Person _brother;
            public Person Brother
            {
                get { return _brother; }
                set
                {
                    if (value == _brother)
                        return;

                    _brother = value;

                    OnPropertyChanged("Brother");
                }
            }

            public int OnAgeChangedCalledCount { get; set; }
            private void OnAgeChanged()
            {
                this.OnAgeChangedCalledCount++;
            }

            public int OnBrotherChangedCalledCount { get; set; }
            private void OnBrotherChanged()
            {
                this.OnBrotherChangedCalledCount++;
            }

            public int OnBrotherAgeChangedCalledCount { get; set; }
            private void OnBrotherAgeChanged()
            {
                this.OnBrotherAgeChangedCalledCount++;
            }
        }

        [Test]
        public void Test()
        {
            var myClass = new MyClass();
            myClass.CallCount = 0;
            
            int callCount = 0;
            myClass.PropertyChanged += (sender, args) => callCount++;

            myClass.Collection.Add(1);

            Assert.AreEqual(1, myClass.CallCount);
        }

        public class MyClass : ReactiveObject
        {

            private ContinuousCollection<int> _collection;
            public ContinuousCollection<int> Collection
            {
                get { return _collection; }
                set
                {
                    if (value == _collection)
                        return;

                    _collection = value;
                    OnPropertyChanged("Collection");
                }
            }
            private int _callCount;
            public int CallCount
            {
                get { return _callCount; }
                set
                {
                    if (value == _callCount)
                        return;

                    _callCount = value;
                    OnPropertyChanged("CallCount");
                }
            }
            
            public MyClass()
            {
                this.Collection = new ContinuousCollection<int>();
            }

            static MyClass()
            {
                var dependsOn = Register<MyClass>();
                dependsOn.Call(obj => obj.UpdateCollection())
                    .OnChanged(obj => obj.Collection.Count);
            }
            
            public void UpdateCollection()
            {
                this.CallCount++;
            }

        }
    }
}
