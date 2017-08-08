using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class Appointment
    {
        public long Appointmentid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Regardingobjectid { get; set; }
        public string Regardingobjecttypeid { get; set; }
        public string Regardingobjectname { get; set; }
        public DateTime Startdate { get; set; } = DateTime.MinValue;
        public DateTime Enddate { get; set; } = DateTime.MinValue;
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public virtual User Owner { get; set; }
        public string Ownerid { get; set; }
        public string State { get; set; }
    }
}
