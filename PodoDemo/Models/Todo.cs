using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class Todo
    {
        public long Todoid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Regardingobjectid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public string Ownerid { get; set; }
        public string State { get; set; }
    }
}
