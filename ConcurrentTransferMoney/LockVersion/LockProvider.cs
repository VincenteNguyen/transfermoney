using System.Collections.Concurrent;

namespace ConcurrentTransferMoney.LockVersion
{
    public class LockProvider<T>
    {
        private readonly ConcurrentDictionary<T, object> _lstLocks = new ConcurrentDictionary<T, object>();

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