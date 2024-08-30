using DAL.Contracts;
using Entities.Models;
using Helpers.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class UserRepository : BaseRepository<User, Guid>, IUserRepository
    {
        public UserRepository(MarketplaceContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<User> GetAllUsersWithRoles()
        {
            return GetAll().Where(x => x.Status == (int)DeleteSatus.Active).Include(u => u.Roles).ToList();
        }
        public override User GetById(Guid id)
        {
            return context.Include(x => x.Roles).FirstOrDefault(x => x.Id.Equals(id) && x.Status == (int)DeleteSatus.Active);
        }
    }
}
