using FormulaOneTeams.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneTeams.Api.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Team> Teams { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}