using DAL.Contracts;
using DAL.UoW;
using Domain.Contracts;
using DTO.SaleDTO;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    internal class SaleDomain : DomainBase, ISaleDomain
    {
        public SaleDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private ISaleRepository _saleRepository => _unitOfWork.GetRepository<ISaleRepository>();
        private IUserRepository _userRepository => 
            _unitOfWork.GetRepository<IUserRepository>();
        private ISaleListingRepository _saleListingRepository => _unitOfWork.GetRepository<ISaleListingRepository>();

        public Guid CreateSale(SaleCreateDTO dto)
        {
            if(_saleRepository.Find(x => x.Listing.Id.Equals(dto.ListingId)).Any())
            {
                throw new Exception("There is already a sale record for this listing");
            }

            var buyer = _userRepository.GetById(dto.BuyerId);
            var listing = _saleListingRepository.GetById(dto.ListingId);

            if (buyer == null)
                throw new Exception("Buyer user could not be found");
            else if (listing == null)
                throw new Exception("Listing could not be found");
            else if (buyer.Balance < listing.Price)
                throw new Exception("Insufficient balance");

                var newSale = new Sale
            {
                Buyer = buyer,
                Time = DateTime.Now,
                Amount = listing.Price,
                Listing = listing
            };
            var savedSale = _saleRepository.Add(newSale);

            buyer.Balance -= newSale.Amount;
            listing.Seller.Balance += newSale.Amount;
            _userRepository.Update(buyer);
            _userRepository.Update(listing.Seller);
            _saleListingRepository.Remove(listing);
            _unitOfWork.Save();

            return savedSale.Id;
        }

        public List<SaleReadDTO> GetAllSales()
        {
            var entities = _saleRepository.GetAll().Include(x => x.Buyer).Include(x => x.Listing).ToList();
            if (!entities.Any())
                return null;

            var dtos = new List<SaleReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new SaleReadDTO
                {
                    Id = entity.Id,
                    BuyerId = entity.Buyer.Id,
                    SellerId = entity.Listing.SellerId,
                    ListingId = entity.Listing.Id,
                    Amount = entity.Amount,
                    TimeOfSale = entity.Time
                });
            }
            return dtos;
        }

        public SaleReadDTO GetSale(Guid id)
        {
            var entity = _saleRepository.Find(x => x.Id.Equals(id)).FirstOrDefault();
            if (entity == null)
                return null;

            var dto = new SaleReadDTO
            {
                Id = entity.Id,
                BuyerId = entity.Buyer.Id,
                SellerId = entity.Listing.SellerId,
                ListingId = entity.Listing.Id,
                Amount = entity.Amount,
                TimeOfSale = entity.Time
            };
            return dto;
        }

        public List<SaleReadDTO> GetSalesByBuyerId(Guid buyerId)
        {
            var entities = _saleRepository.Find(x => x.Buyer.Id.Equals(buyerId));
            if (!entities.Any())
                return null;

            var dtos = new List<SaleReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new SaleReadDTO
                {
                    Id = entity.Id,
                    BuyerId = entity.Buyer.Id,
                    SellerId = entity.Listing.SellerId,
                    ListingId = entity.Listing.Id,
                    Amount = entity.Amount,
                    TimeOfSale = entity.Time
                });
            }
            return dtos;
        }

        public List<SaleReadDTO> GetSalesBySellerId(Guid sellerId)
        {
            var entities = _saleRepository.Find(x => x.Listing.SellerId.Equals(sellerId));
            if (!entities.Any())
                return null;

            var dtos = new List<SaleReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new SaleReadDTO
                {
                    Id = entity.Id,
                    BuyerId = entity.Buyer.Id,
                    SellerId = entity.Listing.SellerId,
                    ListingId = entity.Listing.Id,
                    Amount = entity.Amount,
                    TimeOfSale = entity.Time
                });
            }
            return dtos;
        }
    }
}
