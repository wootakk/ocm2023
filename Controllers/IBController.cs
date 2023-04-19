using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISFMOCM_Project.Controllers
{
    /// <summary>
    /// IB is Initial Budget
    /// </summary>
    /// 
    [Authorize]
    [CustomAuthorize(Roles = ("Admin,Cabinate,Management"))]
    public class IBController : Controller
    {
        // GET: IB
        ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
        [CustomAuthorize(Roles = ("Admin,Cabinate,Management"))]
        public ActionResult ListInitialBudget()
        {
            var chapter = db.tbl_AccountChapter;
            return View();
        }

        public ActionResult Details(int id)
        {
            var IBmodel = new InitialBudgetModel();
            try
            {
                var tbl_IB = db.tbl_InitialBudget.FirstOrDefault(ID => ID.InitialBudget_id == id);
                if (tbl_IB != null)
                {
                    IBmodel.IB_id = tbl_IB.InitialBudget_id;
                    var acc = db.tbl_Account.Where(ID => ID.Acc_no == tbl_IB.Acc_no).OrderByDescending(ID => ID.Acc_no).FirstOrDefault();
                    IBmodel.acc_name = acc.Acc_name;
                    IBmodel.IB_date = tbl_IB.InitialBudget_date;
                    IBmodel.IB_budget = tbl_IB.Budget;
                    IBmodel.Direct_Paid =(decimal) tbl_IB.Direct_Paid;
                    IBmodel.PettyCash = (decimal) tbl_IB.PettyCash;

                }
            }
            catch (Exception ex)
            {
            }
            return View(IBmodel);
        }

        [CustomAuthorize(Roles = "Admin")]
        // GET: IB/Create
        public ActionResult Create()
        {
            //ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
            ViewBag.acc_no = new SelectList(GetSubAccountWithUnit(), "Value", "Display");
            return View();
        }

        public List<InitialBudgetDropdownModel> GetSubAccountWithUnit()
        {
            var account = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5 || acc.Acc_no == Constants.OtherBurdenAccountNumber);
            var accountList = new List<InitialBudgetDropdownModel>();
            foreach (var acc in account)
            {
                accountList.Add(new InitialBudgetDropdownModel() { Acc_no = acc.Acc_no, Name = acc.Acc_name, Unit_id = 0 });

                acc.tbl_Unit = db.tbl_Unit.Where(s => s.status == false && string.Compare(s.Acc_no, acc.Acc_no) == 0).ToList();

                if (acc.tbl_Unit.Count()>0)
                    foreach (var unit in acc.tbl_Unit)
                    {
                        accountList.Add(new InitialBudgetDropdownModel() { Acc_no = acc.Acc_no, Name = unit.Unit_name, Unit_id = unit.Unit_id , Unit_code = (int) unit.Unit_code , OrderNumber = (int) unit.Order_Number});
                    }
            }
            return accountList.OrderBy(a => a.Acc_no).ToList();
        }
        // POST: IB/Create
        [HttpPost]
        public ActionResult Create(InitialBudgetModel IBmodel)
        {
            //ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
            //try
            //{
            //    // TODO: Add insert logic here

            //    var tbl_IB = new tbl_InitialBudget();

            //    //tbl_IB.InitialBudget_id = IBmodel.IB_id;
            //    tbl_IB.Acc_no = IBmodel.acc_no;
            //    tbl_IB.status = false;
            //    tbl_IB.InitialBudget_date = DateTime.Now;
            //    tbl_IB.Budget = IBmodel.IB_budget;
            //    db.tbl_InitialBudget.Add(tbl_IB);
            //    db.SaveChanges();
            //    //return RedirectToAction("Message", new { Message = Constants.MessageParameter.SaveSuccessful });
            //}
            //catch (Exception ex)
            //{
            //}
            //return RedirectToAction("Index");

            ViewBag.acc_no = new SelectList(GetSubAccountWithUnit(), "Value", "Display");
            try
            {
                // TODO: Add insert logic here

                var tbl_IB = new tbl_InitialBudget();
                string[] arr = IBmodel.acc_no.Split(',');
                tbl_IB.Unit_id = int.Parse(arr[1]);
                tbl_IB.Acc_no = arr[0];
                tbl_IB.status = false;
                tbl_IB.InitialBudget_date = DateTime.Now;
                tbl_IB.Budget = IBmodel.IB_budget;


                db.tbl_InitialBudget.Add(tbl_IB);
                db.SaveChanges();
                //return RedirectToAction("Message", new { Message = Constants.MessageParameter.SaveSuccessful });
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("ListInitialBudget");
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult CreateBudgets()
        {
            ViewBag.Accounts = CommonClass.GetAllAccountModels();
            List<InitialBudgetModel> IB = CommonClass.InitialBudgetModel(db.tbl_InitialBudget.Where(s => s.status == false).ToList());
            List<AccountInitialBudgetModel> list = new List<AccountInitialBudgetModel>();
            if (IB != null)
            {

                foreach (var item in IB)
                {
                    list.Add(new AccountInitialBudgetModel() { Acc_no = item.acc_no, Budget = item.IB_budget, ID = item.IB_id });
                }
            }
            InitialBudgetModel model = new InitialBudgetModel();
            model.AccoutBudgets = list;
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateBudgets(InitialBudgetModel IB)
        {
            if (IB != null)
            {
                for (var i = 0; i < IB.ACCs.Length; i++)
                {
                    tbl_InitialBudget ib = new tbl_InitialBudget();
                    ib.Acc_no = IB.ACCs[i];
                    ib.Budget = IB.BUDGETs[i];
                    ib.InitialBudget_date = DateTime.Now;
                    ib.status = false;
                    db.tbl_InitialBudget.Add(ib);

                }
                db.SaveChanges();
            }
            return RedirectToAction("ListInitialBudget");
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult EditBudgets(string IBYear)
        {
            ViewBag.Accounts = GetSubAccountWithUnit();

            List<AccountInitialBudgetModel> list = new List<AccountInitialBudgetModel>();
            var subAccounts = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5);
            foreach (var subAccount in subAccounts)
            {
                var subAccountIB = CommonReportFunctions.GetSubAccountInitialBudget(subAccount.Acc_no,IBYear);
                if(subAccountIB!=null)
                    list.Add(new AccountInitialBudgetModel()
                    {
                        Unit_id = (int) subAccountIB.Unit_id,
                        Budget = subAccountIB.Budget,
                        Acc_no = subAccountIB.Acc_no,
                        PettyCash = (decimal) subAccountIB.PettyCash,
                        Direct_Paid = (decimal) subAccountIB.Direct_Paid,
                        ID = subAccountIB.InitialBudget_id
                    });

                if (subAccount.tbl_Unit.Any())
                {
                    var units = subAccount.tbl_Unit.Where(u => u.Unit_code == 0).OrderBy(u => u.Order_Number);
                    foreach (var unit in units)
                    {
                        var unitIb = CommonReportFunctions.GetUnitInitialBudget(subAccount.Acc_no, unit.Unit_id,IBYear);
                        if (unitIb != null)
                        {
                            list.Add(new AccountInitialBudgetModel()
                            {
                                Unit_id = (int)unitIb.Unit_id,
                                Budget = unitIb.Budget,
                                Acc_no = unitIb.Acc_no,
                                PettyCash = (decimal)unitIb.PettyCash,
                                Direct_Paid = (decimal)unitIb.Direct_Paid,
                                ID = unitIb.InitialBudget_id
                            });
                        }
                        var subUnits = db.tbl_Unit.Where(u => u.Unit_code == unit.Unit_id);
                        foreach (var subUnit in subUnits)
                        {
                            var subUnitIb = CommonReportFunctions.GetUnitInitialBudget(subAccount.Acc_no,subUnit.Unit_id,IBYear);
                            if (subUnitIb != null)
                            {
                                list.Add(new AccountInitialBudgetModel()
                                {
                                    Unit_id = (int)subUnitIb.Unit_id,
                                    Budget = subUnitIb.Budget,
                                    Acc_no = subUnitIb.Acc_no,
                                    PettyCash = (decimal)subUnitIb.PettyCash,
                                    Direct_Paid = (decimal)subUnitIb.Direct_Paid,
                                    ID = subUnitIb.InitialBudget_id
                                });
                            }
                        }
                    }
                }
            }

            InitialBudgetModel model = new InitialBudgetModel();
            model.AccoutBudgets = list;
            model.IBYear = IBYear;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBudgets(int[] IDs, string[] ACCs, decimal[] BUDGETs,decimal[] PETTYCASHs, decimal[] DIRECT_PAIDs,string IBYear)
        {
            if (IDs != null)
            {
                for (var i = 0; i < IDs.Length; i++)
                {
                    if (IDs[i] == 0)
                    {
                        string[] arr = ACCs[i].Split(',');
                        tbl_InitialBudget IB = new tbl_InitialBudget();
                        IB.Acc_no = arr[0];
                        IB.Budget = BUDGETs[i];
                        if (string.Compare(DateTime.Now.Year.ToString(), IBYear) == 0)
                            IB.InitialBudget_date = DateTime.Now;
                        else
                            IB.InitialBudget_date = new DateTime(Convert.ToInt32(IBYear), 12, 31);
                        IB.Unit_id = int.Parse(arr[1]);
                        IB.status = false;
                        IB.Direct_Paid = DIRECT_PAIDs[i];
                        IB.PettyCash = PETTYCASHs[i];
                        db.tbl_InitialBudget.Add(IB);
                        db.SaveChanges();
                    }
                    else
                    {
                        string[] arr = ACCs[i].Split(',');
                        int ID = IDs[i];
                        tbl_InitialBudget IB = db.tbl_InitialBudget.SingleOrDefault(id => id.InitialBudget_id == ID);
                        IB.Budget = BUDGETs[i];
                        IB.Acc_no = arr[0];
                        IB.Unit_id = int.Parse(arr[1]);
                        IB.Direct_Paid = DIRECT_PAIDs[i];
                        IB.PettyCash = PETTYCASHs[i];
                        db.Entry(IB).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("ListInitialBudget");
        }

        [CustomAuthorize(Roles = "Admin")]
        // GET: IB/Edit/5
        public ActionResult Edit(int id)
        {
            var tblIB = db.tbl_InitialBudget.FirstOrDefault(ID => ID.InitialBudget_id == id && ID.status == false);
            if (tblIB == null) return HttpNotFound();
            //if (tblIB.InitialBudget_date.Value.Year < DateTime.Now.Year) return RedirectToAction("ListInitialBudget");
            var IBmodel = new InitialBudgetModel();
            ViewBag.Acc = GetSubAccountWithUnit();

            try
            {
                if (tblIB != null)
                {
                    IBmodel.IB_id = tblIB.InitialBudget_id;
                    IBmodel.acc_no = tblIB.Acc_no;
                    IBmodel.IB_date = tblIB.InitialBudget_date;
                    IBmodel.IB_budget = tblIB.Budget;
                    IBmodel.Unit_id = (int)(tblIB.Unit_id != null ? tblIB.Unit_id : 0);
                    IBmodel.Direct_Paid = (decimal) tblIB.Direct_Paid;
                    IBmodel.PettyCash = (decimal) tblIB.PettyCash;
                    ViewBag.acc_no = new SelectList(ViewBag.Acc, "Value", "Display", (IBmodel.acc_no + "," + IBmodel.Unit_id));
                    return View(IBmodel);
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("ListInitialBudget");

        }

        // POST: IB/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, InitialBudgetModel IBmodel)
        {
            var tblIB = db.tbl_InitialBudget.FirstOrDefault(ID => ID.InitialBudget_id == id && ID.status == false);

            ViewData["AccountList"] = Function.CommonClass.GetAccount(tblIB.Acc_no);
            try
            {
                if (tblIB != null)
                {
                    string[] arr = IBmodel.acc_no.Split(',');
                    tblIB.Acc_no = arr[0];
                    tblIB.Budget = IBmodel.IB_budget;
                    tblIB.Unit_id = int.Parse(arr[1]);
                    tblIB.PettyCash = (decimal) IBmodel.PettyCash;
                    tblIB.Direct_Paid = (decimal) IBmodel.Direct_Paid;
                    db.Entry(tblIB).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("ListInitialBudget");
        }

        [CustomAuthorize(Roles = "Admin")]
        // GET: IB/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var tblIB = db.tbl_InitialBudget.FirstOrDefault(ID => ID.InitialBudget_id == id);
                if (tblIB != null)
                {
                    tblIB.status = true;
                    db.Entry(tblIB).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("ListInitialBudget");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        public JsonResult RemoveIB(int id)
        {
            if (id == null) return null;
            tbl_InitialBudget IbInDb = db.tbl_InitialBudget.SingleOrDefault(ID => ID.InitialBudget_id == id);
            if (IbInDb == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            IbInDb.status = true;
            db.Entry(IbInDb).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
