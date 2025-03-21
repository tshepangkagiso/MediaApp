using MediaAppWebApi.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace MediaAppWebApi.Data
{
    public class MediaDbContext : DbContext
    {
        public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options) { }

        public DbSet<MediaFile> MediaFiles { get; set; }
    }
}
