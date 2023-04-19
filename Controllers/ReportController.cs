using ISFMOCM_Project.Entity;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Web.Mvc;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ISFMOCM_Project.Models;
using ISFMOCM_Project.Function;
using System;
using System.Data.SqlClient;
using System.Net.Cache;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    [CustomAuthorize(Roles = ("Admin,Cabinate,Management"))]

    public class ReportController : Controller
    {
        //public const int MILLION_RIEL = 1000000;
        private const int NORMAL = 0;
        private const int PETTYCASH = 1;
        private const int WATER = 2;
        private const int ELECTRICITY = 3;
        private const int ALLOWANCE = 4;
        private const int CONTRIBUTION = 5;
        private const int TELECOM = 6;

        ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
        [CustomAuthorize(Roles = ("Admin,Cabinate"))]
        public ActionResult AdvancedCreditAndNormalCreditReport(string st, string et)
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.LocalReport.EnableHyperlinks = true;
            rv.SizeToReportContent = true;
            rv.AsyncRendering = false;
            rv.ZoomMode = ZoomMode.FullPage;

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("chapter"));
            dt.Columns.Add(new DataColumn("chapterName"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("accName"));
            dt.Columns.Add(new DataColumn("subNo"));
            dt.Columns.Add(new DataColumn("subName"));
            dt.Columns.Add(new DataColumn("unitName"));
            dt.Columns.Add(new DataColumn("initialBudget"));
            dt.Columns.Add(new DataColumn("initialBudgetEM"));
            dt.Columns.Add(new DataColumn("pettyMandate"));
            dt.Columns.Add(new DataColumn("normalFCV"));
            dt.Columns.Add(new DataColumn("normalMandate"));
            dt.Columns.Add(new DataColumn("unitBudget"));
            dt.Columns.Add(new DataColumn("pettyCash"));
            dt.Columns.Add(new DataColumn("startDate"));
            dt.Columns.Add(new DataColumn("endDate"));
            dt.Columns.Add(new DataColumn("unitIbEM"));
            dt.Columns.Add(new DataColumn("unitPettycash"));
            dt.Columns.Add(new DataColumn("unitPettyMandate"));
            dt.Columns.Add(new DataColumn("unitNormalFCV"));
            dt.Columns.Add(new DataColumn("unitNormalMandate"));
            dt.Columns.Add(new DataColumn("unitId"));


            if (!(string.IsNullOrEmpty(st) || string.IsNullOrEmpty(et)))
            {
                var startDate = Convert.ToDateTime(st);
                var endDate = Convert.ToDateTime(et);
                var accounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5);
                foreach (var account in accounts)
                {
                    if (account.tbl_Unit.Count > 0)
                    {
                        var units = account.tbl_Unit.OrderBy(acc => acc.Order_Number).Where(u => u.Unit_code == 0);
                        var accountchapter = db.tbl_AccountChapter.Find(account.AccChapter_id);
                        foreach (var item in units)
                        {
                            DataRow dr = dt.NewRow();
                            //var chapter = account.tbl_AccountChapter.AccChapter_code;
                            //var chapterName = account.tbl_AccountChapter.AccChapter_name;
                            var chapter = accountchapter.AccChapter_code;
                            var chapterName = accountchapter.AccChapter_name;
                            var accNo = account.Acc_code;
                            var accName = db.tbl_Account
                                .SingleOrDefault(acc => acc.Acc_no == account.Acc_code)
                                .Acc_name;
                            var subNo = account.Acc_no;
                            var subName = account.Acc_name;
                            var initialBudget = CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no) == null ?
                                 0 : CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no).Budget;
                            var transfer = CommonReportFunctions.GetTransferDetails(subNo, item.Unit_id, startDate, endDate);
                            var pettyMandate = CommonReportFunctions.GetPettyMandatesBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                0 : CommonReportFunctions.GetPettyMandatesBySubAccount(account.Acc_no, startDate, endDate).Sum(a => a.Mandate_amount);
                            var normalFcv = CommonReportFunctions.GetNormaFcvsBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                0 : CommonReportFunctions.GetNormaFcvsBySubAccount(account.Acc_no, startDate, endDate).Sum(f => f.FCV_amount);
                            var pettyCash = CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no) == null ?
                                0 : CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no).PettyCash;
                            var normalMandates = CommonReportFunctions.GetNormalMandateDetailsBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                0 : CommonReportFunctions.GetNormalMandateDetailsBySubAccount(account.Acc_no, startDate, endDate).Sum(m => m.Mandate_amount);

                            var unitBudget = CommonReportFunctions.GetUnitInitialBudget(item.Acc_no, item.Unit_id) == null ?
                                0 : CommonReportFunctions.GetUnitInitialBudget(item.Acc_no, item.Unit_id).Budget;
                            var unitPettyCash = CommonReportFunctions.GetUnitInitialBudget(item.Acc_no, item.Unit_id) == null ?
                                0 : CommonReportFunctions.GetUnitInitialBudget(item.Acc_no, item.Unit_id).PettyCash;
                            var unitPettyMandate = CommonReportFunctions.GetUnitPettyMandate(subNo, item.Unit_id, startDate, endDate) == null ?
                                0 : CommonReportFunctions.GetUnitPettyMandate(subNo, item.Unit_id, startDate, endDate).Sum(u => u.Mandate_amount);
                            var unitNormalFCV = CommonReportFunctions.GetUnitNormalFCV(subNo, item.Unit_id, startDate, endDate) == null ?
                                0 : CommonReportFunctions.GetUnitNormalFCV(subNo, item.Unit_id, startDate, endDate).Sum(f => f.FCV_amount);
                            var unitNormalMandate = CommonReportFunctions.GetUnitNormalMandate(subNo, item.Unit_id, startDate, endDate) == null ?
                                0 : CommonReportFunctions.GetUnitNormalMandate(subNo, item.Unit_id, startDate, endDate).Sum(m => m.Mandate_amount);
                            var previousPettyMandate = CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(subNo, startDate) == null ?
                                0 : CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(subNo, startDate).Sum(a => a.Mandate_amount);
                            var previousNormalMandate = CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(subNo, startDate) == null ?
                                0 : CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(subNo, startDate).Sum(a => a.Mandate_amount);

                            var previousUnitPettyMandate = CommonReportFunctions
                                .GetPreviousPettyMandateDetailBySubAccount(subNo, startDate)
                                .Where(u => u.tbl_Mandate.Unit_id == item.Unit_id);

                            var previousUnitNormalMandate = CommonReportFunctions
                                .GetPreviousNormalMandateDetailBySubAccount(subNo, startDate)
                                .Where(u => u.tbl_Mandate.Unit_id == item.Unit_id);

                            var transferEm = (transfer != null
                                ? (transfer.Sum(t => t.Transfer_add) + transfer.Sum(t => t.Transfer_increase) -
                                   transfer.Sum(t => t.Transfer_decrease))
                                : 0);

                            var initialBudgetEm = initialBudget + transferEm - previousNormalMandate - previousPettyMandate;

                            var unitIbEM = unitBudget + transferEm
                                - (previousUnitNormalMandate != null ? 0 : previousUnitNormalMandate.Sum(a => a.Mandate_amount))
                                - (previousUnitPettyMandate != null ? 0 : previousUnitPettyMandate.Sum(a => a.Mandate_amount));


                            dr["chapter"] = chapter;
                            dr["chapterName"] = chapterName;
                            dr["accNo"] = accNo;
                            dr["accName"] = accName;
                            dr["subNo"] = subNo;
                            dr["subName"] = subName;
                            if (item.Order_Number == 0)
                            {
                                dr["unitName"] = item.Unit_name;

                            }
                            else
                            {
                                dr["unitName"] = CommonClass.GetKhmerNumber(item.Order_Number.ToString()) + " - " + item.Unit_name;
                            }
                            dr["unitBudget"] = unitBudget != null ? unitBudget : 0;
                            dr["initialBudget"] = initialBudget;
                            dr["initialBudgetEM"] = initialBudgetEm;
                            dr["pettyMandate"] = pettyMandate;
                            dr["normalFCV"] = normalFcv;
                            dr["normalMandate"] = normalMandates;
                            dr["pettyCash"] = pettyCash;
                            dr["titleDate"] = CommonReportFunctions.GetTitleDate(startDate, endDate);
                            dr["startDate"] = st;
                            dr["endDate"] = et;
                            dr["unitIbEM"] = unitIbEM;
                            dr["unitPettycash"] = unitPettyCash;
                            dr["unitPettyMandate"] = unitPettyMandate;
                            dr["unitNormalFCV"] = unitNormalFCV;
                            dr["unitNormalMandate"] = unitNormalMandate;
                            dr["unitId"] = item.Unit_id;
                            dt.Rows.Add(dr);

                            var subUnit = db.tbl_Unit.Where(u => u.Unit_code == item.Unit_id);
                            if (subUnit != null && subUnit.Any())
                            {
                                foreach (var unit in subUnit)
                                {
                                    dr = dt.NewRow();
                                    //chapter = account.tbl_AccountChapter.AccChapter_code;
                                    //chapterName = account.tbl_AccountChapter.AccChapter_name;
                                    chapter = accountchapter.AccChapter_code;
                                    chapterName = accountchapter.AccChapter_name;
                                    accNo = account.Acc_code;
                                    accName = db.tbl_Account
                                       .SingleOrDefault(acc => acc.Acc_no == account.Acc_code)
                                       .Acc_name;
                                    subNo = account.Acc_no;
                                    subName = account.Acc_name;
                                    initialBudget = CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no) == null ?
                                       0 : CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no).Budget;
                                    transfer = CommonReportFunctions.GetTransferDetails(subNo, unit.Unit_id, startDate, endDate);
                                    pettyMandate = CommonReportFunctions.GetPettyMandatesBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                       0 : CommonReportFunctions.GetPettyMandatesBySubAccount(account.Acc_no, startDate, endDate).Sum(a => a.Mandate_amount);
                                    normalFcv = CommonReportFunctions.GetNormaFcvsBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                       0 : CommonReportFunctions.GetNormaFcvsBySubAccount(account.Acc_no, startDate, endDate).Sum(f => f.FCV_amount);
                                    pettyCash = CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no) == null ?
                                       0 : CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no).PettyCash;
                                    normalMandates = CommonReportFunctions.GetNormalMandateDetailsBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                       0 : CommonReportFunctions.GetNormalMandateDetailsBySubAccount(account.Acc_no, startDate, endDate).Sum(m => m.Mandate_amount);
                                    unitBudget = CommonReportFunctions.GetUnitInitialBudget(unit.Acc_no, unit.Unit_id) == null ?
                                        0 : CommonReportFunctions.GetUnitInitialBudget(unit.Acc_no, unit.Unit_id).Budget;
                                    unitPettyCash = CommonReportFunctions.GetUnitInitialBudget(unit.Acc_no, unit.Unit_id) == null ?
                                        0 : CommonReportFunctions.GetUnitInitialBudget(unit.Acc_no, unit.Unit_id).PettyCash;
                                    unitPettyMandate = CommonReportFunctions.GetUnitPettyMandate(subNo, unit.Unit_id, startDate, endDate) == null ?
                                        0 : CommonReportFunctions.GetUnitPettyMandate(subNo, unit.Unit_id, startDate, endDate).Sum(u => u.Mandate_amount);
                                    unitNormalFCV = CommonReportFunctions.GetUnitNormalFCV(subNo, unit.Unit_id, startDate, endDate) == null ?
                                        0 : CommonReportFunctions.GetUnitNormalFCV(subNo, unit.Unit_id, startDate, endDate).Sum(f => f.FCV_amount);
                                    unitNormalMandate = CommonReportFunctions.GetUnitNormalMandate(subNo, unit.Unit_id, startDate, endDate) == null ?
                                        0 : CommonReportFunctions.GetUnitNormalMandate(subNo, unit.Unit_id, startDate, endDate).Sum(m => m.Mandate_amount);
                                    previousPettyMandate = CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(subNo, startDate) == null ?
                                        0 : CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(subNo, startDate).Sum(a => a.Mandate_amount);
                                    previousNormalMandate = CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(subNo, startDate) == null ?
                                        0 : CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(subNo, startDate).Sum(a => a.Mandate_amount);

                                    previousUnitPettyMandate = CommonReportFunctions
                                        .GetPreviousPettyMandateDetailBySubAccount(subNo, startDate)
                                        .Where(u => u.tbl_Mandate.Unit_id == item.Unit_id);

                                    previousUnitNormalMandate = CommonReportFunctions
                                        .GetPreviousNormalMandateDetailBySubAccount(subNo, startDate)
                                        .Where(u => u.tbl_Mandate.Unit_id == item.Unit_id);

                                    transferEm = (transfer != null
                                        ? (transfer.Sum(t => t.Transfer_add) + transfer.Sum(t => t.Transfer_increase) -
                                           transfer.Sum(t => t.Transfer_decrease))
                                        : 0);

                                    initialBudgetEm = initialBudget + transferEm - previousNormalMandate - previousPettyMandate;

                                    unitIbEM = unitBudget + transferEm
                                                   - (previousUnitNormalMandate != null ? 0 : previousUnitNormalMandate.Sum(a => a.Mandate_amount))
                                                   - (previousUnitPettyMandate != null ? 0 : previousUnitPettyMandate.Sum(a => a.Mandate_amount));

                                    dr["chapter"] = chapter;
                                    dr["chapterName"] = chapterName;
                                    dr["accNo"] = accNo;
                                    dr["accName"] = accName;
                                    dr["subNo"] = subNo;
                                    dr["subName"] = subName;
                                    dr["unitName"] = unit.Unit_name;
                                    dr["unitBudget"] = unitBudget != null ? unitBudget : 0;
                                    dr["initialBudget"] = initialBudget;
                                    dr["initialBudgetEM"] = initialBudgetEm;
                                    dr["pettyMandate"] = pettyMandate;
                                    dr["normalFCV"] = normalFcv;
                                    dr["normalMandate"] = normalMandates;
                                    dr["pettyCash"] = pettyCash;
                                    dr["titleDate"] = CommonReportFunctions.GetTitleDate(startDate, endDate);
                                    dr["startDate"] = st;
                                    dr["endDate"] = et;
                                    dr["unitIbEM"] = unitIbEM;
                                    dr["unitPettycash"] = unitPettyCash;
                                    dr["unitPettyMandate"] = unitPettyMandate;
                                    dr["unitNormalFCV"] = unitNormalFCV;
                                    dr["unitNormalMandate"] = unitNormalMandate;
                                    dr["unitId"] = unit.Unit_id;

                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        var accountchapter = db.tbl_AccountChapter.Find(account.AccChapter_id);
                        var chapter = accountchapter.AccChapter_code;
                        var chapterName = accountchapter.AccChapter_name;
                        var accNo = account.Acc_code;
                        var accName = db.tbl_Account
                                        .SingleOrDefault(acc => acc.Acc_no == account.Acc_code)
                                        .Acc_name;
                        var subNo = account.Acc_no;
                        var subName = account.Acc_name;
                        var initialBudget = CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no) == null ?
                                        0 : CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no).Budget;
                        var transfer = CommonReportFunctions.GetTransferDetails(subNo, 0, startDate, endDate);
                        var pettyMandate = CommonReportFunctions.GetPettyMandatesBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                        0 : CommonReportFunctions.GetPettyMandatesBySubAccount(account.Acc_no, startDate, endDate).Sum(a => a.Mandate_amount);
                        var normalFcv = CommonReportFunctions.GetNormaFcvsBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                        0 : CommonReportFunctions.GetNormaFcvsBySubAccount(account.Acc_no, startDate, endDate).Sum(f => f.FCV_amount);
                        var pettyCash = CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no) == null ?
                                        0 : CommonReportFunctions.GetCurrentYearInitialBudget(account.Acc_no).PettyCash;
                        var normalMandates = CommonReportFunctions.GetNormalMandateDetailsBySubAccount(account.Acc_no, startDate, endDate) == null ?
                                        0 : CommonReportFunctions.GetNormalMandateDetailsBySubAccount(account.Acc_no, startDate, endDate).Sum(m => m.Mandate_amount);
                        var previousPettyMandate = CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(subNo, startDate) == null ?
                            0 : CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(subNo, startDate).Sum(a => a.Mandate_amount);
                        var previousNormalMandate = CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(subNo, startDate) == null ?
                            0 : CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(subNo, startDate).Sum(a => a.Mandate_amount);

                        var transferEm = (transfer != null
                            ? (transfer.Sum(t => t.Transfer_add) + transfer.Sum(t => t.Transfer_increase) -
                               transfer.Sum(t => t.Transfer_decrease))
                            : 0);

                        var initialBudgetEm = initialBudget + transferEm - previousNormalMandate - previousPettyMandate;

                        dr["chapter"] = chapter;
                        dr["chapterName"] = chapterName;
                        dr["accNo"] = accNo;
                        dr["accName"] = accName;
                        dr["subNo"] = subNo;
                        dr["subName"] = subName;
                        dr["unitName"] = "";
                        dr["initialBudget"] = initialBudget;
                        dr["initialBudgetEM"] = initialBudgetEm;
                        dr["pettyMandate"] = pettyMandate;
                        dr["normalFCV"] = normalFcv;
                        dr["normalMandate"] = normalMandates;
                        dr["pettyCash"] = pettyCash;
                        dr["titleDate"] = CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["startDate"] = st;
                        dr["endDate"] = et;
                        dt.Rows.Add(dr);

                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\AdvanceCreditAndNormalCreditReportSample.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("AdvancedCreditAndNormalCredit", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult SalaryReport(string st, string et)
        {
            ViewBag.Title = "របាយការណ៍សរុបចំណាយប្រាក់បៀវត្ស";
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.LocalReport.EnableHyperlinks = true;
            rv.SizeToReportContent = true;
            rv.AsyncRendering = false;
            rv.ZoomMode = ZoomMode.FullPage;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("header"));
            dt.Columns.Add(new DataColumn("sub_no"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("fcv_no"));
            dt.Columns.Add(new DataColumn("fcv_date"));
            dt.Columns.Add(new DataColumn("mandate_no"));
            dt.Columns.Add(new DataColumn("mandate_date"));
            dt.Columns.Add(new DataColumn("amount"));
            dt.Columns.Add(new DataColumn("mandate_lm"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate);
            if (mandates != null && mandates.Count > 0)
            {
                mandates = mandates.Where(b => b.tbl_Mandate.Salary == true).ToList();
                foreach (var mandate in mandates)
                {
                    var dr = dt.NewRow();
                    var mandateLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate);
                    var header = "របាយការណ៍សរុបចំណាយប្រាក់បៀវត្ស" + "\n" +
                                CommonReportFunctions.GetTitleDate(startDate, endDate);
                    var fcv = db.tbl_FCV.FirstOrDefault(f => f.FCV_Identity == mandate.FCV_Identity);
                    dr["header"] = header;
                    dr["sub_no"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["fcv_no"] = mandate.FCV_no;
                    dr["fcv_date"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                    dr["mandate_no"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandate_date"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandate_lm"] = mandateLm == null ? 0 : mandateLm.Sum(b => b.Mandate_amount);
                    dr["amount"] = mandate.Mandate_amount;

                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["header"] = "របាយការណ៍សរុបចំណាយប្រាក់បៀវត្ស";
                dt.Rows.Add(dr);
            }
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\SalaryReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("SalaryDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            //ExportReoprt("pdf",rv);

            return View();
        }
        private void ExportReoprt(string filetype, ReportViewer rptviewer)
        {
            try
            {
                //SqlDataAdapter adp = new SqlDataAdapter("SpGetMovie", con);
                //DemoDataSet ds = new DemoDataSet();
                //adp.Fill(ds, "SpGetMovie");
                //ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;

                byte[] bytes = rptviewer.LocalReport.Render(filetype, null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = MIMETYPE;
                Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + extension);
                Response.BinaryWrite(bytes);
                Response.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ActionResult FcvCompareMandateByChapter(string st, string et)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("chapterNo"));
            dt.Columns.Add(new DataColumn("chapterName"));
            dt.Columns.Add(new DataColumn("initialBudget"));
            dt.Columns.Add(new DataColumn("initialBudgetEM"));
            dt.Columns.Add(new DataColumn("transferIncrease"));
            dt.Columns.Add(new DataColumn("transferDecrease"));
            dt.Columns.Add(new DataColumn("fcvLM"));
            dt.Columns.Add(new DataColumn("fcvTM"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("mandateTM"));

            if (!(string.IsNullOrEmpty(st) && string.IsNullOrEmpty(et)))
            {
                string ibYear = CommonClass.GetCurrentYear();
                var startDate = Convert.ToDateTime(st);
                var endDate = Convert.ToDateTime(et);
                var chapters = db.tbl_AccountChapter;
                foreach (var chapter in chapters)
                {
                    var chapterNo = chapter.AccChapter_code;
                    var chapterName = chapter.AccChapter_name;
                    var initalBudget = CommonReportFunctions.GetInitaBudgetsByChpaterId(chapter.AccChapter_id,ibYear);
                    var transfer = CommonReportFunctions.GetTransferDetailsByDateRange(chapter.AccChapter_id, startDate, endDate);
                    var fcvLM = CommonReportFunctions.GetPreviouseFCVByChapter(chapter.AccChapter_id, startDate);
                    var fcvTM = CommonReportFunctions.GetFCVByChapter(chapter.AccChapter_id, startDate, endDate);

                    var mandateLM = CommonReportFunctions.GetPreviousMandateDetialsByChapter(chapter.AccChapter_id, startDate);
                    var mandateTM = CommonReportFunctions.GetMandateDetialsByChapter(chapter.AccChapter_id, startDate, endDate);
                    var mandateNullLM= CommonReportFunctions.GetPreviousMandateDetialsByChapter(chapter.AccChapter_id, startDate,true);
                    var mandateNullTM= CommonReportFunctions.GetMandateDetialsByChapter(chapter.AccChapter_id, startDate, endDate,true);

                    var previousTransfer = CommonReportFunctions.GetPreviousTransferDetailsByChapter(chapter.AccChapter_id, startDate);
                    var previousMandate = CommonReportFunctions.GetPreviousMandateDetialsByChapter(chapter.AccChapter_id, startDate);
                    var tf = previousTransfer != null ? (previousTransfer.Sum(t => t.Transfer_increase) + previousTransfer.Sum(t => t.Transfer_add) - previousTransfer.Sum(t => t.Transfer_decrease)) : 0;
                    var initalBudgetEm = (initalBudget != null ? initalBudget.Sum(b => b.Budget) : 0);
                                        //+ tf
                                        //- (previousMandate != null ? previousMandate.Sum(a => a.Mandate_amount) : 0);

                    var dr = dt.NewRow();
                    dr["titleDate"] = CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["chapterNo"] = chapterNo;
                    dr["chapterName"] = chapterName;
                    dr["initialBudget"] = initalBudget != null ? initalBudget.Sum(b => b.Budget) : 0;
                    dr["initialBudgetEM"] = initalBudgetEm;
                    dr["transferIncrease"] = transfer != null ? (transfer.Sum(a => a.Transfer_add) + transfer.Sum(i => i.Transfer_increase)) : 0;
                    dr["transferDecrease"] = transfer != null ? transfer.Sum(d => d.Transfer_decrease) : 0;
                    dr["fcvLM"] = fcvLM != null ? fcvLM.Sum(f => f.FCV_amount) : 0;
                    dr["fcvTM"] = fcvTM != null ? fcvTM.Sum(f => f.FCV_amount) : 0;

                    dr["mandateLM"] = (mandateLM != null ? mandateLM.Sum(m => m.Mandate_amount) : 0)+ (mandateNullLM != null ? mandateNullLM.Sum(m => m.Mandate_amount) : 0);
                    dr["mandateTM"] =(mandateTM != null ? mandateTM.Sum(m => m.Mandate_amount) : 0) + (mandateNullTM != null ? mandateNullTM.Sum(m => m.Mandate_amount) : 0);

                    dt.Rows.Add(dr);
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\FCVCompareMandateByChapter.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("FCVCompareMandateByChapter", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult FcvCompareMandateByAccounts(string st, string et)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("chapterNo"));
            dt.Columns.Add(new DataColumn("chapterName"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("accName"));
            dt.Columns.Add(new DataColumn("subNo"));
            dt.Columns.Add(new DataColumn("subName"));
            dt.Columns.Add(new DataColumn("unitName"));
            dt.Columns.Add(new DataColumn("initialBudget"));
            dt.Columns.Add(new DataColumn("initialBudgetEM"));
            dt.Columns.Add(new DataColumn("transferIncrease"));
            dt.Columns.Add(new DataColumn("transferDecrease"));
            dt.Columns.Add(new DataColumn("fcvLM"));
            dt.Columns.Add(new DataColumn("fcvTM"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("mandateTM"));
            dt.Columns.Add(new DataColumn("unitBudget"));
            dt.Columns.Add(new DataColumn("unitBudgetLM"));
            dt.Columns.Add(new DataColumn("unitTransferAdd"));
            dt.Columns.Add(new DataColumn("unitTransferDec"));
            dt.Columns.Add(new DataColumn("unitFCVLM"));
            dt.Columns.Add(new DataColumn("unitFCVTM"));
            dt.Columns.Add(new DataColumn("unitMandateLM"));
            dt.Columns.Add(new DataColumn("unitMandateTM"));

            if (!(string.IsNullOrEmpty(st) || string.IsNullOrEmpty(et)))
            {
                var startDate = Convert.ToDateTime(st);
                var endDate = Convert.ToDateTime(et);
                var subAccounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5);
                foreach (var subAccount in subAccounts)
                {
                    if (subAccount.tbl_Unit.Count > 0)
                    {
                        var units = subAccount.tbl_Unit.Where(code => code.Unit_code == 0).OrderBy(o => o.Order_Number);
                        foreach (var unit in units)
                        {
                            var titleDate = CommonReportFunctions.GetTitleDate(startDate, endDate);
                            var chapterNo = subAccount.tbl_AccountChapter.AccChapter_code;
                            var chapterName = subAccount.tbl_AccountChapter.AccChapter_name;
                            var accNo = subAccount.Acc_code;
                            var accName = db.tbl_Account.FirstOrDefault(acc => acc.Acc_no == subAccount.Acc_code).Acc_name;
                            var subNo = subAccount.Acc_no;
                            var subName = subAccount.Acc_name;
                            var initialBudget = CommonReportFunctions.GetCurrentYearInitialBudget(subNo);
                            var initialBudgetEm = CommonReportFunctions.GetCurrentYearInitialBudget(subNo);
                            var transfer = CommonReportFunctions.GetTransferDetailsByDateRange(subNo, startDate, endDate);
                            var fcvLM = CommonReportFunctions.GetPreviousFcvsByAccount(subNo, startDate);
                            var fcvTM = CommonReportFunctions.GetFcvByDateRange(subNo, startDate, endDate);
                            var mandateLM = CommonReportFunctions.GetPreviousMandateDetailByAccount(subNo, startDate);
                            var mandateTM = CommonReportFunctions.GetMandateDetailByDateRange(subNo, startDate, endDate);
                            var transferAdd = transfer != null
                                ? (transfer.Sum(t => t.Transfer_add) + transfer.Sum(t => t.Transfer_increase))
                                : 0;
                            var transferDec = transfer != null ? transfer.Sum(t => t.Transfer_decrease) : 0;

                            //Unit Parts
                            var unitIb = CommonReportFunctions.GetUnitInitialBudget(unit.Acc_no, unit.Unit_id);
                            var unitFcvLm = CommonReportFunctions.GetUnitPreviousFcvs(unit.Acc_no, unit.Unit_id, startDate);
                            var unitFcvTm = CommonReportFunctions.GetUnitFcvs(unit.Acc_no, unit.Unit_id, startDate, endDate);
                            var unitMandateLm = CommonReportFunctions.GetPreviousUnitMandateDetails(unit.Acc_no, unit.Unit_id, startDate);
                            var unitMandateTm = CommonReportFunctions.GetUnitMandateDetails(unit.Acc_no, unit.Unit_id, startDate, endDate);


                            var dr = dt.NewRow();
                            dr["titleDate"] = titleDate;
                            dr["chapterNo"] = chapterNo;
                            dr["chapterName"] = chapterName;
                            dr["accNo"] = accNo;
                            dr["accName"] = accName;
                            dr["subNo"] = subNo;
                            dr["subName"] = subName;
                            dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                            dr["initialBudgetEM"] = initialBudgetEm != null ? initialBudgetEm.Budget : 0;
                            dr["transferIncrease"] = transferAdd;
                            dr["transferDecrease"] = transferDec;
                            dr["fcvLM"] = fcvLM != null ? fcvLM.Sum(f => f.FCV_amount) : 0;
                            dr["fcvTM"] = fcvTM != null ? fcvTM.Sum(f => f.FCV_amount) : 0;
                            dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(m => m.Mandate_amount) : 0;
                            dr["mandateTM"] = mandateTM != null ? mandateTM.Sum(m => m.Mandate_amount) : 0;
                            dr["unitBudget"] = unitIb != null ? unitIb.Budget : 0;
                            dr["unitBudgetLM"] = unitIb != null ? unitIb.Budget : 0;
                            dr["unitTransferAdd"] = 0;
                            dr["unitTransferDec"] = 0;
                            dr["unitFCVLM"] = unitFcvLm != null ? unitFcvLm.Sum(b => b.FCV_amount) : 0;
                            dr["unitFCVTM"] = unitFcvTm != null ? unitFcvTm.Sum(b => b.FCV_amount) : 0;
                            dr["unitMandateLM"] = unitMandateLm != null
                                ? unitMandateLm.Sum(m => m.Mandate_amount)
                                : 0;
                            dr["unitMandateTM"] = unitMandateTm != null
                                ? unitMandateTm.Sum(m => m.Mandate_amount)
                                : 0;
                            if (unit.Order_Number == 0)
                            {
                                dr["unitName"] = unit.Unit_name;
                            }
                            else
                            {
                                dr["unitName"] = CommonClass.GetKhmerNumber(unit.Order_Number.ToString()) + " - " + unit.Unit_name;
                            }
                            dt.Rows.Add(dr);

                            //Check for SubUnit
                            var subUnits = db.tbl_Unit.Where(u => u.Unit_code == unit.Unit_id);
                            foreach (var subUnit in subUnits)
                            {
                                titleDate = CommonReportFunctions.GetTitleDate(startDate, endDate);
                                chapterNo = subAccount.tbl_AccountChapter.AccChapter_code;
                                chapterName = subAccount.tbl_AccountChapter.AccChapter_name;
                                accNo = subAccount.Acc_code;
                                accName = db.tbl_Account.FirstOrDefault(acc => acc.Acc_no == subAccount.Acc_code).Acc_name;
                                subNo = subAccount.Acc_no;
                                subName = subAccount.Acc_name;
                                initialBudget = CommonReportFunctions.GetCurrentYearInitialBudget(subNo);
                                initialBudgetEm = CommonReportFunctions.GetCurrentYearInitialBudget(subNo);
                                transfer = CommonReportFunctions.GetTransferDetailsByDateRange(subNo, startDate, endDate);
                                fcvLM = CommonReportFunctions.GetPreviousFcvsByAccount(subNo, startDate);
                                fcvTM = CommonReportFunctions.GetFcvByDateRange(subNo, startDate, endDate);
                                mandateLM = CommonReportFunctions.GetPreviousMandateDetailByAccount(subNo, startDate);
                                mandateTM = CommonReportFunctions.GetMandateDetailByDateRange(subNo, startDate, endDate);
                                transferAdd = transfer != null
                                    ? (transfer.Sum(t => t.Transfer_add) + transfer.Sum(t => t.Transfer_increase))
                                    : 0;
                                transferDec = transfer != null ? transfer.Sum(t => t.Transfer_decrease) : 0;

                                //Unit Parts
                                unitIb = CommonReportFunctions.GetUnitInitialBudget(subUnit.Acc_no, subUnit.Unit_id);
                                unitFcvLm = CommonReportFunctions.GetUnitPreviousFcvs(subUnit.Acc_no, subUnit.Unit_id, startDate);
                                unitFcvTm = CommonReportFunctions.GetUnitFcvs(subUnit.Acc_no, subUnit.Unit_id, startDate, endDate);
                                unitMandateLm = CommonReportFunctions.GetPreviousUnitMandateDetails(subUnit.Acc_no, subUnit.Unit_id, startDate);
                                unitMandateTm = CommonReportFunctions.GetUnitMandateDetails(subUnit.Acc_no, subUnit.Unit_id, startDate, endDate);


                                dr = dt.NewRow();
                                dr["titleDate"] = titleDate;
                                dr["chapterNo"] = chapterNo;
                                dr["chapterName"] = chapterName;
                                dr["accNo"] = accNo;
                                dr["accName"] = accName;
                                dr["subNo"] = subNo;
                                dr["subName"] = subName;
                                dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                                dr["initialBudgetEM"] = initialBudgetEm != null ? initialBudgetEm.Budget : 0;
                                dr["transferIncrease"] = transferAdd;
                                dr["transferDecrease"] = transferDec;
                                dr["fcvLM"] = fcvLM != null ? fcvLM.Sum(f => f.FCV_amount) : 0;
                                dr["fcvTM"] = fcvTM != null ? fcvTM.Sum(f => f.FCV_amount) : 0;
                                dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(m => m.Mandate_amount) : 0;
                                dr["mandateTM"] = mandateTM != null ? mandateTM.Sum(m => m.Mandate_amount) : 0;
                                dr["unitBudget"] = unitIb != null ? unitIb.Budget : 0;
                                dr["unitBudgetLM"] = unitIb != null ? unitIb.Budget : 0;
                                dr["unitTransferAdd"] = 0;
                                dr["unitTransferDec"] = 0;
                                dr["unitFCVLM"] = unitFcvLm != null ? unitFcvLm.Sum(b => b.FCV_amount) : 0;
                                dr["unitFCVTM"] = unitFcvTm != null ? unitFcvTm.Sum(b => b.FCV_amount) : 0;
                                dr["unitMandateLM"] = unitMandateLm != null
                                    ? unitMandateLm.Sum(m => m.Mandate_amount)
                                    : 0;
                                dr["unitMandateTM"] = unitMandateTm != null
                                    ? unitMandateTm.Sum(m => m.Mandate_amount)
                                    : 0;
                                dr["unitName"] = subUnit.Unit_name;
                                dt.Rows.Add(dr);
                            }

                        }
                    }
                    else
                    {
                        var titleDate = CommonReportFunctions.GetTitleDate(startDate, endDate);
                        var chapterNo = subAccount.tbl_AccountChapter.AccChapter_code;
                        var chapterName = subAccount.tbl_AccountChapter.AccChapter_name;
                        var accNo = subAccount.Acc_code;
                        var accName = db.tbl_Account.FirstOrDefault(acc => acc.Acc_no == subAccount.Acc_code).Acc_name;
                        var subNo = subAccount.Acc_no;
                        var subName = subAccount.Acc_name;
                        var initialBudget = CommonReportFunctions.GetCurrentYearInitialBudget(subNo);
                        var initialBudgetEm = CommonReportFunctions.GetCurrentYearInitialBudget(subNo);
                        var transfer = CommonReportFunctions.GetTransferDetailsByDateRange(subNo, startDate, endDate);
                        var fcvLM = CommonReportFunctions.GetPreviousFcvsByAccount(subNo, startDate);
                        var fcvTM = CommonReportFunctions.GetFcvByDateRange(subNo, startDate, endDate);
                        var mandateLM = CommonReportFunctions.GetPreviousMandateDetailByAccount(subNo, startDate);
                        var mandateTM = CommonReportFunctions.GetMandateDetailByDateRange(subNo, startDate, endDate);
                        var transferAdd = transfer != null
                            ? (transfer.Sum(t => t.Transfer_add) + transfer.Sum(t => t.Transfer_increase))
                            : 0;
                        var transferDec = transfer != null ? transfer.Sum(t => t.Transfer_decrease) : 0;

                        var dr = dt.NewRow();
                        dr["titleDate"] = titleDate;
                        dr["chapterNo"] = chapterNo;
                        dr["chapterName"] = chapterName;
                        dr["accNo"] = accNo;
                        dr["accName"] = accName;
                        dr["subNo"] = subNo;
                        dr["subName"] = subName;
                        dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                        dr["initialBudgetEM"] = initialBudgetEm != null ? initialBudgetEm.Budget : 0;
                        dr["transferIncrease"] = transferAdd;
                        dr["transferDecrease"] = transferDec;
                        dr["fcvLM"] = fcvLM != null ? fcvLM.Sum(f => f.FCV_amount) : 0;
                        dr["fcvTM"] = fcvTM != null ? fcvTM.Sum(f => f.FCV_amount) : 0;
                        dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(m => m.Mandate_amount) : 0;
                        dr["mandateTM"] = mandateTM != null ? mandateTM.Sum(m => m.Mandate_amount) : 0;
                        dr["unitName"] = "";
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\FcvCompareMandateByAccounts.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("FcvCompareMandateByAccounts", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult GenericFcvReport(string accountNumber, int unitId, bool advanced, string st, string et)
        {
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);

            if (advanced)
            {
                if (!string.IsNullOrEmpty(accountNumber) && unitId > 0)
                {
                    ViewBag.Title = "របាយការណ៍សរុបបុរេប្រទានថវិកា";
                    ViewBag.ReportViewer = GetAdvancedFcvReportViewerByAccount(accountNumber,unitId ,startDate, endDate);
                    return View();
                }
                else if (!string.IsNullOrEmpty(accountNumber))
                {
                    ViewBag.Title = "របាយការណ៍សរុបបុរេប្រទានថវិកា";
                    ViewBag.ReportViewer = GetAdvancedFcvReportViewerByAccount(accountNumber,-1, startDate, endDate);
                    return View();
                }
                else
                {
                    ViewBag.Title = "របាយការណ៍បុរេប្រទានថវិកា តាមអនុគណនី";
                    ViewBag.ReportViewer = GetTotalAdvancedFcvReportViewer(startDate, endDate);
                    return View();
                }
            }
            else
            {

                if (!string.IsNullOrEmpty(accountNumber) && unitId > 0)
                {
                    ViewBag.Title = "បាយការណ៍សលាកប័ត្រចំណាយ តាមអនុគណនី ";
                    ViewBag.ReportViewer =
                        GetFcvReportViewerByAccount(accountNumber, unitId, startDate, endDate);
                    return View();
                }
                else if (!string.IsNullOrEmpty(accountNumber))
                {
                    ViewBag.Title = "របាយការណ៍សលាកប័ត្រចំណាយ តាមអនុគណនី ";
                    ViewBag.ReportViewer = GetFcvReportViewerByAccount(accountNumber, -1, startDate, endDate);
                    return View();
                }
                else
                {
                    ViewBag.Title = "របាយការណ៍សរុបសលាកប័ត្រចំណាយ";
                    ViewBag.ReportViewer = GetTotalFcvReportViewer(startDate, endDate);
                    return View();
                }

            }
        }


        public ActionResult GenericMandateReport(string accountNumber, int type, string st, string et, int unitId = -1)
        {
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");
            ViewBag.unitId = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);

            switch (type)
            {
                case NORMAL:
                    ViewBag.Title = "របាយការណ៍សរុបចំណាយអាណត្តិបើកប្រាក់";
                    if (!string.IsNullOrEmpty(accountNumber) && unitId > 0)
                    {
                        ViewBag.ReportViewer = GetMandateReportViewerByAccount(accountNumber, unitId, startDate, endDate);
                        return View();
                    }else if (!string.IsNullOrEmpty(accountNumber))
                    {
                        ViewBag.ReportViewer = GetMandateReportViewerByAccount(accountNumber,-1, startDate, endDate);
                        return View();
                    }
                    else
                    {

                        ViewBag.ReportViewer = GetTotalReportMandateReportViewer(startDate, endDate);
                        return View();
                    }

                case PETTYCASH:
                    ViewBag.Title = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា";
                    ViewBag.ReportViewer = string.IsNullOrEmpty(accountNumber)
                        ? GetTotalPettycashMandateReportViewer(startDate, endDate)
                        : GetPettycashMandateReportViewerByAccountAndUnit(accountNumber, (int)unitId, startDate, endDate);
                    return View();

                case WATER:
                    ViewBag.Title = "របាយការណ៍សរុបចំណាយថ្លៃប្រើប្រាស់ទឹក";
                    ViewBag.ReportViewer = string.IsNullOrEmpty(accountNumber)
                        ? GetTotalWaterMandateReportViewer(startDate, endDate)
                        : GetWaterMandateReportViewerByAccountAndUnit(accountNumber, (int)unitId, startDate, endDate);
                    return View();

                case ELECTRICITY:
                    ViewBag.Title = "របាយការណ៍សរុបចំណាយថ្លៃប្រើប្រាស់អគ្គិសនី";
                    ViewBag.ReportViewer = string.IsNullOrEmpty(accountNumber)
                        ? GetTotalElectricityMandateReportViewer(startDate, endDate)
                        : GetElectricityMandateReportViewerByAccountAndUnit(accountNumber, (int)unitId, startDate, endDate);
                    return View();

                case ALLOWANCE:
                    ViewBag.Title = "របាយការណ៍សរុបចំណាយប្រាក់ឧបត្ថម្ភ";
                    ViewBag.ReportViewer = string.IsNullOrEmpty(accountNumber)
                        ? GetTotalAllowanceMandateReportViewer(startDate, endDate)
                        : GetAllowanceMandateReportViewerByAccountAndUnit(accountNumber, (int)unitId, startDate, endDate);
                    return View();

                case CONTRIBUTION:
                    ViewBag.Title = "របាយការណ៍សរុបចំណាយបង់វិភាគទាន";
                    ViewBag.ReportViewer = string.IsNullOrEmpty(accountNumber)
                        ? GetTotalContributionMandateReportViewer(startDate, endDate)
                        : GetContributionMandateReportViewerByAccountAndUnit(accountNumber, (int)unitId, startDate, endDate);
                    return View();
                case TELECOM:
                    ViewBag.Title = "របាយការណ៍សរុបចំណាយទូរគមនាគមន៍";
                    ViewBag.ReportViewer = string.IsNullOrEmpty(accountNumber)
                        ? GetTotalTelecomMandateReportViewer(startDate, endDate)
                        : GetTelecomMandateReportViewerByAccountAndUnit(accountNumber, (int)unitId, startDate, endDate);
                    return View();
            }
            return View();
        }
        //Child Report Viewer
        public ReportViewer GetTotalAdvancedFcvReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("header"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("description"));
            dt.Columns.Add(new DataColumn("number"));
            dt.Columns.Add(new DataColumn("date"));
            dt.Columns.Add(new DataColumn("amount"));
            dt.Columns.Add(new DataColumn("totalLM"));

            var fcvs = CommonReportFunctions.GetAllFvcs(startDate, endDate).Where(f => f.Advance == true).ToList();
            var fcvLm = CommonReportFunctions.GetPreviouseFCVByDateRage(startDate).Where(f => f.Advance == true);
            if (fcvs != null && fcvs.Count > 0)
            {
                foreach (var fcv in fcvs.OrderBy(f => f.FCV_no))
                {
                    var dr = dt.NewRow();
                    dr["header"] = "របាយការណ៍សរុបបុរេប្រទានថវិកា" + "\n" +
                                   CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = fcv.Acc_no;
                    dr["description"] = fcv.Commitment_desc;
                    dr["number"] = fcv.FCV_no;
                    dr["date"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                    dr["amount"] = fcv.FCV_amount;
                    dr["totalLM"] = fcvLm != null ? fcvLm.Sum(f => f.FCV_amount) : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["header"] = "របាយការណ៍សរុបបុរេប្រទានថវិកា";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalFCVReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalFCVDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetAdvancedFcvReportViewerByAccount(string accountNumber,int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("fcvNo"));
            dt.Columns.Add(new DataColumn("fcvDate"));
            dt.Columns.Add(new DataColumn("fcvAmount"));
            dt.Columns.Add(new DataColumn("fcvLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var fcvs = CommonReportFunctions.GetFcvByDateRange(startDate, endDate).Where(f => f.Acc_no == accountNumber).Where(f => f.Advance == true).ToList();
            var fcvLm = CommonReportFunctions.GetPreviouseFCVByDateRage(startDate).Where(f => f.Acc_no == accountNumber).Where(f => f.Advance == true);
            var Ib = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
            string unitName = "";
            if (unitId > 0)
            {
                fcvs = CommonReportFunctions.GetUnitFcvs(accountNumber, unitId, startDate, endDate)
                    .Where(f => f.Advance == true).ToList();
                fcvLm = CommonReportFunctions.GetUnitPreviousFcvs(accountNumber, unitId, startDate)
                    .Where(f => f.Advance == true).ToList();
                Ib = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);
                //unitName = db.tbl_Unit.SingleOrDefault(u => u.Unit_id == unitId).Unit_name;
                unitName = db.tbl_Responsible_Unit.FirstOrDefault(u => u.responsible_unit_id == unitId).responsible_unit_name;

            }

            if (fcvs.Count > 0)
            {
                foreach (var fcv in fcvs.OrderBy(f => f.FCV_no))
                {
                    var dr = dt.NewRow();
                    if(unitId > 0)
                        dr["titleDate"] = "របាយការណ៍បុរេប្រទានថវិកា" + "\n" + "តាមអនុគណនី " +
                                          CommonClass.GetKhmerNumber(accountNumber) + "​ ~ " + unitName+ "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                    else
                        dr["titleDate"] = "របាយការណ៍បុរេប្រទានថវិកា" + "\n" + "តាមអនុគណនី " +
                                          CommonClass.GetKhmerNumber(accountNumber) + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = fcv.Acc_no;
                    dr["desc"] = fcv.Commitment_desc;
                    dr["fcvNo"] = fcv.FCV_no;
                    dr["fcvDate"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                    dr["fcvAmount"] = fcv.FCV_amount;
                    dr["fcvLM"] = fcvLm != null ? fcvLm.Sum(f => f.FCV_amount) : 0;
                    dr["initialBudget"] = Ib != null ? Ib.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍បុរេប្រទានថវិកា" + "\n" + "តាមអនុគណនី ";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericFCVByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericFCVByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetFcvReportViewerByAccount(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("fcvNo"));
            dt.Columns.Add(new DataColumn("fcvDate"));
            dt.Columns.Add(new DataColumn("fcvAmount"));
            dt.Columns.Add(new DataColumn("fcvLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var fcvs = CommonReportFunctions.GetFcvByDateRange(startDate, endDate).Where(f => f.Acc_no == accountNumber).ToList();
            var fcvLm = CommonReportFunctions.GetPreviouseFCVByDateRage(startDate).Where(f => f.Acc_no == accountNumber);
            var Ib = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
            string unitName = "";
            if (unitId > 0)
            {
                fcvs = CommonReportFunctions.GetUnitFcvs(accountNumber, unitId, startDate, endDate);
                fcvLm = CommonReportFunctions.GetUnitPreviousFcvs(accountNumber, unitId, startDate);
                Ib = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);
                //unitName = db.tbl_Unit.FirstOrDefault(u => u.Unit_id == unitId).Unit_name;
                unitName = db.tbl_Responsible_Unit.FirstOrDefault(u => u.responsible_unit_id == unitId).responsible_unit_name;
            }

            decimal inititalBudget= Ib != null ?(decimal) Ib.Budget : 0;
            decimal tranferAmount = CommonClass.GetTransferTotalAmountbyAccount(accountNumber, endDate.Year.ToString());

            if (fcvs.Count > 0)
            {
                foreach (var fcv in fcvs.OrderBy(f => f.FCV_no))
                {
                    var dr = dt.NewRow();
                    if (unitId > 0)
                        dr["titleDate"] = "របាយការណ៍សលាកប័ត្រចំណាយ" + "\n" + "តាមអនុគណនី " +
                                          CommonClass.GetKhmerNumber(accountNumber) + " ~ " + unitName + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                    else
                        dr["titleDate"] = "របាយការណ៍សលាកប័ត្រចំណាយ" + "\n" + "តាមអនុគណនី " +
                                          CommonClass.GetKhmerNumber(accountNumber) + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = fcv.Acc_no;
                    dr["desc"] = fcv.Commitment_desc;
                    dr["fcvNo"] = fcv.FCV_no;
                    dr["fcvDate"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                    dr["fcvAmount"] = fcv.FCV_amount;
                    dr["fcvLM"] = fcvLm != null ? fcvLm.Sum(f => f.FCV_amount) : 0;
                    //dr["initialBudget"] = Ib != null ? Ib.Budget : 0;
                    dr["initialBudget"] = inititalBudget + tranferAmount;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍សលាកប័ត្រចំណាយ" + "\n" + "តាមអនុគណនី ";
                //dr["initialBudget"] = Ib != null ? Ib.Budget : 0;
                dr["initialBudget"] = inititalBudget + tranferAmount;
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericFCVByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericFCVByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalFcvReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("header"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("description"));
            dt.Columns.Add(new DataColumn("number"));
            dt.Columns.Add(new DataColumn("date"));
            dt.Columns.Add(new DataColumn("amount"));
            dt.Columns.Add(new DataColumn("totalLM"));

            var fcvs = CommonReportFunctions.GetAllFvcs(startDate, endDate).ToList();
            var fcvLm = CommonReportFunctions.GetPreviouseFCVByDateRage(startDate);

            if (fcvs.Count > 0)
            {
                foreach (var fcv in fcvs.OrderBy(f => f.FCV_no))
                {
                    var dr = dt.NewRow();
                    dr["header"] = "របាយការណ៍សរុបសលាកប័ត្រចំណាយ" + "\n" +
                                   CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = fcv.Acc_no;
                    dr["description"] = fcv.Commitment_desc;
                    dr["number"] = fcv.FCV_no;
                    dr["date"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                    dr["amount"] = fcv.FCV_amount;
                    dr["totalLM"] = fcvLm != null ? fcvLm.Sum(f => f.FCV_amount) : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["header"] = "របាយការណ៍សរុបសលាកប័ត្រចំណាយ" + "\n";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalFCVReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalFCVDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalReportMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).ToList();
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate);

            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍សរុបចំណាយអាណត្តិបើកប្រាក់" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍សរុបចំណាយអាណត្តិបើកប្រាក់" + "\n";
                dt.Rows.Add(dr);

            }
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetMandateReportViewerByAccount(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).ToList();
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate);
            var initialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
            string unitName = "";
            if (unitId > 0)
            {
                mandates = CommonReportFunctions.GetUnitMandateDetails(accountNumber, unitId, startDate, endDate);
                mandateLm = CommonReportFunctions.GetPreviousUnitMandateDetails(accountNumber, unitId, startDate);
                initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);
                unitName = db.tbl_Unit.SingleOrDefault(u => u.Unit_id == unitId).Unit_name;
            }

            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var dr = dt.NewRow();
                    if (unitId < 0)
                        dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n" + "តាមអនុគណនី​ " +
                                          CommonClass.GetKhmerNumber(accountNumber) + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                    else
                        dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n" + "តាមអនុគណនី​ " +
                                          CommonClass.GetKhmerNumber(accountNumber) + " ~ ​ " + unitName + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n" + "តាមអនុគណនី​ ";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalPettycashMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("unitName"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).Where(m => m.tbl_Mandate.PettyCash == true).ToList();
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate).Where(m => m.tbl_Mandate.PettyCash == true);
            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {

                    var unit= (from u in db.tbl_Unit
                               join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                               where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                               select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    dr["unitName"] = unit == null ? string.Empty : unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា" + "\n";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReportWithUnit.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetPettycashMandateReportViewerByAccountAndUnit(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).Where(p => p.tbl_Mandate.PettyCash == true && p.tbl_Mandate.Unit_id == unitId).ToList();
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate).Where(p => p.tbl_Mandate.PettyCash == true && p.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber,unitId);

            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit=(from u in db.tbl_Unit
                     join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                     where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                     select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    //dr["titleDate"] = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា \n ក្នុងអនុគណនី " +
                    //                  CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                    //                  mandate.tbl_Mandate.tbl_Unit.Unit_name + "\n" +
                    //                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["titleDate"] = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា \n ក្នុងអនុគណនី " +
                                      CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                                      unit==null?string.Empty:unit.u.Unit_name + "\n" +
                                      CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.PettyCash : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា ក្នុងអនុគណនី " + "\n" + " ";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalWaterMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("unitName"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).Where(m => m.tbl_Mandate.Water == true).ToList();
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate).Where(m => m.tbl_Mandate.Water == true);
            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {

                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍សរុបចំណាយថ្លៃប្រើប្រាស់ទឹក" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍សរុបចំណាយថ្លៃប្រើប្រាស់ទឹក" + "\n";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReportWithUnit.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetWaterMandateReportViewerByAccountAndUnit(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).Where(p => p.tbl_Mandate.Water == true && p.tbl_Mandate.Unit_id == unitId).ToList();
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate).Where(p => p.tbl_Mandate.Water == true && p.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber,unitId);

            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ចំណាយថ្លៃប្រើប្រាស់ទឹក \n ក្នុងអនុគណនី " +
                                      CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                                      //mandate.tbl_Mandate.tbl_Unit.Unit_name + "\n" +
                                      unit==null?string.Empty:unit.u.Unit_name + "\n" +
                                      CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍ចំណាយថ្លៃប្រើប្រាស់ទឹក ក្នុងអនុគណនី " + "\n" + "";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalElectricityMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("unitName"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).Where(m => m.tbl_Mandate.Electricity == true).ToList();
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate).Where(m => m.tbl_Mandate.Electricity == true);

            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍សរុបចំណាយថ្លៃប្រើប្រាស់អគ្គិសនី" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍សរុបចំណាយថ្លៃប្រើប្រាស់អគ្គិសនី";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReportWithUnit.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetElectricityMandateReportViewerByAccountAndUnit(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).Where(p => p.tbl_Mandate.Electricity == true && p.tbl_Mandate.Unit_id == unitId).ToList();
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate).Where(p => p.tbl_Mandate.Electricity == true && p.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber,unitId);

            if (mandates.Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ចំណាយថ្លៃប្រើប្រាស់អគ្គិសនី \n ក្នុងអនុគណនី " +
                                      CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                                      //mandate.tbl_Mandate.tbl_Unit.Unit_name + "\n" +
                                      unit==null?string.Empty:unit.u.Unit_name + "\n" +
                                      CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍ចំណាយថ្លៃប្រើប្រាស់អគ្គិសនី ក្នុងអនុគណនី " + "\n" + "";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalAllowanceMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("unitName"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).Where(m => m.tbl_Mandate.Allowance == true);
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate).Where(m => m.tbl_Mandate.Allowance == true);
            if (mandates.ToList().Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍សរុបចំណាយប្រាក់ឧបត្ថម្ភ" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍សរុបចំណាយប្រាក់ឧបត្ថម្ភ";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReportWithUnit.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetAllowanceMandateReportViewerByAccountAndUnit(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).Where(p => p.tbl_Mandate.Allowance == true && p.tbl_Mandate.Unit_id == unitId);
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate).Where(p => p.tbl_Mandate.Allowance == true && p.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber,unitId);

            if (mandates.ToList().Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ចំណាយប្រាក់ឧបត្ថម្ភ \n ក្នុងអនុគណនី " +
                                      CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                                      //mandate.tbl_Mandate.tbl_Unit.Unit_name + "\n" +
                                      unit==null?string.Empty:unit.u.Unit_name+ "\n" +
                                      CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍ចំណាយប្រាក់ឧបត្ថម្ភ ក្នុងអនុគណនី " + "\n" + "";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalContributionMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("unitName"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).Where(m => m.tbl_Mandate.Contribution == true);
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate).Where(m => m.tbl_Mandate.Contribution == true);

            if (mandates.ToList().Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍សរុបចំណាយបង់វិភាគទាន" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }
            else
            {

                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍សរុបចំណាយបង់វិភាគទាន";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReportWithUnit.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetContributionMandateReportViewerByAccountAndUnit(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).Where(p => p.tbl_Mandate.Contribution == true && p.tbl_Mandate.Unit_id == unitId);
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate).Where(p => p.tbl_Mandate.Contribution == true && p.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber,unitId);

            if (mandates.ToList().Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ចំណាយបង់វិភាគទាន \n  ក្នុងអនុគណនី " +
                                      CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                                      //mandate.tbl_Mandate.tbl_Unit.Unit_name + "\n" +
                                      unit==null?string.Empty:unit.u.Unit_name + "\n" +
                                      CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {

                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍ចំណាយបង់វិភាគទាន  ក្នុងអនុគណនី " +
                                  CommonClass.GetKhmerNumber(accountNumber) + "\n" + "";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTotalTelecomMandateReportViewer(DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;
            
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("title"));
            dt.Columns.Add(new DataColumn("subAccount"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("unitName"));

            var mandates = CommonReportFunctions.GetAllMandatesDetails(startDate, endDate).Where(m => m.tbl_Mandate.Telecom == true);
            var mandatesLm = CommonReportFunctions.GetAllPreviouseMandatesDetails(startDate).Where(m => m.tbl_Mandate.Telecom == true);

            if (mandates.ToList().Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["title"] = "របាយការណ៍សរុបចំណាយទូរគមនាគមន៍" + "\n" +
                                  CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["subAccount"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandatesLm != null ? mandatesLm.Sum(a => a.Mandate_amount) : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }
            else
            {

                var dr = dt.NewRow();
                dr["title"] = "របាយការណ៍សរុបចំណាយទូរគមនាគមន៍";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericTotalMandateReportWithUnit.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericTotalMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }
        public ReportViewer GetTelecomMandateReportViewerByAccountAndUnit(string accountNumber, int unitId, DateTime startDate, DateTime endDate)
        {
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var mandates = CommonReportFunctions.GetMandateDetailByDateRange(accountNumber, startDate, endDate).Where(p => p.tbl_Mandate.Telecom == true && p.tbl_Mandate.Unit_id == unitId);
            var mandateLm = CommonReportFunctions.GetPreviousMandateDetailByAccount(accountNumber, startDate).Where(p => p.tbl_Mandate.Telecom == true && p.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);

            if (mandates.ToList().Count > 0)
            {
                foreach (var mandate in mandates.OrderBy(m => m.tbl_Mandate.Mandate_no))
                {

                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();
                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ចំណាយទូរគមនាគមន៍ \n  ក្នុងអនុគណនី " +
                                      CommonClass.GetKhmerNumber(accountNumber) + " ~ " +
                                      //mandate.tbl_Mandate.tbl_Unit.Unit_name + "\n" +
                                      unit==null?string.Empty:unit.u.Unit_name + "\n" +
                                      CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = mandate.Acc_no;
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateLm"] = mandateLm != null ? mandateLm.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Budget : 0;
                    dt.Rows.Add(dr);
                }
            }
            else
            {

                var dr = dt.NewRow();
                dr["titleDate"] = "របាយការណ៍ចំណាយទូរគមនាគមន៍  ក្នុងអនុគណនី " +
                                  CommonClass.GetKhmerNumber(accountNumber) + "\n" + "";
                dt.Rows.Add(dr);
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) +
                                        @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            return rv;
        }

        //Nested Reports in Report One

        public ActionResult PettyCashMandateReport(string accountNumber, string st, string et)
        {
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllAccountModels(), "Acc_no", "CodeAndName");

            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);

            if (accountNumber.Length == 5)
            {
                var pettyMandate = CommonReportFunctions.GetPettyMandatesBySubAccount(accountNumber, startDate, endDate);
                var mandateLM = CommonReportFunctions.GetPreviousPettyMandateDetailBySubAccount(accountNumber, startDate);
                var initialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
                if (pettyMandate != null && pettyMandate.Any())
                {
                    foreach (var mandate in pettyMandate.OrderBy(m => m.tbl_Mandate.Mandate_no))
                    {
                        var dr = dt.NewRow();
                        dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n"
                                          + "តាមអនុគណនី "
                                          + CommonClass.GetKhmerNumber(accountNumber)
                                          + "\n"
                                          + CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                        dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                        dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                        dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                        dr["mandateAmount"] = mandate.Mandate_amount;
                        dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(a => a.Mandate_amount) : 0;
                        dr["initialBudget"] = initialBudget != null ? initialBudget.PettyCash : 0;
                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                var pettyMandate = CommonReportFunctions.GetPettyMandatesByAccount(accountNumber, startDate, endDate);
                var mandateLM = CommonReportFunctions.GetPreviousPettyMandateDetailByAccount(accountNumber, startDate);
                var initialBudget = CommonReportFunctions.GetTotalAccountPettyCash(accountNumber);
                if (pettyMandate != null && pettyMandate.Any())
                {
                    foreach (var mandate in pettyMandate.OrderBy(m => m.tbl_Mandate.Mandate_no))
                    {
                        var dr = dt.NewRow();
                        dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n"
                                          + "តាមគណនី "
                                          + CommonClass.GetKhmerNumber(accountNumber)
                                          + "\n"
                                          + CommonReportFunctions.GetTitleDate(startDate, endDate);

                        dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                        dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                        dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                        dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                        dr["mandateAmount"] = mandate.Mandate_amount;
                        dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(a => a.Mandate_amount) : 0;
                        dr["initialBudget"] = initialBudget;
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult NormalMandateReport(string accountNumber, string st, string et)
        {
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllAccountModels(), "Acc_no", "CodeAndName");

            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);
            if (accountNumber.Length == 5)
            {
                var normalyMandate = CommonReportFunctions.GetNormalMandateDetailsBySubAccount(accountNumber, startDate, endDate);
                var mandateLM = CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(accountNumber, startDate);
                var initialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
                if (normalyMandate != null && normalyMandate.Any())
                {
                    foreach (var mandate in normalyMandate.OrderBy(m => m.tbl_Mandate.Mandate_no))
                    {
                        var dr = dt.NewRow();
                        dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n"
                                          + "តាមអនុគណនី "
                                          + CommonClass.GetKhmerNumber(accountNumber)
                                          + "\n"
                                          + CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                        dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                        dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                        dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                        dr["mandateAmount"] = mandate.Mandate_amount;
                        dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(a => a.Mandate_amount) : 0;
                        dr["initialBudget"] = initialBudget != null ? initialBudget.Direct_Paid : 0;
                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                var normalyMandate = CommonReportFunctions.GetNormalMandateDetailsByAccount(accountNumber, startDate, endDate);
                var mandateLM = CommonReportFunctions.GetPreviousNormalMandateDetailByAccount(accountNumber, startDate);
                var initialBudget = CommonReportFunctions.GetTotalAccountDirectPaid(accountNumber);
                if (normalyMandate != null && normalyMandate.Any())
                {
                    foreach (var mandate in normalyMandate.OrderBy(m => m.tbl_Mandate.Mandate_no))
                    {
                        var dr = dt.NewRow();
                        dr["titleDate"] = "របាយការណ៍ចំណាយអាណត្តិបើកប្រាក់" + "\n"
                                          + "តាមគណនី "
                                          + CommonClass.GetKhmerNumber(accountNumber)
                                          + "\n"
                                          + CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                        dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                        dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                        dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                        dr["mandateAmount"] = mandate.Mandate_amount;
                        dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(a => a.Mandate_amount) : 0;
                        dr["initialBudget"] = initialBudget;
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\GenericMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericMandateByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult NormalFCVReport(string accountNumber, string st, string et)
        {
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllAccountModels(), "Acc_no", "CodeAndName");

            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("fcvNo"));
            dt.Columns.Add(new DataColumn("fcvDate"));
            dt.Columns.Add(new DataColumn("fcvAmount"));
            dt.Columns.Add(new DataColumn("fcvLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);
            if (accountNumber.Length == 5)
            {
                var normalFcvs = CommonReportFunctions.GetNormaFcvsBySubAccount(accountNumber, startDate, endDate);
                var mandateLM = CommonReportFunctions.GetPreviousNormalFcvsBySubAccount(accountNumber, startDate);
                var initialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
                if (normalFcvs != null && normalFcvs.Any())
                {
                    foreach (var fcv in normalFcvs.OrderBy(f => f.FCV_no))
                    {
                        var dr = dt.NewRow();
                        var ib = initialBudget.Direct_Paid == 0 && initialBudget.PettyCash == 0
                            ? initialBudget.Budget
                            : initialBudget.Direct_Paid;
                        dr["titleDate"] = "របាយការណ៍សលាកប័ត្រចំណាយ" + "\n" + "តាមអនុគណនី " +
                                          CommonClass.GetKhmerNumber(accountNumber) + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                        dr["desc"] = fcv.Commitment_desc;
                        dr["fcvNo"] = fcv.FCV_no;
                        dr["fcvDate"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                        dr["fcvAmount"] = fcv.FCV_amount;
                        dr["fcvLM"] = mandateLM != null ? mandateLM.Sum(a => a.FCV_amount) : 0;
                        dr["initialBudget"] = ib;
                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                var normalFcvs = CommonReportFunctions.GetNormalFcvsByAccount(accountNumber, startDate, endDate);
                var mandateLM = CommonReportFunctions.GetPreviousNormalFcvsByAccount(accountNumber, startDate);
                var initialBudget = CommonReportFunctions.GetTotalAccountDirectPaid(accountNumber);
                if (normalFcvs != null && normalFcvs.Any())
                {
                    foreach (var fcv in normalFcvs.OrderBy(f => f.FCV_no))
                    {
                        var dr = dt.NewRow();
                        dr["titleDate"] = "របាយការណ៍សលាកប័ត្រចំណាយ" + "\n" + "តាមគណនី " +
                                          CommonClass.GetKhmerNumber(accountNumber) + "\n" +
                                          CommonReportFunctions.GetTitleDate(startDate, endDate);
                        dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                        dr["desc"] = fcv.Commitment_desc;
                        dr["fcvNo"] = fcv.FCV_no;
                        dr["fcvDate"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                        dr["fcvAmount"] = fcv.FCV_amount;
                        dr["fcvLM"] = mandateLM != null ? mandateLM.Sum(a => a.FCV_amount) : 0;
                        dr["initialBudget"] = initialBudget;
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\GenericFCVByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("GenericFCVByAccountDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult PettycashUnitMandateByAccountReport(string accountNumber, int unitId, string st, string et)
        {
            ViewBag.unitId = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");

            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));
            dt.Columns.Add(new DataColumn("unitName"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);
            if (accountNumber == "62028" && unitId == 30) unitId = 2;
            var pettyMandate = CommonReportFunctions.GetPettyMandatesBySubAccount(accountNumber, startDate, endDate).Where(m => m.tbl_Mandate.Unit_id == unitId);
            var mandateLM = CommonReportFunctions.GetPreviousPettyMandateDetailByAccount(accountNumber, startDate).Where(m => m.tbl_Mandate.Unit_id == unitId);
            if (pettyMandate != null && pettyMandate.Any())
            {
                foreach (var mandate in pettyMandate)
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍រជ្ជទេយ្យបុរេប្រទានថវិកា ក្នុងអនុគណនី " + CommonClass.GetKhmerNumber(accountNumber)
                                      + " \n របស់អង្គភាព "
                                      //+ mandate.tbl_Mandate.tbl_Unit.Unit_name
                                      + unit==null?string.Empty:unit.u.Unit_name
                                      + "\n"
                                      + CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.PettyCash : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\PettycashUnitMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("PettycashUnitMandateByAccount", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult NormalUnitMandateByAccount(string accountNumber, int unitId, string st, string et)
        {
            ViewBag.unitId = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");

            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("mandateNo"));
            dt.Columns.Add(new DataColumn("mandateDate"));
            dt.Columns.Add(new DataColumn("mandateAmount"));
            dt.Columns.Add(new DataColumn("mandateLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));
            dt.Columns.Add(new DataColumn("unitName"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);
            var normalMandate = CommonReportFunctions.GetUnitNormalMandate(accountNumber, unitId, startDate, endDate);
            var mandateLM = CommonReportFunctions.GetPreviousNormalMandateDetailBySubAccount(accountNumber, startDate).Where(m => m.tbl_Mandate.Unit_id == unitId);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);
            if (normalMandate != null && normalMandate.Any())
            {
                foreach (var mandate in normalMandate)
                {
                    var unit = (from u in db.tbl_Unit
                                join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                where u_res.responsible_unit_id == mandate.tbl_Mandate.Unit_id
                                select new { u, u_res }).FirstOrDefault();

                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ទូទាត់ធម្មតា ក្នុងអនុគណនី " + CommonClass.GetKhmerNumber(accountNumber)
                                      + " \n របស់អង្គភាព "
                                      //+ mandate.tbl_Mandate.tbl_Unit.Unit_name
                                      + unit==null?string.Empty:unit.u.Unit_name
                                      + "\n"
                                      + CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                    dr["desc"] = mandate.tbl_Mandate.Mandate_desc;
                    dr["mandateNo"] = mandate.tbl_Mandate.Mandate_no;
                    dr["mandateDate"] = mandate.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy");
                    dr["mandateAmount"] = mandate.Mandate_amount;
                    dr["mandateLM"] = mandateLM != null ? mandateLM.Sum(a => a.Mandate_amount) : 0;
                    dr["initialBudget"] = initialBudget != null ? initialBudget.Direct_Paid : 0;
                    //dr["unitName"] = mandate.tbl_Mandate.tbl_Unit.Unit_name;
                    dr["unitName"] = unit==null?string.Empty:unit.u.Unit_name;
                    dt.Rows.Add(dr);
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\PettycashUnitMandateByAccountReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("PettycashUnitMandateByAccount", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult NormalUnitFcvByAccount(string accountNumber, int unitId, string st, string et)
        {
            ViewBag.accountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");
            ViewBag.unitId = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");

            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            rv.LocalReport.EnableHyperlinks = true;

            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("titleDate"));
            dt.Columns.Add(new DataColumn("accNo"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("fcvNo"));
            dt.Columns.Add(new DataColumn("fcvDate"));
            dt.Columns.Add(new DataColumn("fcvAmount"));
            dt.Columns.Add(new DataColumn("fcvLM"));
            dt.Columns.Add(new DataColumn("initialBudget"));

            var startDate = Convert.ToDateTime(st);
            var endDate = Convert.ToDateTime(et);
            var initialBudget = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unitId);
            if (accountNumber == "62028" && unitId == 30) unitId = 2;
            var normalFcvs = CommonReportFunctions.GetNormaFcvsBySubAccount(accountNumber, startDate, endDate).Where(u => u.unit_id == unitId);
            var mandateLM = CommonReportFunctions.GetPreviousNormalFcvsBySubAccount(accountNumber, startDate).Where(u => u.unit_id == unitId);
            if (normalFcvs != null && normalFcvs.Any())
            {
                foreach (var fcv in normalFcvs)
                {
                    var dr = dt.NewRow();
                    dr["titleDate"] = "របាយការណ៍ទូទាត់ធម្មតា ក្នុងអនុគណនី " + CommonClass.GetKhmerNumber(accountNumber)
                                      + " \n របស់អង្គភាព "
                                      + db.tbl_Unit.FirstOrDefault(u => u.Unit_id == unitId).Unit_name
                                      + "\n"
                                      + CommonReportFunctions.GetTitleDate(startDate, endDate);
                    dr["accNo"] = CommonClass.GetKhmerNumber(accountNumber);
                    dr["desc"] = fcv.Commitment_desc;
                    dr["fcvNo"] = fcv.FCV_no;
                    dr["fcvDate"] = fcv.FCV_date.Value.ToString("dd/MM/yyyy");
                    dr["fcvAmount"] = fcv.FCV_amount;
                    dr["fcvLM"] = mandateLM != null ? mandateLM.Sum(a => a.FCV_amount) : 0;
                    dr["initialBudget"] = initialBudget.PettyCash == 0 && initialBudget.Direct_Paid == 0
                        ? initialBudget.Budget
                        : initialBudget.Direct_Paid;
                    dt.Rows.Add(dr);
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\NormalUnitFcvByAccount.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("NormalUnitFcvByAccount", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }

        //Report Number 5-7
        public ActionResult FcvReport(string st, string et, string subNo)
        {
            return View();
        }
        // GET: Report
        //USED
        public ActionResult RPAccountMandate(string Acc_no)
        {

            ViewBag.Acc_no = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");
            var param = new ReportParameter();
            var rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            //rv.Width = Unit.Percentage(100);
            //rv.Height = Unit.Percentage(100);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("account_no"));
            dt.Columns.Add(new DataColumn("desc"));
            dt.Columns.Add(new DataColumn("letter_no"));
            dt.Columns.Add(new DataColumn("letter_date"));
            dt.Columns.Add(new DataColumn("fcv_no"));
            dt.Columns.Add(new DataColumn("fcv_date"));
            dt.Columns.Add(new DataColumn("fcv_amount"));
            dt.Columns.Add(new DataColumn("fcv_balance"));
            dt.Columns.Add(new DataColumn("mandate_no"));
            dt.Columns.Add(new DataColumn("mandate_date"));
            dt.Columns.Add(new DataColumn("mandate_amount"));
            dt.Columns.Add(new DataColumn("mandate_balance"));
            dt.Columns.Add(new DataColumn("init_budget"));

            if (!string.IsNullOrEmpty(Acc_no))
            {
                var AccNo = Acc_no;
                var FCV = db.tbl_FCV.Where(f => f.Acc_no == AccNo && f.status == false && f.FCV_no != "0" && f.FCV_date.Value.Year == DateTime.Now.Year);
                if (FCV != null && FCV.Any())
                {
                    var IB = db.tbl_InitialBudget.Where(acc => acc.Acc_no == AccNo && acc.InitialBudget_date.Value.Year == DateTime.Now.Year);
                    var Budget = IB != null ? IB.Sum(b => b.Budget) : 0;
                    var MandateBalance = Budget;
                    var FCVBalace = Budget;
                    foreach (var f in FCV.OrderBy(f => f.FCV_no))
                    {
                        var Mandates = db.tbl_MandateDetail.Where(m => m.FCV_no == f.FCV_no && m.tbl_Mandate.status == false && m.tbl_Mandate.Mandate_no != "0" && m.tbl_Mandate.Mandate_date.Value.Year == DateTime.Now.Year);
                        if (Mandates.Any())
                        {
                            foreach (var row in Mandates)
                            {
                                DataRow dr = dt.NewRow();
                                dr["account_no"] = row.Acc_no;
                                dr["desc"] = f.Commitment_desc;
                                dr["letter_no"] = f.Letter_no;
                                dr["letter_date"] = f.Letter_date != null ? f.Letter_date.Value.ToString("dd/MM/yyyy") : "";
                                dr["fcv_no"] = f.FCV_no;
                                dr["fcv_date"] = f.FCV_date != null ? f.FCV_date.Value.ToString("dd/MM/yyyy") : "";
                                dr["fcv_amount"] = f.FCV_amount;
                                dr["fcv_balance"] = FCVBalace - f.FCV_amount;
                                dr["mandate_no"] = row.tbl_Mandate.Mandate_no;
                                dr["mandate_date"] = row.tbl_Mandate.Mandate_date != null ? row.tbl_Mandate.Mandate_date.Value.ToString("dd/MM/yyyy") : "";
                                dr["mandate_amount"] = row.Mandate_amount;
                                dr["mandate_balance"] = MandateBalance - row.Mandate_amount;
                                dr["init_budget"] = Budget;
                                dt.Rows.Add(dr);
                                MandateBalance = MandateBalance - row.Mandate_amount;
                            }
                        }
                        else
                        {
                            DataRow dRow = dt.NewRow();
                            dRow["account_no"] = AccNo;
                            dRow["desc"] = f.Commitment_desc;
                            dRow["letter_no"] = f.Letter_no;
                            dRow["letter_date"] = f.Letter_date != null ? f.Letter_date.Value.ToString("dd/MM/yyyy") : "";
                            dRow["fcv_no"] = f.FCV_no;
                            dRow["fcv_date"] = f.FCV_date != null ? f.FCV_date.Value.ToString("dd/MM/yyyy") : "";
                            dRow["fcv_amount"] = f.FCV_amount;
                            dRow["fcv_balance"] = FCVBalace - f.FCV_amount;
                            dRow["mandate_no"] = "";
                            dRow["mandate_date"] = "";
                            dRow["init_budget"] = Budget;
                            dt.Rows.Add(dRow);
                        }
                        FCVBalace -= f.FCV_amount;
                    }
                }
            }

            param = new ReportParameter("ACC", CommonClass.GetKhmerNumber(Acc_no));

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\MandateReport_BasedOn_FCV_SubAccount.rdlc";
            rv.LocalReport.DataSources.Clear();
            rv.LocalReport.SetParameters(param);
            ReportDataSource rds = new ReportDataSource("SubAccountFVCMandate", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult RPToalFCVCompareToMandate(string date)
        {
            var Month = DateTime.Now.Month;
            if (!string.IsNullOrEmpty(date))
                Month = int.Parse(date);
            var Year = DateTime.Now.Year;
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("type"));
            dt.Columns.Add(new DataColumn("chapter"));
            dt.Columns.Add(new DataColumn("account_no"));
            dt.Columns.Add(new DataColumn("sub_account"));
            dt.Columns.Add(new DataColumn("acc_name"));
            dt.Columns.Add(new DataColumn("financial_rule"));
            dt.Columns.Add(new DataColumn("budget_early_month"));
            dt.Columns.Add(new DataColumn("transfer_add"));
            dt.Columns.Add(new DataColumn("transfer_decrease"));
            dt.Columns.Add(new DataColumn("ocm_mandate_pre"));
            dt.Columns.Add(new DataColumn("ocm_mandate_in"));
            dt.Columns.Add(new DataColumn("bank_mandate_pre"));
            dt.Columns.Add(new DataColumn("bank_mandate_in"));
            dt.Columns.Add(new DataColumn("madate_total"));
            dt.Columns.Add(new DataColumn("year"));
            dt.Columns.Add(new DataColumn("month"));
            dt.Columns.Add(new DataColumn("chapter_name"));
            dt.Columns.Add(new DataColumn("sub_name"));
            dt.Columns.Add(new DataColumn("type_name"));

            if (!string.IsNullOrEmpty(date))
            {
                var Accounts = from acc in db.tbl_Account where acc.Acc_no.Length == 4 select acc;
                var SubAccounts = from acc in db.tbl_Account
                                  where acc.Acc_no.Length == 5
                                  select acc;

                foreach (var account in Accounts)
                {
                    var SubAcc = from sub in SubAccounts
                                 where sub.Acc_no.Substring(0, 4) == account.Acc_no
                                 select sub;
                    if (SubAcc != null && SubAcc.Any())
                    {
                        foreach (var row in SubAcc)
                        {
                            DataRow dr = dt.NewRow();

                            var IB = CommonReportFunction.GetInititalBudgetList(row.Acc_no, Year);
                            var TransferLM = CommonReportFunction.GetTransferPreviousMonthList(row.Acc_no, Month);
                            var TransferTM = CommonReportFunction.GetTransferList(row.Acc_no, Month);
                            var OCMPre = CommonReportFunction.GetFCVPreviousMonthList(row.Acc_no, Month);
                            var OCMIn = CommonReportFunction.GetFCVList(row.Acc_no, Month);
                            var BankPre = CommonReportFunction.GetMandatePreviousMonthList(row.Acc_no, Month);
                            var BankIn = CommonReportFunction.GetMandateList(row.Acc_no, Month);

                            dr["type"] = row.tbl_AccountChapter.tbl_AccountType.AccType_id;
                            dr["type_name"] = "ប្រភេទទី" + CommonClass.GetKhmerNumber(row.tbl_AccountChapter.tbl_AccountType.AccType_id.ToString()) + "៖​ " + row.tbl_AccountChapter.tbl_AccountType.AccType_name;
                            dr["chapter"] = row.tbl_AccountChapter.AccChapter_code;
                            dr["account_no"] = account.Acc_no;
                            dr["sub_account"] = row.Acc_no;
                            dr["chapter_name"] = row.tbl_AccountChapter.AccChapter_name;
                            dr["acc_name"] = account.Acc_name;
                            dr["sub_name"] = row.Acc_name;
                            dr["financial_rule"] = IB != null ? IB.Sum(b => b.Budget) : 0;

                            var BudgetEM = (IB != null ? IB.Sum(b => b.Budget) : 0)
                                            + (TransferLM != null ? TransferLM.Sum(t => t.Transfer_add) : 0)
                                            + (TransferLM != null ? TransferLM.Sum(t => t.Transfer_increase) : 0)
                                            - (TransferLM != null ? TransferLM.Sum(t => t.Transfer_decrease) : 0);

                            dr["budget_early_month"] = BudgetEM;
                            dr["transfer_add"] = TransferTM != null && TransferTM.Any() ? TransferTM.Sum(t => t.Transfer_increase) : 0;
                            dr["transfer_decrease"] = TransferTM != null && TransferTM.Any() ? TransferTM.Sum(t => t.Transfer_decrease) : 0;
                            dr["ocm_mandate_pre"] = OCMPre != null && OCMPre.Any() ? OCMPre.Sum(b => b.AmountAfterProcurement) : 0;
                            dr["ocm_mandate_in"] = OCMIn != null && OCMIn.Any() ? OCMIn.Sum(b => b.AmountAfterProcurement) : 0;
                            dr["bank_mandate_pre"] = BankPre != null && BankPre.Any() ? BankPre.Sum(b => b.Mandate_amount) : 0;
                            dr["bank_mandate_in"] = BankIn != null && BankIn.Any() ? BankIn.Sum(b => b.Mandate_amount) : 0;
                            dr["month"] = CommonClass.GetMonthName(Month);
                            dr["year"] = CommonClass.GetKhmerNumber(Year.ToString());
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        var IB = CommonReportFunction.GetInititalBudgetList(account.Acc_no, Year);
                        var TransferLM = CommonReportFunction.GetTransferPreviousMonthList(account.Acc_no, Month);
                        var TransferTM = CommonReportFunction.GetTransferList(account.Acc_no, Month);
                        var OCMPre = CommonReportFunction.GetFCVPreviousMonthList(account.Acc_no, Month);
                        var OCMIn = CommonReportFunction.GetFCVList(account.Acc_no, Month);
                        var BankPre = CommonReportFunction.GetMandatePreviousMonthList(account.Acc_no, Month);
                        var BankIn = CommonReportFunction.GetMandateList(account.Acc_no, Month);

                        dr["type"] = account.tbl_AccountChapter.tbl_AccountType.AccType_id;
                        dr["type_name"] = "ប្រភេទទី" + CommonClass.GetKhmerNumber(account.tbl_AccountChapter.tbl_AccountType.AccType_id.ToString()) + "៖​ " + account.tbl_AccountChapter.tbl_AccountType.AccType_name;
                        dr["chapter"] = account.tbl_AccountChapter.AccChapter_code;
                        dr["account_no"] = account.Acc_no;
                        dr["chapter_name"] = account.tbl_AccountChapter.AccChapter_name;
                        dr["acc_name"] = account.Acc_name;
                        dr["financial_rule"] = IB != null ? IB.Sum(b => b.Budget) : 0;

                        var BudgetEM = (IB != null ? IB.Sum(b => b.Budget) : 0)
                                               + (TransferLM != null ? TransferLM.Sum(t => t.Transfer_add) : 0)
                                               + (TransferLM != null ? TransferLM.Sum(t => t.Transfer_increase) : 0)
                                               - (TransferLM != null ? TransferLM.Sum(t => t.Transfer_decrease) : 0);

                        dr["budget_early_month"] = BudgetEM;
                        dr["transfer_add"] = TransferTM != null && TransferTM.Any() ? TransferTM.Sum(t => t.Transfer_increase) : 0;
                        dr["transfer_decrease"] = TransferTM != null && TransferTM.Any() ? TransferTM.Sum(t => t.Transfer_decrease) : 0;
                        dr["ocm_mandate_pre"] = OCMPre != null && OCMPre.Any() ? OCMPre.Sum(b => b.AmountAfterProcurement) : 0;
                        dr["ocm_mandate_in"] = OCMIn != null && OCMIn.Any() ? OCMIn.Sum(b => b.AmountAfterProcurement) : 0;
                        dr["bank_mandate_pre"] = BankPre != null && BankPre.Any() ? BankPre.Sum(b => b.Mandate_amount) : 0;
                        dr["bank_mandate_in"] = BankIn != null && BankIn.Any() ? BankIn.Sum(b => b.Mandate_amount) : 0;
                        dr["month"] = CommonClass.GetMonthName(Month);
                        dr["year"] = CommonClass.GetKhmerNumber(Year.ToString());
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\TotalFCVCompareToMandateReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("ExpensedReportByDate", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult RPUnitExpenditure(string date)
        {
            var Month = DateTime.Now.Month;
            var Year = DateTime.Now.Year;
            if (!string.IsNullOrEmpty(date))
                Month = int.Parse(date);
            //Number 1 is the primary for OCM in database
            int Unit_id = 1;
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);

            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("month"));
            dt.Columns.Add(new DataColumn("year"));
            dt.Columns.Add(new DataColumn("chapter_no"));
            dt.Columns.Add(new DataColumn("chapter_name"));
            dt.Columns.Add(new DataColumn("financial_rule"));
            dt.Columns.Add(new DataColumn("initial_budget_this_month"));
            dt.Columns.Add(new DataColumn("transfer_add"));
            dt.Columns.Add(new DataColumn("transfer_decrease"));
            dt.Columns.Add(new DataColumn("ocm_from_first_month"));
            dt.Columns.Add(new DataColumn("ocm_this_month"));
            dt.Columns.Add(new DataColumn("national_bank_from_first_month"));
            dt.Columns.Add(new DataColumn("national_bank_this_month"));
            //dt.Columns.Add(new DataColumn("unit"));

            var Chapter = db.tbl_AccountChapter;
            if (Chapter.Any())
            {
                foreach (var ch in Chapter)
                {
                    DataRow dr = dt.NewRow();
                    var IB = db.tbl_InitialBudget.Where(c => c.tbl_Account.tbl_AccountChapter.AccChapter_id == ch.AccChapter_id && c.status == false);

                    //var Transfer = from transfer in db.tbl_TransferDetail.ToList()
                    //               join accounts in ch.tbl_Account
                    //               on transfer.Acc_no equals accounts.Acc_no
                    //               where transfer.tbl_Transfer.status == false &&
                    //                     transfer.tbl_Transfer.Transfer_date != null &&
                    //                     transfer.tbl_Transfer.Transfer_date.Value.Month == Month &&
                    //                     transfer.tbl_Transfer.Transfer_date.Value.Year == Year
                    //               select new
                    //               {
                    //                   transfer.Transfer_add,
                    //                   transfer.Transfer_decrease
                    //               };

                    var Transfer = from transfer in db.tbl_TransferDetail
                                   join accounts in ch.tbl_Account on transfer.Acc_no equals accounts.Acc_no
                                   join ttransfer in db.tbl_Transfer on transfer.Transfer_no equals ttransfer.Transfer_no
                                   where ttransfer.status == false && ttransfer.Transfer_date != null && ttransfer.Transfer_date.Value.Month == Month && ttransfer.Transfer_date.Value.Year == Year
                                   select new
                                   {
                                       transfer.Transfer_add,
                                       transfer.Transfer_decrease
                                   };

                    var OCM_Pre = from mandate in db.tbl_MandateDetail.ToList()
                                  join account in ch.tbl_Account
                                  on mandate.Acc_no equals account.Acc_no
                                  join fcv in db.tbl_FCV
                                  on mandate.FCV_no equals fcv.FCV_no
                                  where mandate.tbl_Mandate.status == false &&
                                        mandate.tbl_Mandate.Unit_id == Unit_id &&
                                        mandate.tbl_Mandate.Mandate_date != null &&
                                        mandate.tbl_Mandate?.Mandate_date.Value.Month < Month &&
                                        mandate.tbl_Mandate?.Mandate_date.Value.Year == Year &&
                                        fcv.status == false
                                  select fcv;

                    var OCM_In = from mandate in db.tbl_MandateDetail.ToList()
                                 join account in ch.tbl_Account
                                 on mandate.Acc_no equals account.Acc_no
                                 join fcv in db.tbl_FCV
                                 on mandate.FCV_no equals fcv.FCV_no
                                 where mandate.tbl_Mandate.status == false &&
                                       mandate.tbl_Mandate.Unit_id == Unit_id &&
                                       mandate.tbl_Mandate.Mandate_date != null &&
                                       mandate.tbl_Mandate.Mandate_date.Value.Month == Month &&
                                       mandate.tbl_Mandate.Mandate_date.Value.Year == Year &&
                                       fcv.status == false
                                 select fcv;

                    var Bank_Pre = from mandate in db.tbl_MandateDetail.ToList()
                                   join accounts in ch.tbl_Account
                                   on mandate.Acc_no equals accounts.Acc_no
                                   where mandate.tbl_Mandate.status == false &&
                                         mandate.tbl_Mandate.Unit_id == Unit_id &&
                                         mandate.tbl_Mandate.Mandate_date != null &&
                                         mandate.tbl_Mandate.Mandate_date.Value.Month < Month &&
                                         mandate.tbl_Mandate.Mandate_date.Value.Year == Year
                                   select mandate;

                    var Bank_In = from mandate in db.tbl_MandateDetail.ToList()
                                  join accounts in ch.tbl_Account
                                  on mandate.Acc_no equals accounts.Acc_no
                                  where mandate.tbl_Mandate.status == false &&
                                        mandate.tbl_Mandate.Unit_id == Unit_id &&
                                        mandate.tbl_Mandate.Mandate_date != null &&
                                        mandate.tbl_Mandate.Mandate_date.Value.Month == Month &&
                                        mandate.tbl_Mandate.Mandate_date.Value.Year == Year
                                  select mandate;

                    var IBEarlyMonth = IB != null && IB.Any() ? IB.Sum(b => b.Budget) + Transfer.Sum(t => t.Transfer_add) : 0;

                    dr["month"] = CommonClass.GetMonthName(Month);
                    dr["year"] = CommonClass.GetKhmerNumber(Year.ToString());
                    dr["chapter_no"] = ch.AccChapter_code;
                    dr["chapter_name"] = ch.AccChapter_name;
                    dr["financial_rule"] = IB != null && IB.Any() ? IB.Sum(b => b.Budget) : 0;
                    dr["initial_budget_this_month"] = IBEarlyMonth;
                    dr["transfer_add"] = Transfer != null && Transfer.Any() ? Transfer.Sum(t => t.Transfer_add) : 0;
                    dr["transfer_decrease"] = Transfer != null && Transfer.Any() ? Transfer.Sum(t => t.Transfer_decrease) : 0;
                    dr["ocm_from_first_month"] = OCM_Pre != null && OCM_Pre.Any() ? OCM_Pre.Sum(o => o.AmountAfterProcurement) : 0;
                    dr["ocm_this_month"] = OCM_In != null && OCM_In.Any() ? OCM_In.Sum(o => o.AmountAfterProcurement) : 0;
                    dr["national_bank_from_first_month"] = Bank_Pre != null && Bank_Pre.Any() ? Bank_Pre.Sum(b => b.Mandate_amount) : 0;
                    dr["national_bank_this_month"] = Bank_In != null && Bank_In.Any() ? Bank_In.Sum(b => b.Mandate_amount) : 0;

                    dt.Rows.Add(dr);
                }
            }
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\ExpenditureReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("UnitExpenditure", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult RPToalFCVCompareToMandate_byMonth(string date)
        {
            var Month = DateTime.Now.Month;
            var Year = DateTime.Now.Year;
            if (!string.IsNullOrEmpty(date))
                Month = int.Parse(date);

            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("chapter"));
            dt.Columns.Add(new DataColumn("chapter_name"));
            dt.Columns.Add(new DataColumn("account_no"));
            dt.Columns.Add(new DataColumn("sub_account"));
            dt.Columns.Add(new DataColumn("expand_rank"));
            dt.Columns.Add(new DataColumn("financial_rule"));
            dt.Columns.Add(new DataColumn("fcv_pre"));
            dt.Columns.Add(new DataColumn("fcv_in"));
            dt.Columns.Add(new DataColumn("mandate_pre"));
            dt.Columns.Add(new DataColumn("mandate_in"));
            dt.Columns.Add(new DataColumn("month"));
            dt.Columns.Add(new DataColumn("new_budget"));
            dt.Columns.Add(new DataColumn("account_name"));
            dt.Columns.Add(new DataColumn("ch_ib"));
            dt.Columns.Add(new DataColumn("ch_new_ib"));
            dt.Columns.Add(new DataColumn("ch_pre_fcv"));
            dt.Columns.Add(new DataColumn("ch_in_fcv"));
            dt.Columns.Add(new DataColumn("ch_pre_mandate"));
            dt.Columns.Add(new DataColumn("ch_in_mandate"));
            dt.Columns.Add(new DataColumn("acc_ib"));
            dt.Columns.Add(new DataColumn("acc_new_ib"));
            dt.Columns.Add(new DataColumn("acc_pre_fcv"));
            dt.Columns.Add(new DataColumn("acc_in_fcv"));
            dt.Columns.Add(new DataColumn("acc_pre_mandate"));
            dt.Columns.Add(new DataColumn("acc_in_mandate"));

            var Accounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 4);
            var SubAccounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5);

            if (!string.IsNullOrEmpty(date))
            {
                foreach (var acc in Accounts)
                {
                    var data = from subAccount in SubAccounts
                               where subAccount.Acc_no.Substring(0, 4) == acc.Acc_no
                               select subAccount;
                    if (data != null && data.Any())
                    {
                        foreach (var sub in data)
                        {
                            var IB = CommonReportFunction.GetInititalBudgetList(sub.Acc_no, Year);
                            var Transfer = CommonReportFunction.GetTransferList(sub.Acc_no, Month);
                            var FCVPre = CommonReportFunction.GetFCVPreviousMonthList(sub.Acc_no, Month);
                            var FCVIn = CommonReportFunction.GetFCVList(sub.Acc_no, Month);
                            var MandatePre = CommonReportFunction.GetMandatePreviousMonthList(sub.Acc_no, Month);
                            var MandateIn = CommonReportFunction.GetMandateList(sub.Acc_no, Month);
                            var NewBudget = (IB != null ? IB.Sum(b => b.Budget) : 0) + (Transfer != null ? Transfer.Sum(t => t.Transfer_add) : 0);

                            DataRow dr = dt.NewRow();
                            dr["chapter"] = sub.tbl_AccountChapter.AccChapter_code;
                            dr["account_no"] = sub.Acc_no.Substring(0, 4);
                            dr["sub_account"] = sub.Acc_no;
                            dr["chapter_name"] = sub.tbl_AccountChapter.AccChapter_name;
                            dr["account_name"] = acc.Acc_name;
                            dr["expand_rank"] = sub.Acc_name; //Sub Acount Name
                            dr["financial_rule"] = (IB != null ? IB.Sum(b => b.Budget) : 0);
                            dr["new_budget"] = NewBudget;
                            dr["fcv_pre"] = FCVPre.Any() ? FCVPre.Sum(b => b.AmountAfterProcurement) : 0;
                            dr["fcv_in"] = FCVIn.Any() ? FCVIn.Sum(b => b.AmountAfterProcurement) : 0;
                            dr["mandate_pre"] = MandatePre.Any() ? MandatePre.Sum(b => b.Mandate_amount) : 0;
                            dr["mandate_in"] = MandateIn.Any() ? MandateIn.Sum(a => a.Mandate_amount) : 0;
                            dr["month"] = CommonClass.GetMonthName(Month);

                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        var IB = CommonReportFunction.GetInititalBudgetList(acc.Acc_no, Year);
                        var Transfer = CommonReportFunction.GetTransferList(acc.Acc_no, Month);
                        var FCVPre = CommonReportFunction.GetFCVPreviousMonthList(acc.Acc_no, Month);
                        var FCVIn = CommonReportFunction.GetFCVList(acc.Acc_no, Month);
                        var MandatePre = CommonReportFunction.GetMandatePreviousMonthList(acc.Acc_no, Month);
                        var MandateIn = CommonReportFunction.GetMandateList(acc.Acc_no, Month);
                        var NewBudget = (IB != null ? IB.Sum(b => b.Budget) : 0) + (Transfer != null ? Transfer.Sum(t => t.Transfer_add) : 0);

                        DataRow dr = dt.NewRow();

                        dr["chapter"] = acc.tbl_AccountChapter.AccChapter_code;
                        dr["account_no"] = acc.Acc_no;
                        dr["chapter_name"] = acc.tbl_AccountChapter.AccChapter_name;
                        dr["account_name"] = acc.Acc_name;
                        dr["financial_rule"] = (IB != null ? IB.Sum(b => b.Budget) : 0);
                        dr["new_budget"] = NewBudget;
                        dr["fcv_pre"] = FCVPre.Any() ? FCVPre.Sum(b => b.AmountAfterProcurement) : 0;
                        dr["fcv_in"] = FCVIn.Any() ? FCVIn.Sum(b => b.AmountAfterProcurement) : 0;
                        dr["mandate_pre"] = MandatePre.Any() ? MandatePre.Sum(b => b.Mandate_amount) : 0;
                        dr["mandate_in"] = MandateIn.Any() ? MandateIn.Sum(a => a.Mandate_amount) : 0;
                        dr["month"] = CommonClass.GetMonthName(Month);
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\TotalFCVCompareToMandateReportByMonth.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("TotalFCVCompareToMandate", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult RPYearlyOCMExpenditure()
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("type_no"));
            dt.Columns.Add(new DataColumn("type_name"));
            dt.Columns.Add(new DataColumn("chapter_no"));
            dt.Columns.Add(new DataColumn("chapter_name"));
            dt.Columns.Add(new DataColumn("acc_no"));
            dt.Columns.Add(new DataColumn("acc_name"));
            dt.Columns.Add(new DataColumn("sub_no"));
            dt.Columns.Add(new DataColumn("sub_name"));
            dt.Columns.Add(new DataColumn("budget"));
            dt.Columns.Add(new DataColumn("account_budget"));

            var Accounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 4);
            var SubAccounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5);
            if (Accounts != null && Accounts.Any())
            {
                foreach (var account in Accounts)
                {
                    var subAccount = from sub in SubAccounts
                                     where sub.Acc_no.Substring(0, 4) == account.Acc_no
                                     select sub;
                    if (subAccount.Any())
                    {
                        foreach (var row in subAccount)
                        {
                            DataRow dr = dt.NewRow();
                            var IB = db.tbl_InitialBudget.Where(b => b.status == false && b.Acc_no == row.Acc_no &&
                                                                b.InitialBudget_date.Value.Year == DateTime.Now.Year);
                            dr["type_no"] = row.tbl_AccountChapter.tbl_AccountType.AccType_id;
                            dr["type_name"] = "ប្រភេទទី" + CommonClass.GetKhmerNumber(account.tbl_AccountChapter.AccType_id.ToString()) + "​​៖​ " + account.tbl_AccountChapter.tbl_AccountType.AccType_name;
                            dr["chapter_no"] = row.tbl_AccountChapter.AccChapter_code;
                            dr["chapter_name"] = row.tbl_AccountChapter.AccChapter_name;
                            dr["acc_no"] = account.Acc_no;
                            dr["acc_name"] = account.Acc_name;
                            dr["sub_no"] = row.Acc_no;
                            dr["sub_name"] = row.Acc_name;
                            dr["budget"] = IB != null && IB.Any() ? IB.Sum(b => b.Budget) : 0;
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        var IB = db.tbl_InitialBudget.Where(b => b.status == false && b.Acc_no == account.Acc_no &&
                                                               b.InitialBudget_date.Value.Year == DateTime.Now.Year);
                        dr["type_no"] = account.tbl_AccountChapter.tbl_AccountType.AccType_id;
                        dr["type_name"] = "ប្រភេទទី" + CommonClass.GetKhmerNumber(account.tbl_AccountChapter.AccType_id.ToString()) + "​​៖​ " + account.tbl_AccountChapter.tbl_AccountType.AccType_name;
                        dr["chapter_no"] = account.tbl_AccountChapter.AccChapter_code;
                        dr["chapter_name"] = account.tbl_AccountChapter.AccChapter_name;
                        dr["acc_no"] = account.Acc_no;
                        dr["acc_name"] = account.Acc_name;
                        dr["budget"] = IB != null && IB.Any() ? IB.Sum(b => b.Budget) : 0;
                        dt.Rows.Add(dr);
                    }
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\YearlyOCMExpenditureReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("YearlyOCMExpenditure", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public ActionResult PettyCashReport()
        {
            var rv = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                Width = Unit.Percentage(100),
                Height = Unit.Percentage(100),
                AsyncRendering = true,
                CssClass = "width: 100%; border-color: 1px solid red;"
            };
            var dt = new DataTable();

            dt.Columns.Add(new DataColumn("type_no"));
            dt.Columns.Add(new DataColumn("type_name"));
            dt.Columns.Add(new DataColumn("chapter_no"));
            dt.Columns.Add(new DataColumn("chapter_name"));
            dt.Columns.Add(new DataColumn("acc_no"));
            dt.Columns.Add(new DataColumn("acc_name"));
            dt.Columns.Add(new DataColumn("sub_no"));
            dt.Columns.Add(new DataColumn("sub_name"));
            dt.Columns.Add(new DataColumn("budget"));
            dt.Columns.Add(new DataColumn("pettycash"));
            dt.Columns.Add(new DataColumn("selected_budget"));
            dt.Columns.Add(new DataColumn("unit_name"));
            dt.Columns.Add(new DataColumn("unit_ib"));
            dt.Columns.Add(new DataColumn("unit_selected_budget"));
            dt.Columns.Add(new DataColumn("unit_petty_cash"));
            dt.Columns.Add(new DataColumn("unit_direct_cash"));
            dt.Columns.Add(new DataColumn("direct_cash"));

            var subAccounts = db.tbl_Account.Where(
                acc => acc.Acc_no.Length == 5 &&
                (acc.tbl_AccountChapter.tbl_AccountType.AccType_id == 1 ||
                    acc.tbl_AccountChapter.tbl_AccountType.AccType_id == 3) &&
                    acc.tbl_AccountChapter.AccChapter_code != "63" &&
                    acc.tbl_AccountChapter.AccChapter_code != "64"
                );

            foreach (var subAccount in subAccounts)
            {
                if (subAccount.tbl_Unit.Count > 0)
                {
                    var units = subAccount.tbl_Unit.OrderBy(acc => acc.Order_Number).Where(u => u.Unit_code == 0);
                    foreach (var unit in units)
                    {
                        var subIb = CommonReportFunctions.GetSubAccountInitialBudget(subAccount.Acc_no);
                        //                        var subAccountSelectedBudget =CommonReportFunctions.GetSubAccountSelectedBudget(subAccount.Acc_no);
                        //                        var subAccountSelectedPettycash =CommonReportFunctions.GetSubAccountSelectedPettyCash(subAccount.Acc_no);
                        //                        var subAccountSelectedDirectcash =CommonReportFunctions.GetSubAccountSelectedDirectCash(subAccount.Acc_no);
                        var unitIb = CommonReportFunctions.GetUnitInitialBudget(unit.Acc_no, unit.Unit_id);
                        var selectedUnitBudget = unitIb.PettyCash == 0 && unitIb.Direct_Paid == 0
                            ? 0
                            : unitIb.Budget;

                        var dr = dt.NewRow();
                        dr["type_no"] = subAccount.tbl_AccountChapter.AccType_id;
                        dr["type_name"] = "ប្រភេទទី" +
                                          CommonClass.GetKhmerNumber(
                                              subAccount.tbl_AccountChapter.AccType_id.ToString()) + "​ៈ " +
                                          subAccount.tbl_AccountChapter.tbl_AccountType.AccType_name;
                        dr["chapter_no"] = subAccount.tbl_AccountChapter.AccChapter_code;
                        dr["chapter_name"] = subAccount.tbl_AccountChapter.AccChapter_name;
                        dr["acc_no"] = subAccount.Acc_code;
                        dr["acc_name"] = db.tbl_Account.FirstOrDefault(a => a.Acc_no == subAccount.Acc_code).Acc_name;
                        dr["sub_no"] = subAccount.Acc_no;
                        dr["sub_name"] = subAccount.Acc_name;
                        dr["unit_name"] = unit.Order_Number == 0
                            ? unit.Unit_name
                            : CommonClass.GetKhmerNumber(unit.Order_Number.ToString()) + "-" + unit.Unit_name;
                        dr["unit_ib"] = unitIb.Budget;
                        dr["budget"] = subIb.Budget;
                        dr["unit_ib"] = unitIb.Budget;
                        dr["unit_selected_budget"] = selectedUnitBudget;
                        dr["unit_petty_cash"] = unitIb.PettyCash;
                        dr["unit_direct_cash"] = unitIb.Direct_Paid;
                        dr["direct_cash"] = subIb.Direct_Paid == 0 ? subIb.Budget : subIb.Direct_Paid;
                        dr["pettycash"] = subIb.PettyCash;
                        dr["selected_budget"] = subIb.Budget;


                        dt.Rows.Add(dr);

                        var subUnits = db.tbl_Unit.Where(s => s.Unit_code == unit.Unit_id);
                        foreach (var subUnit in subUnits)
                        {
                            subIb = CommonReportFunctions.GetSubAccountInitialBudget(subAccount.Acc_no);
                            //                            subAccountSelectedBudget = CommonReportFunctions.GetSubAccountSelectedBudget(subAccount.Acc_no);
                            //                            subAccountSelectedPettycash = CommonReportFunctions.GetSubAccountSelectedPettyCash(subAccount.Acc_no);
                            //                            subAccountSelectedDirectcash = CommonReportFunctions.GetSubAccountSelectedDirectCash(subAccount.Acc_no);
                            unitIb = CommonReportFunctions.GetUnitInitialBudget(subUnit.Acc_no, subUnit.Unit_id);
                            selectedUnitBudget = unitIb.PettyCash == 0 && unitIb.Direct_Paid == 0
                                ? 0
                                : unitIb.Budget;

                            dr = dt.NewRow();
                            dr["type_no"] = subAccount.tbl_AccountChapter.AccType_id;
                            dr["type_name"] = "ប្រភេទទី" +
                                              CommonClass.GetKhmerNumber(
                                                  subAccount.tbl_AccountChapter.AccType_id.ToString()) + "​ៈ " +
                                              subAccount.tbl_AccountChapter.tbl_AccountType.AccType_name;
                            dr["chapter_no"] = subAccount.tbl_AccountChapter.AccChapter_code;
                            dr["chapter_name"] = subAccount.tbl_AccountChapter.AccChapter_name;
                            dr["acc_no"] = subAccount.Acc_code;
                            dr["acc_name"] = db.tbl_Account.FirstOrDefault(a => a.Acc_no == subAccount.Acc_code).Acc_name;
                            dr["sub_no"] = subAccount.Acc_no;
                            dr["sub_name"] = subAccount.Acc_name;
                            dr["unit_name"] = subUnit.Unit_name;
                            dr["unit_ib"] = unitIb.Budget;
                            dr["budget"] = subIb.Budget;
                            dr["unit_selected_budget"] = selectedUnitBudget;
                            dr["unit_petty_cash"] = unitIb.PettyCash;
                            dr["unit_direct_cash"] = unitIb.Direct_Paid;
                            dr["direct_cash"] = subIb.Direct_Paid == 0 ? subIb.Budget : subIb.Direct_Paid;
                            dr["pettycash"] = subIb.PettyCash;
                            dr["selected_budget"] = subIb.Budget;
                            dt.Rows.Add(dr);

                        }
                    }
                }
                else
                {
                    var subIb = CommonReportFunctions.GetSubAccountInitialBudget(subAccount.Acc_no);
                    //                    var subAccountSelectedBudget = CommonReportFunctions.GetSubAccountSelectedBudget(subAccount.Acc_no);
                    //                    var subAccountSelectedPettycash = CommonReportFunctions.GetSubAccountSelectedPettyCash(subAccount.Acc_no);
                    //                    var subAccountSelectedDirectcash = CommonReportFunctions.GetSubAccountSelectedDirectCash(subAccount.Acc_no);
                    subAccount.tbl_AccountChapter = db.tbl_AccountChapter.Find(subAccount.AccChapter_id);
                    subAccount.tbl_AccountChapter.tbl_AccountType = db.tbl_AccountType.Find(subAccount.tbl_AccountChapter.AccType_id);
                    var dr = dt.NewRow();
                    dr["type_no"] = subAccount.tbl_AccountChapter.AccType_id;
                    dr["type_name"] = "ប្រភេទទី" +
                                      CommonClass.GetKhmerNumber(
                                          subAccount.tbl_AccountChapter.AccType_id.ToString()) + "​ៈ " +
                                      subAccount.tbl_AccountChapter.tbl_AccountType.AccType_name;
                    dr["chapter_no"] = subAccount.tbl_AccountChapter.AccChapter_code;
                    dr["chapter_name"] = subAccount.tbl_AccountChapter.AccChapter_name;
                    dr["acc_no"] = subAccount.Acc_code;
                    dr["acc_name"] = db.tbl_Account.FirstOrDefault(a => a.Acc_no == subAccount.Acc_code).Acc_name;
                    dr["sub_no"] = subAccount.Acc_no;
                    dr["sub_name"] = subAccount.Acc_name;
                    dr["unit_name"] = "";
                    dr["budget"] =subIb==null?0: subIb.Budget;
                    dr["direct_cash"] =subIb==null?0: subIb.Direct_Paid == 0 ? subIb.Budget : subIb.Direct_Paid;
                    dr["pettycash"] = subIb==null?0: subIb.PettyCash;
                    dr["selected_budget"] =subIb==null?0: subIb.Budget;
                    dt.Rows.Add(dr);
                }

            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\InitialPettyCashReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("InitialPettyCash", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }
        public JsonResult GetReportMasterInfomationByAccount(string AccNO)
        {
            if (AccNO == "") return null;
            decimal AccTotalBudget = db.tbl_InitialBudget.Where(ID => ID.Acc_no == AccNO).Sum(B => B.Budget).GetValueOrDefault();
            decimal AccTotalTransfer = db.tbl_TransferDetail.Where(ID => ID.Acc_no == AccNO).Sum(T => T.Transfer_add).GetValueOrDefault();
            decimal AccNewBudget = AccTotalBudget + AccTotalTransfer;
            AccNewBudget = AccNewBudget == 0 ? 1 : AccNewBudget;

            var AccFCVLM = db.tbl_FCV.Where(ID => ID.Acc_no == AccNO).Sum(A => A.AmountAfterProcurement).GetValueOrDefault();
            var AccFCVLMPercent = AccFCVLM / AccNewBudget;
            var AccFCVLMAvailable = AccNewBudget - AccFCVLM;

            var AccTotalFCVMEF = db.tbl_FCV.Where(ID => ID.Acc_no == AccNO).Sum(M => M.MEF_amount).GetValueOrDefault();
            var AccTotalFCVMEFPercent = AccTotalFCVMEF / AccNewBudget;
            var AccTotalFCVMEFAvailable = AccNewBudget - AccTotalBudget;

            var AccTotalMandateLMQuery = db.tbl_MandateDetail.Where(ID => ID.Acc_no == AccNO && ID.FCV_no != "0");
            decimal AccTotalMandateLM = 0;
            if (AccTotalMandateLMQuery.Any())
                AccTotalMandateLM = Convert.ToDecimal(AccTotalMandateLMQuery.Sum(M => M.Mandate_amount));

            var AccTotalMandateLMPercent = AccTotalMandateLM / AccNewBudget;
            var AccTotalMandateLMAvailable = AccNewBudget - AccTotalMandateLM;

            var FCV = from fcv in db.tbl_FCV
                      join mandate in db.tbl_MandateDetail
                      on fcv.Acc_no equals mandate.FCV_no
                      join account in db.tbl_Account
                      on mandate.Acc_no equals AccNO
                      where mandate.FCV_no != "0"
                      select fcv;

            var AccTotalMandateMEF = FCV.Sum(M => M.MEF_amount).GetValueOrDefault();
            var AccTotalMandateMEFPercent = AccTotalMandateMEF / AccNewBudget;
            var AccTotalMandateMEFAvailable = AccNewBudget - AccTotalMandateMEF;
            return Json(
                        new
                        {
                            AccTotalBudget = AccTotalBudget,
                            AccTotalTransfer = AccTotalTransfer,
                            AccNewBudget = AccNewBudget,

                            AccFCVLM = AccFCVLM,
                            AccFCVLMPercent = AccFCVLMPercent,
                            AccFCVLMAvailable = AccFCVLMAvailable,

                            AccTotalFCVMEF = AccTotalFCVMEF,
                            AccTotalFCVMEFPercent = AccTotalFCVMEFPercent,
                            AccTotalFCVMEFAvailable = AccTotalFCVMEFAvailable,

                            AccTotalMandateLM = AccTotalMandateLM,
                            AccTotalMandateLMPercent = AccTotalMandateLMPercent,
                            AccTotalMandateLMAvailable = AccTotalMandateLMAvailable,

                            AccTotalMandateMEF = AccTotalMandateMEF,
                            AccTotalMandateMEFPercent = AccTotalMandateMEFPercent,
                            AccTotalMandateMEFAvailable = AccTotalMandateMEFAvailable

                        }
                            , JsonRequestBehavior.AllowGet
                       );

        }
        public JsonResult GetReportMasterInfomationByChapter(string ChapterNO)
        {
            if (ChapterNO == "") return null;
            decimal ChapterTotalBudget = db.tbl_InitialBudget.Where(ID => ID.tbl_Account.tbl_AccountChapter.AccChapter_code == ChapterNO).Sum(B => B.Budget).GetValueOrDefault();
            decimal ChapterTotalTransfer = db.tbl_TransferDetail.Where(ID => ID.tbl_Account.tbl_AccountChapter.AccChapter_code == ChapterNO).Sum(T => T.Transfer_add).GetValueOrDefault();
            decimal ChapterNewBudget = ChapterTotalBudget + ChapterTotalTransfer;
            ChapterNewBudget = ChapterNewBudget == 0 ? 1 : ChapterNewBudget;

            decimal ChapterFCVLM = db.tbl_FCV.Where(ID => ID.tbl_Account.tbl_AccountChapter.AccChapter_code == ChapterNO).Sum(A => A.AmountAfterProcurement).GetValueOrDefault();
            decimal ChapterFCVLMPercent = ChapterFCVLM / ChapterNewBudget;
            decimal ChapterFCVLMAvailable = ChapterNewBudget - ChapterFCVLM;

            var ChapterTotalFCVMEF = db.tbl_FCV.Where(ID => ID.tbl_Account.tbl_AccountChapter.AccChapter_code == ChapterNO).Sum(M => M.MEF_amount).GetValueOrDefault();
            var ChapterTotalFCVMEFPercent = ChapterTotalFCVMEF / ChapterNewBudget;
            var ChapterTotalFCVMEFAvailable = ChapterNewBudget - ChapterTotalFCVMEF;

            var ChapterTotalMandateLMQuery = db.tbl_MandateDetail.Where(ID => ID.tbl_Account.tbl_AccountChapter.AccChapter_code == ChapterNO && ID.FCV_no != "0");
            decimal ChapterTotalMandateLM = 0;
            if (ChapterTotalMandateLMQuery.Any())
                ChapterTotalMandateLM = Convert.ToDecimal(ChapterTotalMandateLMQuery.Sum(M => M.Mandate_amount));

            var ChapterTotalMandateLMPercent = ChapterTotalMandateLM / ChapterNewBudget;
            var ChapterTotalMandateLMAvailable = ChapterNewBudget - ChapterTotalMandateLM;

            var FCV = from fcv in db.tbl_FCV
                      join mandate in db.tbl_MandateDetail
                      on fcv.Acc_no equals mandate.FCV_no
                      join account in db.tbl_Account
                      on mandate.tbl_Account.tbl_AccountChapter.AccChapter_code equals ChapterNO
                      where mandate.FCV_no != "0"
                      select fcv;

            var ChapterTotalMandateMEF = FCV.Sum(M => M.MEF_amount).GetValueOrDefault();
            var ChapterTotalMandateMEFPercent = ChapterTotalMandateMEF / ChapterNewBudget;
            var ChapterTotalMandateMEFAvailable = ChapterNewBudget - ChapterTotalMandateMEF;
            return Json(
                        new
                        {
                            ChapterTotalBudget = ChapterTotalBudget,
                            ChapterTotalTransfer = ChapterTotalTransfer,
                            ChapterNewBudget = ChapterNewBudget,

                            ChapterFCVLM = ChapterFCVLM,
                            ChapterFCVLMPercent = ChapterFCVLMPercent,
                            ChapterFCVLMAvailable = ChapterFCVLMAvailable,

                            ChapterTotalFCVMEF = ChapterTotalFCVMEF,
                            ChapterTotalFCVMEFPercent = ChapterTotalFCVMEFPercent,
                            ChapterTotalFCVMEFAvailable = ChapterTotalFCVMEFAvailable,

                            ChapterTotalMandateLM = ChapterTotalMandateLM,
                            ChapterTotalMandateLMPercent = ChapterTotalMandateLMPercent,
                            ChapterTotalMandateLMAvailable = ChapterTotalMandateLMAvailable,

                            ChapterTotalMandateMEF = ChapterTotalMandateMEF,
                            ChapterTotalMandateMEFPercent = ChapterTotalMandateMEFPercent,
                            ChapterTotalMandateMEFAvailable = ChapterTotalMandateMEFAvailable

                        }
                            , JsonRequestBehavior.AllowGet
                       );

        }
        public ActionResult ListReport()
        {
            ViewBag.Acc_no = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");
            ViewBag.Unit_id = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");

            return View();
        }
        public ActionResult ReportMenu()
        {
            var model = new ReportMenuViewModel();
            ViewBag.AccountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");
            //ViewBag.UnitId = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");
            ViewBag.UnitId = new SelectList(db.tbl_Responsible_Unit, "responsible_unit_id", "responsible_unit_name");

            return View(model);
        }
        [HttpPost]
        public ActionResult ReportMenu(ReportMenuViewModel model)
        {

            ViewBag.AccountNumber = new SelectList(CommonClass.GetAllSubAccounts(), "Acc_no", "CodeAndName");
            //ViewBag.UnitId = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name");
            ViewBag.UnitId = new SelectList(db.tbl_Responsible_Unit, "responsible_unit_id", "responsible_unit_name");

            var selectedOption = model.OptionNumber;
            switch (selectedOption)
            {
                case 1: return RedirectToAction("AdvancedCreditAndNormalCreditReport", new { st = model.StartDate, et = model.EndDate });
                case 2: return RedirectToAction("SalaryReport", new { st = model.StartDate, et = model.EndDate });
                case 3: return RedirectToAction("FcvCompareMandateByChapter", new { st = model.StartDate, et = model.EndDate });
                case 4: return RedirectToAction("FcvCompareMandateByAccounts", new { st = model.StartDate, et = model.EndDate });
                case 5: return RedirectToAction("GenericFcvReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, advanced = false, st = model.StartDate, et = model.EndDate });
                case 6: return RedirectToAction("GenericMandateReport", new { unitId = model.UnitId, type = 0, st = model.StartDate, et = model.EndDate, accountNumber = model.AccountNumber });
                case 7: return RedirectToAction("GenericFcvReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, advanced = true, st = model.StartDate, et = model.EndDate });
                case 8: return RedirectToAction("GenericMandateReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, type = 1, st = model.StartDate, et = model.EndDate });
                case 9: return RedirectToAction("GenericMandateReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, type = 2, st = model.StartDate, et = model.EndDate });
                case 10: return RedirectToAction("GenericMandateReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, type = 3, st = model.StartDate, et = model.EndDate });
                case 11: return RedirectToAction("GenericMandateReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, type = 4, st = model.StartDate, et = model.EndDate });
                case 12: return RedirectToAction("GenericMandateReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, type = 5, st = model.StartDate, et = model.EndDate });
                case 13: return RedirectToAction("PettyCashReport");
                case 14: return RedirectToAction("GenericMandateReport", new { accountNumber = model.AccountNumber, unitId = model.UnitId, type = 6, st = model.StartDate, et = model.EndDate });
            }

            return View(model);
        }

    }


}