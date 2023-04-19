using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISFMOCM_Project.Models
{
    public class FCVModel
    {
        [DisplayName("លរ")]
        public int FCV_id { get; set; }
        [DisplayName("លេខសលាកប័ត្រ")]
        public string FCV_no { get; set; }
        [DisplayName("លេខគណនី")]
        [Required]
        public string Acc_no { get; set; }
        [DisplayName("កាលបរិច្ឆេទ-សលកប")]
        public Nullable<System.DateTime> FCVDate { get; set; }
        [DisplayName("ចំនួនទឹកប្រាក់ស្នើសុំៈ")]
        public Nullable<decimal> FCV_amount { get; set; }
        [DisplayName("លេខលិខិត")]
        public string Letter_no { get; set; }
        [DisplayName("កាលបរិច្ឆេទ")]
        public Nullable<System.DateTime> Letter_date { get; set; }
        [DisplayName("កាលបរិច្ឆេទសហវឯកភាព")]
        public Nullable<System.DateTime> MEF_date { get; set; }
        [DisplayName("ទឹកប្រាក់ឯកភាព")]
        public Nullable<decimal> MEF_amount { get; set; }
        [DisplayName("ទឹកប្រាក់កែសម្រួល")]
        public Nullable<decimal> AmountAfterProcurement { get; set; }
        [DisplayName("កម្មវត្ថុនៃកិច្ចសន្យា")]
        public string Commitment_desc { get; set; }
        [DisplayName("ផ្នែកទទួលបន្ទុកចំណាយៈ")]
        public string Dep_of_commitment { get; set; }

        [DisplayName("ផ្នែកទទួលបន្ទុកចំណាយៈ")]
        public int Department_of_commitment { get; set; }

        [DisplayName("សក្ខីប័ត្រ")]
        public string Documentation { get; set; }
        [DisplayName("ទឹកប្រាក់ជាអក្សរ")]
        public string AmountInLetter { get; set; }
        [DisplayName("ឥណទានអនុម័ត")]
        public decimal? Initial_Budget { get; set; }
        [DisplayName("ការបង្វែរ")]
        public decimal? Transfer_Budget { get; set; }
        [DisplayName("សរុបឥណទាន")]
        public decimal? Total_Budget { get; set; }
        [DisplayName("ការសន្យាពីមុន")]
        public decimal? Reference_Budget { get; set; }
        [DisplayName("ឥណទានទំនេរ")]
        public decimal? Available_Budget { get; set; }
        [DisplayName("អត្រា")]
        public decimal? Rate { get; set; }    
        public ICollection<AccountBudget> AccountsBudget { get; set; }

        public virtual tbl_Account Account { get; set; }
        public virtual ICollection<tbl_MandateDetail> MandateDetail { get; set; }
        public bool Status { get; set; }
        public DateTime created_date { get; set; }
        [DisplayName("ប្រាក់ខែ")]
        public bool Salary { get; set; }
        [DisplayName("អគ្គិសនី")]
        public bool Electricity { get; set; }
        [DisplayName("ទឹក")]
        public bool Water { get; set; }
        [DisplayName("បង់វិភាគទាន")]
        public bool Contribution { get; set; }
        [DisplayName("បុរេប្រទាន")]
        public bool Advance { get; set; }
        [DisplayName("លេខបុរេប្រទាន")]
        public string AdvanceNumber { get; set; }
        [DisplayName("ថ្ងៃខែបុរេប្រទាន")]
        public Nullable<System.DateTime> Advance_date { get; set; }
        [DisplayName("ប្រាក់ឧបត្ថម្ភ")]
        public bool Allowance { get; set; }

        [Display(Name = "រជ្ជទេយ្យបុរេប្រទាន")]
        public Boolean PettyCash { get; set; }
        [Display(Name = "ផ្សេងៗ")]
        public Boolean Other { get; set; }

        [Display(Name = "ទូរគមនាគមន៍")]
        public Boolean Telecom { get; set; }

        public int FCV_Identity { get; set; }
        public string FCVYear { get; set; }
    }

    public class AccountBudget
    {
        public string AccountNO { get; set; }
        public decimal Initial_Budget { get; set; }
        public decimal Transfer_Budget { get; set; }
        public decimal Total_Budget { get; set; }
        public decimal Reference_Budget { get; set; }
        public decimal Available_Budget { get; set; }
        public int FCV_id { get; set; }
    }
    
}