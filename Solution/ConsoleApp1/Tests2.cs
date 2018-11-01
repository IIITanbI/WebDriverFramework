using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.Test2
{
    [Parallelizable(ParallelScope.All)]
    [SetUpFixture]
    class TestsSetup2
    {
        [OneTimeSetUp]
        public void OneTimeSetup2()
        {
            Thread.Sleep(2000);
        }
    }

    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    class Tests2
    {
        [Test]
        public void Test1()
        {
            Thread.Sleep(5000);
        }

        [Test]
        public void Test2()
        {
            Thread.Sleep(5000);
        }
    }
}
