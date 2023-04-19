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
    public class TransferModel
    {
        [Key]
        public int transfer_id { get; set; }
        [DisplayName("លេខប្រកាស")]
        public string transfer_no { get; set; }
        [DisplayName("កាលបរិច្ឆេទប្រកាស")]
        public DateTime? transfer_date { get; set; }
        [DisplayName("បរិយាយ")]
        public string transfer_desc { get; set; }

        public List<tbl_TransferDetail> transferDetail { get; set; }

        public virtual ICollection<TransferDetailModel> TransferDetail { get; set; }
    }
    public class TransferDetailModel
    {
        [Key]
        public int transfer_detail_id { get; set; }
        public decimal[] transfer_increase { get; set; }
        public decimal[] transfer_decrease { get; set;} 
        public decimal[] transfer_add { get; set; }
        public string Acc_no { get; set;} 
        public string transfer_no { get; set; }
        public virtual AccountsModel Account { get; set; }
        public virtual TransferModel Transfer { get; set; }
    }
    public class ListTransfer
    {
        public int transfer_id { get; set; }

        [Required(ErrorMessage = "សូមបញ្ចូលទិន្នន័យ")]
        //[Remote("CheckTransferNumberExists", "Transfer",  ErrorMessage = "Transfer no Already Exist")]
        [DisplayName("លេខបង្វែរ")]
        public string transfer_no { get; set; }
        [DisplayName("គណនី")]
        [Required]
        public string[] Acc_no { get; set; }
        [DisplayName("កាលបរិច្ឆេទ")]
        public DateTime? transfer_date { get; set; }
        [DisplayName("បរិយាយ")]
        public string transfer_desc { get; set; }
        [DisplayName("កើន")]
        [Required]
        [MinLength(1)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "សូមបញ្ចូលទិន្នន័យជាលេខ")]
        public decimal[] transfer_increase { get; set; }
        [DisplayName("ថយ")]
        [Required]
        [MinLength(1)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "សូមបញ្ចូលទិន្នន័យជាលេខ")]
        public decimal[] transfer_decrease { get; set; }
        [DisplayName("បន្ថែម")]
        [Required]
        [MinLength(1)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "សូមបញ្ចូលទិន្នន័យជាលេខ")]
        public decimal[] transfer_add { get; set; }
        public int[] transfer_detail_id { set; get; }
        public List<tbl_Account> Accounts { get; set; }
        public tbl_Transfer Transfer { get; set; }
        public int NumberOfNewRowAdded { get; set; }
        public bool Status { get; set; }
        public int []Unit_id { get; set; }
        public string Transfer_Year { get; set; }
    }

    public class AccountWithUnitModel
    {
        public int unitId{ get; set; }
        public string unitName { set; get; }
        public string accNo { get; set; }
        public string accName { get; set; }
        public int unitOrderNumber { get; set; }

        public string Dispaly
        {
            get
            {
                if (unitId != 0)
                {
                    if(unitOrderNumber == 0) return accNo + " - " + unitName;
                    else return accNo + " - " + unitOrderNumber + " - " + unitName;
                }
                else
                {
                    return accNo + " - "  + accName;
                }
            }
        }
    }
}