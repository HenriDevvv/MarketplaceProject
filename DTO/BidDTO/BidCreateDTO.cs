using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.BidDTO
{
    public class BidCreateDTO
    {
        public Guid UserId { get; set; }
        public Guid AuctionListingId { get; set; }
        public decimal Amount { get; set; }
    }
}
