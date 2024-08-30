using Domain.Concrete;
using Domain.Contracts;
using Domain.SchedulingAndProcessing.Concrete;
using Domain.SchedulingAndProcessing.Contracts;
using Lamar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DI
{
    public class DomainRegistry : ServiceRegistry
    {
        public DomainRegistry()
        {
            IncludeRegistry<DomainUnitOfWorkRegistry>();
            For<IUserDomain>().Use<UserDomain>();
            For<IRoleDomain>().Use<RoleDomain>();
            For<ISaleListingDomain>().Use<SaleListingDomain>();
            For<IAuctionListingDomain>().Use<AuctionListingDomain>();
            For<ISaleDomain>().Use<SaleDomain>();
            For<IAuctionProcessor>().Use<AuctionProcessor>();
            For<IAuctionScheduler>().Use<AuctionScheduler>();
        }
    }
}
