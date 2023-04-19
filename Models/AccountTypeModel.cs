using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class AccountTypeModel
    {
        [DisplayName("ប្រភេទ")]
        public int AccType_id { get; set; }
        [DisplayName("ប្រភេទ")]
        public string AccType_name { get; set; }

        public virtual ICollection<tbl_AccountChapter> AccountChapter { get; set; }
    }
}