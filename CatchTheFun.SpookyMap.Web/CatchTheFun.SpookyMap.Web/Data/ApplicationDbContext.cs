using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CatchTheFun.SpookyMap.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EventLocation> EventLocations { get; set; }
    }
}
