using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ContinuousLinq.UnitTests
{
    public static class ClinqTestFactory
    {
        public static ObservableCollection<Person> CreateTwoPersonSource()
        {
            return new ObservableCollection<Person>() 
            {
                new Person("Bob", 10), 
                new Person("Jim", 20),
            };
        }

        public static ObservableCollection<Person> CreateSixPersonSource()
        {
            ObservableCollection<Person> source = new ObservableCollection<Person>() ;
            for (int i = 0; i < 6; i++)
            {
                source.Add(new Person(i.ToString(), i * 10));
            }
            return source;
        }
    }
}
