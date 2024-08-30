﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.AuctionListingDTO
{
    public class AuctionListingCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SellerId { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}