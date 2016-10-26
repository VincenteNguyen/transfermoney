using System;
using System.Linq;
using System.Threading;
using ConcurrentTransferMoney.Controllers;
using ConcurrentTransferMoney.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Threading.Tasks;
using ConcurrentTransferMoney.Entities;
using System.Collections.Generic;
using System.Web.Http;

namespace ConcurrentTransferMoney.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerTest
    {
        private int _amount;
        private int _numOfTransfer;
        private AccountsController _controller;
        private ApplicationDbContext _context;
        private Account _beforeFromAccount;
        private Account _beforeToAccount;
        private BankTransferModel _transferModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = new ApplicationDbContext();
            _context.Database.Initialize(true);
            _controller = new AccountsController(_context);
            _beforeFromAccount = _context.Accounts.FirstOrDefault();
            _beforeToAccount = _context.Accounts.FirstOrDefault(x => x.Id != _beforeFromAccount.Id);
            _amount = 100;
            _numOfTransfer = 1000;
            _transferModel = CreateBankTransferModel();
        }


        [TestMethod]
        public async Task TransferUsingQueueDemo()
        {
            //Arrange
            Console.WriteLine("Before transfer");
            Console.WriteLine(await _controller.GetAccountInformation(_beforeFromAccount.Id, _beforeToAccount.Id));

            // Act
            var result = await _controller.TransferUsingQueueDemo(_transferModel, _numOfTransfer);
            Thread.Sleep(100 * _numOfTransfer);

            // Assert
            var context2 = new ApplicationDbContext();
            var afterFromAccount = context2.Accounts.Find(_beforeFromAccount.Id);
            var afterToAccount = context2.Accounts.Find(_beforeToAccount.Id);

            Console.WriteLine("After transfer");
            Console.WriteLine(await _controller.GetAccountInformation(afterFromAccount.Id, afterToAccount.Id));
            Assert.IsNotNull(result);
            Assert.AreEqual(afterFromAccount.Balance, _beforeFromAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterFromAccount.TransferCount);
            Assert.AreEqual(afterToAccount.Balance, _beforeToAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterToAccount.TransferCount);
        }

        private BankTransferModel CreateBankTransferModel()
        {
            var transferModel = new BankTransferModel
            {
                FromAccountId = _beforeFromAccount.Id,
                ToAccountId = _beforeToAccount.Id,
                Amount = _amount
            };
            return transferModel;
        }

        //[TestMethod]
        public void TransferUsingQueue()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public async Task TransferUsingLockDemo()
        {
            //Arrange
            Console.WriteLine("Before transfer");
            Console.WriteLine(await _controller.GetAccountInformation(_beforeFromAccount.Id, _beforeToAccount.Id));

            // Act
            var result = await _controller.TransferUsingLockDemo(_transferModel, _numOfTransfer);

            // Assert
            var context2 = new ApplicationDbContext();
            var afterFromAccount = context2.Accounts.Find(_beforeFromAccount.Id);
            var afterToAccount = context2.Accounts.Find(_beforeToAccount.Id);

            Console.WriteLine("After transfer");
            Console.WriteLine(await _controller.GetAccountInformation(afterFromAccount.Id, afterToAccount.Id));
            Assert.IsNotNull(result);
            Assert.AreEqual(afterFromAccount.Balance, _beforeFromAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterFromAccount.TransferCount);
            Assert.AreEqual(afterToAccount.Balance, _beforeToAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterToAccount.TransferCount);
        }

        //[TestMethod]
        public void TransferUsingLock()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}