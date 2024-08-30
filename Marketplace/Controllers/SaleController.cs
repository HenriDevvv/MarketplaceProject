using Domain.Contracts;
using Domain.UoW;
using DTO.SaleDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private IDomainUnitOfWork _domainUnitOfWork;
        public SaleController(IDomainUnitOfWork domainUnitOfWork)
        {
            _domainUnitOfWork = domainUnitOfWork;
        }
        private ISaleDomain _saleDomain => _domainUnitOfWork.GetDomain<ISaleDomain>();

        [HttpGet]
        [Route("GetAllSales")]
        public IActionResult GetAllSales()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dtos = _saleDomain.GetAllSales();
            if (dtos == null)
            {
                return NotFound("No sales in database");
            }
            return Ok(dtos);
        }

        [HttpGet]
        [Route("GetSalesBySellerId/{id}")]
        public IActionResult GetSalesBySellerId(Guid id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dtos = _saleDomain.GetSalesBySellerId(id);
            if(dtos == null)
            {
                return NotFound($"No sales in database with seller ID {id}");
            }
            return Ok(dtos);
        }

        [HttpGet]
        [Route("GetSalesByBuyerId/{id}")]
        public IActionResult GetSalesByBuyerId(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dtos = _saleDomain.GetSalesByBuyerId(id);
            if (dtos == null)
            {
                return NotFound($"No sales in database with buyer ID {id}");
            }
            return Ok(dtos);
        }

        [HttpGet]
        [Route("GetSale/{id}")]
        public IActionResult GetSale(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var dto = _saleDomain.GetSale(id);
            if(dto == null)
            {
                return NotFound($"No sale in database with ID {id}");
            }
            return Ok(dto);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] SaleCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var newSaleId = _saleDomain.CreateSale(dto);
                return CreatedAtAction(nameof(GetSale), new {id = newSaleId }, _saleDomain.GetSale(newSaleId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
