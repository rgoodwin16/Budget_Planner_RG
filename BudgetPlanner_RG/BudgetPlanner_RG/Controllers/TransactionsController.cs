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
    [Authorize]
    [RoutePrefix("api/HouseHoldAccounts/Transactions")]
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHoldAccounts/Transactions - GET ALL TRANSACTIONS FOR THIS HOUSEHOLD ACCOUNT
        [HttpPost, Route("Index")]
        public IHttpActionResult Index(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            try
            {
                var transactions = db.HouseHoldAccounts.Where(a => a.HouseHoldId == user.HouseHoldId && a.id == id).FirstOrDefault().Transactions;
                return Ok(transactions);
            }

            catch(NullReferenceException)
            {
                return Ok("No transactions found.");
            }
 
        }

        // POST: api/HouseHoldAccounts/Transactions - CREATE TRANSACTION
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Create")]
        public async Task<IHttpActionResult> Create(Transaction trans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            trans.Created = DateTimeOffset.Now;

            var account = db.HouseHoldAccounts.Find(trans.HouseHoldAccountId);

            if (trans.Type == "Credit")
            {
                account.Balance = account.Balance + trans.Amount;
            }

            else
            {
                account.Balance = account.Balance - trans.Amount;
            }

            db.Transactions.Add(trans);
            await db.SaveChangesAsync();

            return Ok(trans);
        }

        // GET: api/HouseHoldAccounts/Transactions/5 - GET TRANSACTION
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Details")]
        public async Task<IHttpActionResult> Details(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var transaction = await db.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        //POST: api/HouseHoldAccounts/Transactions/5 - EDIT TRANSACTION
        [HttpPost, Route("Edit")]
        public async Task<IHttpActionResult> Edit(Transaction model)
        {
            var oldTrans = db.Transactions.FirstOrDefault(t => t.id == model.id);
            var account = db.HouseHoldAccounts.Find(model.HouseHoldAccountId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if descriptio has changed
            if (oldTrans.Description != model.Description)
            {
                oldTrans.Description = model.Description;
                await db.SaveChangesAsync();
            }

            //check if amount has changed
            if (oldTrans.Amount != model.Amount)
            {
                if (model.Type == "Debit")
                {
                    account.Balance = account.Balance + oldTrans.Amount;
                    var newBalance = account.Balance - model.Amount;
                    account.Balance = newBalance;

                    oldTrans.Amount = model.Amount;

                    await db.SaveChangesAsync();
                }

                else if(model.Type == "Credit")
                {
                    account.Balance -= oldTrans.Amount;
                    account.Balance += model.Amount;

                    oldTrans.Amount = model.Amount;
                    await db.SaveChangesAsync();
                }

                await db.SaveChangesAsync();
            }

            //check if category changed
            if (oldTrans.CategoryId != model.CategoryId)
            {
                oldTrans.CategoryId = model.CategoryId;
                await db.SaveChangesAsync();
            }

            //check reconcile
            if (oldTrans.Reconcile != model.Reconcile)
            {
                oldTrans.Reconcile = model.Reconcile;
                await db.SaveChangesAsync();
            }

            oldTrans.Updated = DateTimeOffset.Now;
            await db.SaveChangesAsync();

            return Ok(oldTrans);
        }

        // DELETE: api/Transactions/5 - DELETE TRANSACTION
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.HouseHold.HouseHoldAccounts.FirstOrDefault(a => a.id == transaction.HouseHoldAccountId);

            if (transaction == null)
            {
                return Ok("No transaction found.");
            }

            if (transaction.Type == "Debit")
            {
                account.Balance = account.Balance + transaction.Amount;
                await db.SaveChangesAsync();
            }

            else if (transaction.Type == "Credit")
            {
                account.Balance = account.Balance - transaction.Amount;
                await db.SaveChangesAsync();
            }

            
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