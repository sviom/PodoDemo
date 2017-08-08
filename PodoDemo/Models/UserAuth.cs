using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models
{
    public class UserAuth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Userid { get; set; }
        public string Read { get; set; }
        public string Modify { get; set; }
        public string Write { get; set; }
        public string Delete { get; set; }
        public string Submenuid { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public virtual SubMenu Submenu { get; set; }
        public virtual User User { get; set; }
    }

    public class UserAuthSearch
    {
        public string Userid { get; set; }
        public string Menuid { get; set; }
        public string Submenuid { get; set; }
    }
}
