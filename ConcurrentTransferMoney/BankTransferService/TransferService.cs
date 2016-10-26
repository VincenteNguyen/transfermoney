using ConcurrentTransferMoney.LockVersion;
using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.BankTransferService
{
    public class TransferService
    {
        private static readonly LockProvider<int> AccountLocks = new LockProvider<int>(); 

        public void Transfer(int fromAccountId, int toAccountId, int amount)
        {
            using (var db = new ApplicationDbContext())
            {
                var fromAccount = db.Accounts.Find(fromAccountId);
                var toAccount = db.Accounts.Find(toAccountId);
                fromAccount.Balance -= amount;
                fromAccount.TransferCount++;
                toAccount.Balance += amount;
                db.SaveChanges();
            }
        }

        public void TransferUsingLock(int fromAccountId, int toAccountId, int amount)
        {
            var fromAccountLocker = AccountLocks.GetLock(fromAccountId);
            var toAccountLocker = AccountLocks.GetLock(toAccountId);
            if (fromAccountId < toAccountId)
            {
                lock (fromAccountLocker)
                {
                    lock (toAccountLocker)
                    {
                        Transfer(fromAccountId, toAccountId, amount);
                    }
                }
            }
            else
            {
                lock (toAccountLocker)
                {
                    lock (fromAccountLocker)
                    {
                        Transfer(fromAccountId, toAccountId, amount);
                    }
                }
            }
        }
    }
}