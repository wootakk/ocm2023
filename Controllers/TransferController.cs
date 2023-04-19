using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    [CustomAuthorize(Roles = "Admin")]
    public class TransferController : Controller
    {
        ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
        // GET: Transfer
        public ActionResult TransferList(string page, string transfer_no, string transfer_date)
        {
            DateTime? transfer_d = null;
            if (!string.IsNullOrEmpty(transfer_date))
            {
                transfer_d = DateTime.Parse(transfer_date);
            }
            var Transfer = new List<TransferModel>();
            var transferList = from tbl in db.tbl_Transfer
                               where tbl.status == false
                               orderby tbl.Transfer_id
                               descending
                               select tbl;

            if (!string.IsNullOrEmpty(transfer_no))
            {
                transferList = from tbl in db.tbl_Transfer
                               where tbl.status == false &&
                               tbl.Transfer_no == transfer_no
                               orderby tbl.Transfer_id
                               descending
                               select tbl;
            }
            else if (transfer_date != null)
            {
                transferList = from tbl in db.tbl_Transfer
                               where tbl.status == false &&
                               tbl.Transfer_date == transfer_d
                               orderby tbl.Transfer_id
                               descending
                               select tbl;
            }
            else if ((transfer_date != null) && (!string.IsNullOrEmpty(transfer_no)))
            {
                transferList = from tbl in db.tbl_Transfer
                               where tbl.status == false &&
                               tbl.Transfer_no == transfer_no &&
                               tbl.Transfer_date == Convert.ToDateTime(transfer_date)
                               orderby tbl.Transfer_id descending
                               select tbl;
            }
            else
            {
                transferList = from tbl in db.tbl_Transfer
                               where tbl.status == false
                               orderby tbl.Transfer_id
                               descending
                               select tbl;
            }

            List<ListTransfer> transfermodel = new List<ListTransfer>();
            if (transferList.Any())
            {
                foreach (tbl_Transfer _transfer in transferList)
                {
                    //var TrandDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == _transfer.Transfer_no).ToList();
                    var TrandDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_id == _transfer.Transfer_id).ToList();
                    var Accounts = new List<tbl_Account>();
                    foreach (var acc in TrandDetail)
                    {
                        var account = db.tbl_Account.SingleOrDefault(ID => ID.Acc_no == acc.Acc_no);
                        Accounts.Add(account);
                    }
                    decimal[] increase = new decimal[TrandDetail.Count];
                    decimal[] decrease = new decimal[TrandDetail.Count];
                    decimal[] add = new decimal[TrandDetail.Count];
                    for (int i = 0; i < TrandDetail.Count; i++)
                    {
                        increase[i] = (decimal)TrandDetail[i].Transfer_increase;
                        decrease[i] = (decimal)TrandDetail[i].Transfer_decrease;
                        add[i] = (decimal)TrandDetail[i].Transfer_add;
                    }
                    transfermodel.Add(new ListTransfer() { transfer_id = _transfer.Transfer_id, transfer_no = _transfer.Transfer_no, transfer_date = _transfer.Transfer_date, transfer_desc = _transfer.Transfer_desc, Accounts = Accounts, transfer_increase = increase, transfer_decrease = decrease, transfer_add = add });
                }
            }
            return View(transfermodel);
        }

        // GET: Transfer/Details/5
        public ActionResult DetailsTransfer(int id)
        {
            ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in db.tbl_Account)
            {
                accounts.Add(ISFMOCM_Project.Function.CommonClass.AccountModel(acc));
            }

            var transfer = db.tbl_Transfer.SingleOrDefault(ID => ID.Transfer_id == id);
            if (transfer.created_date.Value.Year < DateTime.Now.Year) return RedirectToAction("TransferList");
            //var transferDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == transfer.Transfer_no).ToList();
            var transferDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_id == transfer.Transfer_id).ToList();
            int[] transferDetail_id = new int[transferDetail.Count];
            string[] acc_no = new string[transferDetail.Count];
            decimal[] increase = new decimal[transferDetail.Count];
            decimal[] decrease = new decimal[transferDetail.Count];
            decimal[] add = new decimal[transferDetail.Count];
            int[] unit_id = new int[transferDetail.Count];
            for (int i = 0; i < transferDetail.Count; i++)
            {
                transferDetail_id[i] = (int)transferDetail[i].TransferDetail_id;
                increase[i] = (decimal)transferDetail[i].Transfer_increase;
                decrease[i] = (decimal)transferDetail[i].Transfer_decrease;
                add[i] = (decimal)transferDetail[i].Transfer_add;
                acc_no[i] = transferDetail[i].Acc_no;
                unit_id[i] = (int)transferDetail[i].Unit_id;
            }

            var ListTransfer = new ListTransfer() { transfer_no = transfer.Transfer_no, transfer_date = transfer.Transfer_date, transfer_desc = transfer.Transfer_desc, Acc_no = acc_no, Transfer = transfer, transfer_detail_id = transferDetail_id, transfer_increase = increase, transfer_decrease = decrease, transfer_add = add, Unit_id = unit_id };

            var Acc_no = new List<AccountWithUnitModel>();
            foreach (var account in db.tbl_Account)
            {
                if (account.tbl_Unit.Count > 0)
                {
                    Acc_no.Add(new AccountWithUnitModel()
                    {
                        unitId = 0,
                        accNo = account.Acc_no,
                        accName = account.Acc_name,
                        unitName = "",
                        unitOrderNumber = 0
                    });

                    foreach (var unit in account.tbl_Unit)
                    {
                        Acc_no.Add(new AccountWithUnitModel()
                        {
                            unitId = unit.Unit_id,
                            accNo = account.Acc_no,
                            accName = account.Acc_name,
                            unitName = unit.Unit_name,
                            unitOrderNumber = (int)unit.Order_Number
                        });
                    }
                }
                else
                {
                    Acc_no.Add(new AccountWithUnitModel()
                    {
                        unitId = 0,
                        accNo = account.Acc_no,
                        accName = account.Acc_name,
                        unitName = "",
                        unitOrderNumber = 0
                    });
                }
            }
            ViewBag.Acc_no = Acc_no;
            return View(ListTransfer);
        }


        // GET: Transfer/Create
        public ActionResult AddTransfer()
        {
            ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
            return View();
        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]
        // POST: Transfer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransfer(ListTransfer collect_transfer)
        {
            ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
            if (ModelState.IsValid)
            {
                var transfer = new tbl_Transfer();
                transfer.Transfer_no = collect_transfer.transfer_no;
                transfer.Transfer_date = collect_transfer.transfer_date;
                transfer.Transfer_desc = collect_transfer.transfer_desc;
                transfer.status = false;
                if (string.Compare(DateTime.Now.Year.ToString(), collect_transfer.Transfer_Year) == 0)
                    transfer.created_date = DateTime.Now;
                else
                    transfer.created_date = new DateTime(Convert.ToInt32(collect_transfer.Transfer_Year), 1, 1);
                db.tbl_Transfer.Add(transfer);
                db.SaveChanges();
                try
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    for (var i = 0; i < collect_transfer.Acc_no.Length; i++)
                    {
                        var accNo = collect_transfer.Acc_no[i].Split(',');
                        var transferDetail = new tbl_TransferDetail
                        {
                            TransferDetail_id=transfer.Transfer_id,
                            Transfer_no = transfer.Transfer_no,
                            Acc_no = accNo[0],
                            Transfer_increase = collect_transfer.transfer_increase[i],
                            Transfer_decrease = collect_transfer.transfer_decrease[i],
                            Transfer_add = collect_transfer.transfer_add[i],
                            Unit_id = Convert.ToInt32(accNo[1]),
                            Transfer_id=transfer.Transfer_id,
                            
                        };

                        db.tbl_TransferDetail.Add(transferDetail);
                    }
                    db.SaveChanges();
                }
                finally
                {
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
                //return View(collect_transfer);
            }
            return RedirectToAction("TransferList");
        }

        [CustomAuthorize(Roles = "Admin,Data Entry")]

        // GET: Transfer/Edit/5
        public ActionResult EditTransfer(int id)
        {
            ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in db.tbl_Account)
            {
                accounts.Add(ISFMOCM_Project.Function.CommonClass.AccountModel(acc));
            }

            var transfer = db.tbl_Transfer.SingleOrDefault(ID => ID.Transfer_id == id);
            //if (transfer.created_date.Value.Year < DateTime.Now.Year) return RedirectToAction("TransferList");
            //var transferDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == transfer.Transfer_no).ToList();
            var transferDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_id == transfer.Transfer_id).ToList();
            int[] transferDetail_id = new int[transferDetail.Count];
            string[] acc_no = new string[transferDetail.Count];
            decimal[] increase = new decimal[transferDetail.Count];
            decimal[] decrease = new decimal[transferDetail.Count];
            decimal[] add = new decimal[transferDetail.Count];
            int[] unit_id = new int[transferDetail.Count];
            for (int i = 0; i < transferDetail.Count; i++)
            {
                transferDetail_id[i] = (int)transferDetail[i].TransferDetail_id;
                increase[i] = (decimal)transferDetail[i].Transfer_increase;
                decrease[i] = (decimal)transferDetail[i].Transfer_decrease;
                add[i] = (decimal)transferDetail[i].Transfer_add;
                acc_no[i] = transferDetail[i].Acc_no;
                unit_id[i] = (int)transferDetail[i].Unit_id;
            }

            var ListTransfer = new ListTransfer() { transfer_no = transfer.Transfer_no, transfer_date = transfer.Transfer_date, transfer_desc = transfer.Transfer_desc, Acc_no = acc_no, Transfer = transfer, transfer_detail_id = transferDetail_id, transfer_increase = increase, transfer_decrease = decrease, transfer_add = add, Unit_id = unit_id };

            var Acc_no = new List<AccountWithUnitModel>();
            foreach (var account in db.tbl_Account)
            {
                if (account.tbl_Unit.Count > 0)
                {
                    Acc_no.Add(new AccountWithUnitModel()
                    {
                        unitId = 0,
                        accNo = account.Acc_no,
                        accName = account.Acc_name,
                        unitName = "",
                        unitOrderNumber = 0
                    });

                    foreach (var unit in account.tbl_Unit)
                    {
                        Acc_no.Add(new AccountWithUnitModel()
                        {
                            unitId = unit.Unit_id,
                            accNo = account.Acc_no,
                            accName = account.Acc_name,
                            unitName = unit.Unit_name,
                            unitOrderNumber = (int)unit.Order_Number
                        });
                    }
                }
                else
                {
                    Acc_no.Add(new AccountWithUnitModel()
                    {
                        unitId = 0,
                        accNo = account.Acc_no,
                        accName = account.Acc_name,
                        unitName = "",
                        unitOrderNumber = 0
                    });
                }
            }
            ViewBag.Acc_no = Acc_no;
            return View(ListTransfer);
        }

        [HttpPost]
        public ActionResult EditTransfer(int id, ListTransfer transfermodel)
        {
            try
            {
                ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
                var transfer = db.tbl_Transfer.FirstOrDefault(ID => ID.Transfer_id == id);
                if (transfer != null)
                {

                    //var transferDetails = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == transfer.Transfer_no).ToList();
                    var transferDetails = db.tbl_TransferDetail.Where(ID => ID.Transfer_id == transfer.Transfer_id).ToList();
                    if (transferDetails != null)
                    {
                        foreach (var item in transferDetails)
                        {
                            int tid = item.TransferDetail_id;
                            tbl_TransferDetail tdetail = db.tbl_TransferDetail.Find(tid);
                            db.tbl_TransferDetail.Remove(tdetail);
                            db.SaveChanges();
                        }
                    }

                    transfer.Transfer_no = transfermodel.transfer_no;
                    transfer.Transfer_date = transfermodel.transfer_date;
                    transfer.Transfer_desc = transfermodel.transfer_desc;
                    transfer.status = false;
                    db.tbl_Transfer.Add(transfer);
                    db.Entry(transfer).State = EntityState.Modified;
                    db.SaveChanges();

                    /*
                    var transferDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == transfer.Transfer_no).ToList();
                    if (transferDetail != null)
                    {
                        db.tbl_TransferDetail.RemoveRange(transferDetail);

                        for (var i = 0; i < transfermodel.transfer_increase.Length; i++)
                        {
                            var accNo = transfermodel.Acc_no[i].Split(',');
                            var tran_detail_update = new tbl_TransferDetail();
                            tran_detail_update.Transfer_no = transfermodel.transfer_no;
                            tran_detail_update.Acc_no = accNo[0];
                            tran_detail_update.Unit_id = Convert.ToInt32(accNo[1]);
                            tran_detail_update.Transfer_increase = transfermodel.transfer_increase[i];
                            tran_detail_update.Transfer_decrease = transfermodel.transfer_decrease[i];
                            tran_detail_update.Transfer_add = transfermodel.transfer_add[i];
                            db.tbl_TransferDetail.Add(tran_detail_update);
                            db.SaveChanges();
                        }
                    }
                    */                

                    for (var i = 0; i < transfermodel.transfer_increase.Length; i++)
                    {
                        var accNo = transfermodel.Acc_no[i].Split(',');
                        var tran_detail_update = new tbl_TransferDetail();
                        tran_detail_update.Transfer_no = transfermodel.transfer_no;
                        tran_detail_update.Acc_no = accNo[0];
                        tran_detail_update.Unit_id = Convert.ToInt32(accNo[1]);
                        tran_detail_update.Transfer_increase = transfermodel.transfer_increase[i];
                        tran_detail_update.Transfer_decrease = transfermodel.transfer_decrease[i];
                        tran_detail_update.Transfer_add = transfermodel.transfer_add[i];
                        tran_detail_update.Transfer_id = transfer.Transfer_id;
                        db.tbl_TransferDetail.Add(tran_detail_update);
                        db.SaveChanges();
                    }

                }
                return RedirectToAction("TransferList");
            }
            catch(Exception ex)
            {
                
                return RedirectToAction("TransferList");
            }
            finally
            {
                db.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        //public ActionResult DeleteItemTransfer(int id, string no)
        //{
        //    ViewData["AccountList"] = Function.CommonClass.GetAccount(null);
        //    List<AccountModel> accounts = new List<AccountModel>();
        //    foreach (var acc in db.tbl_Account)
        //    {
        //        accounts.Add(ISFMOCM_Project.Function.CommonClass.AccountModel(acc));
        //    }
        //    var transfer = db.tbl_Transfer.SingleOrDefault(ID => ID.Transfer_id == id);
        //    var transferDetail = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == transfer.Transfer_no).ToList();
        //    int[] transferDetail_id = new int[transferDetail.Count];
        //    string[] acc_no = new string[transferDetail.Count];
        //    decimal[] increase = new decimal[transferDetail.Count];
        //    decimal[] decrease = new decimal[transferDetail.Count];
        //    decimal[] add = new decimal[transferDetail.Count];
        //    for (int i = 0; i < transferDetail.Count; i++)
        //    {
        //        transferDetail_id[i] = (int)transferDetail[i].TransferDetail_id;
        //        increase[i] = (decimal)transferDetail[i].Transfer_increase;
        //        decrease[i] = (decimal)transferDetail[i].Transfer_decrease;
        //        add[i] = (decimal)transferDetail[i].Transfer_add;
        //        acc_no[i] = transferDetail[i].Acc_no;
        //    }
        //    var ListTransfer = new ListTransfer() { transfer_no = transfer.Transfer_no, transfer_date = transfer.Transfer_date, transfer_desc = transfer.Transfer_desc, Acc_no = acc_no, Transfer = transfer, transfer_detail_id = transferDetail_id, transfer_increase = increase, transfer_decrease = decrease,transfer_add=add };
        //    List<AccountModel> Acc_no = new List<AccountModel>();
        //    foreach (var acc in db.tbl_Account)
        //    {
        //        Acc_no.Add(ISFMOCM_Project.Function.CommonClass.AccountModel(acc));
        //    }
        //    ViewBag.Acc_no = Acc_no;

        //    return View(ListTransfer);
        //}
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult DeleteItemTransfer(int id)
        {
            try
            {
                var transfer = db.tbl_Transfer.SingleOrDefault(ID => ID.Transfer_id == id);
                //var transfer_detail = db.tbl_TransferDetail.Where(ID => ID.Transfer_no == transfer.Transfer_no);
                //db.tbl_TransferDetail.RemoveRange(transfer_detail);
                //db.tbl_Transfer.Remove(transfer);
                transfer.status = true;
                db.Entry(transfer).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("TransferList");
        }

        public JsonResult CheckTransferNumberExists(string transfer_no)
        {
            var trasfer = db.tbl_Transfer.Where(id => id.Transfer_no == transfer_no);
            if (trasfer.Any())
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
