using DAL.Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    internal class RoleRepository : BaseRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(MarketplaceContext dbContext) : base(dbContext)
        {
        }
    }
}
