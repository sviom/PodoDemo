using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class Organization
    {
        public long Organizationid { get; set; }
        public string Name { get; set; }
        public DateTime Createdate { get; set; }
        public string Memo { get; set; }
    }
}
