using System;
using System.Collections.Generic;

namespace PodoDemo.Models
{
    public partial class OptionMaster
    {
        public OptionMaster()
        {
            OptionMasterDetail = new HashSet<OptionMasterDetail>();
        }

        public long Masterid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Isused { get; set; }
        public string Defaultvalue { get; set; }
        public string Ownerid { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public bool Issystem { get; set; }

        public virtual ICollection<OptionMasterDetail> OptionMasterDetail { get; set; }
    }
}
