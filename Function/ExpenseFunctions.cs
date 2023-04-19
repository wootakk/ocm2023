using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISFMOCM_Project.Entity;

namespace ISFMOCM_Project.Function
{
    public class ExpenseFunctions
    {
        public static decimal GetChapterInitialBudget(string chapterCode)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            var data = db.tbl_InitialBudget.Where(
                ib =>
                    ib.tbl_Account.tbl_AccountChapter.AccChapter_code == chapterCode &&
                    ib.status == false &&
                    ib.InitialBudget_date.Value.Year == DateTime.Now.Year
            );
            if (data != null && data.Any())
            {
                return (decimal) data.Sum(b => b.Budget);
            }
            return 0;
        }

        public static decimal GetChapterPettycash(string chapterCode,string year=null)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            List<tbl_InitialBudget> data = new List<tbl_InitialBudget>();
            //var data = db.tbl_InitialBudget.Where(
            //    ib =>
            //        ib.tbl_Account.tbl_AccountChapter.AccChapter_code == chapterCode &&
            //        ib.status == false &&
            //        ib.InitialBudget_date.Value.Year == DateTime.Now.Year
            //);
            if(string.IsNullOrEmpty(year))
                data = db.tbl_InitialBudget.Where(
                    ib =>
                        ib.tbl_Account.tbl_AccountChapter.AccChapter_code == chapterCode &&
                        ib.status == false &&
                        ib.InitialBudget_date.Value.Year == DateTime.Now.Year
                ).ToList();
            else
                data = db.tbl_InitialBudget.Where(
                    ib =>
                        ib.tbl_Account.tbl_AccountChapter.AccChapter_code == chapterCode &&
                        ib.status == false &&
                        string.Compare(ib.InitialBudget_date.Value.Year.ToString(),year)==0
                ).ToList();
            if (data != null && data.Any())
            {
                return (decimal)data.Sum(b => b.PettyCash);
            }
            return 0;
        }

        public static SelectList GetAccountChapterForExpense()
        {
            var accounts = CommonClass.GetAllSubAccounts()
                .Where(c => c.AccChapter.AccChapter_code == "60" || c.AccChapter.AccChapter_code == "61");
            return new SelectList(accounts, "Acc_no", "Acc_no");
        }

    }
}