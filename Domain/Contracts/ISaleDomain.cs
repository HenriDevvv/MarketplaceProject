using DTO.SaleDTO;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface ISaleDomain
    {
        public List<SaleReadDTO> GetAllSales();
        public List<SaleReadDTO> GetSalesBySellerId(Guid sellerId);
        public List<SaleReadDTO> GetSalesByBuyerId(Guid buyerId);
        public SaleReadDTO GetSale(Guid id);
        public Guid CreateSale(SaleCreateDTO dto);
    }
}
