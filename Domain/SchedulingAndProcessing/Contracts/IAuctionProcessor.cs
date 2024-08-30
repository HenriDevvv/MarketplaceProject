using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SchedulingAndProcessing.Contracts
{
    public interface IAuctionProcessor
    {
        void EndAuction(Guid auctionId);
    }
}
