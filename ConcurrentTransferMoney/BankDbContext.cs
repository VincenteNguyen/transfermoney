using System.Data.Entity;
using ConcurrentTransferMoney.Entities;

namespace ConcurrentTransferMoney
{
    public class BankDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}