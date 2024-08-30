using DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IUserDomain
    {
        public List<UserReadDTO> GetAllUsers();
        public UserReadDTO GetUserById(Guid id);
        public bool UsernameExists(string username);
        public Guid CreateUser(UserCreateDTO newUser);
        public Guid CreateAdminUser(UserCreateDTO newUser);
        public bool UpdateUser(UserUpdateDTO user);
        public bool DeleteUser(Guid id);
        public UserReadDTO GetUserByUsername(string username);
        public bool CheckPassword(UserReadDTO dto, string password);
    }
}
