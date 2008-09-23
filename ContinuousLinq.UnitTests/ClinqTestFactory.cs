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

        public static ObservableCollection<Person> CreateSixPersonSourceWithDuplicates()
        {
            Person bob = new Person("Bob", 10);
            Person jim = new Person("Jim", 20);

            ObservableCollection<Person> source = CreateSixPersonSource();
            source[0] = bob;
            source[1] = bob;
            source[2] = bob;

            source[4] = jim;
            source[5] = jim;

            return source;
        }
    }
}
