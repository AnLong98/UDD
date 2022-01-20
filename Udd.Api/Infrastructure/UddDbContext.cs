using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Models;

namespace Udd.Api.Infrastructure
{
    public class UddDbContext : DbContext
    {
        public DbSet<City> Cities { get; set; }

        public UddDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UddDbContext).Assembly);
        }
    }
}
