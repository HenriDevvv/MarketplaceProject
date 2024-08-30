using DTO.SaleListingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ISaleListingDomain
    {
        public List<SaleListingReadDTO> GetAllListings();
        public List<SaleListingReadDTO> GetListingsBySellerId(Guid sellerId);
        public SaleListingReadDTO GetListing(Guid id);
        public Guid CreateListing(SaleListingCreateDTO dto);
        public bool UpdateListing(SaleListingUpdateDTO dto);
        public bool DeleteListing(Guid id);
    }
}
