using Athos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Athos.Api.Data
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

        public DbSet<Review> Reviews { get; set; }
    }
}
