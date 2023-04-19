using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ISFMOCM_Project.Entity;

namespace ISFMOCM_Project.Models
{
    public class InitialBudgetModel
    {
        [Key]
        public int IB_id { get; set; }

        [DisplayName("លេខគណនី")]
        [Required]
        public string acc_no { get; set; }
        public string sub_acc_no { get; set; }

        [DisplayName("ឈ្មោះគណនី")]
        [Required]
        public string acc_name { get; set; }

        [DisplayName("កាលបរិច្ឆេទ")]
        [DataType(DataType.Date)]
        public DateTime? IB_date { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Required]
        [DisplayName("ឥណទានអនុម័ត")]
        public decimal? IB_budget { get; set; }

        public bool Status { get; set; }
        public string acc_type { get; set; }
        public string acc_chap_code { get; set; }
        public string acc_chap_name { get; set; }
        [Display(Name = "ទូទាត់ត្រង់")]
        public decimal Direct_Paid { get; set; }
        [Display(Name = "រជ្ជទេយ្យ")]
        public decimal PettyCash { get; set; }
        public List<AccountInitialBudgetModel> AccoutBudgets { get; set; }
        public int[] IDs { get; set; }
        public string[] ACCs { get; set; }
        public decimal?[] BUDGETs { get; set; }
        public decimal[] DIRECT_PAIDs { get; set; }
        public decimal[] PETTYCASHs { get; set; }
        public int Unit_id { get; set; }
        public string sub_acc_name { get; set; }
        public string IBYear { get; set; }
    }

    public class AccountInitialBudgetModel
    {
        [Key]
        public int ID { get; set; }
        public string Acc_no { get; set; }
        public decimal? Budget { get; set; }
        public int Unit_id { get; set; }
        public decimal Direct_Paid { get; set; }
        public decimal PettyCash { get; set; }
    }

    public class InitialBudgetDropdownModel
    {
        public int Unit_id { get; set; }
        public string Acc_no { get; set; }
        public string Name { get; set; }
        public int OrderNumber { set; get; }
        public int Unit_code { get; set; }
        public string Display
        {
            get
            {
                if (OrderNumber == 0 && Unit_code == 0)
                {
                    return Acc_no + " - " + Name;
                }else if (OrderNumber >= 0 && Unit_code == 0)
                {
                    return Acc_no + " - " + OrderNumber + " - " + Name;
                }
                else
                {
                    return Acc_no + " -  " + Name;
                }
            }
        }

        public string Value
        {
            get { return Acc_no + "," + Unit_id; }
        }
    }

    public class InitialBudgetTableViewModel
    {
        public int id { get; set; }
        public string chapter_no { get; set; }
        public string chapter_name { get; set; }
        public string account_no { get; set; }
        public string account_name { get; set; }
        public string sub_account_no { get; set; }
        public string sub_account_name{ get; set; }
        public Decimal budget { get; set; }
        public DateTime budget_date { get; set; }
        public List<AccountUnitModel> account_unit { get; set; }
        public int? unit_id { get; set; }
        public int  Unit_Code { get; set; }
    }

    public class AccountUnitModel
    {
        public int id { get; set; }
        public string unit_name { get; set; }
        public decimal budget { get; set; }
    }

    public class InitialBudgetListModel{
        public string ChapterNo { get; set; }
        public string ChapterName { get; set; }
        public List<AccModel> Accounts { set; get; }

        public class AccModel
        {
            public string AccName { get; set; }
            public string AccNo { get; set; }
            public List<SubAccModel> SubAccounts { set; get; }
        }
        public class SubAccModel
        {
            public string AccName { get; set; }
            public string AccNo { get; set; }
            public List<UnitModel> Units;
        }

        public class UnitModel
        {
            public string UnitName { get; set; }
            public int  OrderNumber { get; set; }
            public int UnitId { get; set; }
            public int UnitCode { get; set; }
            public List<SubUnit> SubUnits { set; get; }
        }

        public class SubUnit
        {
            public string UnitName { get; set; }
            public int OrderNumber { get; set; }
            public int UnitId { get; set; }
            public int UnitCode { get; set; }
        }
    }


    

}