using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BudgetPlanner_RG.Models
{
    public class Invitation
    {
        public string Code { get; set; }

        [Key, Column(Order=0)]
        public int? HouseHoldId { get; set; }

        [Key, Column(Order=1)]
        public string InvitedEmail { get; set; }
    }
}