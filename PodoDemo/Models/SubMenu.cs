using System;
using System.Collections.Generic;

namespace PodoDemo.Models
{
    public partial class SubMenu
    {
        public SubMenu()
        {
            UserAuth = new HashSet<UserAuth>();
        }

        public string Id { get; set; }
        public long Mainmenuid { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public bool Isused { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public bool Ismanager { get; set; }
        public string Menuurl { get; set; }

        public virtual Menu Mainmenu { get; set; }

        public virtual ICollection<UserAuth> UserAuth { get; set; }
    }
}
