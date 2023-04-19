using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class AccountsModel
    {
        [Key]
        [DisplayName("Account ID")]
        public int Acc_id { get; set; }

       
        [DisplayName("Account Number")]
        public string Acc_no { get; set; }

        [Required]
        [DisplayName("Account Chapter")]
        public int AccChapter_id { get; set; }

        [Required]
        [DisplayName("Account Code")]
        public string Acc_code { get; set; }

        [Required]
        [DisplayName("Account Name")]
        public string Acc_name { get; set; }

        [DisplayName("Account Description")]
        public string Acc_desc { get; set; }
        [DisplayName("Account Chapter")]
        public tbl_AccountChapter AccountChapter { get; set; }
        public ICollection<tbl_FCV> FCV { get; set; }
        public ICollection<tbl_InitialBudget> InitialBudget { get; set; }
        public ICollection<tbl_InitialPettyCashDetail> InitialPettyCashDetail { get; set; }
        public ICollection<tbl_MandateDetail> MandateDetail { get; set; }
        public ICollection<tbl_TransferDetail> TransferDetail { get; set; }
    }
}