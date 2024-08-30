using Domain.Contracts;
using Domain.UoW;
using DTO.SaleListingDTO;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SaleListingController : ControllerBase
    {
        private IDomainUnitOfWork _domainUnitOfWork;
        public SaleListingController(IDomainUnitOfWork domainUnitOfWork)
        {
            _domainUnitOfWork = domainUnitOfWork;
        }
        private ISaleListingDomain _saleListingDomain => _domainUnitOfWork.GetDomain<ISaleListingDomain>();
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAllListings()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var saleListingDtos = _saleListingDomain.GetAllListings();
            if (saleListingDtos.Any())
            {
                return Ok(saleListingDtos);
            }
            return NotFound("No sale listings in database");
        }

        [HttpGet]
        [Route("getById/{id}")]
        public IActionResult GetListing(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var saleListingDto = _saleListingDomain.GetListing(id);
            if (saleListingDto == null)
                return NotFound($"No listing with id {id}");
            return Ok(saleListingDto);
        }

        [HttpGet]
        [Route("getBySellerId /{id}")]
        public IActionResult GetListingBySellerId(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var saleListingDtos = _saleListingDomain.GetListingsBySellerId(id);
            if (saleListingDtos.Any())
            {
                return Ok(saleListingDtos);
            }
            return NotFound("No sale listings in database");
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([FromBody] SaleListingCreateDTO newSaleListing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var newId = _saleListingDomain.CreateListing(newSaleListing);
            return CreatedAtAction(nameof(GetListing), new {id = newId}, _saleListingDomain.GetListing(newId));
        }
        [HttpPut("update")]
        public IActionResult Update([FromBody] SaleListingUpdateDTO dto)
        {
            if(!ModelState.IsValid)
            { 
                return BadRequest(); 
            }

            if(_saleListingDomain.UpdateListing(dto))
            {
                return Ok();
            }
                
            return BadRequest($"Listing with id {dto.Id} does not exist");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (_saleListingDomain.DeleteListing(id))
            {
                return Ok();
            }
            return BadRequest($"Listing with id {id} does not exist");
        }
    }
}
