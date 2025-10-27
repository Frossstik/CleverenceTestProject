using ConsoleApp1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    public class CountServerTests
    {
        [Fact]
        public void AddToCountTest()
        {
            CountServer.Reset();
            CountServer.AddToCount(5);
            Assert.Equal(5, CountServer.GetCount());
            CountServer.AddToCount(10);
            Assert.Equal(15, CountServer.GetCount());
        }
        [Fact]
        public void ResetTest()
        {
            CountServer.Reset(20);
            Assert.Equal(20, CountServer.GetCount());
            CountServer.Reset();
            Assert.Equal(0, CountServer.GetCount());
        }

        [Fact]
        public void ParallelWritersTest()
        {
            CountServer.Reset();

            int writers = 32;
            int iterations = 10_000;
            int delta = 1;

            Parallel.For(0, writers, _ =>
            {
                for (int i = 0; i < iterations; i++)
                    CountServer.AddToCount(delta);
            });

            int expected = writers * iterations * delta;
            Assert.Equal(expected, CountServer.GetCount());
        }
    }
}
