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
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Transactions - GET ALL TRANSACTIONS FOR THIS HOUSEHOLD ACCOUNT
        public IHttpActionResult GetTransactions(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var transactions = db.HouseHoldAccounts.Where(a => a.HouseHoldId == user.HouseHoldId && a.id == id).FirstOrDefault().Transactions;

            return Ok(transactions);
        }

        // POST: api/Transactions - CREATE TRANSACTION
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> PostTransaction(Transaction trans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            trans.Created = DateTimeOffset.Now;

            db.Transactions.Add(trans);

            var account = db.HouseHoldAccounts.Find(trans.HouseHoldAccountId);

            account.Balance = account.Balance + trans.Amount;

            await db.SaveChangesAsync();

            return Ok(trans);
        }

        // GET: api/Transactions/5 - GET TRANSACTION
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransaction(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT: api/Transactions/5 - EDIT TRANSACTION
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransaction(int id, string desc, string status, decimal ? amount, int catId, bool rec)
        {
            var trans = db.Transactions.Find(id);
            var bal = trans.HouseHoldAccount.Balance;
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trans.id)
            {
                return BadRequest();
            }

            if (!string.IsNullOrWhiteSpace(desc))
            {
                trans.Description = desc;
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                trans.Status = status;
            }

            if (amount != null)
            {
                bal -= trans.Amount;
                trans.Amount = (int)amount;
                bal += (int)amount;
            }

            if (catId != null)
            {
                trans.CategoryId = catId;
            }

            if (trans.Reconcile != rec)
            {
                trans.Reconcile = rec;
            }

            trans.Updated = DateTimeOffset.Now;

            await db.SaveChangesAsync();
            return Ok(trans);


        }

        // DELETE: api/Transactions/5 - DELETE TRANSACTION
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.HouseHold.HouseHoldAccounts.FirstOrDefault(a => a.id == transaction.HouseHoldAccountId);

            if (transaction == null)
            {
                return NotFound();
            }

            account.Balance -= transaction.Amount;

            db.Transactions.Remove(transaction);
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

        private bool TransactionExists(int id)
        {
            return db.Transactions.Count(e => e.id == id) > 0;
        }
    }
}