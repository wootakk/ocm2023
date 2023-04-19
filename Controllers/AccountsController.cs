using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Models;
using System.Web.Security;
using ISFMOCM_Project.Function;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    [CustomAuthorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        // GET: Accounts
        ISFMOCM_DBEntities context = new ISFMOCM_DBEntities();

        public ActionResult Index()
        {
                var acclist = new List<AccountModel>();
            try
            {
            var tbl_acc = from tbl_Acc in context.tbl_Account orderby tbl_Acc.Acc_name descending select tbl_Acc;
                if (tbl_acc.Any())
                {
                    foreach (var acc in tbl_acc)
                    {
                        var acc_ch = context.tbl_AccountChapter.Where(ID => ID.AccChapter_id == acc.AccChapter_id).OrderByDescending(ID => ID.AccChapter_id).FirstOrDefault();
                        if(acc_ch != null)
                        {
                            acclist.Add(new AccountModel() { Acc_id = (int)acc.Acc_id, Acc_no=acc.Acc_no,Acc_code=acc.Acc_code,Acc_name=acc.Acc_name,Acc_desc=acc.Acc_desc,AccountChapter=acc_ch.AccChapter_name });
                        }
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return View(acclist);
            //List<AccountModel> accounts = new List<AccountModel>();

            //var tbl_Account = context.tbl_Account.Include(t => t.tbl_AccountChapter);
            //foreach (var ac in tbl_Account)
            //{
            //    AccountModel acc = ConvertEntityModelToModel(ac);
            //    accounts.Add(acc);
            //}
            //return View(accounts);
        }

        public ActionResult Create()
        {
            ViewBag.AccChapter_id = new SelectList(context.tbl_AccountChapter, "AccChapter_id", "AccChapter_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountModel objAccount)
        {
            ViewBag.AccChapter_id = new SelectList(context.tbl_AccountChapter, "AccChapter_id", "AccChapter_name");

            if (!ModelState.IsValid)
            {
                return View("Create");
            }

            tbl_Account tbl_acc = new tbl_Account();
            tbl_acc.Acc_no = objAccount.Acc_no.ToString() ;
            tbl_acc.AccChapter_id =objAccount.AccChapter_id;
            tbl_acc.Acc_code = objAccount.Acc_code;
            tbl_acc.Acc_name = objAccount.Acc_name;
            tbl_acc.Acc_desc = objAccount.Acc_desc;
            context.tbl_Account.Add(tbl_acc);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Account tbl_Account = context.tbl_Account.SingleOrDefault(a => a.Acc_no == id);

            AccountModel ac = new AccountModel();
            ac.AccChapter_id = tbl_Account.AccChapter_id;
            ac.Acc_code = tbl_Account.Acc_code;
            ac.Acc_desc = tbl_Account.Acc_desc;
            ac.Acc_name = tbl_Account.Acc_name;
            ac.Acc_no = tbl_Account.Acc_no;

            if (tbl_Account == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccChapter_id = new SelectList(context.tbl_AccountChapter, "AccChapter_id", "AccChapter_name", tbl_Account.AccChapter_id);
            return View(ac);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Acc_id,Acc_no,AccChapter_id,Acc_code,Acc_name,Acc_desc")] AccountModel account)
        {
            if (ModelState.IsValid)
            {
                tbl_Account acc = new tbl_Account();
                acc.Acc_no = account.Acc_no;
                acc.Acc_id = account.Acc_id;
                acc.AccChapter_id = account.AccChapter_id;
                acc.Acc_code = account.Acc_code;
                acc.Acc_name = account.Acc_name;
                acc.Acc_desc = account.Acc_desc;

                context.tbl_Account.Add(acc);
                context.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                //tbl_Account account = db.tbl_Account.Find(tbl_Account.Acc_no);
                //account = tbl_Account; 
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccChapter_id = new SelectList(context.tbl_AccountChapter, "AccChapter_id", "AccChapter_code", account.AccChapter_id);
            return View(account);
        }

        public ActionResult Details(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Account acc = context.tbl_Account.SingleOrDefault(a => a.Acc_no == id);
            if(acc == null)
            {
                return HttpNotFound();
            }
            var FoundAccount = ConvertEntityModelToModel(acc);
            var AccChapter = context.tbl_AccountChapter.FirstOrDefault(ID => ID.AccChapter_id == FoundAccount.AccChapter_id);
            FoundAccount.AccountChapter = AccChapter.AccChapter_name;
            return View(FoundAccount);
        }

        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tbl_Account tbl_Account = context.tbl_Account.SingleOrDefault(a => a.Acc_no == id);
        //    if (tbl_Account == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var FoundAccount = ConvertEntityModelToModel(tbl_Account);
        //    var AccChapter = context.tbl_AccountChapter.FirstOrDefault(ID => ID.AccChapter_id == FoundAccount.AccChapter_id);
        //    FoundAccount.AccountChapter = AccChapter.AccChapter_name;
        //    return View(FoundAccount);
        //}

        // POST: Accounts/Delete/5
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tbl_Account tbl_Account = context.tbl_Account.SingleOrDefault(a => a.Acc_no == id);
            context.tbl_Account.Remove(tbl_Account);
            context.SaveChanges();
            return RedirectToAction("Index");
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
            tbl_AccountChapter chapter = context.tbl_AccountChapter.SingleOrDefault(a => a.AccChapter_id == account.AccChapter_id);
            acc.AccountChapter = chapter.ToString();
            return acc;
        }

        public JsonResult CheckAccountNumberExists(string Acc_no)
        {
            var acc = context.tbl_Account.Where(id => id.Acc_no == Acc_no);
            if (acc.Any())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }


    }
}