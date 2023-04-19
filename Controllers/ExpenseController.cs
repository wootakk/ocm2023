using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using EntityState = System.Data.Entity.EntityState;

namespace ISFMOCM_Project.Controllers
{
    [CustomAuthorize(Roles = "Cabinate")]
    public class ExpenseController : Controller
    {
        ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
        // GET: Expense
        public ActionResult Index()
        {
            //            var expenses = db.tbl_Expense.Where(e => e.status == false);
            //            var model = new List<ExpenseModel>();
            //            foreach (var expense in expenses)
            //            {
            //                var expenseDetail = new List<ExpenseDetail>();
            //                foreach (var expDetail in expense.tbl_ExpenseDetail)
            //                {
            //                    expenseDetail.Add(new ExpenseDetail()
            //                    {
            //                        Account = expDetail.Acc_no,
            //                        Amount = (decimal)expDetail.amount,
            //                        Description = expDetail.description
            //                    });
            //                }
            //                model.Add(new ExpenseModel()
            //                {
            //                    ExpenseDetail = expenseDetail,
            //                    ExpenseDate = (DateTime)expense.expense_date,
            //                    ExpenseId = expense.expense_id
            //                });
            //            }
            //            return View(model);

            var expenseToday = db.tbl_Expense.Where(
                ex =>
                    ex.expense_date.Value.Day == DateTime.Now.Day &&
                    ex.expense_date.Value.Month == DateTime.Now.Month &&
                    ex.expense_date.Value.Year == DateTime.Now.Year &&
                    ex.status == false
                );

            if (expenseToday.Any())
            {
                if (User.Identity.GetUserName() != "Sanset")
                    return RedirectToAction("Details", new { id = expenseToday.FirstOrDefault().expense_id });
                else
                    return RedirectToAction("EditExpense", new { id = expenseToday.FirstOrDefault().expense_id });
            }
            else
            {
                if (User.Identity.GetUserName() != "Sanset")
                    return RedirectToAction("ListExpense");
                else
                    return RedirectToAction("CreateExpense");


            }

        }


        public ActionResult SearchExpense(string Acc_no, string Amount)
        {
            var expenses = new List<ExpenseDetail>();

            var result = from expense in db.tbl_Expense
                join expDetail in db.tbl_ExpenseDetail on expense.expense_id equals expDetail.expense_id
                         where expense.status == false
                select new { expense.expense_date, expDetail.description, expDetail.amount, expDetail.Acc_no, expense.expense_id ,expDetail.id };

            if (!string.IsNullOrEmpty(Acc_no) && !string.IsNullOrEmpty(Amount))
            {
                var amount = decimal.Parse(Amount);
                result = result.Where(e => e.Acc_no == Acc_no && e.amount == amount);
            }
            else if (!string.IsNullOrEmpty(Acc_no))
            {
                result = result.Where(e => e.Acc_no == Acc_no);
            }
            else if (!string.IsNullOrEmpty(Amount))
            {
                var amount = decimal.Parse(Amount);
                result = result.Where(e => e.amount == amount);
            }

            

            foreach (var expense in result)
            {
                expenses.Add(new ExpenseDetail()
                {
                    Account = expense.Acc_no,
                    Description = expense.description,
                    Amount = (decimal)expense.amount,
                    ExpenseDate = (DateTime)expense.expense_date,
                    ExpenseId = expense.expense_id,
                    Id =  expense.id
                });
            }


            return View(expenses);
        }

        public ActionResult ListExpense(string Acc_no, string Amount)
        {

            if (!string.IsNullOrEmpty(Acc_no) || !string.IsNullOrEmpty(Amount))
            {
                return RedirectToAction("SearchExpense", new { Acc_no = Acc_no, Amount = Amount });
            }

            var expenses = db.tbl_Expense.Where(e => e.status == false).OrderBy(d => d.expense_date);
            var model = new List<ExpenseModel>();
            foreach (var expense in expenses)
            {
                model.Add(new ExpenseModel()
                {
                    ExpenseDate = (DateTime)expense.expense_date,
                    ExpenseId = expense.expense_id
                });
            }
            return View(model);
        }

        public ActionResult CreateExpense()
        {
            if (User.Identity.GetUserName() != "Sanset")
                return RedirectToAction("ListExpense");

            var accounts = CommonClass.GetAllSubAccounts()
                .Where(c => c.AccChapter.AccChapter_code == "60" || c.AccChapter.AccChapter_code == "61");
            ViewBag.Accounts = new SelectList(accounts, "Acc_no", "Acc_no");
            return View();
        }
        [HttpPost]
        public ActionResult CreateExpense(ExpenseViewModel model)
        {
            if (User.Identity.GetUserName() != "Sanset")
                return Redirect("Index");

            var expense = new tbl_Expense();
            var isRecordExist = db.tbl_Expense.FirstOrDefault(e => e.expense_date == model.ExpenseDate && e.status == false);
            if (isRecordExist != null)
            {
                expense = isRecordExist;
            }
            else
            {
                if (string.Compare(DateTime.Now.Year.ToString(), model.ExpenseYear) == 0)
                    expense.created_date = DateTime.Now;
                else
                    expense.created_date = new DateTime(Convert.ToInt32(model.ExpenseYear), 1, 1);
                expense.expense_date = model.ExpenseDate;
                db.tbl_Expense.Add(expense);
                db.SaveChanges();
            }

            var parentAccountNumber = "";
            if (model.Accounts != null)
            {
                for (var i = 0; i < model.Accounts.Length; i++)
                {
                    if (!string.IsNullOrEmpty(model.Accounts[i]))
                    {
                        parentAccountNumber = model.Accounts[i];
                    }
                    if (model.Accounts[i] == "" && model.Amount[i] == 0 && model.Description[i].Equals(""))
                        continue;
                    var expensDetail = new tbl_ExpenseDetail
                    {
                        expense_id = expense.expense_id,
                        Acc_no = parentAccountNumber,
                        description = model.Description[i],
                        amount = model.Amount[i]
                    };
                    db.tbl_ExpenseDetail.Add(expensDetail);
                }
            }
            db.SaveChanges();
            return RedirectToAction("EditExpense", new { id = expense.expense_id });
        }
        public ActionResult EditExpense(int id)
        {
            if (User.Identity.GetUserName() != "Sanset")
                return RedirectToAction("Index");

            var expense = db.tbl_Expense.SingleOrDefault(e => e.expense_id == id && e.status == false);
            var expenseViewModel = new ExpenseViewModel();

            if (expense == null)
                return HttpNotFound();
            expense.tbl_ExpenseDetail = db.tbl_ExpenseDetail.Where(s => s.expense_id == expense.expense_id).ToList();

            var size = expense.tbl_ExpenseDetail.ToList().Count;
            var expenseDetail = expense.tbl_ExpenseDetail.OrderBy(acc => acc.Acc_no).ToArray();
            var accounts = new string[size];
            var amounts = new decimal[size];
            var descriptions = new string[size];

            for (var i = 0; i < size; i++)
            {
                accounts[i] = expenseDetail[i].Acc_no;
                amounts[i] = (decimal)expenseDetail[i].amount;
                descriptions[i] = expenseDetail[i].description;
            }
            expenseViewModel.id = expense.expense_id;
            expenseViewModel.Accounts = accounts;
            expenseViewModel.Amount = amounts;
            expenseViewModel.Description = descriptions;
            expenseViewModel.ExpenseDate = (DateTime)expense.expense_date;
            return View(expenseViewModel);
        }
        [HttpPost]
        public ActionResult EditExpense(int id, ExpenseViewModel model)
        {
            if (User.Identity.GetUserName() != "Sanset")
                return Redirect("Index");

            var parentAccountNumber = "";

            //Check if the editing record changed date
            var expense = db.tbl_Expense.SingleOrDefault(i => i.expense_id == id && i.status == false);
            if (expense.expense_date == model.ExpenseDate)
            {
                expense.expense_date = model.ExpenseDate;
                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //Check if the new date already exist 
                var isRecordExist = db.tbl_Expense.FirstOrDefault(e => e.status == false && e.expense_date == model.ExpenseDate);
                if (isRecordExist == null)
                {
                    expense.expense_date = model.ExpenseDate;
                    db.Entry(expense).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    expense.status = true;
                    db.Entry(expense).State = EntityState.Modified;
                    //Move all the new record items to existing record
                    if (model.Accounts != null && model.Amount != null)
                    {
                        for (var i = 0; i < model.Accounts.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(model.Accounts[i]))
                            {
                                parentAccountNumber = model.Accounts[i];
                            }
                            var expensDetail = new tbl_ExpenseDetail
                            {
                                expense_id = isRecordExist.expense_id,
                                Acc_no = parentAccountNumber,
                                description = model.Description[i],
                                amount = model.Amount[i]
                            };
                            db.tbl_ExpenseDetail.Add(expensDetail);
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("ListExpense");
                }

            }

            //Remove the previous
            var previousExpenseDetail = db.tbl_ExpenseDetail.Where(i => i.expense_id == id);
            if (previousExpenseDetail != null)
            {
                db.tbl_ExpenseDetail.RemoveRange(previousExpenseDetail);
                db.SaveChanges();
            }
            //Add New 
            if (model.Accounts != null && model.Amount != null)
            {
                for (var i = 0; i < model.Accounts.Length; i++)
                {
                    if (!string.IsNullOrEmpty(model.Accounts[i]))
                    {
                        parentAccountNumber = model.Accounts[i];
                    }
                    var expensDetail = new tbl_ExpenseDetail
                    {
                        expense_id = expense.expense_id,
                        Acc_no = parentAccountNumber,
                        description = model.Description[i],
                        amount = model.Amount[i]
                    };
                    db.tbl_ExpenseDetail.Add(expensDetail);
                }
            }
            db.SaveChanges();
            return RedirectToAction("ListExpense");
        }
        public ActionResult Details(int id,string year)
        {
            var expense = db.tbl_Expense.SingleOrDefault(e => e.expense_id == id && e.status == false);
            var expenseViewModel = new ExpenseViewModel();

            if (expense == null)
                return HttpNotFound();
            expense.tbl_ExpenseDetail = db.tbl_ExpenseDetail.Where(s => s.expense_id == expense.expense_id).ToList();
            var size = expense.tbl_ExpenseDetail.ToList().Count;
            var expenseDetail = expense.tbl_ExpenseDetail.OrderBy(acc => acc.Acc_no).ToArray();
            var accounts = new string[size];
            var amounts = new decimal[size];
            var descriptions = new string[size];

            for (var i = 0; i < size; i++)
            {
                accounts[i] = expenseDetail[i].Acc_no;
                amounts[i] = (decimal)expenseDetail[i].amount;
                descriptions[i] = expenseDetail[i].description;
            }
            expenseViewModel.id = expense.expense_id;
            expenseViewModel.Accounts = accounts;
            expenseViewModel.Amount = amounts;
            expenseViewModel.Description = descriptions;
            expenseViewModel.ExpenseDate = (DateTime)expense.expense_date;
            return View(expenseViewModel);
        }

        public JsonResult getAccountName(string accountNumber,string Year=null)
        {
            if (string.IsNullOrEmpty(accountNumber)) return null;
            var name = db.tbl_Account.SingleOrDefault(acc => acc.Acc_no == accountNumber).Acc_name;
            var pettyCash = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber,Year).PettyCash;
            var budget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber,Year).Budget;
            return Json(new { name = name, pettyCash = pettyCash, budget = budget }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getAllSubAccountNumber()
        {
            var subAccounts = db.tbl_Account.Where(acc =>
                                            acc.Acc_no.Length == 5 && (acc.tbl_AccountChapter.AccChapter_code ==
                                            "60" || acc.tbl_AccountChapter.AccChapter_code == "61"));
            List<string> result = new List<string>();
            foreach (var sub in subAccounts)
            {
                result.Add(sub.Acc_no);
            }

            return Json(new { subAccounts = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var expense = db.tbl_Expense.SingleOrDefault(i => i.expense_id == id);
            expense.status = true;
            db.Entry(expense).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListExpense");
        }

        public ActionResult DeleteExpenseDetail(int id)
        {
            var expense = db.tbl_ExpenseDetail.SingleOrDefault(e => e.id == id);
            db.tbl_ExpenseDetail.Remove(expense);
            db.SaveChanges();
            return RedirectToAction("ListExpense");
        }


        public ActionResult DailyExpenseReport(string st, string et,string year=null)
        {
            DateTime startDate, endDate;
            if (string.IsNullOrEmpty(st) || string.IsNullOrEmpty(et))
            {
                startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
                endDate = DateTime.Now;
            }
            else
            {
                startDate = Convert.ToDateTime(st);
                endDate = Convert.ToDateTime(et);
            }

            if (!string.IsNullOrEmpty(year))
            {
                startDate = Convert.ToDateTime("1-1-" + Convert.ToInt32(year));
                endDate = Convert.ToDateTime("31-12-" + Convert.ToInt32(year));
            }

            var rv = new ReportViewer
            {
                Width = Unit.Percentage(100),
                ProcessingMode = ProcessingMode.Local,
                AsyncRendering = false,
                SizeToReportContent = true,
                Height = Unit.Percentage(100),
                ShowBackButton = true,
                PageCountMode = PageCountMode.Actual
            };

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("chapterNo"));
            dt.Columns.Add(new DataColumn("chapterName"));
            dt.Columns.Add(new DataColumn("subNo"));
            dt.Columns.Add(new DataColumn("subName"));
            dt.Columns.Add(new DataColumn("subPettycash"));
            dt.Columns.Add(new DataColumn("expenseDesc"));
            dt.Columns.Add(new DataColumn("expenseAmount"));

            var accounts = db.tbl_Account.Where(acc =>
                acc.Acc_no.Length == 5 && (acc.tbl_AccountChapter.AccChapter_code == "60" || acc.tbl_AccountChapter.AccChapter_code == "61"));
            foreach (var account in accounts)
            {
                var expenses = CommonReportFunctions.GetExpensesByDate(account.Acc_no, startDate, endDate);
                if (expenses != null && expenses.Any())
                {
                    account.tbl_AccountChapter = db.tbl_AccountChapter.Find(account.AccChapter_id);
                    foreach (var expense in expenses)
                    {
                        var dr = dt.NewRow();
                        dr["titleDate"] = CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["chapterNo"] = account.tbl_AccountChapter.AccChapter_code;
                        dr["chapterName"] = account.tbl_AccountChapter.AccChapter_name;
                        dr["subNo"] = account.Acc_no;
                        dr["subName"] = account.Acc_name;
                        dr["subPettycash"] = CommonReportFunctions.GetSubAccountInitialBudget(account.Acc_no,year).PettyCash;
                        dr["expenseDesc"] = expense.description;
                        dr["expenseAmount"] = expense.amount;
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    account.tbl_AccountChapter = db.tbl_AccountChapter.Find(account.AccChapter_id);
                    var dr = dt.NewRow();
                    dr["titleDate"] = CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["chapterNo"] = account.tbl_AccountChapter.AccChapter_code;
                    dr["chapterName"] = account.tbl_AccountChapter.AccChapter_name;
                    dr["subNo"] = account.Acc_no;
                    dr["subName"] = account.Acc_name;
                    dr["subPettycash"] = CommonReportFunctions.GetSubAccountInitialBudget(account.Acc_no,year).PettyCash;
                    dt.Rows.Add(dr);
                }
            }



            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\DailyExpenseReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("DailyExpenseDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }

        //        public ActionResult Create()
        //        {
        //            var accounts = CommonClass.GetAllSubAccounts()
        //                .Where(c => c.AccChapter.AccChapter_code == "60" || c.AccChapter.AccChapter_code == "61");
        //            ViewBag.Accounts = new SelectList(accounts, "Acc_no", "CodeAndName");
        //            return View();
        //        }
        //        [HttpPost]
        //        public ActionResult Create(ExpenseViewModel model)
        //        {
        //            var parentAccountNumber = "";
        //            var expense = new tbl_Expense();
        //            expense.created_date = DateTime.Now;
        //            expense.expense_date = model.ExpenseDate;
        //            db.tbl_Expense.Add(expense);
        //            db.SaveChanges();
        //            for (var i = 0; i < model.Accounts.Length; i++)
        //            {
        //                if (!string.IsNullOrEmpty(model.Accounts[i]))
        //                {
        //                    parentAccountNumber = model.Accounts[i];
        //                }
        //                var expensDetail = new tbl_ExpenseDetail
        //                {
        //                    expense_id = expense.expense_id,
        //                    Acc_no = parentAccountNumber,
        //                    description = model.Description[i],
        //                    amount = model.Amount[i]
        //                };
        //                db.tbl_ExpenseDetail.Add(expensDetail);
        //            }
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        public ActionResult Edit(int id)
        //        {
        //            var expense = db.tbl_Expense.SingleOrDefault(e => e.expense_id == id);
        //            var expenseViewModel = new ExpenseViewModel();
        //
        //            if(expense==null)
        //                return  HttpNotFound();
        //            var size = expense.tbl_ExpenseDetail.ToList().Count;
        //            var expenseDetail = expense.tbl_ExpenseDetail.ToArray();
        //            var accounts = new string[size];
        //            var amounts = new decimal[size];
        //            var descriptions = new string[size];
        //
        //            for (var i = 0; i < size; i++)
        //            {
        //                accounts[i] = expenseDetail[i].Acc_no;
        //                amounts[i] = (decimal) expenseDetail[i].amount;
        //                descriptions[i] = expenseDetail[i].description;
        //            }
        //
        //            expenseViewModel.Accounts = accounts;
        //            expenseViewModel.Amount = amounts;
        //            expenseViewModel.Description = descriptions;
        //            expenseViewModel.ExpenseDate = (DateTime) expense.expense_date;
        //
        //            return View(expenseViewModel);
        //        }
        //        [HttpPost]
        //        public ActionResult Edit(int id, ExpenseViewModel model)
        //        {
        //            var parentAccountNumber = "";
        //            var expense = db.tbl_Expense.SingleOrDefault(i => i.expense_id == id);
        //            expense.expense_date = model.ExpenseDate;
        //            db.Entry(expense).State = EntityState.Modified;
        //            db.SaveChanges();
        //            //Remove the previous
        //            var previousExpenseDetail = db.tbl_ExpenseDetail.Where(i => i.expense_id == id);
        //            if (previousExpenseDetail != null)
        //            {
        //                db.tbl_ExpenseDetail.RemoveRange(previousExpenseDetail);
        //                db.SaveChanges();
        //            }
        //            //Add New 
        //            for (var i = 0; i < model.Accounts.Length; i++)
        //            {
        //                if (!string.IsNullOrEmpty(model.Accounts[i]))
        //                {
        //                    parentAccountNumber = model.Accounts[i];
        //                }
        //                var expensDetail = new tbl_ExpenseDetail
        //                {
        //                    expense_id = expense.expense_id,
        //                    Acc_no = parentAccountNumber,
        //                    description = model.Description[i],
        //                    amount = model.Amount[i]
        //                };
        //                db.tbl_ExpenseDetail.Add(expensDetail);
        //            }
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }


    }
}