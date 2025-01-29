using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ITnetworkProjekt.Models;

namespace ITnetworkProjekt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ITnetworkProjekt.Models.Insurance> Insurance { get; set; } = default!;
        public DbSet<ITnetworkProjekt.Models.InsuredPerson> InsuredPerson { get; set; } = default!;


    }
}
