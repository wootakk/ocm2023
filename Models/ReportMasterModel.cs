using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class ReportMasterModel
    {
        public string selectedOption { get; set; }
        public string AccFCV { get; set; }
        public string AccMandate { get; set; }
        public int Unit_id { get; set; }
        public int SubAccount_id { get; set; }
        public int Chapter_id { get; set; }
        public string UnitName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal AccTotalBudget { get; set; }
        public decimal AccTotalTransfer { get; set; }
        public decimal AccNewBudget { get; set; }
        public decimal AccFCVLM { get; set; }
        public decimal AccFCVLMPercent { get; set; }
        public decimal AccFCVLMAvailable { get; set; }
        public decimal AccTotalFCVMEF { get; set; }
        public decimal AccTotalFCVMEFPercent { get; set; }
        public decimal AccTotalFCVMEFAvailable { get; set; }
        public decimal AccTotalMandateLM { get; set; }
        public decimal AccTotalMandateLMPercent { get; set; }
        public decimal AccTotalMandateLMAvailable { get; set; }
        public decimal AccTotalMandateMEF { get; set; }
        public decimal AccTotalMandateMEFPercent { get; set; }
        public decimal AccTotalMandateMEFAvailable { get; set; }

        public decimal ChapterTotalBudget { get; set; }
        public decimal ChapterTotalTransfer { get; set; }
        public decimal ChapterNewBudget { get; set; }
        public decimal ChapterFCVLM { get; set; }
        public decimal ChapterFCVLMPercent { get; set; }
        public decimal ChapterFCVLMAvailable { get; set; }
        public decimal ChapterTotalFCVMEF { get; set; }
        public decimal ChapterTotalFCVMEFPercent { get; set; }
        public decimal ChapterTotalFCVMEFAvailable { get; set; }
        public decimal ChapterTotalMandateLM { get; set; }
        public decimal ChapterTotalMandateLMPercent { get; set; }
        public decimal ChapterTotalMandateLMAvailable { get; set; }
        public decimal ChapterTotalMandateMEF { get; set; }
        public decimal ChapterTotalMandateMEFPercent { get; set; }
        public decimal ChapterTotalMandateMEFAvailable { get; set; }

    }
}