using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.Test1
{
    [Parallelizable(ParallelScope.All)]
    [SetUpFixture]
    class TestsSetup1
    {
        [OneTimeSetUp]
        public void OneTimeSetup1()
        {
            Thread.Sleep(2000);
        }
    }


    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    class Tests1
    {
        [Flags]
        enum mEnum
        {
            None,
            Tutorial = 1,
            Mission = 2,
            Group = 4
        }
        [Test]
        public void Test1()
        {
            var list1 = new List<int> { 1, 2, 3, 4, 5 };
            var list2 = new List<int> { 1, 2, 13, 14, 5 };

            Thread.Sleep(5000);
            //CollectionAssert.AreEqual(list1, list2);
            //Thread.Sleep(5000);
        }

        [Test]
        public void Test2()
        {
            Thread.Sleep(5000);
        }

       // [Order(1)]
        [Test]
        public void Test3()
        {
            Thread.Sleep(5000);
        }

       //[Order(2)]
        [Test]
        public void Test4()
        {
            Thread.Sleep(5000);
        }
    }
}
