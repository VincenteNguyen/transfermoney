using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ConcurrentTransferMoney;
using ConcurrentTransferMoney.Entities;
using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.Controllers
{
    public class AccountsController : ApiController
    {
        private BankDbContext db = new BankDbContext();

        // GET: api/Accounts
        public IQueryable<Account> GetAccounts()
        {
            return db.Accounts;
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

            db.Accounts.AddRange(newAccounts);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("accounts/transfer")]
        public async Task<IHttpActionResult> Transfer([FromUri]BankTransferModel transferModel)
        {
            
            //db.Accounts.AddRange(newAccounts);
            await db.SaveChangesAsync();

            return Ok();
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> GetAccount(int id)
        {
            Account account = await db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
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

            db.Entry(account).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> PostAccount(Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = account.Id }, account);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> DeleteAccount(int id)
        {
            Account account = await db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            db.Accounts.Remove(account);
            await db.SaveChangesAsync();

            return Ok(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(int id)
        {
            return db.Accounts.Count(e => e.Id == id) > 0;
        }
    }
}