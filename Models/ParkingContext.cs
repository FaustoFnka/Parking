using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parking.Models;
using Microsoft.EntityFrameworkCore;

namespace Parking.Models
{
    public class ParkingContext: DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options)
        {
        }
        public DbSet<PriceTable> PriceTable { get; set; }
        public DbSet<VehicleControl> VehicleControl { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PriceTable>().HasKey(m => m.Id);
            base.OnModelCreating(builder);
            builder.Entity<VehicleControl>().HasKey(m => m.Id);
            base.OnModelCreating(builder);
        }
    }
}