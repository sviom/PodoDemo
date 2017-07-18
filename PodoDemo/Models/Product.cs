using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class Product
    {
        public long Productid { get; set; }
        public string Name { get; set; }
        public string Maker { get; set; } = "";
        public string Ownerid { get; set; } = "";
        public string Origin { get; set; } = "";
        public string Productcode { get; set; } = "";
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
    }

    public class ProductSearch
    {
        public string Name { get; set; }
        public string Productcode { get; set; } = "";
        public string Maker { get; set; } = "";
        public string Ownerid { get; set; } = "";
    }
}
