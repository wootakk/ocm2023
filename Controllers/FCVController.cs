using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using EntityState = System.Data.Entity.EntityState;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    public class FCVController : Controller
    {
        ISFMOCM_DBEntities context = new ISFMOCM_DBEntities();

        [AllowAnonymous]
        public ActionResult ListFcv(string fcv_id, string fcv_no, string letter_no)
        {
            var FCV = new List<FCVModel>();
            if (!string.IsNullOrEmpty(fcv_id))
            {
                var id = int.Parse(fcv_id);
                var fcv = context.tbl_FCV.Where(ID => ID.FCV_Identity == id).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            else if (!string.IsNullOrEmpty(fcv_no))
            {
                var fcv = context.tbl_FCV.Where(ID => ID.FCV_no == fcv_no).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            else if (!string.IsNullOrEmpty(letter_no))
            {
                var fcv = context.tbl_FCV.Where(ID => ID.Letter_no == letter_no).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            else
            {
                var fcv = context.tbl_FCV.Where(ID => ID.status == false).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            return View(FCV);
        }

        // GET: FCV
        [AllowAnonymous]
        public ActionResult Index(string page, string fcv_id, string fcv_no, string letter_no)
        {
            var FCV = new List<FCVModel>();
            if (!string.IsNullOrEmpty(fcv_id))
            {
                var id = int.Parse(fcv_id);
                var fcv = context.tbl_FCV.Where(ID => ID.FCV_Identity == id).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            else if (!string.IsNullOrEmpty(fcv_no))
            {
                var fcv = context.tbl_FCV.Where(ID => ID.FCV_no == fcv_no).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            else if (!string.IsNullOrEmpty(letter_no))
            {
                var fcv = context.tbl_FCV.Where(ID => ID.Letter_no == letter_no).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            else
            {
                var fcv = context.tbl_FCV.Where(ID => ID.status == false).OrderByDescending(ID => ID.created_date);
                foreach (var F in fcv)
                {
                    FCV.Add(CommonClass.ConvertEntityModelToModel(F));
                }
            }
            return View(FCV);
        }

        // GET: FCV/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var tbFCV = context.tbl_FCV.SingleOrDefault(F => F.FCV_Identity == id);
            if (tbFCV == null) return new HttpNotFoundResult();
            if (tbFCV.created_date.Value.Year < DateTime.Now.Year) return RedirectToAction("Index");
            var FCV = CommonClass.ConvertEntityModelToModel(tbFCV);
            ViewBag.Acc_no = CommonClass.GetAllAccountModels();
            ViewBag.AccountsBudget = CommonClass.GetAccountsBudget();
            //ViewBag.Department_of_commitment = new SelectList(context.tbl_Unit.Where(s => s.status != true), "unit_id", "unit_name", tbFCV.unit_id);
            ViewBag.Department_of_commitment = new SelectList(context.tbl_Responsible_Unit.Where(s => s.active==true), "responsible_unit_id", "responsible_unit_name", tbFCV.unit_id);
            return View(FCV);
        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]
        // GET: FCV/Create
        public ActionResult Create()
        {
            ViewBag.AccountsBudget = CommonClass.GetAccountsBudget();
            ViewBag.Acc_no = new SelectList(CommonClass.GetAllAccountModels()
                .Where(acc => acc.Acc_no.Length == 5 ||
                              acc.Acc_no == Constants.OtherBurdenAccountNumber)
                , "Acc_no", "CodeAndName");

            //ViewBag.Department_of_commitment = new SelectList(context.tbl_Unit.Where(u => u.status != true), "unit_id", "unit_name");
            ViewBag.Department_of_commitment = new SelectList(context.tbl_Responsible_Unit.Where(u => u.active==true), "responsible_unit_id", "responsible_unit_name");

            int fcv = 0;
            if (context.tbl_FCV.Count() != 0)
            {
                fcv = Convert.ToInt16(context.tbl_FCV.OrderByDescending(id => id.FCV_Identity).FirstOrDefault().FCV_Identity);
            }
            ViewBag.count = fcv + 1;
            ViewBag.Year = new SelectList(Models.YearModel.GetAllYears(), "year", "year");
            return View();
        }

        // POST: FCV/Create
        [HttpPost]
        public ActionResult Create(FCVModel FCVModel)
        {
            tbl_FCV FCV = new tbl_FCV();

            if (string.IsNullOrEmpty(FCVModel.FCV_no))
            {
                FCV.FCV_no = "0";
            }
            else
            {
                FCV.FCV_no = FCVModel.FCV_no;
            }
            if (ModelState.IsValid)
            {
                FCV.Acc_no = FCVModel.Acc_no;
                FCV.FCV_amount = FCVModel.FCV_amount;
                FCV.FCV_date = FCVModel.FCVDate;
                FCV.Letter_no = FCVModel.Letter_no;
                FCV.Letter_date = FCVModel.Letter_date;
                FCV.MEF_date = FCVModel.MEF_date;
                FCV.MEF_amount = FCVModel.MEF_amount;
                FCV.AmountAfterProcurement = FCVModel.AmountAfterProcurement;
                FCV.Commitment_desc = FCVModel.Commitment_desc;
                FCV.Documentation = FCVModel.Documentation;
                FCV.AmountInLetter = FCVModel.AmountInLetter;
                //FCV.unit_id = context.tbl_Unit.SingleOrDefault(i => i.Unit_id == FCVModel.Department_of_commitment).Unit_id;
                //FCV.Dep_of_commitment = context.tbl_Unit.SingleOrDefault(i => i.Unit_id == FCVModel.Department_of_commitment).Unit_name;
                FCV.unit_id = FCVModel.Department_of_commitment;
                FCV.Dep_of_commitment = context.tbl_Responsible_Unit.SingleOrDefault(i => i.responsible_unit_id == FCVModel.Department_of_commitment).responsible_unit_name;
                FCV.rate = FCVModel.Rate;
                FCV.status = false;
                if (string.Compare(DateTime.Now.Year.ToString(), FCVModel.FCVYear) == 0)
                    FCV.created_date = DateTime.Now;
                else
                    FCV.created_date = new DateTime(Convert.ToInt32(FCVModel.FCVYear), 1, 1);
                FCV.Salary = FCVModel.Salary;
                FCV.Electricity = FCVModel.Electricity;
                FCV.Water = FCVModel.Water;
                FCV.Advance = FCVModel.Advance;
                FCV.Contribution = FCVModel.Contribution;
                FCV.Allowance = FCVModel.Allowance;
                FCV.Other = FCVModel.Other;
                FCV.Telecom = FCVModel.Telecom;
                FCV.PettyCash = FCVModel.PettyCash;
                var lastId = context.tbl_FCV.OrderByDescending(i => i.FCV_id).FirstOrDefault();
                FCV.FCV_Identity =lastId==null?1: lastId.FCV_Identity + 1;
                context.tbl_FCV.Add(FCV);
                context.SaveChanges();
                return RedirectToAction("Edit", new { id = FCV.FCV_Identity });
            }

            //ViewBag.AccountsBudget = GetAccountsBudget();
            //ViewBag.Acc_no = new SelectList(CommonClass.GetAllAccountModels(), "Acc_no", "CodeAndName");
            //ViewBag.Department_of_commitment = new SelectList(context.tbl_Unit.Where(u => u.status != true), "unit_id", "unit_name");
            return View(FCVModel);
        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]
        // GET: FCV/Edit/5
        public ActionResult Edit(int? id)
        {
            var tbFCV = context.tbl_FCV.SingleOrDefault(F => F.FCV_Identity == id);
            if (tbFCV == null) return new HttpNotFoundResult();
            //modify when 2019 access 2018
            //if (tbFCV.created_date.Value.Year < DateTime.Now.Year) return RedirectToAction("Index");
            var FCV = CommonClass.ConvertEntityModelToModel(tbFCV);
            ViewBag.Acc_no = CommonClass.GetAllAccountModels().Where(acc => acc.Acc_no.Length == 5 || acc.Acc_no == Constants.OtherBurdenAccountNumber);
            ViewBag.AccountsBudget = CommonClass.GetAccountsBudget();
            //ViewBag.Department_of_commitment = new SelectList(context.tbl_Unit.Where(s => s.status != true), "unit_id", "unit_name", tbFCV.unit_id);
            ViewBag.Department_of_commitment = new SelectList(context.tbl_Responsible_Unit.Where(s => s.active==true), "responsible_unit_id", "responsible_unit_name", tbFCV.unit_id);
            return View(FCV);
        }

        // POST: FCV/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FCVModel FCVModel)
        {

            var FCV = context.tbl_FCV.SingleOrDefault(ID => ID.FCV_Identity == id);
            if (FCV == null) { return new HttpNotFoundResult(); }

            FCV.FCV_no = FCVModel.FCV_no;
            FCV.Acc_no = FCVModel.Acc_no;
            FCV.FCV_amount = FCVModel.FCV_amount;
            FCV.FCV_date = FCVModel.FCVDate;
            FCV.Letter_no = FCVModel.Letter_no;
            FCV.Letter_date = FCVModel.Letter_date;
            FCV.MEF_date = FCVModel.MEF_date;
            FCV.MEF_amount = FCVModel.MEF_amount;
            FCV.AmountAfterProcurement = FCVModel.AmountAfterProcurement;
            FCV.Commitment_desc = FCVModel.Commitment_desc;
            //FCV.unit_id = context.tbl_Unit.SingleOrDefault(i => i.Unit_id == FCVModel.Department_of_commitment).Unit_id;
            //FCV.Dep_of_commitment = context.tbl_Unit.SingleOrDefault(i => i.Unit_id == FCVModel.Department_of_commitment).Unit_name;
            FCV.unit_id = FCVModel.Department_of_commitment;
            FCV.Dep_of_commitment = context.tbl_Responsible_Unit.SingleOrDefault(i => i.responsible_unit_id == FCVModel.Department_of_commitment).responsible_unit_name;
            FCV.Documentation = FCVModel.Documentation;
            FCV.AmountInLetter = FCVModel.AmountInLetter;
            FCV.rate = FCVModel.Rate;
            FCV.status = false;
            FCV.Salary = FCVModel.Salary;
            FCV.Electricity = FCVModel.Electricity;
            FCV.Water = FCVModel.Water;
            FCV.Advance = FCVModel.Advance;
            FCV.Contribution = FCVModel.Contribution;
            FCV.Allowance = FCVModel.Allowance;
            FCV.Other = FCVModel.Other;
            FCV.Telecom = FCVModel.Telecom;
            FCV.PettyCash = FCVModel.PettyCash;
            context.tbl_FCV.Add(FCV);
            context.Entry(FCV).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            //Udpate the FCVNumber in MandateDetail

            var mandateDetail = context.tbl_MandateDetail.Where(i => i.FCV_Identity == FCV.FCV_Identity);
            if (mandateDetail != null && mandateDetail.Any())
            {
                foreach (var md in mandateDetail)
                {
                    md.FCV_no = FCV.FCV_no;
                    context.Entry(md).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
            }
            return RedirectToAction("Edit", new { id = FCV.FCV_Identity });
        }

        [CustomAuthorize(Roles = "Admin")]
        // POST: FCV/Delete/5
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var FCV = context.tbl_FCV.SingleOrDefault(ID => ID.FCV_Identity == id);
                if (FCV == null) { return HttpNotFound(); }
                //context.tbl_FCV.Remove(FCV);
                FCV.status = true;
                context.Entry(FCV).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [AllowAnonymous]
        public ActionResult viewReport(int id)
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);

            DataTable dt = new DataTable();
            dt.Columns.Add("ministry");
            dt.Columns.Add("department");
            dt.Columns.Add("address");
            dt.Columns.Add("acc_ch");
            dt.Columns.Add("acc_ch_name");
            dt.Columns.Add("acc_name");
            dt.Columns.Add("sub_acc");
            dt.Columns.Add("sub_acc_name");
            dt.Columns.Add("acc");
            dt.Columns.Add("acc_no");
            dt.Columns.Add("sub_acc_no");
            dt.Columns.Add("initailBudget");
            dt.Columns.Add("transfer_amount");
            dt.Columns.Add("beforepay");
            dt.Columns.Add("last_budget");
            dt.Columns.Add("thisRequestAmount");
            dt.Columns.Add("TotalAmount");
            dt.Columns.Add("CommitmentDescription");
            dt.Columns.Add("DepartmentOfCommitment");
            dt.Columns.Add("Documentation");
            dt.Columns.Add("fcv_id");
            dt.Columns.Add("year");

            DataRow dr = dt.NewRow();
            var FCV = context.tbl_FCV.SingleOrDefault(ID => ID.FCV_Identity == id);
            var accountChapter = (from acc in context.tbl_Account
                                  join acc_ch in context.tbl_AccountChapter on acc.AccChapter_id equals acc_ch.AccChapter_id
                                  where acc.Acc_no == FCV.Acc_no
                                  select acc_ch).FirstOrDefault();
            FCV.tbl_Account = context.tbl_Account.Where(s => s.Acc_no == FCV.Acc_no).FirstOrDefault();

            dr["ministry"] = "ទីស្ដីការគណះរដ្ឋមន្ត្រី";
            dr["department"] = FCV.Dep_of_commitment;
            dr["address"] = "Location";
            dr["acc_ch_name"] = accountChapter==null?string.Empty:accountChapter.AccChapter_name;
            dr["acc_ch"] = accountChapter == null ? string.Empty : accountChapter.AccChapter_code;
            if (FCV.Acc_no.Length == 4)
            {
                dr["acc_no"] = FCV.tbl_Account.Acc_no;
                dr["acc_name"] = FCV.tbl_Account.Acc_name;
                dr["sub_acc"] = "";
                dr["sub_acc_name"] = "";
            }
            else
            {
                var acc_no = FCV.tbl_Account.Acc_no.Substring(0, 4);
                dr["acc_no"] = FCV.tbl_Account.Acc_no.Substring(0, 4);
                dr["acc_name"] = context.tbl_Account.FirstOrDefault(acc => acc.Acc_no == acc_no).Acc_name;
                dr["sub_acc_no"] = FCV.tbl_Account.Acc_no;
                dr["sub_acc_name"] = FCV.tbl_Account.Acc_name;
            }
            //var accountUnit =
            //    context.tbl_Unit.FirstOrDefault(acc => acc.Acc_no == FCV.Acc_no && acc.Unit_id == FCV.unit_id);
            List<tbl_InitialBudget> IB = new List<tbl_InitialBudget>();
            List<tbl_TransferDetail> TF = new List<tbl_TransferDetail>();
            List<tbl_FCV> REF = new List<tbl_FCV>();
            //if (accountUnit != null)
            //{
            //    IB = CommonClass.GetUnitInitialBudget(FCV.Acc_no, (int) FCV.unit_id);
            //    TF = CommonClass.GetTransferDetail(FCV.Acc_no, (int) FCV.unit_id);
            //    REF = (from fcv in context.tbl_FCV
            //        where fcv.Acc_no == FCV.Acc_no &&
            //              fcv.status == false &&
            //              fcv.unit_id == FCV.unit_id &&
            //              fcv.created_date.Value.Year == DateTime.Now.Year &&
            //              fcv.FCV_Identity < FCV.FCV_Identity
            //        select fcv).ToList();
            //}
            string year = FCV.created_date.Value.Year.ToString();

            IB = CommonClass.GetInitialBudget(FCV.Acc_no,year).Where(u => u.Unit_id == 0).ToList();
            TF = CommonClass.GetTransferDetail(FCV.Acc_no,year);
            REF = (from fcv in context.tbl_FCV
                   where fcv.Acc_no == FCV.Acc_no &&
                         fcv.status == false &&
                         //fcv.created_date.Value.Year == DateTime.Now.Year &&
                         string.Compare(fcv.created_date.Value.Year.ToString(),year)==0 &&
                         fcv.FCV_Identity < FCV.FCV_Identity
                   select fcv).ToList();

            var InitialBudget = IB != null && IB.Any() ? IB.Sum(b => b.Budget) : 0;
            var Transfer = TF != null && TF.Any()
                            ? TF.Sum(t => t.Transfer_add) +
                            TF.Sum(t => t.Transfer_increase) -
                            TF.Sum(t => t.Transfer_decrease) : 0;
            var ReferenceAmount = REF != null && REF.Any() ? REF.Sum(r => r.FCV_amount) : 0;

            dr["initailBudget"] = InitialBudget;
            dr["transfer_amount"] = Transfer;
            dr["beforepay"] = ReferenceAmount;
            dr["last_budget"] = (InitialBudget + Transfer) - ReferenceAmount;

            dr["thisRequestAmount"] = FCV.FCV_amount;
            dr["TotalAmount"] = (InitialBudget + Transfer) - ReferenceAmount - FCV.FCV_amount;
            dr["CommitmentDescription"] = FCV.Commitment_desc;
            dr["DepartmentOfCommitment"] = FCV.Dep_of_commitment;
            dr["Documentation"] = FCV.Documentation;
            dr["fcv_id"] = FCV.FCV_Identity;
            //dr["year"] = CommonClass.GetKhmerNumber(DateTime.Now.Year.ToString());
            dr["year"] = CommonClass.GetKhmerNumber(FCV.created_date.Value.Year.ToString());
            dt.Rows.Add(dr);

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\FCVPaymentAndControlFinance.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("FAC_Payment", dt);
            rv.LocalReport.DataSources.Add(rds);

            ViewBag.ReportViewer = rv;
            return View();
        }

        public JsonResult GetAccountBudgetInformation(string accountNumber, int fcvId, int unitId,string year)
        {
            List<tbl_InitialBudget> initialBudget = null;
            var amountReference = new List<tbl_FCV>();

            initialBudget = CommonClass.GetInitialBudget(accountNumber,year);
            var transfer = CommonClass.GetTransferDetail(accountNumber,year);
            decimal fcvAmount = 0;
            if (fcvId < 0)
            {
                amountReference = CommonClass.GetFcvByAccount(accountNumber,year);
            }
            else
            {
                //amountReference = context.tbl_FCV.Where(fcv =>
                //    fcv.Acc_no == accountNumber &&
                //    fcv.FCV_Identity < fcvId &
                //    fcv.created_date.Value.Year == DateTime.Now.Year &
                //    fcv.status == false
                //).ToList();

                amountReference = context.tbl_FCV.Where(fcv =>
                    fcv.Acc_no == accountNumber &&
                    fcv.FCV_Identity < fcvId &
                    string.Compare(fcv.created_date.Value.Year.ToString(),year)==0 &
                    fcv.status == false
                ).ToList();

                var f = context.tbl_FCV.SingleOrDefault(F => F.FCV_Identity == fcvId);
                fcvAmount = (decimal)f.FCV_amount;
            }

            var arr = amountReference.ToArray();
            foreach (var item in arr)
            {
                var a = item.FCV_Identity;
            }
            var InitialBudget = initialBudget != null && initialBudget.Any() ? initialBudget.First().Budget : 0;
            var Transfer = transfer != null && transfer.Any()
                ? transfer.Sum(t => t.Transfer_add) +
                  transfer.Sum(t => t.Transfer_increase) -
                  transfer.Sum(t => t.Transfer_decrease)
                : 0;
            var TotalBudget = InitialBudget + Transfer;
            var AmountReference = amountReference != null && amountReference.Any()
                ? amountReference.Sum(a => a.FCV_amount)
                : 0;
            var AvailableBudget = TotalBudget - AmountReference - fcvAmount;

            return Json(new
            {
                InitialBudget = InitialBudget,
                Transfer = Transfer,
                TotalBudget = TotalBudget,
                AmountReference = AmountReference,
                AvailableBudget = AvailableBudget

            }, JsonRequestBehavior.AllowGet
            );

            //else
            //{
            //    var transfer = CommonClass.GetTransferDetail(accountNumber,unitId);
            //    decimal fcvAmount = 0;
            //    if (fcvId < 0)
            //    {
            //        amountReference = CommonClass.GetFcvByAccount(accountNumber,unitId);
            //    }
            //    else
            //    {
            //        amountReference = context.tbl_FCV.Where(fcv =>
            //            fcv.Acc_no == accountNumber &&
            //            fcv.FCV_Identity < fcvId &&
            //            fcv.unit_id == unitId &&
            //            fcv.created_date.Value.Year == DateTime.Now.Year &
            //            fcv.status == false
            //        ).ToList();

            //        var f = context.tbl_FCV.SingleOrDefault(F => F.FCV_Identity == fcvId);
            //        fcvAmount = (decimal)f.FCV_amount;
            //    }

            //    var arr = amountReference.ToArray();
            //    foreach (var item in arr)
            //    {
            //        var a = item.FCV_Identity;
            //    }
            //    var InitialBudget = initialBudget != null && initialBudget.Any() ? initialBudget.First().Budget : 0;
            //    var Transfer = transfer != null && transfer.Any()
            //        ? transfer.Sum(t => t.Transfer_add) +
            //          transfer.Sum(t => t.Transfer_increase) -
            //          transfer.Sum(t => t.Transfer_decrease)
            //        : 0;
            //    var TotalBudget = InitialBudget + Transfer;
            //    var AmountReference = amountReference != null && amountReference.Any()
            //        ? amountReference.Sum(a => a.FCV_amount)
            //        : 0;
            //    var AvailableBudget = TotalBudget - AmountReference - fcvAmount;

            //    return Json(new
            //        {
            //            InitialBudget = InitialBudget,
            //            Transfer = Transfer,
            //            TotalBudget = TotalBudget,
            //            AmountReference = AmountReference,
            //            AvailableBudget = AvailableBudget

            //        }, JsonRequestBehavior.AllowGet
            //    );
            //}

        }
    }
}
