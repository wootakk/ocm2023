using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class InitialPettyCashDetailModel
    {
        public int InitialPettyCashID { get; set; }
        public int InitialPettyCash_id { get; set; }
        [Required(ErrorMessage = "សូមជ្រើសរើសទិន្នន័យ")]
        public int Unit_id { get; set; }
        public string[] Acc_no { get; set; }
        [DisplayName("ទឹកប្រាក់")]
        public decimal[] Budget { get; set; }

        public int[] InitialPettyCashDetailID { set; get; }
        public Nullable<DateTime> InitialPettyCash_date { get; set; }

        public virtual AccountsModel Account { get; set; }
        public tbl_InitialPettyCash InitialPettyCash { get; set; }
        [DisplayName("អង្គភាព ឬ អាជ្ញាធរ")]
        public tbl_Unit Unit { get; set; }
    }
}