using Domain.Contracts;
using Domain.UoW;
using DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Marketplace.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IDomainUnitOfWork _domainUnitOfWork;
        public UserController(IDomainUnitOfWork domainUnitOfWork)
        {
            _domainUnitOfWork = domainUnitOfWork;
        }
        private IUserDomain _userDomain => _domainUnitOfWork.GetDomain<IUserDomain>();
        // GET: api/<UserController>
        [HttpGet]
        [Route("getAll")]
        public IActionResult Get()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var userDtos = _userDomain.GetAllUsers();
            if(userDtos.Count== 0)
            {
                return NotFound("No users in database");
            }
            return Ok(userDtos);
        }

        [HttpGet]
        [Route("getById/{id}")]
        public IActionResult GetById(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var userDto = _userDomain.GetUserById(id);
            if(userDto == null)
            {
                return NotFound($"No user found with id {id}");
            }
            return Ok(userDto);
        }

        // POST api/<UserController>
        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        public IActionResult Create([FromBody] UserCreateDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (_userDomain.UsernameExists(newUser.Username))
                return BadRequest("Username already exists");
            var newUserId = _userDomain.CreateUser(newUser);
            return CreatedAtAction(nameof(GetById), new { id = newUserId }, _userDomain.GetUserById(newUserId));
        }
        [HttpPost]
        [Route("createAdmin")]
        public IActionResult CreateAdmin([FromBody] UserCreateDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (_userDomain.UsernameExists(newUser.Username))
                return BadRequest("Username already exists");
            var newUserId = _userDomain.CreateAdminUser(newUser);
            return CreatedAtAction(nameof(GetById), new { id = newUserId }, _userDomain.GetUserById(newUserId));
        }
        // PUT api/<UserController>/5
        [HttpPut("Update")]
        public IActionResult Update([FromBody] UserUpdateDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (_userDomain.UpdateUser(user))
            {
                return Ok();
            }
            return BadRequest("User doesn't exist");
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (_userDomain.DeleteUser(id))
            {
                return Ok();
            }
            return BadRequest("User doesn't exist");
        }
    }
}
