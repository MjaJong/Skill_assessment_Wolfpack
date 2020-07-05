using Microsoft.EntityFrameworkCore;

namespace WolfAPI.Models
{
    /// <summary>
    /// Database context for the API.
    /// </summary>
    public class WolfContext : DbContext
    {
        public WolfContext(DbContextOptions<WolfContext> options) : base(options)
        {
        }

        public DbSet<WolfItem> WolfItems { get; set; }
    }
}
