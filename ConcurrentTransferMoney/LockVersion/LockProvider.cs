using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ConcurrentTransferMoney.LockVersion
{
    /// <summary>
    ///     Provides a mechanism to lock based on a data item being retrieved
    /// </summary>
    /// <typeparam name="T">Type of the data being used as a key</typeparam>
    public class LockProvider<T>
    {
        private readonly ConcurrentDictionary<T, object> _lstLocks = new ConcurrentDictionary<T, object>();
        //private readonly object _syncRoot = new object();

        /// <summary>
        ///     Gets an object suitable for locking the specified data item
        /// </summary>
        /// <param name="key">The data key</param>
        /// <returns></returns>
        public object GetLock(T key)
        {
            if (!_lstLocks.ContainsKey(key))
            {
                _lstLocks[key] = new object();
            }
            return _lstLocks[key];
        }
    }
}