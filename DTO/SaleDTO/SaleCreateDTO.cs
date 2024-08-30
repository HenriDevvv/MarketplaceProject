using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SaleDTO
{
    public class SaleCreateDTO
    {
        public Guid BuyerId { get; set; }
        public Guid ListingId { get; set; }
    }
}
