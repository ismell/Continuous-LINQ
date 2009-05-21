using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace ContinuousLinq.UnitTests
{
    [DebuggerDisplay("Name: {Name}, Age: {Age}")]
    public class Person : INotifyPropertyChanged
    {
        #region Fields

        private string _name;
        private int _age;
        private Person _brother;
        private ObservableCollection<Person> _parents;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Person()
        {
        }

        public Person(string name, int age)
        {
            _name = name;
            _age = age;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

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

        public int AddYearsToAge(int amount)
        {
            this.Age += amount;
            return this.Age; ;
        }

        public int SubtractYearsFromAge(int amount)
        {
            this.Age -= amount;
            return this.Age; ;
        }

        #endregion
    }
}
