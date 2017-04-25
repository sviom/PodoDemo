using System;
using System.Collections.Generic;

namespace PodoDemo.Models
{
    public partial class Menu
    {
        public Menu()
        {
            SubMenu = new HashSet<SubMenu>();
        }

        public long Id { get; set; }
        public long Order { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public bool Isused { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubMenu> SubMenu { get; set; }
    }
}
