using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BudgetPlanner_RG.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public string DisplayName { get; set; }
        public int? HouseHoldId  { get; set; }

        public virtual HouseHold HouseHold { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BudgetPlanner_RG.Models.BudgetItem> BudgetItems { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_RG.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_RG.Models.HouseHold> HouseHolds { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_RG.Models.HouseHoldAccount> HouseHoldAccounts { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_RG.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<BudgetPlanner_RG.Models.Invitation> Invitations { get; set; }
    }
}