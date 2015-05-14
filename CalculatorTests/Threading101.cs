using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CalculatorTests
{
    [TestClass]
    public class Threading101
    {
        [TestMethod]
        public void TestMethod1()
        {

            var results = Enumerable.Range(20000, 20002)
                .Select(a => new { Num = a, Count = GetFactors(a).Count() }).ToList();

        }

        static IEnumerable<int> GetFactors(int n)
        {
            return from a in Enumerable.Range(1, n)
                   where n % a == 0
                   select a;
        }
        
    }

}
