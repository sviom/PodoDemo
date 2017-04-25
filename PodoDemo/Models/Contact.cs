using System;
using System.Collections.Generic;

namespace PodoDemo.Models
{
    public partial class Contact
    {
        public long Contactid { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public long? Accountid { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Detail { get; set; }
        public long? Bossid { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public bool Isdeleted { get; set; }
        public string Ownerid { get; set; }

        public virtual Account Account { get; set; }
    }
}
