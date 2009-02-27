using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformanceTest performanceTest = new PerformanceTest();
            performanceTest.TestDynamicInvoke();
            //performanceTest.SelectTest();
            //performanceTest.SelectLinearUpdateTest();
            //performanceTest.SelectUnrelatedPropertyLinearUpdateTest();
            //performanceTest.WhereTest();
            //performanceTest.ContinuousSumWithoutPausing();
            //performanceTest.ContinuousSumWithPausing();
        }
    }
}
