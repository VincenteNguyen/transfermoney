using System.Linq;
using System.Threading;
using ConcurrentTransferMoney.Controllers;
using ConcurrentTransferMoney.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ConcurrentTransferMoney.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerTest
    {
        [TestCase(1, 2, 100, 1000, TestName = "Test Transfer Using Queue Demo")]
        public void TransferUsingQueueDemo(int fromAccountId, int toAccountId, int amount, int numOfTransfer)
        {
            // Arrange
            //var controller = new AccountsController();
            //var prevFromAccount = controller.GetAccount1(fromAccountId);
            //var prevToAccount = controller.GetAccount1(toAccountId);
            //var pcQ = controller.Queue;
            //var transferModel = new BankTransferModel
            //{
            //    FromAccountId = fromAccountId,
            //    ToAccountId = toAccountId,
            //    Amount = amount
            //};
            //// Act
            
            //var result = controller.TransferUsingQueueDemo(transferModel, numOfTransfer);

            //Thread.Sleep(10000);
            //var currentFromAccount = controller.GetAccount1(fromAccountId);
            //var currentToAccount = controller.GetAccount1(toAccountId);

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual(currentFromAccount.Balance, prevFromAccount.Balance);
            //Assert.AreEqual(currentToAccount.Balance, prevToAccount.Balance);
        }

        [TestMethod]
        public void TransferUsingQueue()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TransferUsingLockDemo()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void TransferUsingLock()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}