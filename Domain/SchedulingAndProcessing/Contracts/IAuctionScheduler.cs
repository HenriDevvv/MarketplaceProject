using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SchedulingAndProcessing.Contracts
{
    public interface IAuctionScheduler
    {
        public void ScheduleAuctionEnd(Guid id, DateTime endTime, IServiceProvider serviceProvider);
    }
}
