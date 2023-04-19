using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ISFMOCM_Project.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        //public virtual DbSet<AccountModel> Accounts { get; set; }
        //public virtual DbSet<AccountChapterModel> AccountChapters { get; set; }
        //public virtual DbSet<AccountTypeModel> AccountTypes { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        //public System.Data.Entity.DbSet<ISFMOCM_Project.Models.UnitModel> UnitModels { get; set; }

        //public System.Data.Entity.DbSet<ISFMOCM_Project.Models.InitialBudgetModel> InitialBudgetModels { get; set; }

        //public System.Data.Entity.DbSet<ISFMOCM_Project.Models.TransferModel> TransferModels { get; set; }
    }
}