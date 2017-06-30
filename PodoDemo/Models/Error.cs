using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class Error
    {
        public long Id { get; set; }
        public string Errortitle { get; set; }
        public string Errorcontent { get; set; }
        public string Createuser { get; set; }
        public DateTime Createdate { get; set; }
    }
}
