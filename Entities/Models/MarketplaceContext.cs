using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class MarketplaceContext : DbContext
    {
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AuctionListing> AuctionListings { get; set; } = null!;
        public DbSet<SaleListing> SaleListings { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
    }
}
