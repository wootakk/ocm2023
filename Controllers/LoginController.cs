using ISFMOCM_Project.Entity;
using ISFMOCM_Project.Function;
using ISFMOCM_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ISFMOCM_Project.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        // GET: IB
        ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
        public ActionResult UserList(string page)
        {
            var IB = new List<UserInforViewModel>();
            try
            {
                var IBlist = from tbl in db.AspNetUsers orderby tbl.Id descending select tbl;
                if (IBlist.Any())
                {
                    foreach (var _IB in IBlist)
                    {

                        //var acc = db.tbl_Account.Where(ID => ID.Acc_no == _IB.Acc_no).OrderByDescending(ID => ID.Acc_no).FirstOrDefault();
                        //if (acc != null)
                        //{
                            IB.Add(new UserInforViewModel() { First_name = _IB.FirstName, Last_name = _IB.LastName, user_name = _IB.UserName, Email = _IB.Email});
                        //}

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
            return View(IB);
        }
        public ActionResult Register()
        {
            return View();
        }
    }
}