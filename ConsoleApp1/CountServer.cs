using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    using System;
    using System.Threading;

    public static class CountServer
    {
        private static int _count;

        private static readonly ReaderWriterLockSlim _rw = 
            new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public static int GetCount()
        {
            _rw.EnterReadLock();
            try
            {
                return _count;
            }
            finally
            {
                _rw.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            _rw.EnterWriteLock();
            try
            {
                checked
                {
                    _count += value;
                }
            }
            finally
            {
                _rw.ExitWriteLock();
            }
        }
        public static void Reset(int value = 0)
        {
            _rw.EnterWriteLock();
            try
            {
                _count = value;
            }
            finally
            {
                _rw.ExitWriteLock();
            }
        }
    }

}
