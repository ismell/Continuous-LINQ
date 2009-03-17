using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ContinuousLinq;
using ContinuousLinq.Aggregates;
using ContinuousLinq.Expressions;

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
                
                if(_source[index].Age != result[index])
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
                
                if(sum.CurrentValue <= 0)
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
    }
}
