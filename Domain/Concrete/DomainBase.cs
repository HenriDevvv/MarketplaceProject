using DAL.UoW;
using LamarCodeGeneration.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    internal class DomainBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        public DomainBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
