using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public User? Buyer { get; set; }
        public DateTime Time { get; set; }
        public decimal Amount { get; set; }
        public Listing Listing { get; set; }
    }
}
