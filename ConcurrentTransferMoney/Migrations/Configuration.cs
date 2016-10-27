using System.Data.Entity.Migrations;
using ConcurrentTransferMoney.Entities;
using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Accounts.AddOrUpdate(
                new Account {Balance = 10000},
                new Account {Balance = 5000});
            context.SaveChanges();
        }
    }
}