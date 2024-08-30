using DAL.Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class SaleRepository : BaseRepository<Sale, Guid>, ISaleRepository
    {
        public SaleRepository(MarketplaceContext dbContext) : base(dbContext)
        {
        }
        public override IEnumerable<Sale> Find(Expression<Func<Sale, bool>> predicate)
        {
            return context.Where(predicate).Include(x => x.Buyer).Include(x => x.Listing);
        }
    }
}
