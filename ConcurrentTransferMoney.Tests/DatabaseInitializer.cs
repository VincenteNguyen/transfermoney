using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Security.Cryptography.X509Certificates;
using ConcurrentTransferMoney.Entities;
using ConcurrentTransferMoney.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace ConcurrentTransferMoney.Tests
{
    public class MyDatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // Add entities to database.
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
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