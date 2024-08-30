using DAL.Contracts;
using DAL.UoW;
using Domain.Contracts;
using DTO.UserDTO;
using Entities.Models;
using Helpers.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    internal class UserDomain : DomainBase, IUserDomain
    {
        public UserDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private IUserRepository _userRepository => _unitOfWork.GetRepository<IUserRepository>();
        private IRoleRepository _roleRepository => _unitOfWork.GetRepository<IRoleRepository>();

        public bool CheckPassword(UserReadDTO dto, string password)
        {
            return _userRepository.GetById(dto.Id).Password.Equals(password);
        }

        public Guid CreateAdminUser(UserCreateDTO newUser)
        {
            var user = new User()
            {
                Username = newUser.Username,
                Password = newUser.Password,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Balance = 5000,
                Status = (int)DeleteSatus.Active
            };
            var roles = _roleRepository.Find(x => x.Name.Equals("Buyer") || x.Name.Equals("Seller") || x.Name.Equals("Admin")).ToList();
            user.Roles.AddRange(roles);
            var savedUser = _userRepository.Add(user);
            _unitOfWork.Save();
            return savedUser.Id;
        }

        public Guid CreateUser(UserCreateDTO newUser)
        {
            var user = new User() 
            {
                Username = newUser.Username, 
                Password = newUser.Password,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Balance = 5000,
                Status = (int)DeleteSatus.Active
            };
            var roles = _roleRepository.Find(x => x.Name.Equals("Buyer") || x.Name.Equals("Seller")).ToList();
            user.Roles.AddRange(roles);
            var savedUser = _userRepository.Add(user);
            _unitOfWork.Save();
            return savedUser.Id;
        }

        public bool DeleteUser(Guid id)
        {
            var userToBeDeleted = _userRepository.GetById(id);
            if (userToBeDeleted != null)
            {
                _userRepository.Remove(userToBeDeleted);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public List<UserReadDTO> GetAllUsers()
        {
            var userEntities = _userRepository.GetAllUsersWithRoles();
            var userDtos = new List<UserReadDTO>();
            foreach (var userEntity in userEntities)
            {
                var roles = new List<string>();
                foreach (var role in userEntity.Roles)
                {
                    roles.Add(role.Name);
                }
                userDtos.Add(new UserReadDTO()
                {
                    Id = userEntity.Id,
                    Username = userEntity.Username,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Balance = userEntity.Balance,
                    Roles = roles
                });
            }
            return userDtos;
        }

        public UserReadDTO GetUserById(Guid id)
        {
            var userEntity = _userRepository.GetById(id);
            if (userEntity == null)
                return null;
            var roles = new List<string>();
            foreach (var role in userEntity.Roles)
            {
                roles.Add(role.Name);
            }
            var userReadDTO = new UserReadDTO()
            {
                Id = userEntity.Id,
                Username = userEntity.Username,
                Email = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Balance = userEntity.Balance,
                Roles = roles
            };
            return userReadDTO;
        }

        public UserReadDTO GetUserByUsername(string username)
        {
            var userEntity = _userRepository.Find(x => x.Username.Equals(username)).FirstOrDefault();
            if (userEntity == null)
                return null;
            var roles = new List<string>();
            foreach (var role in userEntity.Roles)
            {
                roles.Add(role.Name);
            }
            var userReadDTO = new UserReadDTO()
            {
                Id = userEntity.Id,
                Username = userEntity.Username,
                Email = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Balance = userEntity.Balance,
                Roles = roles
            };
            return userReadDTO;

        }

        public bool UpdateUser(UserUpdateDTO user)
        {
            var existingUser = _userRepository.GetById(user.Id);
            if(existingUser != null)
            {
                existingUser.Username = user.Username; 
                existingUser.Password = user.Password;
                existingUser.Email = user.Email;
                existingUser.FirstName = user.FirstName; 
                existingUser.LastName = user.LastName; 

                _userRepository.Update(existingUser);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public bool UsernameExists(string username)
        {
            return _userRepository.Find(x => x.Username.Equals(username)).Any();
        }
    }
}
