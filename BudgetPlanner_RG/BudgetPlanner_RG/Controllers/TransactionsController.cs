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
using BudgetPlanner_RG.Libraries;

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

            if (trans.isDebit)
            {
                account.Balance = account.Balance - trans.Amount;
            }

            else
            {
                account.Balance = account.Balance + trans.Amount;
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var oldTrans = db.Transactions.AsNoTracking().FirstOrDefault(t => t.id == model.id);

            if (model.isDebit)
            {
                if (model.Amount > 0)
                    model.Amount *= -1;
            }
            else
                if (model.Amount < 0)
                    model.Amount *= -1;

            //check if amount has changed
            if (oldTrans.Amount != model.Amount)
            {
                var account = db.HouseHoldAccounts.Find(model.HouseHoldAccountId);
                account.Balance -= oldTrans.Amount;
                account.Balance += model.Amount;
            }

            model.Updated = DateTimeOffset.Now;
            
            db.Update<Transaction>(model, "Amount", "Reconcile", "CategoryId", "Description", "isDebit");

            await db.SaveChangesAsync();

            return Ok(model);
        }

        // DELETE: api/Transactions/5 - DELETE TRANSACTION
        [ResponseType(typeof(Transaction))]
        [HttpPost, Route("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var transaction = await db.Transactions.FindAsync(id);
            
            if (transaction == null)
            {
                return Ok("No transaction found.");
            }

            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.HouseHold.HouseHoldAccounts.FirstOrDefault(a => a.id == transaction.HouseHoldAccountId);


            if (transaction.isDebit)
            {
                account.Balance += transaction.Amount;
            }

            else 
            {
                account.Balance -= transaction.Amount;
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