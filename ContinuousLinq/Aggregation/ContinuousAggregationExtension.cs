using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ContinuousLinq.Aggregates
{
    public static class ContinuousAggregationExtension
    {
        #region SUM

        public static ContinuousValue<int> ContinuousSum<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, int>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<double> ContinuousSum<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, double>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<decimal> ContinuousSum<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, decimal>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal,decimal>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<float> ContinuousSum<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, float>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<long> ContinuousSum<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, long>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<int> ContinuousSum<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, int>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<double> ContinuousSum<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, double>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<decimal> ContinuousSum<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, decimal>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<float> ContinuousSum<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, float>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<long> ContinuousSum<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, long>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<int> ContinuousSum<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, int>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<double> ContinuousSum<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, double>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<decimal> ContinuousSum<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, decimal>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<float> ContinuousSum<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, float>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        public static ContinuousValue<long> ContinuousSum<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, long>> sumFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, sumFunc, (list, selector) => list.Sum(selector));
        }

        #endregion

        #region COUNT

        public static ContinuousValue<int> ContinuousCount<T>(
            this ObservableCollection<T> input) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, null, (list, selector) => list.Count);
        }

        public static ContinuousValue<int> ContinuousCount<T>(
            this ReadOnlyObservableCollection<T> input) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, null, (list, selector) => list.Count);
        }

        public static ContinuousValue<int> ContinuousCount<T>(
            this ReadOnlyContinuousCollection<T> input) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, null, (list, selector) => list.Count);
        }

        #endregion

        #region MIN
        #region -- Int
        public static ContinuousValue<int> ContinuousMin<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, int>> minFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, minFunc, (list, selector) => list.Min(selector));
        }
        
        public static ContinuousValue<int> ContinuousMin<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, int>> minFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, minFunc, (list, selector) => list.Min(selector));
        }

        public static ContinuousValue<int> ContinuousMin<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, int>> minFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, minFunc, (list, selector) => list.Min(selector));
        }
        #endregion

        #region -- Double
        public static ContinuousValue<double> ContinuousMin<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, double>> minFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, minFunc, (list, selector) => list.Min(selector));
        }
        public static ContinuousValue<double> ContinuousMin<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, double>> minFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, minFunc, (list, selector) => list.Min(selector));
        }
        public static ContinuousValue<double> ContinuousMin<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, double>> minFunc) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, minFunc, (list, selector) => list.Min(selector));
        }
        #endregion

        #region -- Float
        public static ContinuousValue<float> ContinuousMin<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, float>> minSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, minSelector, (list, selector) => list.Min(selector));
        }
        public static ContinuousValue<float> ContinuousMin<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, float>> minSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, minSelector, (list, selector) => list.Min(selector));
        }
        public static ContinuousValue<float> ContinousMin<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, float>> minSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, minSelector, (list, selector) => list.Min(selector));
        }

        #endregion

        #region -- Long
        public static ContinuousValue<long> ContinuousMin<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, long>> minSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, minSelector, (list, selector) => list.Min(selector));
        }
        public static ContinuousValue<long> ContinousMin<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, long>> minSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, minSelector, (list, selector) => list.Min(selector));
        }
        public static ContinuousValue<long> ContinuousMin<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, long>> minSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, minSelector, (list, selector) => list.Min(selector));
        }
        #endregion
        #endregion

        #region AVG
        #region -- Decimal
        public static ContinuousValue<decimal> ContinuousAverage<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, decimal>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        public static ContinuousValue<decimal> ContinuousAverage<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, decimal>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        public static ContinuousValue<decimal> ContinuousAverage<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, decimal>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        #endregion

        #region -- Float
        public static ContinuousValue<float> ContinuousAverage<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, float>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T,float,  float>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        public static ContinuousValue<float> ContinuousAverage<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, float>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        public static ContinuousValue<float> ContinuousAverage<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, float>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        #endregion

        #region -- Double
        public static ContinuousValue<double> ContinuousAverage<T>(
           this ObservableCollection<T> input,
           Expression<Func<T, double>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        public static ContinuousValue<double> ContinuousAverage<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, double>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        public static ContinuousValue<double> ContinuousAverage<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, double>> averageSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, averageSelector, (list, selector) => list.Average(selector));
        }
        #endregion

        
        #endregion

        #region MAX
        #region -- Int
        public static ContinuousValue<int> ContinuousMax<T>(
           this ObservableCollection<T> input,
           Expression<Func<T, int>> maxSelector) where T : INotifyPropertyChanged
        {           
            return new ContinuousValue<T, int, int>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<int> ContinousMax<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, int>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<int> ContinuousMax<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, int>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, int>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        #endregion

        #region -- Long
        public static ContinuousValue<long> ContinuousMax<T>(
           this ObservableCollection<T> input,
           Expression<Func<T, long>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<long> ContinousMax<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, long>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<long> ContinuousMax<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, long>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, long>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        #endregion

        #region -- Decimal
        public static ContinuousValue<decimal> ContinuousMax<T>(
           this ObservableCollection<T> input,
           Expression<Func<T, decimal>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<decimal> ContinousMax<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, decimal>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<decimal> ContinuousMax<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, decimal>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, decimal>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        #endregion

        #region -- Double
        public static ContinuousValue<double> ContinuousMax<T>(
           this ObservableCollection<T> input,
           Expression<Func<T, double>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<double> ContinousMax<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, double>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<double> ContinuousMax<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, double>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        #endregion

        #region -- Float
        public static ContinuousValue<float> ContinuousMax<T>(
           this ObservableCollection<T> input,
           Expression<Func<T, float>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<float> ContinousMax<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, float>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        public static ContinuousValue<float> ContinuousMax<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, float>> maxSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, float>(input, maxSelector, (list, selector) => list.Max(selector));
        }
        #endregion
        #endregion        

        #region STDDEV
        #region -- Int
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, int>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, int>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, int>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, int, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        #endregion

        #region -- Long
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, long>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, long>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, long>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, long, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        #endregion

        #region -- Decimal
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, decimal>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, decimal>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, decimal>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, decimal, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        #endregion

        #region -- Float
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, float>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, float>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, float>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, float, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        #endregion

        #region -- Double
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ObservableCollection<T> input,
            Expression<Func<T, double>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyObservableCollection<T> input,
            Expression<Func<T, double>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        public static ContinuousValue<double> ContinuousStdDev<T>(
            this ReadOnlyContinuousCollection<T> input,
            Expression<Func<T, double>> columnSelector) where T : INotifyPropertyChanged
        {
            return new ContinuousValue<T, double, double>(input, columnSelector, (list, selector) => StdDev.Compute(selector, list));
        }
        #endregion
        #endregion        
    }
}
