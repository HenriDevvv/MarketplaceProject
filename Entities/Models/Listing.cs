﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public abstract class Listing
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(User))]
        public Guid SellerId { get; set; }
        public User Seller { get; set; }
        public int Status { get; set; }
    }
}
