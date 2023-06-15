using FormulaOneTeams.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneTeams.Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}