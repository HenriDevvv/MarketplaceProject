using Domain.Contracts;
using Domain.UoW;
using DTO.AuctionListingDTO;
using DTO.BidDTO;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Marketplace.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionListingController : ControllerBase
    {
        private IDomainUnitOfWork _domainUnitOfWork;
        public AuctionListingController(IDomainUnitOfWork domainUnitOfWork)
        {
            _domainUnitOfWork = domainUnitOfWork;
        }
        private IAuctionListingDomain _auctionListingDomain => _domainUnitOfWork.GetDomain<IAuctionListingDomain>();
        private Timer? _checker;
        [HttpGet]
        [Route("GetAll")]
       
        
        public IActionResult GetAll()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dtos = _auctionListingDomain.GetAllListings();
            if(dtos == null)
            {
                return NotFound("No auction listings in database");
            }
            return Ok(dtos);
        }
        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult Get(Guid id) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dto = _auctionListingDomain.GetListing(id);
            if(dto == null)
            {
                return NotFound($"No listing with id {id}");
            } 
            return Ok(dto);
        }

        [HttpGet]
        [Route("getBySellerId/{id}")]
        public IActionResult GetListingsBySellerId(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dtos = _auctionListingDomain.GetListingsBySellerId(id);
            if (dtos == null)
            {
                return NotFound("No auction listings in database");
            }
            return Ok(dtos);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] AuctionListingCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var newId = _auctionListingDomain.CreateListing(dto);
            //var timeSpan = dto.EndTime.Subtract(DateTime.Now);
            //_checker = new Timer(ob => EndAuction(newId), null, (long)timeSpan.TotalMilliseconds, (long)Timeout.Infinite);

            return CreatedAtAction(nameof(Get), new {id = newId}, _auctionListingDomain.GetListing(newId));
        }
        [HttpPut]
        [Route("Update")]
        public IActionResult Update([FromBody] AuctionListingUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                if (_auctionListingDomain.UpdateListing(dto))
                    return Ok();
                else
                    return NotFound($"No listing with id {dto.Id} was found!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (_auctionListingDomain.DeleteListing(id))
            {
                return Ok();
            }
            return NotFound($"No listing with id {id} was found!");
        }

        [HttpPost]
        [Route("Bid")]
        public IActionResult CreateBid([FromBody] BidCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                if (_auctionListingDomain.CreateBid(dto))
                    return Ok();
                else
                    return BadRequest("Amount is less than the minimum required to place the bid");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        private void EndAuction(Guid auctionId)
        {
            _auctionListingDomain.EndAuction(auctionId);
        }
    }
}
