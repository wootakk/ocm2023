using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Models;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using ISFMOCM_Project.Function;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    public class MandateController : Controller
    {
        private ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();

        [AllowAnonymous]
        public ActionResult ListMandate(string s_id,string s_no)
        {
            var Mandate = new List<MandateModel>();
            if (!string.IsNullOrEmpty(s_id))
            {
                var id = int.Parse(s_id);
                //var tbl_Mandate = db.tbl_Mandate.Include(t => t.tbl_Unit).Where(ID => ID.Mandate_id == id && ID.status == false).OrderByDescending(ID => ID.created_date);
                var tbl_Mandate = db.tbl_Mandate.Where(ID => ID.Mandate_id == id && ID.status == false).OrderByDescending(ID => ID.created_date);
                foreach (var MD in tbl_Mandate)
                {
                    var md = CommonClass.ConvertEntityModelToModel(MD);

                    var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == md.Mandate_id).ToArray();
                    md.FCV_no = new string[Mdtail.Length];
                    md.Acc_no = new string[Mdtail.Length];
                    md.Mandate_amount = new decimal[Mdtail.Length];
                    for (var i = 0; i < Mdtail.Length; i++)
                    {
                        md.FCV_no[i] = Mdtail[i].FCV_no;
                        md.Acc_no[i] = Mdtail[i].Acc_no;
                        md.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                    }
                    Mandate.Add(md);
                }
            }
            else if (!string.IsNullOrEmpty(s_no))
            {
                //var tbl_Mandate = db.tbl_Mandate.Include(t => t.tbl_Unit).Where(ID => ID.Mandate_no == s_no && ID.status == false).OrderByDescending(i => i.created_date);
                var tbl_Mandate = db.tbl_Mandate.Where(ID => ID.Mandate_no == s_no && ID.status == false).OrderByDescending(i => i.created_date);
                foreach (var MD in tbl_Mandate)
                {
                    var md = CommonClass.ConvertEntityModelToModel(MD);

                    var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == md.Mandate_id).ToArray();
                    md.FCV_no = new string[Mdtail.Length];
                    md.Acc_no = new string[Mdtail.Length];
                    md.Mandate_amount = new decimal[Mdtail.Length];
                    for (var i = 0; i < Mdtail.Length; i++)
                    {
                        md.FCV_no[i] = Mdtail[i].FCV_no;
                        md.Acc_no[i] = Mdtail[i].Acc_no;
                        md.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                    }
                    Mandate.Add(md);
                }
            }
            else
            {
                //var tbl_Mandate = db.tbl_Mandate.Include(t => t.tbl_Unit).Where(mandate => mandate.status == false).OrderByDescending(ID => ID.Mandate_id);
                var tbl_Mandate = db.tbl_Mandate.Where(mandate => mandate.status == false).OrderByDescending(ID => ID.Mandate_id);
                foreach (var MD in tbl_Mandate)
                {
                    var md = CommonClass.ConvertEntityModelToModel(MD);

                    var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == md.Mandate_id).ToArray();
                    md.FCV_no = new string[Mdtail.Length];
                    md.Acc_no = new string[Mdtail.Length];
                    md.Mandate_amount = new decimal[Mdtail.Length];
                    for (var i = 0; i < Mdtail.Length; i++)
                    {
                        md.FCV_no[i] = Mdtail[i].FCV_no;
                        md.Acc_no[i] = Mdtail[i].Acc_no;
                        md.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                    }

                    Mandate.Add(md);
                }
            }
            return View(Mandate.ToList());
        }
    
        // GET: Mandate
        [AllowAnonymous]
        public ActionResult Index(string page, string s_id, string s_no)
        {
            var Mandate = new List<MandateModel>();
            if (!string.IsNullOrEmpty(s_id))
            {
                var id = int.Parse(s_id);
                //var tbl_Mandate = db.tbl_Mandate.Include(t => t.tbl_Unit).Where(ID => ID.Mandate_id == id && ID.status == false).OrderByDescending(ID => ID.created_date);
                var tbl_Mandate = db.tbl_Mandate.Where(ID => ID.Mandate_id == id && ID.status == false).OrderByDescending(ID => ID.created_date);
                foreach (var MD in tbl_Mandate)
                {
                    var md = CommonClass.ConvertEntityModelToModel(MD);

                    var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == md.Mandate_id).ToArray();
                    md.FCV_no = new string[Mdtail.Length];
                    md.Acc_no = new string[Mdtail.Length];
                    md.Mandate_amount = new decimal[Mdtail.Length];
                    for (var i = 0; i < Mdtail.Length; i++)
                    {
                        md.FCV_no[i] = Mdtail[i].FCV_no;
                        md.Acc_no[i] = Mdtail[i].Acc_no;
                        md.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                    }
                    Mandate.Add(md);
                }
            }
            else if (!string.IsNullOrEmpty(s_no))
            {
                //var tbl_Mandate = db.tbl_Mandate.Include(t => t.tbl_Unit).Where(ID => ID.Mandate_no == s_no && ID.status == false).OrderByDescending(i => i.created_date);
                var tbl_Mandate = db.tbl_Mandate.Where(ID => ID.Mandate_no == s_no && ID.status == false).OrderByDescending(i => i.created_date);
                foreach (var MD in tbl_Mandate)
                {
                    var md = CommonClass.ConvertEntityModelToModel(MD);

                    var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == md.Mandate_id).ToArray();
                    md.FCV_no = new string[Mdtail.Length];
                    md.Acc_no = new string[Mdtail.Length];
                    md.Mandate_amount = new decimal[Mdtail.Length];
                    for (var i = 0; i < Mdtail.Length; i++)
                    {
                        md.FCV_no[i] = Mdtail[i].FCV_no;
                        md.Acc_no[i] = Mdtail[i].Acc_no;
                        md.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                    }
                    Mandate.Add(md);
                }
            }
            else
            {
                //var tbl_Mandate = db.tbl_Mandate.Include(t => t.tbl_Unit).Where(mandate => mandate.status == false).OrderByDescending(ID => ID.Mandate_id);
                var tbl_Mandate = db.tbl_Mandate.Where(mandate => mandate.status == false).OrderByDescending(ID => ID.Mandate_id);
                foreach (var MD in tbl_Mandate)
                {
                    var md = CommonClass.ConvertEntityModelToModel(MD);

                    var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == md.Mandate_id).ToArray();
                    md.FCV_no = new string[Mdtail.Length];
                    md.Acc_no = new string[Mdtail.Length];
                    md.Mandate_amount = new decimal[Mdtail.Length];
                    for (var i = 0; i < Mdtail.Length; i++)
                    {
                        md.FCV_no[i] = Mdtail[i].FCV_no;
                        md.Acc_no[i] = Mdtail[i].Acc_no;
                        md.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                    }

                    Mandate.Add(md);
                }
            }
            return View(Mandate.ToList());
        }

        [AllowAnonymous]
        // GET: Mandate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Mandate tbl_Mandate = db.tbl_Mandate.SingleOrDefault(ID => ID.Mandate_id == id && ID.status == false);
            if (tbl_Mandate == null)
            {
                return HttpNotFound();
            }
            //if (tbl_Mandate.created_date.Value.Year < DateTime.Now.Year) return RedirectToAction("Index");
            var Mandate = CommonClass.ConvertEntityModelToModel(tbl_Mandate);
            var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == Mandate.Mandate_id).ToArray();
            Mandate.FCV_Identity = new int[Mdtail.Length];
            Mandate.FCV_no = new string[Mdtail.Length];
            Mandate.Acc_no = new string[Mdtail.Length];

            Mandate.Mandate_amount = new decimal[Mdtail.Length];
            for (var i = 0; i < Mdtail.Length; i++)
            {
                Mandate.FCV_no[i] = Mdtail[i].FCV_no;
                Mandate.Acc_no[i] = Mdtail[i].Acc_no;
                Mandate.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                //var fc_no = Mdtail[i].FCV_no;
                //var FCV_id = db.tbl_FCV.FirstOrDefault(ID => ID.FCV_no == fc_no).FCV_id;
                Mandate.FCV_Identity[i] = (int)Mdtail[i].FCV_Identity;
            }
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in db.tbl_Account)
            {
                accounts.Add(ConvertEntityModelToModel(acc));
            }

            //ViewBag.Unit_id = new SelectList(db.tbl_Unit.Where(s => s.status == false), "Unit_id", "Unit_name", tbl_Mandate.Unit_id);
            ViewBag.Unit_id = new SelectList(db.tbl_Responsible_Unit.Where(s => s.active == true), "responsible_unit_id", "responsible_unit_name",tbl_Mandate.Unit_id);
            ViewBag.Acc_no = new SelectList(accounts, "Acc_no", "CodeAndName");
            ViewBag.Accounts = accounts;
            var FCVs = new List<FCVModel>();
            foreach (tbl_FCV f in db.tbl_FCV.Where(s => s.status == false))
            {
                FCVs.Add(CommonClass.ConvertEntityModelToModel(f));
            }
            ViewBag.FCV_no = FCVs;
            return View(Mandate);
        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]

        // GET: Mandate/Create
        public ActionResult Create()
        {
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in db.tbl_Account)
            {
                accounts.Add(ConvertEntityModelToModel(acc));
            }

            //ViewBag.Unit_id = new SelectList(db.tbl_Unit.Where(s => s.status == false), "Unit_id", "Unit_name");
            ViewBag.Unit_id = new SelectList(db.tbl_Responsible_Unit.Where(s => s.active == true), "responsible_unit_id", "responsible_unit_name");
            ViewBag.Acc_no = new SelectList(accounts, "Acc_no", "CodeAndName");
            ViewBag.FCV_no = new SelectList(db.tbl_FCV.Where(f => f.status == false), "FCV_no", "FCV_no");
            ViewBag.Year = new SelectList(Models.YearModel.GetAllYears(), "year", "year");

            int mandate_id = 0;
            if (db.tbl_Mandate.Count() !=0)
            {
                mandate_id = db.tbl_Mandate.OrderByDescending(id => id.Mandate_id).FirstOrDefault().Mandate_id;
            }
            ViewBag.count = mandate_id + 1;

            return View();
        }

        // POST: Mandate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MandateModel Mandate)
        {

            tbl_Mandate tbMainDate = new tbl_Mandate();
            if (string.IsNullOrEmpty(Mandate.Mandate_no))
            {
                tbMainDate.Mandate_no = "0";
            }
            else
            {
                tbMainDate.Mandate_no = Mandate.Mandate_no;
            }
            tbMainDate.Unit_id = Mandate.Unit_id;
            tbMainDate.Mandate_date = Mandate.Mandate_date;
            tbMainDate.Mandate_desc = Mandate.Mandate_desc.ToString();
            tbMainDate.Reciever = Mandate.Reciever;
            tbMainDate.BankAcc_no = Mandate.BankAcc_no;
            tbMainDate.BankAcc_address = Mandate.BankAcc_address;
            tbMainDate.AmountInWord = Mandate.AmountInWord;
            tbMainDate.USD = Mandate.USD;
            tbMainDate.CurrencyType = Mandate.CurrencyType;
            tbMainDate.MEF_date = Mandate.MEF_date;
            tbMainDate.Salary = Mandate.Salary;
            tbMainDate.Electricity = Mandate.Electricity;
            tbMainDate.Water = Mandate.Water;
            tbMainDate.Contribution = Mandate.Contribution;
            tbMainDate.PettyCash = Mandate.PettyCash;
            tbMainDate.Advance = Mandate.Advance;
            tbMainDate.AdvanceNumber = Mandate.AdvanceNumber;
            tbMainDate.Advance_date = Mandate.Advance_date;
            tbMainDate.Allowance = Mandate.Allowance;
            tbMainDate.Other = Mandate.Other;
            tbMainDate.Telecom = Mandate.Telecom;
            tbMainDate.status = false;
            if (string.Compare(DateTime.Now.Year.ToString(), Mandate.MandateYear) == 0)
                tbMainDate.created_date = DateTime.Now;
            else
                tbMainDate.created_date = new DateTime(Convert.ToInt32(Mandate.MandateYear), 1, 1);
            
            db.tbl_Mandate.Add(tbMainDate);
            db.SaveChanges();

            for (var i = 0; i < Mandate.Mandate_amount.Length; i++)
            {
                tbl_MandateDetail MDetail = new tbl_MandateDetail();
                MDetail.Mandate_id = tbMainDate.Mandate_id;
                MDetail.Acc_no = Mandate.Acc_no[i];
                MDetail.Mandate_amount = Mandate.Mandate_amount[i];
                MDetail.FCV_no = Mandate.FCV_no[i];
                MDetail.FCV_Identity = Mandate.FCV_Identity[i];
                db.tbl_MandateDetail.Add(MDetail);
                db.SaveChanges();
            }

            return RedirectToAction("Edit",new {id = tbMainDate.Mandate_id});

        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]
        // GET: Mandate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Mandate tbl_Mandate = db.tbl_Mandate.SingleOrDefault(ID => ID.Mandate_id == id && ID.status == false);
            if (tbl_Mandate == null)
            {
                return HttpNotFound();
            }
            //if (tbl_Mandate.created_date.Value.Year < DateTime.Now.Year) return RedirectToAction("Index");
            var Mandate = CommonClass.ConvertEntityModelToModel(tbl_Mandate);
            var Mdtail = db.tbl_MandateDetail.Where(m => m.Mandate_id == Mandate.Mandate_id).ToArray();
            Mandate.FCV_Identity = new int[Mdtail.Length];
            Mandate.FCV_no = new string[Mdtail.Length];
            Mandate.Acc_no = new string[Mdtail.Length];

            Mandate.Mandate_amount = new decimal[Mdtail.Length];
            for (var i = 0; i < Mdtail.Length; i++)
            {
                Mandate.FCV_no[i] = Mdtail[i].FCV_no;
                Mandate.Acc_no[i] = Mdtail[i].Acc_no;
                Mandate.Mandate_amount[i] = Convert.ToDecimal(Mdtail[i].Mandate_amount);
                //var fc_no = Mdtail[i].FCV_no;
                //var FCV_id = db.tbl_FCV.FirstOrDefault(ID => ID.FCV_no == fc_no).FCV_id;
                Mandate.FCV_Identity[i] = (int) Mdtail[i].FCV_Identity;
            }
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in db.tbl_Account)
            {
                accounts.Add(ConvertEntityModelToModel(acc));
            }

            //ViewBag.Unit_id = new SelectList(db.tbl_Unit.Where(s => s.status == false), "Unit_id", "Unit_name", tbl_Mandate.Unit_id);
            ViewBag.Unit_id = new SelectList(db.tbl_Responsible_Unit.Where(s => s.active == true), "responsible_unit_id", "responsible_unit_name",tbl_Mandate.Unit_id);
            ViewBag.Acc_no = new SelectList(accounts, "Acc_no", "CodeAndName");
            ViewBag.Accounts = accounts;
            var FCVs = new List<FCVModel>();
            foreach (tbl_FCV f in db.tbl_FCV.Where(s => s.status == false))
            {
                FCVs.Add(CommonClass.ConvertEntityModelToModel(f));
            }
            ViewBag.FCV_no = FCVs;
            return View(Mandate);
        }

        // POST: Mandate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MandateModel Mandate)
        {

            var foundMandate = db.tbl_MandateDetail.Where(id => id.Mandate_id == Mandate.Mandate_id);
            if (foundMandate != null)
            {
                db.tbl_MandateDetail.RemoveRange(foundMandate);
                db.SaveChanges();
            }
            tbl_Mandate tbMainDate = db.tbl_Mandate.FirstOrDefault(id => id.Mandate_id == Mandate.Mandate_id);
            tbMainDate.Mandate_no = Mandate.Mandate_no;
            tbMainDate.Unit_id = Mandate.Unit_id;
            tbMainDate.Mandate_date = Mandate.Mandate_date;
            tbMainDate.Mandate_desc = Mandate.Mandate_desc;
            tbMainDate.Reciever = Mandate.Reciever;
            tbMainDate.BankAcc_no = Mandate.BankAcc_no;
            tbMainDate.BankAcc_address = Mandate.BankAcc_address;
            tbMainDate.AmountInWord = Mandate.AmountInWord;
            tbMainDate.USD = Mandate.USD;
            tbMainDate.CurrencyType = Mandate.CurrencyType;
            tbMainDate.status = false;
            tbMainDate.MEF_date = Mandate.MEF_date;
            tbMainDate.Salary = Mandate.Salary;
            tbMainDate.Electricity = Mandate.Electricity;
            tbMainDate.Water = Mandate.Water;
            tbMainDate.Contribution = Mandate.Contribution;
            tbMainDate.PettyCash = Mandate.PettyCash;
            tbMainDate.Advance = Mandate.Advance;
            tbMainDate.AdvanceNumber = Mandate.AdvanceNumber;
            tbMainDate.Advance_date = Mandate.Advance_date;
            tbMainDate.Allowance = Mandate.Allowance;
            tbMainDate.Other = Mandate.Other;
            tbMainDate.Telecom = Mandate.Telecom;
            db.tbl_Mandate.Add(tbMainDate);
            db.Entry(tbMainDate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            if (tbMainDate != null)
                for (var i = 0; i < Mandate.Mandate_amount.Length; i++)
                {
                    tbl_MandateDetail MDetail = new tbl_MandateDetail();
                    MDetail.Mandate_id = Mandate.Mandate_id;
                    MDetail.Acc_no = Mandate.Acc_no[i];
                    MDetail.Mandate_amount = Mandate.Mandate_amount[i];
                    MDetail.FCV_no = Mandate.FCV_no[i];
                    MDetail.FCV_Identity = Mandate.FCV_Identity[i];
                    db.tbl_MandateDetail.Add(MDetail);
                    db.SaveChanges();
                }

            return RedirectToAction("Edit", new { id = tbMainDate.Mandate_id });

            //ViewBag.Unit_id = new SelectList(db.tbl_Unit, "Unit_id", "Unit_name", tbl_Mandate.Unit_id);
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
                return RedirectToAction("ListMandate");
            tbl_Mandate mandate = db.tbl_Mandate.Find(id);
            //var MD = db.tbl_MandateDetail.Where(ID => ID.Mandate_id == id);
            //db.tbl_MandateDetail.RemoveRange(MD);
            //db.tbl_Mandate.Remove(tbl_Mandate);
            mandate.status = true;
            db.Entry(mandate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListMandate");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
        private AccountModel ConvertEntityModelToModel(tbl_Account account)
        {
            AccountModel acc = new AccountModel();
            acc.Acc_no = account.Acc_no;
            acc.Acc_id = account.Acc_id;
            acc.AccChapter_id = account.AccChapter_id;
            acc.Acc_code = account.Acc_code;
            acc.Acc_name = account.Acc_name;
            acc.Acc_desc = account.Acc_desc;
            //tbl_AccountChapter chapter = from ch in context.tbl_AccountChapter where ch.AccChapter_id == account.AccChapter_id select new tbl_AccountChapter;
            tbl_AccountChapter chapter = db.tbl_AccountChapter.SingleOrDefault(a => a.AccChapter_id == account.AccChapter_id);
            acc.AccountChapter = chapter.ToString();
            return acc;
        }
        [AllowAnonymous]
        public ActionResult MandateReport(int id)
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("Acc_no"));
            dt.Columns.Add(new DataColumn("Amount"));
            dt.Columns.Add(new DataColumn("Desc"));
            dt.Columns.Add(new DataColumn("Total"));
            dt.Columns.Add(new DataColumn("USD"));
            dt.Columns.Add(new DataColumn("Bank_no"));
            dt.Columns.Add(new DataColumn("Amount_in_word"));
            dt.Columns.Add(new DataColumn("Receiver"));
            dt.Columns.Add(new DataColumn("Bank_address"));

            var Mandate_detail = db.tbl_MandateDetail.Where(ID => ID.Mandate_id == id);
            if (Mandate_detail.Any())
            {
                foreach (var md in Mandate_detail)
                {
                    DataRow dr = dt.NewRow();
                    var Acc = md.Acc_no.ToArray();
                    if (Acc.Length == 4)
                        dr["Acc_no"] = "     " + Acc[0] + "     " + Acc[1] + "     " + Acc[2] + "     " + Acc[3];
                    else
                        dr["Acc_no"] = Acc[0] + "     " + Acc[1] + "     " + Acc[2] + "     " + Acc[3] + "     " + Acc[4];

                    dr["Amount"] = md.Mandate_amount;

                    var Mandate = db.tbl_Mandate.SingleOrDefault(ID => ID.Mandate_id == id);
                    dr["Desc"] = Mandate.Mandate_desc;
                    dr["Total"] = Mandate.tbl_MandateDetail.Sum(a => a.Mandate_amount);
                    dr["USD"] = Mandate.USD;
                    dr["Bank_no"] = Mandate.BankAcc_no;
                    dr["Amount_in_word"] = Mandate.AmountInWord;
                    dr["Bank_address"] = Mandate.BankAcc_address;
                    dr["Receiver"] = Mandate.Reciever;
                    dt.Rows.Add(dr);
                }
            }
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\MandateReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("Mandate", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }

        [AllowAnonymous]
        public ActionResult MandatePayment(int id)
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            //rv.Width = Unit.Percentage(100);
            //rv.Height = Unit.Percentage(100);
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("Acc_no"));
            dt.Columns.Add(new DataColumn("Mandate_no"));
            dt.Columns.Add(new DataColumn("Unit"));
            dt.Columns.Add(new DataColumn("Amount"));
            dt.Columns.Add(new DataColumn("Desc"));
            dt.Columns.Add(new DataColumn("Total"));
            dt.Columns.Add(new DataColumn("USD"));
            dt.Columns.Add(new DataColumn("Bank_no"));
            dt.Columns.Add(new DataColumn("Amount_in_word"));
            dt.Columns.Add(new DataColumn("Receiver"));
            dt.Columns.Add(new DataColumn("Bank_address"));
            dt.Columns.Add(new DataColumn("Id"));
            dt.Columns.Add(new DataColumn("Geography"));
            dt.Columns.Add(new DataColumn("Foundation"));
            dt.Columns.Add(new DataColumn("SubAccount_Cluster"));
            dt.Columns.Add(new DataColumn("responsible_unit_code"));
            var Mandate_detail = db.tbl_MandateDetail.Where(ID => ID.Mandate_id == id);   
            if (Mandate_detail.Any())
            {          
                foreach (var md in Mandate_detail)
                 {
                    DataRow dr = dt.NewRow();
                        var Acc = md.Acc_no.ToArray();
                        md.tbl_Mandate = db.tbl_Mandate.Find(md.Mandate_id);  
                    
                        dr["Id"] = md.tbl_Mandate.Mandate_id;
                        dr["Acc_no"] = md.Acc_no;

                        dr["Amount"] = md.Mandate_amount;

                        var Mandate = db.tbl_Mandate.SingleOrDefault(ID => ID.Mandate_id == id);
                        //Mandate.tbl_Unit = db.tbl_Unit.Find(Mandate.Unit_id);
                    //Mandate.tbl_Unit = db.tbl_Unit.Find(Mandate.Unit_id);
                    var res_unit = db.tbl_Responsible_Unit.Find(Mandate.Unit_id);
                    dr["Desc"] = Mandate.Mandate_desc;
                        dr["Mandate_no"] = Mandate.Mandate_no;
                        dr["Unit"] = res_unit.responsible_unit_name;
                        dr["Total"] = Mandate.tbl_MandateDetail.Sum(a => a.Mandate_amount);
                        dr["USD"] = string.IsNullOrEmpty(Mandate.USD) ? string.Empty : string.Format("{0} {1}", Mandate.USD, Mandate.CurrencyType);
                        dr["Bank_no"] = Mandate.BankAcc_no;
                        dr["Amount_in_word"] = Mandate.AmountInWord;
                        dr["Bank_address"] = Mandate.BankAcc_address;
                        dr["Receiver"] = Mandate.Reciever;
                    //var Account = (from ac in db.tbl_Account
                    //               join ru in db.tbl_Responsible_Unit on ac.Responsible_Unit_Id equals ru.responsible_unit_id
                    //               where ac.Acc_no == md.Acc_no
                    //               select new { ac, ru }).FirstOrDefault();
                    

                    var acc_responsble = (from acc in db.tbl_Account
                                          join res in db.tbl_Responsible_Unit on acc.Responsible_Unit_Id equals res.responsible_unit_id
                                          where acc.Acc_no == md.Acc_no
                                          select new { acc, res }).FirstOrDefault();
                    if (acc_responsble == null)
                    {
                        var Account = (from u in db.tbl_Unit
                                       join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                       where u_res.responsible_unit_id == Mandate.Unit_id
                                       select new { u, u_res }).FirstOrDefault();
                        if (Account == null)
                        {
                            dr["Geography"] = string.Empty;
                            dr["Foundation"] = string.Empty;
                            dr["SubAccount_Cluster"] = string.Empty;
                            dr["responsible_unit_code"] = "051";
                        }
                        else
                        {
                            dr["Geography"] = Account.u.Geography;
                            dr["Foundation"] = Account.u.Foundation;
                            dr["SubAccount_Cluster"] = Account.u.SubAccount_Cluster;
                            dr["responsible_unit_code"] = Account.u_res.responsible_unit_code;
                        }
                    }
                    else
                    {
                        if (string.Compare(md.Acc_no, "62028") == 0|| string.Compare(md.Acc_no, "65026") == 0)
                        {
                            var Account = (from u in db.tbl_Unit
                                           join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                           where u_res.responsible_unit_id == Mandate.Unit_id
                                           select new { u, u_res }).FirstOrDefault();
                            dr["Geography"] = Account.u.Geography;
                            dr["Foundation"] = Account.u.Foundation;
                            dr["SubAccount_Cluster"] = Account.u.SubAccount_Cluster;
                            dr["responsible_unit_code"] = Account.u_res.responsible_unit_code;
                        }
                        else
                        {
                            dr["Geography"] = acc_responsble.acc.Geography;
                            dr["Foundation"] = acc_responsble.acc.Foundation;
                            dr["SubAccount_Cluster"] = acc_responsble.acc.SubAccount_Cluster;
                            dr["responsible_unit_code"] = acc_responsble.res.responsible_unit_code;
                        }
                        
                    }

                    
          
                        dt.Rows.Add(dr);
                    }
                }            
            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\MandatePayment.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("MandatePayment", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }

        [AllowAnonymous]
        public ActionResult PublishedMandate(int id)
        {
            ReportViewer rv = new ReportViewer();
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SizeToReportContent = true;
            rv.Width = Unit.Percentage(100);
            rv.Height = Unit.Percentage(100);
            

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Chapter"));
            dt.Columns.Add(new DataColumn("Acc_no"));
            dt.Columns.Add(new DataColumn("Sub_Acc_no"));
            dt.Columns.Add(new DataColumn("Mandate_no"));
            dt.Columns.Add(new DataColumn("Receiver"));
            dt.Columns.Add(new DataColumn("Mandate_amount"));
            dt.Columns.Add(new DataColumn("USD"));
            dt.Columns.Add(new DataColumn("Amount_in_word"));
            dt.Columns.Add(new DataColumn("year"));

            var Mandate_detail = db.tbl_MandateDetail.Where(ID => ID.Mandate_id == id);
            if (Mandate_detail.Any())
            {
                foreach (var md in Mandate_detail)
                {
                    //added by TTERD Dec 2020 17
                    var accountChapter = (from acc in db.tbl_Account
                                                        join acc_ch in db.tbl_AccountChapter on acc.AccChapter_id equals acc_ch.AccChapter_id
                                                        where acc.Acc_no == md.Acc_no
                                                        select acc_ch).FirstOrDefault();
                    md.tbl_Mandate = db.tbl_Mandate.Find(md.Mandate_id);

                    DataRow dr = dt.NewRow();
                    //dr["Chapter"] = md.tbl_Account.tbl_AccountChapter.AccChapter_code;
                    dr["Chapter"] = accountChapter == null?string.Empty: accountChapter.AccChapter_code;
                    dr["Acc_no"] = md.Acc_no.Substring(0, 4);
                    dr["Sub_Acc_no"] = md.Acc_no;
                    dr["Mandate_no"] =md.tbl_Mandate==null?string.Empty:md.tbl_Mandate.Mandate_no;
                    dr["Receiver"] = md.tbl_Mandate.Reciever;
                    dr["Mandate_amount"] = md.Mandate_amount;
                    dr["USD"] =string.IsNullOrEmpty(md.tbl_Mandate.USD)?string.Empty:string.Format("{0} {1}",md.tbl_Mandate.USD,md.tbl_Mandate.CurrencyType);
                    dr["Amount_in_word"] = md.tbl_Mandate.AmountInWord;
                    dr["Year"] = CommonClass.GetKhmerNumber(DateTime.Now.Year.ToString());
                    dt.Rows.Add(dr);
                }
            }

            rv.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"ReportDesign\PublishMandateReport.rdlc";
            rv.LocalReport.DataSources.Clear();
            ReportDataSource rds = new ReportDataSource("PublishMandateDataset", dt);
            rv.LocalReport.DataSources.Add(rds);
            ViewBag.ReportViewer = rv;
            return View();
        }

        public JsonResult GetFCVByFCVNo(string no,string year=null)
        {
            tbl_FCV FCV = new tbl_FCV();
            if(string.IsNullOrEmpty(year))
                FCV = db.tbl_FCV.FirstOrDefault(ID => ID.FCV_no == no && ID.status == false  && ID.created_date.Value.Year == DateTime.Now.Year);
            else
                FCV = db.tbl_FCV.FirstOrDefault(ID => ID.FCV_no == no && ID.status == false && string.Compare(ID.created_date.Value.Year.ToString(),year)==0);
            if (FCV == null)
                return null;
            return Json(new
            {
                FCV_no = FCV.FCV_no,
                Acc_no = FCV.Acc_no,
                FCV_Amount = FCV.FCV_amount,
                FCV_id = FCV.FCV_Identity,
                FCV_rate = FCV.rate,
                Desc = FCV.Commitment_desc,
                Unit_id = FCV.unit_id,

                 Salary = (Boolean)FCV.Salary,
            Electricity = (Boolean)FCV.Electricity,
            Water = (Boolean)FCV.Water,
            Advance = (Boolean)FCV.Advance,
            Contribution = (Boolean)FCV.Contribution,
            PettyCash = (Boolean)FCV.PettyCash,
            Allowance = (Boolean)FCV.Allowance,
            Other = (Boolean)FCV.Other,
            Telecom = (Boolean)FCV.Telecom,

        }, JsonRequestBehavior.AllowGet
                       );
        }
        public JsonResult GetFCVByFCVID(string id,string year=null)
        {
            tbl_FCV FCV = new tbl_FCV(); 
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id)) return null;
            var pID = int.Parse(id);
            if(string.IsNullOrEmpty(year))
                FCV = db.tbl_FCV.FirstOrDefault(ID => ID.FCV_Identity == pID && ID.status == false && ID.created_date.Value.Year == DateTime.Now.Year);
            else
                FCV = db.tbl_FCV.FirstOrDefault(ID => ID.FCV_Identity == pID && ID.status == false && string.Compare(ID.created_date.Value.Year.ToString(),year)==0);
            if (FCV == null)
                return null;
            return Json(new
            {
                FCV_no = FCV.FCV_no,
                Acc_no = FCV.Acc_no,
                FCV_Amount = FCV.FCV_amount,
                FCV_id = FCV.FCV_Identity,
                FCV_rate = FCV.rate,
                Desc = FCV.Commitment_desc,
                Unit_id = FCV.unit_id,
                Salary = (Boolean)FCV.Salary,
                Electricity = (Boolean)FCV.Electricity,
                Water = (Boolean)FCV.Water,
                Advance = (Boolean)FCV.Advance,
                Contribution = (Boolean)FCV.Contribution,
                PettyCash = (Boolean)FCV.PettyCash,
                Allowance = (Boolean)FCV.Allowance,
                Other = (Boolean)FCV.Other,
                Telecom = (Boolean)FCV.Telecom,
            }, JsonRequestBehavior.AllowGet
                       );
        }

        public JsonResult SearchMandateByMandateNumber(string mandateNumber)
        {
            if (string.IsNullOrEmpty(mandateNumber)) return null;
            var Mandates = db.tbl_Mandate.Where(md => md.Mandate_no.Contains(mandateNumber) && md.created_date.Value.Year == DateTime.Now.Year);
            if (Mandates.Any())
            {
                return Json(new
                {
                    MandateNO = Mandates
                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetKhmerMoneyLetter(string money)
        {
            if (string.IsNullOrEmpty(money)) return null;
            else
            {
                var result = NumberToText.ConvertToKhmerLetter(decimal.Parse(money));
                return Json(new { MoneyLetter = result },JsonRequestBehavior.AllowGet);
            }
        }
    }
}
