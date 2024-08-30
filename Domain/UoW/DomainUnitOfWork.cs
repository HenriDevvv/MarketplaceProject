using System;
using Lamar;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UoW
{
    internal class DomainUnitOfWork : IDomainUnitOfWork
    {
        private readonly IContainer _container;

        public DomainUnitOfWork(IContainer container)
        {
            _container = container;
        }
        public TDomain GetDomain<TDomain>() where TDomain : class
        {
            return _container.GetInstance<TDomain>();
        }
    }
}
