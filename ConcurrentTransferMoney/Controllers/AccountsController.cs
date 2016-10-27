using System;
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
        private readonly ProducerConsumerQueue _pcQ = new ProducerConsumerQueue();
        private readonly TransferService _transferService = new TransferService();

        public AccountsController(ApplicationDbContext dbContext)
        {
            _db = dbContext ?? _db;
        }

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
        public async Task<IHttpActionResult> TransferUsingQueueDemo([FromUri] BankTransferModel transferModel,
            int numOfTransfer)
        {
            if (numOfTransfer == 0) numOfTransfer = 1000;
            Parallel.For(0, numOfTransfer, i =>
            {
                _pcQ.EnqueueTask(
                    () =>
                        _transferService.Transfer(transferModel.FromAccountId, transferModel.ToAccountId,
                            transferModel.Amount));
                _pcQ.EnqueueTask(
                    () =>
                        _transferService.Transfer(transferModel.ToAccountId, transferModel.FromAccountId,
                            transferModel.Amount));
            });
            _pcQ.Dispose();
            _pcQ.ConsumeTask.Wait();
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

        [HttpGet]
        [Route("accounts/TransferUsingLockDemo")]
        public async Task<IHttpActionResult> TransferUsingLockDemo([FromUri] BankTransferModel transferModel,
            int numOfTransfer)
        {
            if (numOfTransfer == 0) numOfTransfer = 1000;
            var previousBuilder = await GetAccountInformation(transferModel.FromAccountId, transferModel.ToAccountId);
            Parallel.For(0, numOfTransfer, i =>
            {
                _transferService.TransferUsingLock(transferModel.FromAccountId, transferModel.ToAccountId,
                    transferModel.Amount);
                _transferService.TransferUsingLock(transferModel.ToAccountId, transferModel.FromAccountId,
                    transferModel.Amount);
            });
            var afterBuilder = await GetAccountInformation(transferModel.FromAccountId, transferModel.ToAccountId);
            return Ok(previousBuilder + afterBuilder);
        }

        [HttpGet]
        [Route("accounts/TransferUsingLock")]
        public async Task<IHttpActionResult> TransferUsingLock([FromUri] BankTransferModel transferModel)
        {
            _transferService.TransferUsingLock(transferModel.FromAccountId, transferModel.ToAccountId,
                transferModel.Amount);
            var result = await GetAccountInformation(transferModel.FromAccountId, transferModel.ToAccountId);
            return Ok(result);
        }

        /// <summary>
        ///     Quick check result of two account 1 and 2. Used for Producer Consumer queue
        /// </summary>
        /// <returns>string: Account1.Balance Account2.Balance</returns>
        [HttpGet]
        [Route("accounts/QuickCheckResult")]
        public async Task<IHttpActionResult> QuickCheckResult(int id1, int id2)
        {
            var resultBuilder = await GetAccountInformation(id1, id2);
            return Ok(resultBuilder);
        }

        public async Task<string> GetAccountInformation(int id1, int id2)
        {
            using (var context = new ApplicationDbContext())
            {
                var account1 = await context.Accounts.FindAsync(id1);
                var account2 = await context.Accounts.FindAsync(id2);
                var resultBuilder = new StringBuilder();
                resultBuilder.Append(BuildAccountInformation(account1));
                resultBuilder.Append(new String('-', 20));
                resultBuilder.AppendLine();
                resultBuilder.Append(BuildAccountInformation(account2));
                return resultBuilder.ToString();
            }
        }

        private static string BuildAccountInformation(Account account)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.AppendFormat("Account id {0}: ", account.Id);
            resultBuilder.AppendLine();
            resultBuilder.AppendFormat("Balance: {0}", account.Balance);
            resultBuilder.AppendLine();
            resultBuilder.AppendFormat("Transfer Count: {0}", account.TransferCount);
            resultBuilder.AppendLine();
            return resultBuilder.ToString();
        }

        #region Accounts CRUD by EF

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

        #endregion
    }
}