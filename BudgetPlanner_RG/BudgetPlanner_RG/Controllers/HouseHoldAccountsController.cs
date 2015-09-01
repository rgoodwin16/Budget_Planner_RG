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

namespace BudgetPlanner_RG.Controllers
{
    public class HouseHoldAccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHoldAccounts
        public IQueryable<HouseHoldAccount> GetHouseHoldAccounts()
        {
            return db.HouseHoldAccounts;
        }

        // GET: api/HouseHoldAccounts/5
        [ResponseType(typeof(HouseHoldAccount))]
        public async Task<IHttpActionResult> GetHouseHoldAccount(int id)
        {
            HouseHoldAccount houseHoldAccount = await db.HouseHoldAccounts.FindAsync(id);
            if (houseHoldAccount == null)
            {
                return NotFound();
            }

            return Ok(houseHoldAccount);
        }

        // PUT: api/HouseHoldAccounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutHouseHoldAccount(int id, HouseHoldAccount houseHoldAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != houseHoldAccount.id)
            {
                return BadRequest();
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

        // POST: api/HouseHoldAccounts
        [ResponseType(typeof(HouseHoldAccount))]
        public async Task<IHttpActionResult> PostHouseHoldAccount(HouseHoldAccount houseHoldAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HouseHoldAccounts.Add(houseHoldAccount);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = houseHoldAccount.id }, houseHoldAccount);
        }

        // DELETE: api/HouseHoldAccounts/5
        [ResponseType(typeof(HouseHoldAccount))]
        public async Task<IHttpActionResult> DeleteHouseHoldAccount(int id)
        {
            HouseHoldAccount houseHoldAccount = await db.HouseHoldAccounts.FindAsync(id);
            if (houseHoldAccount == null)
            {
                return NotFound();
            }

            db.HouseHoldAccounts.Remove(houseHoldAccount);
            await db.SaveChangesAsync();

            return Ok(houseHoldAccount);
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