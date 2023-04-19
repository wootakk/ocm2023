using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class InitialBudgetViewModel
    {
        public string Chapter { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
        public string ExpendsRank { get; set; }
        public string FinancialRule { get; set; }
        public decimal NewBudget { get; set; }
        public decimal FCVTotalMonth { get; set; }
        public decimal FCVThisMonth { get; set; }
        public decimal FCVTotal { get; set; }
        public decimal FCVAvailableBalance { get; set; }
        public decimal MandateTotalMonth { get; set; }
        public decimal MandateThisMonth { get; set; }
        public decimal MandateTotal { get; set; }
        public decimal MandateAvailableBalance { get; set; }
    }


}