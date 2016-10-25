using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.BankTransferService
{
    public class TransferService
    {
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
    }
}