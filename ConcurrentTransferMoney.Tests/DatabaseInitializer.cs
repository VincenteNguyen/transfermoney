using System.Data.Entity;
using System.Data.Entity.Migrations;
using ConcurrentTransferMoney.Entities;
using ConcurrentTransferMoney.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentTransferMoney.Tests
{
    public class MyDatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Accounts.AddOrUpdate(
                new Account {Balance = 10000},
                new Account {Balance = 5000});
            context.SaveChanges();
        }
    }

    [TestClass]
    public class Global
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Database.SetInitializer(new MyDatabaseInitializer());
        }
    }
}