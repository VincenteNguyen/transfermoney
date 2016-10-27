using System.Collections.Generic;
using System.Linq;
using ConcurrentTransferMoney.LockVersion;
using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.BankTransferService
{
    public class TransferService
    {
        private static readonly LockProvider<int> AccountLocks = new LockProvider<int>();

        public void Transfer(int fromAccountId, int toAccountId, int amount)
        {
            using (var db = ApplicationDbContext.Create())
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
            var participantIds = new List<int> {fromAccountId, toAccountId}.OrderBy(x => x).ToList();
            lock (AccountLocks.GetLock(participantIds[0]))
            {
                lock (AccountLocks.GetLock(participantIds[1]))
                {
                    Transfer(fromAccountId, toAccountId, amount);
                }
            }
        }
    }
}