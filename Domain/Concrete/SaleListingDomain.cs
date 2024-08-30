using DAL.Contracts;
using DAL.UoW;
using Domain.Contracts;
using DTO.SaleListingDTO;
using Entities.Models;
using Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    internal class SaleListingDomain : DomainBase, ISaleListingDomain
    {
        public SaleListingDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private ISaleListingRepository _saleListingRepository => _unitOfWork.GetRepository<ISaleListingRepository>();
        private IUserRepository _userRepository => _unitOfWork.GetRepository<IUserRepository>();

        public Guid CreateListing(SaleListingCreateDTO dto)
        {
            var seller = _userRepository.GetById(dto.SellerId);
            var newListing = new SaleListing
            {
                Name = dto.Name,
                Description = dto.Description,
                SellerId = dto.SellerId,
                Seller = seller,
                Status = (int)DeleteSatus.Active,
                Price = dto.Price
            };
            var listing = _saleListingRepository.Add(newListing);
            _unitOfWork.Save();
            return listing.Id;
        }

        public bool DeleteListing(Guid id)
        {
            var entity = _saleListingRepository.GetById(id);
            if(entity == null)
            {
                return false;
            }
            _saleListingRepository.Remove(entity);
            _unitOfWork.Save();
            return true;
        }

        public List<SaleListingReadDTO> GetAllListings()
        {
            List<SaleListing> entities = _saleListingRepository.GetAll().ToList();
            var dtos = new List<SaleListingReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new SaleListingReadDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    SellerId = entity.SellerId,
                    Price = entity.Price
                });
            }
            return dtos;
        }

        public SaleListingReadDTO GetListing(Guid id)
        {
            var entity = _saleListingRepository.GetById(id);
            if (entity == null)
                return null;
            var dto = new SaleListingReadDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                SellerId = entity.SellerId,
                Price = entity.Price
            };
            return dto;
        }

        public List<SaleListingReadDTO> GetListingsBySellerId(Guid sellerId)
        {
            var entities = _saleListingRepository.Find(x => x.SellerId.Equals(sellerId));
            var dtos = new List<SaleListingReadDTO>();
            foreach (var entity in entities)
            {
                dtos.Add(new SaleListingReadDTO
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    SellerId = entity.SellerId,
                    Price = entity.Price
                });
            }
            return dtos;
        }

        public bool UpdateListing(SaleListingUpdateDTO dto)
        {
            var entity = _saleListingRepository.GetById(dto.Id);
            if (entity == null) 
                return false;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            _saleListingRepository.Update(entity);
            _unitOfWork.Save();
            return true;
        }
    }
}
