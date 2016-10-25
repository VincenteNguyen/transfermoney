using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConcurrentTransferMoney.BankTransferService
{
    public class TransferService
    {
        public void Transfer(int fromAccountId, int toAccountId, int amount)
        {
            using (var db = new BankDbContext())
            {
                var fromAccount = db.Accounts.Find(fromAccountId);
                var toAccount = db.Accounts.Find(toAccountId);
                fromAccount.Balance -= amount;
                toAccount.Balance += amount;
                db.SaveChanges();
            }
        }
    }
}