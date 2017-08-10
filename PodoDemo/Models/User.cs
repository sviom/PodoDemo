using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PodoDemo.Models
{
    public partial class User
    {
        public User()
        {
            UserAuth = new HashSet<UserAuth>();
            Account = new HashSet<Account>();
            Product = new HashSet<Product>();
            Appointment = new HashSet<Appointment>();
            Todo = new HashSet<Todo>();
            Holiday = new HashSet<Holiday>();
        }

        [Key]
        public string Id { get; set; }
        public string Pw { get; set; }
        public string Name { get; set; }
        public string Engname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public bool Ismaster { get; set; }
        public string Token { get; set; }
        public string Keybox { get; set; }
        public bool Excelauth { get; set; }
        public string Level { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public long Organizationid { get; set; }
        public virtual OptionMasterDetail DepartmentNavigation { get; set; }
        public virtual Organization Organization { get; set; }

        public virtual ICollection<UserAuth> UserAuth { get; set; }
        public virtual ICollection<Account> Account { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<Appointment> Appointment { get; set; }
        public virtual ICollection<Todo> Todo { get; set; }
        public virtual ICollection<Holiday> Holiday { get; set; }
    }
}
