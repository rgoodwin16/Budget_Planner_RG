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
    [RoutePrefix("api/BudgetItems")]
    public class BudgetItemsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: api/BudgetItems - GET ALL BUDGET ITEMS FOR THIS HOUSEHOLD
        [HttpPost,Route("Index")]
        public IHttpActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            try
            {
                var bItems = user.HouseHold.BudgetItems.ToList();
                return Ok(bItems);
            }

            catch (NullReferenceException)
            {
                return Ok("No Budget Items found.");
            }
            
        }

        // POST: api/BudgetItems - CREATE BUDGET ITEM
        [ResponseType(typeof(BudgetItem))]
        [HttpPost, Route("Create")]
        public async Task<IHttpActionResult> Create(BudgetItem model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var budgetItemExits = user.HouseHold.BudgetItems.Any(bi => bi.Name == model.Name);

            if (budgetItemExits) 
            {
                return Ok("You already have a budget item called: " + model.Name + " . Please chose another name.");
            }

            else
            {
                var budgetItem = new BudgetItem()
                {
                    Name = model.Name,
                    Amount = model.Amount,
                    HouseHoldId = (int)user.HouseHoldId,
                    CategoryId = model.CategoryId,
                    Frequency = model.Frequency
                };

                db.BudgetItems.Add(budgetItem);
                await db.SaveChangesAsync();

                return Ok(budgetItem);
            }

        }

        // GET: api/BudgetItems/5 - GET BUDGET ITEM
        [ResponseType(typeof(BudgetItem))]
        [HttpPost,Route("Details")]
        public IHttpActionResult Details(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var budgetItem = user.HouseHold.BudgetItems.FirstOrDefault(bi => bi.id == id);

            if (budgetItem == null)
            {
                return NotFound();
            }

            return Ok(budgetItem);
        }

        // PUT: api/BudgetItems/5 - EDIT BUDGET ITEM
        [ResponseType(typeof(void))]
        [HttpPost,Route("Edit")]
        public async Task<IHttpActionResult> Edit(int id, BudgetItem model)
        {
            var oldBudgetItem = db.BudgetItems.FirstOrDefault(bi => bi.id == model.id);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if name changed
            if (oldBudgetItem.Name != model.Name)
            {
                oldBudgetItem.Name = model.Name;
                await db.SaveChangesAsync();
            }

            //check if amount changed
            if (oldBudgetItem.Amount != oldBudgetItem.Amount)
            {
                oldBudgetItem.Amount = model.Amount;
                await db.SaveChangesAsync();
            }

            //check if frquency changed
            if (oldBudgetItem.Frequency != model.Frequency)
            {
                oldBudgetItem.Frequency = model.Frequency;
                await db.SaveChangesAsync();
            }

            return Ok(oldBudgetItem);

        }

        // DELETE: api/BudgetItems/5 - DELETE BUDGET ITEM
        [ResponseType(typeof(BudgetItem))]
        [HttpPost,Route("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            BudgetItem budgetItem = await db.BudgetItems.FindAsync(id);
            if (budgetItem == null)
            {
                return NotFound();
            }

            db.BudgetItems.Remove(budgetItem);
            await db.SaveChangesAsync();

            return Ok(budgetItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BudgetItemExists(int id)
        {
            return db.BudgetItems.Count(e => e.id == id) > 0;
        }
    }
}