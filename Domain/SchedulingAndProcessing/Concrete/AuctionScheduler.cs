using Domain.SchedulingAndProcessing.Contracts;
using Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SchedulingAndProcessing.Concrete
{
    internal class AuctionScheduler : IAuctionScheduler
    {
        private IAuctionProcessor _auctionProcessor;
        private IServiceProvider _serviceProvider;
        public AuctionScheduler(IAuctionProcessor auctionProcessor, IServiceProvider serviceProvider)
        {
            _auctionProcessor = auctionProcessor;
            _serviceProvider = serviceProvider;
        }

        public async void ScheduleAuctionEnd(Guid id, DateTime endTime, IServiceProvider serviceProvider)
        {
            var delay = endTime.Subtract(DateTime.Now);
            await Task.Delay(delay);
            using(var scope = serviceProvider.CreateScope())
            {
                var processor = scope.ServiceProvider.GetRequiredService<IAuctionProcessor>();
                processor.EndAuction(id);
            }
        }
    }
}
