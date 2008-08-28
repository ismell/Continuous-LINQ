using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ContinuousLinq.UnitTests
{
    public class Person : INotifyPropertyChanged
    {
        #region Fields

        private string _name;
        private int _age;
        private Person _brother;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public Person()
        {
        }

        public Person(string name, int age)
        {
            _name = name;
            _age = age;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {

                if (_name == value)
                    return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                if (_age == value)
                    return;
                _age = value;
                OnPropertyChanged("Age");
            }
        }

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

        private ObservableCollection<Person> _parents;
        public ObservableCollection<Person> Parents
        {
            get { return _parents; }
            set
            {
                if (value == _parents)
                    return;

                _parents = value;
                OnPropertyChanged("Parents");
            }
        }
        #region Members

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged == null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public ReadOnlyContinuousCollection<Person> GetPeopleWithSameAge(ObservableCollection<Person> people)
        {
            return from person in people
                   where person.Age == this.Age
                   select person;
        }

        public ReadOnlyContinuousCollection<Person> GetPeopleWithSameAgeAsBrother(ObservableCollection<Person> people)
        {
            return from person in people
                   where this.Brother != null && person.Age == this.Brother.Age
                   select person;
        }

        #endregion
    }
}
