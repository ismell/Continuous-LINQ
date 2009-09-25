using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ContinuousLinq;
using ContinuousLinq.Aggregates;
using ContinuousLinq.Expressions;
using System.Threading;
using System.ComponentModel;

namespace PerformanceConsole
{

    public class PerformanceTest
    {
        ObservableCollection<Person> _source;

        public PerformanceTest()
        {
            _source = new ObservableCollection<Person>();

            for (int i = 0; i < 3000; i++)
            {
                _source.Add(new Person(i.ToString(), i));
            }
        }

        public void WhereTest()
        {
            Random rand = new Random();

            var result = from p in _source
                         where p.Age > 1500
                         select p;

            DateTime start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                int index = rand.Next(_source.Count);
                _source[index].Age = _source[index].Age > 1500 ? 0 : 1501;
            }

            var duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }


        public void SelectTest()
        {
            Random rand = new Random();

            var result = from p in _source
                         select p.Age;

            DateTime start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                int index = rand.Next(_source.Count);
                _source[index].Age = _source[index].Age > 1500 ? 0 : 1501;

                if (_source[index].Age != result[index])
                    throw new Exception();
            }

            var duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }


        public void SelectLinearUpdateTest()
        {
            Random rand = new Random();

            var result = from p in _source
                         select p.Age;

            DateTime start = DateTime.Now;

            int updateIndex = 0;
            for (int i = 0; i < 1000000; i++)
            {
                if (updateIndex >= _source.Count)
                    updateIndex = 0;

                _source[updateIndex].Age++;

                if (_source[updateIndex].Age != result[updateIndex])
                    throw new Exception();

                updateIndex++;
            }

            var duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }


        public void SelectUnrelatedPropertyLinearUpdateTest()
        {
            Random rand = new Random();

            var result = from p in _source
                         select p;

            DateTime start = DateTime.Now;

            int updateIndex = 0;
            for (int i = 0; i < 1000000; i++)
            {
                if (updateIndex >= _source.Count)
                    updateIndex = 0;

                _source[updateIndex].Age++;

                if (_source[updateIndex].Age != result[updateIndex].Age)
                    throw new Exception();

                updateIndex++;
            }

            var duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }


        public void ContinuousSumWithoutPausing()
        {
            Random rand = new Random();

            DateTime start = DateTime.Now;

            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);

            int updateIndex = 0;
            for (int i = 0; i < 10000; i++)
            {
                if (updateIndex >= _source.Count)
                    updateIndex = 0;

                _source[updateIndex].Age++;

                if (sum.CurrentValue <= 0)
                    throw new Exception();

                updateIndex++;
            }

            var duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }


        public void ContinuousSumWithPausing()
        {
            Random rand = new Random();

            DateTime start = DateTime.Now;

            ContinuousValue<int> sum = _source.ContinuousSum(p => p.Age);

            using (PausedAggregation pausedAggregation = new PausedAggregation())
            {
                int updateIndex = 0;
                for (int i = 0; i < 10000; i++)
                {
                    if (updateIndex >= _source.Count)
                        updateIndex = 0;

                    _source[updateIndex].Age++;
                    if (sum.CurrentValue <= 0)
                        throw new Exception();

                    updateIndex++;
                }
            }

            var duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }

        public void TestDynamicInvoke()
        {
            Random rand = new Random();

            int a = 0;
            Action del = () => { a++; };
            Delegate baseDelegate = del;

            TimeSpan duration;
            DateTime start;

            start = DateTime.Now;

            for (int i = 0; i < 10000; i++)
            {
                del();
            }

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());

            start = DateTime.Now;

            for (int i = 0; i < 10000; i++)
            {
                baseDelegate.DynamicInvoke();
            }

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }

        public void TestDynamicProperty()
        {
            Random rand = new Random();
            Person person = new Person();
            person.Brother = new Person();

            TimeSpan duration;
            DateTime start;

            start = DateTime.Now;

            for (int i = 0; i < 100000; i++)
            {
                Person brother = person.Brother;
                brother.Age++;
            }

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());

            DynamicProperty brotherProperty = DynamicProperty.Create(typeof(Person), "Brother");

            start = DateTime.Now;

            for (int i = 0; i < 100000; i++)
            {
                Person brother = (Person)brotherProperty.GetValue(person);
                brother.Age++;
            }

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }

        public void SortingTest()
        {
            Random rand = new Random();
            int ITEMS = 3000;
            int MAX = ITEMS * 4;
            int[] data = new int[ITEMS];
            
            for (int i=0; i<data.Length; i++) 
            {
                data[i]=rand.Next(MAX);
            }

            TimeSpan duration;
            DateTime start;

            start = DateTime.Now;

            Array.Sort(data);

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
            
            data = new int[ITEMS];
            
            for (int i=0; i<data.Length; i++) 
            {
                data[i]=rand.Next(MAX);
            }

            start = DateTime.Now;

            QuickSort(data, 0, data.Length - 1);
            
            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }

        private int[] QuickSort(int[] a, int i, int j)
        {
            if (i < j)
            {
                int q = Partition(a, i, j);
                a = QuickSort(a, i, q);
                a = QuickSort(a, q + 1, j);
            }
            return a;
        }

        private int Partition(int[] a, int p, int r)
        {
            int x = a[p];
            int i = p - 1;
            int j = r + 1;
            int tmp = 0;
            while (true)
            {
                do
                {
                    j--;
                } while (a[j] > x);
                do
                {
                    i++;
                } while (a[i] < x);
                if (i < j)
                {
                    tmp = a[i];
                    a[i] = a[j];
                    a[j] = tmp;
                }
                else return j;
            }
        }

        private static List<T> CreateListOfPeople<T>(int count) where T : new()
        {
            List<T> innerList = new List<T>();
            for (int j = 0; j < count; j++)
            {
                innerList.Add(new T());
            }
            return innerList;
        }

        private List<List<Person>> _people;
        private List<List<NotifyingPerson>> _notifyingPeople;
        
        private List<ReadOnlyContinuousCollection<Person>> _peopleQueries;
        private List<ReadOnlyContinuousCollection<NotifyingPerson>> _notifyingPeopleQueries;


        public void CreatePeople(int OUTER_LIST_COUNT, int INNER_LIST_COUNT)
        {
            _people = new List<List<Person>>();

            for (int i = 0; i < OUTER_LIST_COUNT; i++)
            {
                List<Person> innerList = CreateListOfPeople<Person>(INNER_LIST_COUNT);
                _people.Add(innerList);
            }
        }
        
        private void CreateQueries(int OUTER_LIST_COUNT, int INNER_LIST_COUNT)
        {
            _peopleQueries = new List<ReadOnlyContinuousCollection<Person>>();
            ObservableCollection<Person> innerList = new ObservableCollection<Person>();
            for (int j = 0; j < INNER_LIST_COUNT; j++)
            {
                innerList.Add(new Person());
            }

            for (int i = 0; i < OUTER_LIST_COUNT; i++)
            {
                var query = from person in innerList
                            where person.Age > 10
                            select person;

                _peopleQueries.Add(query);
            }
        }


        public void CreateNotifyingPeople(int OUTER_LIST_COUNT, int INNER_LIST_COUNT)
        {
            _notifyingPeople = new List<List<NotifyingPerson>>();

            for (int i = 0; i < OUTER_LIST_COUNT; i++)
            {
                List<NotifyingPerson> innerList = CreateListOfPeople<NotifyingPerson>(INNER_LIST_COUNT);
                _notifyingPeople.Add(innerList);
            }
        }

        private void CreateNotifyingQueries(int OUTER_LIST_COUNT, int INNER_LIST_COUNT)
        {
            _notifyingPeopleQueries = new List<ReadOnlyContinuousCollection<NotifyingPerson>>();
            ObservableCollection<NotifyingPerson> innerList = new ObservableCollection<NotifyingPerson>();
            for (int j = 0; j < INNER_LIST_COUNT; j++)
            {
                innerList.Add(new NotifyingPerson());
            }

            for (int i = 0; i < OUTER_LIST_COUNT; i++)
            {
                var query = from person in innerList
                            where person.Age > 10
                            select person;

                _notifyingPeopleQueries.Add(query);
            }
        }

        public void MemoryTest()
        {
            const int OUTER_LIST_COUNT = 500;
            const int INNER_LIST_COUNT = 1000;

            //long totalMemoryBase;
            //Thread.MemoryBarrier();
            //totalMemoryBase = GC.GetTotalMemory(true);
            //Console.WriteLine("TotalMemoryBase: {0}", totalMemoryBase);
            //DoCompleteCollect();

            //CreatePeople(OUTER_LIST_COUNT, INNER_LIST_COUNT);

            //Thread.MemoryBarrier(); 
            //long memoryForJustLists;
            //memoryForJustLists = GC.GetTotalMemory(true) - totalMemoryBase;
            //Console.WriteLine("Simple Lists: {0}", memoryForJustLists);
            
            //_people = null;

            //Thread.MemoryBarrier();
            //long memoryAfterClean = GC.GetTotalMemory(true) - totalMemoryBase;
            //Console.WriteLine("Memory after clean: {0}", memoryAfterClean);
            //totalMemoryBase = GC.GetTotalMemory(true);

            DoCompleteCollect();
            Console.ReadLine();
            CreateNotifyingQueries(OUTER_LIST_COUNT, INNER_LIST_COUNT);
            Console.WriteLine("Created");
            DoCompleteCollect();
            Console.ReadLine();
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine(_notifyingPeopleQueries.Count);
            }
            //long memoryForQueries;
            //Thread.MemoryBarrier();
            //memoryForQueries = GC.GetTotalMemory(true) - totalMemoryBase;
            //Console.WriteLine("Queries: {0}", memoryForJustLists);
            
            //DoCompleteCollect();
            //Console.ReadLine();
        }

        public void CompareQueryCreation()
        {
            const int OUTER_LIST_COUNT = 1000;
            const int INNER_LIST_COUNT = 300;

            TimeSpan duration;
            DateTime start;

            start = DateTime.Now;

            CreateQueries(OUTER_LIST_COUNT, INNER_LIST_COUNT);

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());

            start = DateTime.Now;

            CreateNotifyingQueries(OUTER_LIST_COUNT, INNER_LIST_COUNT);

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
        }

        private static void DoCompleteCollect()
        {
            Thread.MemoryBarrier();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void GetInterfaceTest()
        {
            const int ITERATIONS = 1000000;
            bool result = false;
            TimeSpan duration;
            DateTime start;

            start = DateTime.Now;

            for (int i = 0; i < ITERATIONS; i++)
            {
                result = typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(Person));
            }

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());

            start = DateTime.Now;

            for (int i = 0; i < ITERATIONS; i++)
            {
                result = typeof(Person).GetInterface(typeof(INotifyPropertyChanged).Name) != null;
            }

            duration = DateTime.Now - start;
            Console.WriteLine(duration.ToString());
            Console.WriteLine(result);
        }
    }
}
