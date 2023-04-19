using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISFMOCM_Project.Models
{
    public class AccountModel
    {
        [DisplayName("លេខកូដគណនី")]
        public int Acc_id { get; set; }
        [DisplayName("លេខគណនី")]
        [Remote("CheckAccountNumberExists","Accounts",ErrorMessage = "Account Already Exist")]
        public string Acc_no { get; set; }
        [DisplayName("លេខកូដគណនី")]
        public string Acc_code { get; set; }
        [DisplayName("ជំពូក")]
        public int AccChapter_id { get; set; }
        [DisplayName("ឈ្មោះគណនី")]
        public string Acc_name { get; set; }
        [DisplayName("សេចក្តីពន្យល់")]
        public string Acc_desc { get; set; }
        [DisplayName("ជំពូក")]
        public string AccountChapter { get; set; }
        [DisplayName("ជំពូក")]
        public tbl_AccountChapter AccChapter { get; set; }
        public string CodeAndName
        {
            get
            {
                if (Unit_id == 0)
                    return Acc_no + " - " + Acc_name;
                else
                {
                    return Acc_no + " - " + unitName;
                }
            }
        }

        public string unitName { get; set; }
        public int Unit_id { get; set; }
        public ICollection<tbl_FCV> FCV { get; set; }
        public ICollection<tbl_InitialBudget> InitialBudget { get; set; }
        public ICollection<tbl_InitialPettyCashDetail> InitialPettyCashDetail { get; set; }
        public ICollection<tbl_MandateDetail> MandateDetail { get; set; }
        public ICollection<tbl_TransferDetail> TransferDetail { get; set; }
        public ICollection<tbl_Unit> Units { set; get; }
    }
}