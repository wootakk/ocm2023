using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISFMOCM_Project.Controllers
{
    [Authorize]
    [CustomAuthorize(Roles = "Admin,Data Entry")]
    public class UnitController : Controller
    {
        ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();

        public ActionResult UnitList()
        {
            var unit = new List<UnitModel>();
            try
            {
                var unitlist = from tbl in db.tbl_Unit where tbl.status == false orderby tbl.Unit_id ascending select tbl;
                if (unitlist.Any())
                {
                    foreach (var _unit in unitlist)
                    {
                        string responsible_unit_name = string.Empty;
                        var responsible_unit = db.tbl_Responsible_Unit.Where(s => s.responsible_unit_id == _unit.Responsible_Unit_Id).FirstOrDefault();
                        if (responsible_unit != null)
                            responsible_unit_name = responsible_unit.responsible_unit_name;

                        unit.Add(new UnitModel() {
                            unit_id = _unit.Unit_id,
                            unit_name = _unit.Unit_name,
                            unit_desc = _unit.Unit_desc,
                            responsible_unit_name =responsible_unit_name,
                            unit_number=_unit.unit_number,
                        });

                    }
                    ViewBag.Name = "My Name ";
                    ViewData["Name"] = "My Name";
                    if (TempData.Any())
                    {
                        var tempData = TempData["TempData Name"];
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(unit);
        }
        // GET: Unit
        public ActionResult Index(string page)
        {
            var unit = new List<UnitModel>();
            try
            {
                var unitlist = from tbl in db.tbl_Unit where tbl.status == false orderby tbl.Unit_id ascending select tbl;
                if (unitlist.Any())
                {
                    foreach (var _unit in unitlist)
                    {
                        unit.Add(new UnitModel() { unit_id = _unit.Unit_id, unit_name = _unit.Unit_name, unit_desc = _unit.Unit_desc });
                    }
                    ViewBag.Name = "My Name ";
                    ViewData["Name"] = "My Name";
                    if (TempData.Any())
                    {
                        var tempData = TempData["TempData Name"];
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(unit);
        }

        // GET: Unit/Details/5
        public ActionResult Details(int id)
        {

            var unitmodel = new UnitModel();
            try
            {
                var tbl_unit = db.tbl_Unit.FirstOrDefault(ID => ID.Unit_id == id);
                if (tbl_unit != null)
                {
                    unitmodel.unit_id = tbl_unit.Unit_id;
                    unitmodel.unit_name = tbl_unit.Unit_name;
                    unitmodel.unit_desc = tbl_unit.Unit_desc;

                }
            }
            catch (Exception ex)
            {
            }
            return View(unitmodel);
        }

        // GET: Unit/Create
        public ActionResult Create()
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        // POST: Unit/Create
        [HttpPost]
        public ActionResult Create(UnitModel unitmodel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var tblunit = new tbl_Unit();

            tblunit.Unit_id = unitmodel.unit_id;
            tblunit.Unit_name = unitmodel.unit_name;
            tblunit.Unit_desc = unitmodel.unit_desc;
            tblunit.status = false;
            db.tbl_Unit.Add(tblunit);
            db.SaveChanges();
            //return RedirectToAction("Message", new { Message = Constants.MessageParameter.SaveSuccessful });

            return RedirectToAction("Index");
        }

        // GET: Unit/Edit/5
        public ActionResult Edit(int id)
        {

            var unitdetail = db.tbl_Unit.FirstOrDefault(ID => ID.Unit_id == id);
            var obj_unit = new UnitModel();
            try
            {
                if (unitdetail != null)
                {
                    obj_unit.unit_id = unitdetail.Unit_id;
                    obj_unit.unit_name = unitdetail.Unit_name;
                    obj_unit.unit_desc = unitdetail.Unit_desc;
                }
            }
            catch (Exception ex)
            {
            }
            return View(obj_unit);
        }

        // POST: Unit/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, UnitModel _unitmodel)
        {
            var tblunit = db.tbl_Unit.FirstOrDefault(ID => ID.Unit_id == id);
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                // TODO: Add update logic here
                if (tblunit != null)
                {
                    tblunit.Unit_name = _unitmodel.unit_name;
                    tblunit.Unit_desc = _unitmodel.unit_desc;
                    db.Entry(tblunit).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "Admin")]
        // GET: Unit/Delete/5
        public ActionResult Delete(int id)
        {

            try
            {
                // TODO: Add delete logic here
                var unit = db.tbl_Unit.FirstOrDefault(ID => ID.Unit_id == id);
                if (unit != null)
                {
                    unit.status = true;
                    db.Entry(unit).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("UnitList");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // POST: Unit/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var unit = db.tbl_Unit.SingleOrDefault(ID => ID.Unit_id == id);
                db.tbl_Unit.Remove(unit);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
