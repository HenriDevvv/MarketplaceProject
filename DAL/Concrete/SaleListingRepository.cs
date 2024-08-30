using DAL.Contracts;
using Entities.Models;
using Helpers.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class SaleListingRepository : BaseRepository<SaleListing, Guid>, ISaleListingRepository
    {
        public SaleListingRepository(MarketplaceContext dbContext) : base(dbContext)
        {
        }
        public override IQueryable<SaleListing> GetAll()
        {
            return context.Where(x => x.Status == (int)DeleteSatus.Active);
        }
        public override SaleListing GetById(Guid id)
        {
            return context.Include(x => x.Seller).FirstOrDefault(x => x.Id.Equals(id) && x.Status == (int)DeleteSatus.Active);
        }
    }
}
