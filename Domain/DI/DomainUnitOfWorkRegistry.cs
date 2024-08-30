using Domain.UoW;
using Lamar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DI
{
    internal class DomainUnitOfWorkRegistry : ServiceRegistry
    {
        public DomainUnitOfWorkRegistry()
        {
            For<IDomainUnitOfWork>().Use<DomainUnitOfWork>();
        }
    }
}
