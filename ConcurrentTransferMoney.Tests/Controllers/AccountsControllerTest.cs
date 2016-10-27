using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentTransferMoney.Controllers;
using ConcurrentTransferMoney.Entities;
using ConcurrentTransferMoney.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConcurrentTransferMoney.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerTest
    {
        private readonly int _amount = 100;
        private readonly int _numOfTransfer = 100;
        private Account _beforeFromAccount;
        private Account _beforeToAccount;
        private ApplicationDbContext _context;
        private AccountsController _controller;
        private Stopwatch _stopwatch;
        private BankTransferModel _transferModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = new ApplicationDbContext();
            _context.Database.Initialize(true);
            _controller = new AccountsController(_context);
            _beforeFromAccount = _context.Accounts.FirstOrDefault();
            _beforeToAccount = _context.Accounts.FirstOrDefault(x => x.Id != _beforeFromAccount.Id);
            _transferModel = new BankTransferModel
            {
                FromAccountId = _beforeFromAccount.Id,
                ToAccountId = _beforeToAccount.Id,
                Amount = _amount
            };
        }

        [TestMethod]
        public async Task TransferUsingQueueDemo()
        {
            //Arrange
            _stopwatch = new Stopwatch();
            Console.WriteLine("Before transfer");
            Console.WriteLine(await _controller.GetAccountInformation(_beforeFromAccount.Id, _beforeToAccount.Id));

            // Act
            _stopwatch.Start();
            var result = await _controller.TransferUsingQueueDemo(_transferModel, _numOfTransfer);
            _stopwatch.Stop();
            // Assert
            var context2 = new ApplicationDbContext();
            var afterFromAccount = context2.Accounts.Find(_beforeFromAccount.Id);
            var afterToAccount = context2.Accounts.Find(_beforeToAccount.Id);

            Console.WriteLine("After transfer");
            Console.WriteLine(await _controller.GetAccountInformation(afterFromAccount.Id, afterToAccount.Id));
            Assert.IsNotNull(result);
            Assert.AreEqual(_beforeFromAccount.Balance, afterFromAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterFromAccount.TransferCount);
            Assert.AreEqual(_beforeToAccount.Balance, afterToAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterToAccount.TransferCount);
            Console.WriteLine("Time elapsed: {0}", _stopwatch.Elapsed);
        }

        [TestMethod]
        public async Task TransferUsingQueue()
        {
            //Arrange
            _stopwatch = new Stopwatch();
            Console.WriteLine("Before transfer");
            Console.WriteLine(await _controller.GetAccountInformation(_beforeFromAccount.Id, _beforeToAccount.Id));

            // Act
            _stopwatch.Start();
            Parallel.Invoke(
                () => _controller.TransferUsingQueue(new BankTransferModel
                {
                    FromAccountId = _beforeFromAccount.Id,
                    ToAccountId = _beforeToAccount.Id,
                    Amount = 500
                }),
                () => _controller.TransferUsingQueue(new BankTransferModel
                {
                    FromAccountId = _beforeToAccount.Id,
                    ToAccountId = _beforeFromAccount.Id,
                    Amount = 1000
                }));
            Thread.Sleep(100*2);
            _stopwatch.Stop();

            // Assert
            var context2 = new ApplicationDbContext();
            var afterFromAccount = context2.Accounts.Find(_beforeFromAccount.Id);
            var afterToAccount = context2.Accounts.Find(_beforeToAccount.Id);

            Console.WriteLine("After transfer");
            Console.WriteLine(await _controller.GetAccountInformation(afterFromAccount.Id, afterToAccount.Id));
            Assert.AreEqual(_beforeFromAccount.Balance + 500, afterFromAccount.Balance);
            Assert.AreEqual(1, afterFromAccount.TransferCount);
            Assert.AreEqual(_beforeToAccount.Balance - 500, afterToAccount.Balance);
            Assert.AreEqual(1, afterToAccount.TransferCount);
            Console.WriteLine("Time elapsed: {0}", _stopwatch.Elapsed);
        }

        [TestMethod]
        public async Task TransferUsingLockDemo()
        {
            //Arrange
            _stopwatch = new Stopwatch();
            Console.WriteLine("Before transfer");
            Console.WriteLine(await _controller.GetAccountInformation(_beforeFromAccount.Id, _beforeToAccount.Id));

            // Act
            _stopwatch.Start();
            var result = await _controller.TransferUsingLockDemo(_transferModel, _numOfTransfer);
            _stopwatch.Stop();

            // Assert
            var context2 = new ApplicationDbContext();
            var afterFromAccount = context2.Accounts.Find(_beforeFromAccount.Id);
            var afterToAccount = context2.Accounts.Find(_beforeToAccount.Id);

            Console.WriteLine("After transfer");
            Console.WriteLine(await _controller.GetAccountInformation(afterFromAccount.Id, afterToAccount.Id));
            Assert.IsNotNull(result);
            Assert.AreEqual(_beforeFromAccount.Balance, afterFromAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterFromAccount.TransferCount);
            Assert.AreEqual(_beforeToAccount.Balance, afterToAccount.Balance);
            Assert.AreEqual(_numOfTransfer, afterToAccount.TransferCount);
            Console.WriteLine("Time elapsed: {0}", _stopwatch.Elapsed);
        }

        [TestMethod]
        public async Task TransferUsingLock()
        {
            //Arrange
            _stopwatch = new Stopwatch();
            Console.WriteLine("Before transfer");
            Console.WriteLine(await _controller.GetAccountInformation(_beforeFromAccount.Id, _beforeToAccount.Id));

            // Act
            _stopwatch.Start();
            Parallel.Invoke(
                () => _controller.TransferUsingLock(new BankTransferModel
                {
                    FromAccountId = _beforeFromAccount.Id,
                    ToAccountId = _beforeToAccount.Id,
                    Amount = 500
                }),
                () => _controller.TransferUsingLock(new BankTransferModel
                {
                    FromAccountId = _beforeToAccount.Id,
                    ToAccountId = _beforeFromAccount.Id,
                    Amount = 1000
                }));
            _stopwatch.Stop();

            // Assert
            var context2 = new ApplicationDbContext();
            var afterFromAccount = context2.Accounts.Find(_beforeFromAccount.Id);
            var afterToAccount = context2.Accounts.Find(_beforeToAccount.Id);

            Console.WriteLine("After transfer");
            Console.WriteLine(await _controller.GetAccountInformation(afterFromAccount.Id, afterToAccount.Id));
            Assert.AreEqual(_beforeFromAccount.Balance + 500, afterFromAccount.Balance);
            Assert.AreEqual(1, afterFromAccount.TransferCount);
            Assert.AreEqual(_beforeToAccount.Balance - 500, afterToAccount.Balance);
            Assert.AreEqual(1, afterToAccount.TransferCount);
            Console.WriteLine("Time elapsed: {0}", _stopwatch.Elapsed);
        }
    }
}