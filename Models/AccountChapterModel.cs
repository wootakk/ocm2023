using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class AccountChapterModel
    {
        [DisplayName("កូដជំពូក")]
        public int AccChapter_id { get; set; }
        [DisplayName("ប្រភេទ")]
        public int AccType_id { get; set; }
        [DisplayName("កូដជំពូក")]
        public string AccChapter_code { get; set; }
        [DisplayName("ឈ្មោះជំពូក")]
        public string AccChapter_name { get; set; }
        [DisplayName("លំដាប់រៀង")]
        public Nullable<int> AccChapter_order { get; set; }

        public virtual ICollection<tbl_Account> Account { get; set; }
        public virtual tbl_AccountType AccountType { get; set; }
    }
}