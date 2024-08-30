using DAL.Concrete;
using DAL.Contracts;
using Lamar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DI
{
    public class RepositoryRegistry : ServiceRegistry
    {
        public RepositoryRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IRoleRepository>().Use<RoleRepository>();
            For<ISaleRepository>().Use<SaleRepository>();
            For<IUserRepository>().Use<UserRepository>();
            For<ISaleListingRepository>().Use<SaleListingRepository>();
            For<IAuctionListingRepository>().Use<AuctionListingRepository>();
        }   
    }      
}           
