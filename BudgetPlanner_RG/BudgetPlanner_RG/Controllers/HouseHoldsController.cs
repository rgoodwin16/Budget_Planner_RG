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
    public class HouseHoldsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HouseHolds
        public IQueryable<HouseHold> GetHouseHolds()
        {
            return db.HouseHolds;
        }

        // GET: api/HouseHolds/5
        [ResponseType(typeof(HouseHold))]
        public async Task<IHttpActionResult> GetHouseHold(int id)
        {
            HouseHold houseHold = await db.HouseHolds.FindAsync(id);
            if (houseHold == null)
            {
                return NotFound();
            }

            return Ok(houseHold);
        }

        // PUT: api/HouseHolds/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutHouseHold(int id, HouseHold houseHold)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != houseHold.id)
            {
                return BadRequest();
            }

            db.Entry(houseHold).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseHoldExists(id))
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

        // POST: api/HouseHolds
        [ResponseType(typeof(HouseHold))]
        public async Task<IHttpActionResult> PostHouseHold(HouseHold houseHold)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HouseHolds.Add(houseHold);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = houseHold.id }, houseHold);
        }

        // DELETE: api/HouseHolds/5
        [ResponseType(typeof(HouseHold))]
        public async Task<IHttpActionResult> DeleteHouseHold(int id)
        {
            HouseHold houseHold = await db.HouseHolds.FindAsync(id);
            if (houseHold == null)
            {
                return NotFound();
            }

            db.HouseHolds.Remove(houseHold);
            await db.SaveChangesAsync();

            return Ok(houseHold);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HouseHoldExists(int id)
        {
            return db.HouseHolds.Count(e => e.id == id) > 0;
        }
    }
}