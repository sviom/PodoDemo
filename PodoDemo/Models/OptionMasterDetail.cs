using System;
using System.Collections.Generic;

namespace PodoDemo.Models
{
    public partial class OptionMasterDetail
    {
        public OptionMasterDetail()
        {
            User = new HashSet<User>();
        }

        public string Optionid { get; set; }
        public long Masterid { get; set; }
        public string Name { get; set; }
        public long Order { get; set; }
        public bool Isused { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> User { get; set; }
        public virtual OptionMaster Master { get; set; }
    }
}
