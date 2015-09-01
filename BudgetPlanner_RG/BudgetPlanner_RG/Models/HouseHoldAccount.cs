using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner_RG.Models
{
    public class HouseHoldAccount
    {
        public HouseHoldAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        
        public int id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public int HoseHoldId { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public virtual HouseHold HouseHold { get; set; }
    }
}