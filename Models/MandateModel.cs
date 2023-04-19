using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class MandateModel
    {
        public int Mandate_id { get; set; }
        [DisplayName("លេខអាណត្តិ")]
        public string Mandate_no { get; set; }
        [DisplayName("អង្គភាព ឬអាជ្ញាធរ")]
        public int Unit_id { get; set; }
        [DisplayName("កាលបរិច្ឆេទអាណត្តិ")]
        public Nullable<System.DateTime> Mandate_date { get; set; }
        [DisplayName("កម្មវត្ថុចំណាយ")]
        public string Mandate_desc { get; set; }
        [DisplayName("អត្ថគាហកៈ")]
        public string Reciever { get; set; }
        [DisplayName("លេខគណនី")]
        public string BankAcc_no { get; set; }
        [DisplayName("រតនាគារ")]
        public string BankAcc_address { get; set; }
        [DisplayName("ទឹកប្រាក់ជាអក្សរ")]
        public string AmountInWord { get; set; }
        [DisplayName("រូបិយប័ណ្ណ")]
        public string USD { get; set; }
        [DisplayName("កាលបរិច្ឆេទ-ឯកភាព")]
        public Nullable<System.DateTime> MEF_date { get; set; }
        [DisplayName("ប្រាក់ខែ")]
        public bool Salary { get; set; }
        [DisplayName("អគ្គិសនី")]
        public bool Electricity { get; set; }
        [DisplayName("ទឹក")]
        public bool Water { get; set; }
        [DisplayName("បង់វិភាគទាន")]
        public bool Contribution { get; set; }
        [DisplayName("រជ្ជទេយ្យបុរេប្រទាន")]
        public bool PettyCash { get; set; }
        [DisplayName("បុរេប្រទាន")]
        public bool Advance { get; set; }
        [DisplayName("លេខបុរេប្រទាន")]
        public string AdvanceNumber { get; set; }
        [DisplayName("ថ្ងៃខែបុរេប្រទាន")]
        public Nullable<System.DateTime> Advance_date { get; set; }
        [DisplayName("ប្រាក់ឧបត្ថម្ភ")]
        public bool Allowance { get; set; }
        [Display(Name = "ផ្សេងៗ")]
        public Boolean Other { get; set; }
        [Display(Name = "ទូរគមនាគមន៍")]
        public Boolean Telecom { get; set; }

        [DisplayName("ទឹកប្រាក់")]
        [Required]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "សូមបញ្ចូលទិន្នន័យជាលេខ")]
        public decimal[] Mandate_amount { get; set; }
        [DisplayName("គណនី")]
        [Required]
        public string[] Acc_no { get; set; }
        [DisplayName("លសកប")]
        public string[] FCV_no { get; set; }
        public int[] FCV_id { get; set; }
        [DisplayName("អង្គភាព ឬអាជ្ញាធរ")]
        public tbl_Unit tbl_Unit { get; set; }
        [DisplayName("")]
        public MandateDetail MandateDetail { get; set; }
        public bool Status { get; set; }
        public DateTime created_date { get; set; }
        public int []FCV_Identity { get; set; }
        public string MandateYear { get; set; }
        public string CurrencyType { get; set; }
    }
    public class MandateDetail
    {
        [DisplayName("ទឹកប្រាក់")]
        [Required]
        [MinLength(1)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "សូមបញ្ចូលទិន្នន័យជាលេខ")]
        public decimal[] Mandate_amount { get; set; }
        [DisplayName("គណនី")]
        [Required]
        public string[] Acc_no { get; set; }
        [DisplayName("លសកប")]
        public string[] FCV_no { get; set; }
    }
}