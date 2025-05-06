using Athos.ReviewAutomation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Athos.ReviewAutomation.Infrastructure.Data
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

        public DbSet<DbReview> Reviews { get; set; } = null!;
    }
}
