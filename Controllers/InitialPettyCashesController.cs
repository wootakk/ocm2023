using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    public class InitialPettyCashesController : Controller
    {
        ISFMOCM_DBEntities context = new ISFMOCM_DBEntities();
        // GET: InitialPettyCashes
        public ActionResult Index(string page)
        {
            List<InitialPettyCashViewModel> ModelList = new List<InitialPettyCashViewModel>();
            var PettyCash = context.tbl_InitialPettyCash.Where(s => s.status == false).OrderByDescending(s=>s.InitialPettyCash_id);
            foreach (tbl_InitialPettyCash p in PettyCash)
            {
                var Detail = context.tbl_InitialPettyCashDetail.Where(ID => ID.InitialPettyCash_id == p.InitialPettyCash_id).ToList();
                var Accounts = new List<tbl_Account>();
                foreach (var acc in Detail)
                {
                    var account = context.tbl_Account.SingleOrDefault(ID => ID.Acc_no == acc.Acc_no);
                    //var account = context.tbl_Account.SingleOrDefault(ID => ID.Acc_no == acc.Acc_no);
                    Accounts.Add(new tbl_Account() { Acc_no = account.Acc_no,Acc_name = account.Acc_name });
                }
                decimal[] budget = new decimal[Detail.Count];
                for (int i = 0; i < Detail.Count; i++)
                {
                    budget[i] = (decimal)Detail[i].Budget;
                }
                var Unit = context.tbl_Unit.SingleOrDefault(ID => ID.Unit_id == p.Unit_id);
                var PettycashDate = (DateTime) p.tbl_InitialPettyCashDetail.FirstOrDefault().InitialPettyCash_date;
                ModelList.Add(new InitialPettyCashViewModel() { InitialPettyCashID = p.InitialPettyCash_id, Accounts = Accounts, Budget = budget, Unit = Unit , InitialPettyCashDate = PettycashDate });

            }
            var List = from pettyCash in context.tbl_InitialPettyCash
                       where pettyCash.status == false
                       join pettyDetail in context.tbl_InitialPettyCashDetail
                       on pettyCash.InitialPettyCash_id equals pettyDetail.InitialPettyCash_id
                       join accounts in context.tbl_Account
                       on pettyDetail.Acc_no equals accounts.Acc_no
                       join unit in context.tbl_Unit
                       on pettyCash.Unit_id equals unit.Unit_id
                       where unit.status == false
                       select new
                       {
                           InitialPettyCashID = pettyCash.InitialPettyCash_id,
                           Accounts = accounts,
                           Budget = pettyDetail.Budget,
                           Unit = unit,
                       };


            var g = ModelList.GroupBy(s => s.InitialPettyCashID, s => s.Unit);
            return View(ModelList);
        }
        [CustomAuthorize(Roles = "Admin,Data Entry")]
        public ActionResult Create()
        {
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in context.tbl_Account)
            {
                accounts.Add(ConvertEntityModelToModel(acc));
            }

            ViewBag.Acc_no = new SelectList(accounts, "Acc_no", "CodeAndName");

            //ViewData["UnitList"] = Function.CommonClass.GetUnit(0);
            ViewBag.Unit_id = new SelectList(context.tbl_Unit.Where(s => s.status == false), "Unit_id", "Unit_name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(InitialPettyCashDetailModel PettyCash)
        {

            tbl_InitialPettyCash petty = new tbl_InitialPettyCash();

            petty.Unit_id = PettyCash.Unit_id;
            petty.status = false;
            context.tbl_InitialPettyCash.Add(petty);
            context.SaveChanges();

            for (var i = 0; i < PettyCash.Budget.Length; i++)
            {
                tbl_InitialPettyCashDetail PettyDetail = new tbl_InitialPettyCashDetail
                {
                    InitialPettyCash_id = petty.InitialPettyCash_id,
                    Acc_no = PettyCash.Acc_no[i],
                    Budget = PettyCash.Budget[i],
                    InitialPettyCash_date = System.DateTime.Now,
                };

                context.tbl_InitialPettyCashDetail.Add(PettyDetail);
            }
            context.SaveChanges();

            return RedirectToAction("Index");

        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]
        public ActionResult Edit(int id)
        {
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in context.tbl_Account)
            {
                accounts.Add(ConvertEntityModelToModel(acc));
            }

            var InitialPettyCash = context.tbl_InitialPettyCash.SingleOrDefault(ID => ID.InitialPettyCash_id == id && ID.status == false);
            var Detail = context.tbl_InitialPettyCashDetail.Where(ID => ID.InitialPettyCash_id == InitialPettyCash.InitialPettyCash_id).ToList();
            int[] DetailIDs = new int[Detail.Count];
            string[] AccNO = new string[Detail.Count];
            decimal[] budget = new decimal[Detail.Count];
            for (int i = 0; i < Detail.Count; i++)
            {
                DetailIDs[i] = (int)Detail[i].InitialPettyCashDetail_id;
                budget[i] = (decimal)Detail[i].Budget;
                AccNO[i] = Detail[i].Acc_no;
            }
            var Unit_ID = InitialPettyCash.Unit_id;
            var Unit = context.tbl_Unit.SingleOrDefault(ID => ID.Unit_id == Unit_ID && ID.status == false);
            var Model = new InitialPettyCashDetailModel() { Unit_id = Unit_ID, Budget = budget, Acc_no = AccNO, InitialPettyCash = InitialPettyCash, Unit = Unit, InitialPettyCashID = InitialPettyCash.InitialPettyCash_id, InitialPettyCashDetailID = DetailIDs };
            ViewBag.Unit_id = new SelectList(context.tbl_Unit.Where(s => s.status == false), "Unit_id", "Unit_name", Unit_ID);
            List<AccountModel> Acc_no = new List<AccountModel>();
            foreach (var acc in context.tbl_Account)
            {
                Acc_no.Add(ConvertEntityModelToModel(acc));
            }
            ViewBag.Acc_no = Acc_no;
            return View(Model);
        }

        [HttpPost]
        public ActionResult Edit(InitialPettyCashDetailModel PettyCash)
        {

            int pID = PettyCash.InitialPettyCashID;
            tbl_InitialPettyCash FoundPettyCash = context.tbl_InitialPettyCash.SingleOrDefault(ID => ID.InitialPettyCash_id == pID && ID.status == false);
            FoundPettyCash.Unit_id = PettyCash.Unit_id;
            context.Entry(FoundPettyCash).State = EntityState.Modified;
            context.SaveChanges();

            for (var i = 0; i < PettyCash.InitialPettyCashDetailID.Length; i++)
            {
                int id = PettyCash.InitialPettyCashDetailID[i];
                var Detail = context.tbl_InitialPettyCashDetail.SingleOrDefault(ID => ID.InitialPettyCashDetail_id == id);
                if (Detail != null)
                {
                    context.tbl_InitialPettyCashDetail.Remove(Detail);
                    context.SaveChanges();
                }
            }

            for (var i = 0; i < PettyCash.Budget.Length; i++)
            {
                tbl_InitialPettyCashDetail Detail = new tbl_InitialPettyCashDetail();
                Detail.Acc_no = PettyCash.Acc_no[i];
                Detail.Budget = PettyCash.Budget[i];
                Detail.InitialPettyCash_id = FoundPettyCash.InitialPettyCash_id;
                Detail.InitialPettyCash_date = System.DateTime.Now;
                context.tbl_InitialPettyCashDetail.Add(Detail);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in context.tbl_Account)
            {
                accounts.Add(ConvertEntityModelToModel(acc));
            }

            var InitialPettyCash = context.tbl_InitialPettyCash.SingleOrDefault(ID => ID.InitialPettyCash_id == id && ID.status == false);
            var Detail = context.tbl_InitialPettyCashDetail.Where(ID => ID.InitialPettyCash_id == InitialPettyCash.InitialPettyCash_id).ToList();
            int[] DetailIDs = new int[Detail.Count];
            for (int i = 0; i < Detail.Count; i++)
            {
                DetailIDs[i] = (int)Detail[i].InitialPettyCashDetail_id;
            }

            var Unit_ID = InitialPettyCash.Unit_id;
            decimal[] budget = new decimal[Detail.Count];
            for (int i = 0; i < Detail.Count; i++)
            {
                budget[i] = (decimal)Detail[i].Budget;
            }
            string[] AccNO = new string[Detail.Count];
            for (int i = 0; i < Detail.Count; i++)
            {
                AccNO[i] = Detail[i].Acc_no;
            }
            var Unit = context.tbl_Unit.SingleOrDefault(ID => ID.Unit_id == Unit_ID && ID.status == false);
            var Model = new InitialPettyCashViewModel() { Unit_id = Unit_ID, Budget = budget, Acc_no = AccNO, InitialPettyCash = InitialPettyCash, Unit = Unit, InitialPettyCashID = InitialPettyCash.InitialPettyCash_id, InitialPettyCashDetailID = DetailIDs };
            //ViewBag.Acc_no = new SelectList(accounts, "Acc_no", "CodeAndName");
            ViewBag.Unit_id = new SelectList(context.tbl_Unit.Where(S => S.status == false), "Unit_id", "Unit_name", Unit_ID);
            List<AccountModel> Acc_no = new List<AccountModel>();
            foreach (var acc in context.tbl_Account)
            {
                Acc_no.Add(ConvertEntityModelToModel(acc));
            }
            ViewBag.Acc_no = Acc_no;
            return View(Model);
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var Init = context.tbl_InitialPettyCash.SingleOrDefault(ID => ID.InitialPettyCash_id == id);
            Init.status = true;
            context.Entry(Init).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");

        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var Detail = context.tbl_InitialPettyCashDetail.Where(ID => ID.InitialPettyCash_id == id);
        //    context.tbl_InitialPettyCashDetail.RemoveRange(Detail);
        //    tbl_InitialPettyCash PettyCash = context.tbl_InitialPettyCash.SingleOrDefault(ID => ID.InitialPettyCash_id == id);
        //    context.tbl_InitialPettyCash.Remove(PettyCash);
        //    context.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
            acc.AccChapter = chapter;
            return acc;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}