using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BudgetPlanner_RG.Models;
using Microsoft.AspNet.Identity;

namespace BudgetPlanner_RG.Controllers
{
    public class HouseHoldAccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHoldAccounts - LIST ALL ACCOUNTS FOR THIS USER'S HOUSEHOLD
        public IHttpActionResult GetHouseHoldAccounts()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var accounts = user.HouseHold.HouseHoldAccounts.ToList();

            return Ok(accounts);
            
        }

        // POST: api/HouseHoldAccounts - CREATE ACCOUNT
        [ResponseType(typeof(HouseHoldAccount))]
        public async Task<IHttpActionResult> PostHouseHoldAccount(HouseHoldAccount houseHoldAccount, string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = db.Users.Find(User.Identity.GetUserId());

            var account = new HouseHoldAccount()
            {
                Name = name,
                Balance = 0,
                HouseHoldId = (int)user.HouseHoldId
            };


            db.HouseHoldAccounts.Add(houseHoldAccount);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = houseHoldAccount.id }, houseHoldAccount);
        }

        // GET: api/HouseHoldAccounts/5 - GET ACCOUNT
        [ResponseType(typeof(HouseHoldAccount))]
        public async Task<IHttpActionResult> GetHouseHoldAccount(int id)
        {
            HouseHoldAccount houseHoldAccount = await db.HouseHoldAccounts.FindAsync(id);
            if (houseHoldAccount == null || houseHoldAccount.isArchived)
            {
                return NotFound();
            }

            return Ok(houseHoldAccount);
        }

        // PUT: api/HouseHoldAccounts/5 - EDIT ACCOUNT
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutHouseHoldAccount(int id, HouseHoldAccount houseHoldAccount, string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != houseHoldAccount.id)
            {
                return BadRequest();
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                houseHoldAccount.Name = name;
            }

            db.Entry(houseHoldAccount).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseHoldAccountExists(id))
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

        // DELETE: api/HouseHoldAccounts/5 - ARCHIVE ACCOUNT
        [ResponseType(typeof(HouseHoldAccount))]
        public async Task<IHttpActionResult> ArchiveHouseHoldAccount(int id)
        {
            HouseHoldAccount houseHoldAccount = await db.HouseHoldAccounts.FindAsync(id);

            if (houseHoldAccount == null)
            {
                return NotFound();
            }

            houseHoldAccount.isArchived = true;

            foreach (var trans in houseHoldAccount.Transactions)
            {
                trans.isArchived = true;
            }

            db.HouseHoldAccounts.Remove(houseHoldAccount);
            await db.SaveChangesAsync();

            return Ok(houseHoldAccount);
        }

        // PUT: api/HouseHoldAccounts/5 - ADJUST ACCOUNT BALANCE
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AdjustBalance(int id, decimal newBalance)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.HouseHold.HouseHoldAccounts.FirstOrDefault(a => a.id == id);

            var adjBal = account.Balance - newBalance;
            db.Transactions.Add(new Transaction()

            {
                Description = "User Adjusted Balance",
                Amount = adjBal,
                CategoryId = 1,
                Created = DateTimeOffset.Now,
                HouseHoldAccountId = id

            });

            account.Balance -= adjBal;
            await db.SaveChangesAsync();
            return Ok();

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HouseHoldAccountExists(int id)
        {
            return db.HouseHoldAccounts.Count(e => e.id == id) > 0;
        }
    }
}