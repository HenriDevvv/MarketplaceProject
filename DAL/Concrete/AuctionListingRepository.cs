using DAL.Contracts;
using Entities.Models;
using Helpers.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class AuctionListingRepository : BaseRepository<AuctionListing, Guid>, IAuctionListingRepository
    {
        public AuctionListingRepository(MarketplaceContext dbContext) : base(dbContext)
        {
        }
        public override IQueryable<AuctionListing> GetAll()
        {
            return context.Where(x => x.Status == (int)DeleteSatus.Active).Include(x => x.CurrentHighestBid);
        }
        public override AuctionListing GetById(Guid id)
        {
            return context.Include(x => x.Seller).Include(x => x.CurrentHighestBid).ThenInclude(x => x.User).FirstOrDefault(x => x.Id.Equals(id) && x.Status == (int)DeleteSatus.Active);
        }
        public override IEnumerable<AuctionListing> Find(Expression<Func<AuctionListing, bool>> predicate)
        {
            return context.Where(predicate).Include(x => x.CurrentHighestBid);
        }
    }
}
