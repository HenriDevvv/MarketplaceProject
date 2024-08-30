using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SaleDTO
{
    public class SaleReadDTO
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }
        public Guid ListingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TimeOfSale { get; set; }
    }
}
