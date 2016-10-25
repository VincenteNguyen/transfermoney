﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ConcurrentTransferMoney.BankTransferService;
using ConcurrentTransferMoney.Entities;
using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.Controllers
{
    public class AccountsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly ProducerConsumerQueue _pcQ = new ProducerConsumerQueue(1);
        private readonly TransferService _transferService = new TransferService();
        // GET: api/Accounts
        public IQueryable<Account> GetAccounts()
        {
            return _db.Accounts;
        }

        [HttpGet]
        [Route("accounts/seed")]
        public async Task<IHttpActionResult> SeedCodeFirst(int numOfAccount)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var newAccounts = new List<Account>(numOfAccount);
            for (var i = 0; i < numOfAccount; i++)
            {
                newAccounts.Add(new Account
                {
                    Balance = random.Next(1, 10)*1000
                });
            }

            _db.Accounts.AddRange(newAccounts);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("accounts/TransferUsingQueueDemo")]
        public async Task<IHttpActionResult> TransferUsingQueueDemo()
        {
            Parallel.For(0, 1000, i =>
            {
                _pcQ.EnqueueTask(() => _transferService.Transfer(1, 2, 100));
                _pcQ.EnqueueTask(() => _transferService.Transfer(2, 1, 99));
            });
            return Ok("Your transfer request will be process");
        }

        [HttpGet]
        [Route("accounts/TransferUsingQueue")]
        public async Task<IHttpActionResult> TransferUsingQueue([FromUri] BankTransferModel transferModel)
        {
            _pcQ.EnqueueTask(
                () =>
                    _transferService.Transfer(transferModel.FromAccountId, transferModel.ToAccountId,
                        transferModel.Amount));
            return Ok("Your transfer request will be process");
        }

        /// <summary>
        ///     Quick check result of two account 1 and 2
        /// </summary>
        /// <returns>string: Account1.Balance Account2.Balance</returns>
        [HttpGet]
        [Route("accounts/QuickCheckResult")]
        public async Task<IHttpActionResult> QuickCheckResult(int id1, int id2)
        {
            var account1 = await _db.Accounts.FindAsync(id1);
            var account2 = await _db.Accounts.FindAsync(id2);
            var resultBuilder = new StringBuilder();
            GetValue(resultBuilder, account1);
            resultBuilder.Append(new String('-', 60));
            GetValue(resultBuilder, account2);
            return Ok(resultBuilder);
        }

        private static void GetValue(StringBuilder resultBuilder, Account account)
        {
            resultBuilder.AppendFormat("Account id {0}: ", account.Id);
            resultBuilder.AppendLine();
            resultBuilder.AppendFormat("Balance: {0}", account.Balance);
            resultBuilder.AppendLine();
            resultBuilder.AppendFormat("Transfer Count: {0}", account.TransferCount);
            resultBuilder.AppendLine();
        }

        // GET: api/Accounts/5
        [ResponseType(typeof (Account))]
        public async Task<IHttpActionResult> GetAccount(int id)
        {
            var account = await _db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof (void))]
        public async Task<IHttpActionResult> PutAccount(int id, Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != account.Id)
            {
                return BadRequest();
            }

            _db.Entry(account).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof (Account))]
        public async Task<IHttpActionResult> PostAccount(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new {id = account.Id}, account);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof (Account))]
        public async Task<IHttpActionResult> DeleteAccount(int id)
        {
            var account = await _db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();

            return Ok(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(int id)
        {
            return _db.Accounts.Count(e => e.Id == id) > 0;
        }
    }
}