using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class InitialPettyCashViewModel
    {
        //public InitialPettyCashModel InitialPettyCash { get; set; }
        //public AccountsModel Account { get; set; }
        //public List<InitialPettyCashDetailModel> InitialPettyCashDetail { get; set; }

        public int InitialPettyCashID { get; set; }
        [Required]
        public int Unit_id { get; set; }
        [Required]
        public string []Acc_no { get; set; }
        [DisplayName("ទឹកប្រាក់")]
        public decimal []Budget { get; set; }
        [Required]
        public int [] InitialPettyCashDetailID { set; get; }
        public List<tbl_Account> Accounts { get; set; }
        public tbl_InitialPettyCash InitialPettyCash { get; set; }
        [DisplayName("អង្គភាព ឬ អាជ្ញាធរ")]
        public tbl_Unit Unit { get; set; }
        public DateTime InitialPettyCashDate { get; set; }
    }

}