using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SaleListingDTO
{
    public class SaleListingCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SellerId { get; set; }
        public decimal Price { get; set; }
    }
}
