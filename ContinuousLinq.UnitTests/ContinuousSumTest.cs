using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using NUnit.Framework;
using ContinuousLinq.Aggregates;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ContinuousLinq.WeakEvents;

namespace ContinuousLinq.UnitTests
{
    [TestFixture]
    public class ContinuousSumTest
    {
        private ObservableCollection<Person> _source;

        [SetUp]
        public void Setup()
        {
            _source = ClinqTestFactory.CreateTwoPersonSource();
        }

        [Test]
        public void SumIntegers_ImmediatelyAfterConstruction_SumCompleted()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);

            Assert.AreEqual(30, sum.CurrentValue);
        }

        [Test]
        public void SumIntegers_ChangeValueOfMonitoredPropertyCollection_SumUpdated()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);
            _source[0].Age++;

            Assert.AreEqual(31, sum.CurrentValue);
        }

        //[Test]
        //public void SumIntegers_ChangeValueOfMonitoredPropertyCollection_PostChangeLambda()
        //{
        //    int monitoredSum = 0;

        //    ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age, newVal => monitoredSum = newVal);
        //    _source[0].Age += 12;

        //    Assert.AreEqual(sum.CurrentValue, 42);
        //    Assert.AreEqual(monitoredSum, sum.CurrentValue);
        //}

        [Test]
        public void SumIntegers_ChangeValueOfMonitoredPropertyCollection_PropertyChangedEventFires()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);
            int callCount = 0;
            sum.PropertyChanged += (sender, args) =>
            { 
                callCount++;
                Assert.AreEqual("CurrentValue", args.PropertyName);
            };

            _source[0].Age++;

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void SumIntegers_AddItemToCollection_SumUpdated()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);
            _source.Add(new Person() { Age = 10 });
            Assert.AreEqual(40, sum.CurrentValue);
        }

        [Test]
        public void SumIntegers_RemoveItemFromCollection_SumUpdated()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);
            _source.Remove(_source[0]);
            Assert.AreEqual(20, sum.CurrentValue);
        }

        [Test]
        public void SumIntegers_ReplaceItemInCollection_SumUpdated()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);
            _source[0] = new Person() { Age = 50 };
            Assert.AreEqual(70, sum.CurrentValue);
        }

        [Test]
        public void SumIntegers_MoveItemInCollection_SumTheSame()
        {
            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);
            _source.Move(0, 1);

            Assert.AreEqual(30, sum.CurrentValue);
        }
    }

    [TestFixture]
    public class ContinuousMaxTest
    {
        private static ContinuousCollection<NotifiableItem> Items { get; set; }

        private static ContinuousValue<double> MaxValueCV { get; set; }

        private static double Max { get; set; }

        [Test]
        [Ignore]
        public void Test()
        {
            // Collection with 1 item to make this test simple
            Items = new ContinuousCollection<NotifiableItem>
                        {
                            new NotifiableItem { TestValue1 = 3, TestValue2 = 10 },                            
                        };

            // Start with TestValue1
            MaxValueCV = Items.ContinuousMax(item => item.TestValue1, value => Max = value);
            Console.WriteLine("MaxValueCV = " + MaxValueCV.CurrentValue);
            Console.WriteLine("Max = " + Max);
            Console.WriteLine();

            // Switch to TestValue2
            MaxValueCV = Items.ContinuousMax(item => item.TestValue2, value => Max = value);
            Console.WriteLine("MaxValueCV = " + MaxValueCV.CurrentValue);
            Console.WriteLine("Max = " + Max);
            Console.WriteLine();
            GC.Collect();
            WeakPropertyChangedEventManager.RemoveCollectedEntries();
            GC.Collect();
            // Now set TestValue1
            Items[0].TestValue1 = 20;
            Console.WriteLine("(BUG)");
            Console.WriteLine("MaxValueCV = " + MaxValueCV.CurrentValue);
            // BUG: Max is set to 20 when it should be 10
            Console.WriteLine("Max = " + Max);

            Assert.AreEqual(10, Max);

            Console.Write("Hit enter to continue...");
            Console.ReadLine();
        }


        public class NotifiableItem : INotifyPropertyChanged
        {
            #region Fields

            private double _testValue1;
            private double _testValue2;

            #endregion

            #region Properties

            public double TestValue1
            {
                get { return _testValue1; }
                set
                {
                    if (value == _testValue1)
                        return;

                    _testValue1 = value;
                    OnPropertyChanged("TestValue1");
                }
            }

            public double TestValue2
            {
                get { return _testValue2; }
                set
                {
                    if (value == _testValue2)
                        return;

                    _testValue2 = value;
                    OnPropertyChanged("TestValue2");
                }
            }

            #endregion

            #region Members

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged == null)
                    return;

                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }
    }
}
