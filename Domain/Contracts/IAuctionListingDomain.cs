using DTO.AuctionListingDTO;
using DTO.BidDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IAuctionListingDomain
    {
        public List<AuctionListingReadDTO> GetAllListings();
        public AuctionListingReadDTO GetListing(Guid id);
        public List<AuctionListingReadDTO> GetListingsBySellerId(Guid sellerId);
        public Guid CreateListing(AuctionListingCreateDTO dto);
        public bool UpdateListing(AuctionListingUpdateDTO dto);
        public bool DeleteListing(Guid id);
        public bool CreateBid(BidCreateDTO dto);
        public void EndAuction(Guid auctionId);
    }
}
