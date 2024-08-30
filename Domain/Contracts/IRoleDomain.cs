using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IRoleDomain
    {
        public List<Role> GetAllRoles();
        public Role GetRole(int id);
        public Role GetRole(string roleName);
    }
}
