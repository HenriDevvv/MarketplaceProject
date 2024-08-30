using DAL.Contracts;
using DAL.UoW;
using Domain.Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    internal class RoleDomain : DomainBase, IRoleDomain
    {
        public RoleDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private IRoleRepository _roleRepository => _unitOfWork.GetRepository<IRoleRepository>();

        public List<Role> GetAllRoles()
        {
            return _roleRepository.GetAll().ToList();
        }

        public Role GetRole(int id)
        {
            return _roleRepository.GetById(id);
        }

        public Role GetRole(string roleName)
        {
            return _roleRepository.Find(x => x.Name == roleName).FirstOrDefault(); 
        }
    }
}
