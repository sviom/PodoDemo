using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PodoDemo.Models
{
    public partial class Account
    {
        public Account()
        {
            Contact = new HashSet<Contact>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Accountid { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Homepage { get; set; }
        public string Ceo { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string Addresscity { get; set; }
        public string Addressdetail { get; set; }
        public string Addresstype { get; set; }
        public string Biznum { get; set; }
        public DateTime? Founddate { get; set; }
        public string Detail { get; set; }
        public DateTime Createdate { get; set; }
        public string Createuser { get; set; }
        public DateTime Modifydate { get; set; }
        public string Modifyuser { get; set; }
        public bool Isdeleted { get; set; }
        public string Ownerid { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<Contact> Contact { get; set; }
    }

    public class AccountSearch
    {
        public bool IsPop { get; set; }
        public string Name { get; set; }
        public string Ownerid { get; set; }
        public string AccountCustomerType { get; set; }
        public string Phone { get; set; }
    }
}
