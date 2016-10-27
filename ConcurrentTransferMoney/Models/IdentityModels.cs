using System.Data.Entity;
using ConcurrentTransferMoney.Entities;

namespace ConcurrentTransferMoney.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            //AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
        }

        public virtual DbSet<Account> Accounts { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}