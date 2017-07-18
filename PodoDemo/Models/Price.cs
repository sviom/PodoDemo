using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class Price
    {
        public long Priceid { get; set; }
        public long Productid { get; set; } = 0;
        public float Prices { get; set; } = 0;
        public float Cost { get; set; } = 0;
        public string Currency { get; set; } = "";
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public string Ownerid { get; set; }
        public virtual Product Product { get; set; }

    }
}
