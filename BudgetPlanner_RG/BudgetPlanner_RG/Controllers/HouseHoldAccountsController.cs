﻿using System;
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
using WebApiContrib.ModelBinders;

namespace BudgetPlanner_RG.Controllers
{

    [Authorize]
    [RoutePrefix("api/HouseHoldAccounts")]
    public class HouseHoldAccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: api/HouseHoldAccounts - LIST ALL ACCOUNTS FOR THIS USER'S HOUSEHOLD
        [HttpPost,Route("GetAccounts")]
        public IHttpActionResult GetHouseHoldAccounts()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var accounts = user.HouseHold.HouseHoldAccounts.Where(a => a.isArchived == false).ToList();
            
            return Ok(accounts);
            
        }

        // POST: api/HouseHoldAccounts - CREATE ACCOUNT
        [ResponseType(typeof(HouseHoldAccount))]
        [HttpPost, Route("CreateAccount")]
        public async Task<IHttpActionResult> PostHouseHoldAccount(HouseHoldAccount model)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var account = new HouseHoldAccount()
            {
                Name = model.Name,
                Balance = model.Balance,
                HouseHoldId = (int)user.HouseHoldId
            };


            db.HouseHoldAccounts.Add(account);

            var trans = new Transaction()
            {
                Description = "New Account: " + model.Name + " created.",
                Amount = model.Balance,
                HouseHoldAccountId = model.id,
                CategoryId = 1,
                Created = DateTimeOffset.Now,

            };

            db.Transactions.Add(trans);
            
            await db.SaveChangesAsync();

            return Ok(account);
        }

        // POST: api/HouseHoldAccounts/5 - GET ACCOUNT
        [ResponseType(typeof(HouseHoldAccount))]
        [HttpPost, Route("GetAccount")]
        public IHttpActionResult GetHouseHoldAccount(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.HouseHold.HouseHoldAccounts.Where(a => a.id == id && !a.isArchived).FirstOrDefault();

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // POST: api/HouseHoldAccounts/5 - EDIT ACCOUNT

        [HttpPost, Route("EditAccount")]
        public async Task<IHttpActionResult> EditHouseHoldAccount(HouseHoldAccount model)
        {
           
            var oldAccount = db.HouseHoldAccounts.AsNoTracking().FirstOrDefault(a => a.id == model.id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if name changed
            if (oldAccount.Name != model.Name)
            {
                oldAccount.Name = model.Name;
            }

            //check balance
            if (oldAccount.Balance != model.Balance)
            {
                var adjBal = oldAccount.Balance - model.Balance;
                
                db.Transactions.Add(new Transaction()
                {
                    Description = "User Adjusted Balance",
                    Amount = adjBal,
                    CategoryId = 2,
                    Created = DateTimeOffset.Now,
                    HouseHoldAccountId = model.id
                });

                oldAccount.Balance -= adjBal;
                    
            }

            await db.SaveChangesAsync();
            return Ok(oldAccount);
        
        }

        // POST: api/HouseHoldAccounts/5 - ARCHIVE ACCOUNT
        [ResponseType(typeof(HouseHoldAccount))]
        [HttpPost, Route("ArchiveAccount")]
        public async Task<IHttpActionResult> ArchiveAccount(int id)
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            var account = user.HouseHold.HouseHoldAccounts.FirstOrDefault(a => a.id == id);

            if (account == null)
            {
                return NotFound();
            }

            account.isArchived = true;

            foreach (var trans in account.Transactions)
            {
                trans.isArchived = true;
            }
            
            await db.SaveChangesAsync();

            return Ok("The account: " + account.Name + " has been archived.");
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