using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISFMOCM_Project.Function
{
    public class CommonDataModelFunction
    {


        public static SelectList GetUnitSelectList()
        {
            var db = new ISFMOCM_DBEntities();
            List<tbl_Unit> units = new List<tbl_Unit>();
            units.Add(new tbl_Unit() { Unit_id = -1, Unit_name = "សូមជ្រើសរើសអង្គភាព" });
            units.AddRange(db.tbl_Unit);
            return  new SelectList(units, "Unit_id", "Unit_name");
        }


        public static SelectList GetResponsibleUnitSelectList()
        {
            var db = new ISFMOCM_DBEntities();
            List<tbl_Responsible_Unit> units = new List<tbl_Responsible_Unit>();
            units.Add(new tbl_Responsible_Unit() { responsible_unit_id = -1, responsible_unit_name = "សូមជ្រើសរើសអង្គភាព" });
            units.AddRange(db.tbl_Responsible_Unit);
            return new SelectList(units, "responsible_unit_id", "responsible_unit_name");
        }

        public static string GetActiveYear()
        {
            string year = string.Empty;
            using(ISFMOCM_DBEntities db=new ISFMOCM_DBEntities())
            {
                year = db.tbl_year.OrderByDescending(s => s.year).Where(s => s.active == true).Select(s => s.year).FirstOrDefault();
            }
            return year;
        }

    }
}