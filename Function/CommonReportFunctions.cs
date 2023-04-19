using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ISFMOCM_Project.Entity;

namespace ISFMOCM_Project.Function
{
    public class CommonReportFunctions
    {

        public static List<tbl_ExpenseDetail> GetExpensesByDate(string accountNumber,DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_ExpenseDetail.Where(
                ex => 
                    ex.Acc_no == accountNumber &&
                    ex.tbl_Expense.status == false &&
                    ex.tbl_Expense.expense_date >=startDate &&
                    ex.tbl_Expense.expense_date <=endDate
            );
            return data.ToList();
        }

        public static decimal GetSubAccountSelectedBudget(string accountNumber)
        {
            var db = new ISFMOCM_DBEntities();
            decimal selectedBudget=0;
            var units = db.tbl_Unit.Where(u => u.Acc_no == accountNumber).ToList();
            if (units.Count > 0)
            {
                foreach (var unit in units)
                {
                    var unitIb = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unit.Unit_id);
                    if (!(unitIb.Direct_Paid == 0 && unitIb.PettyCash == 0))
                        selectedBudget += (decimal) unitIb.Budget;
                }
            }
            else
            {
                var subAccountInitialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
                if (subAccountInitialBudget.PettyCash == 0 && subAccountInitialBudget.Direct_Paid == 0)
                    selectedBudget = 0;
                else
                    selectedBudget = (decimal) subAccountInitialBudget.Budget;
            }
            return selectedBudget;
        }
        public static decimal GetSubAccountSelectedPettyCash(string accountNumber)
        {
            var db = new ISFMOCM_DBEntities();
            decimal selectedBudget = 0;
            var units = db.tbl_Unit.Where(u => u.Acc_no == accountNumber).ToList();
            if (units.Count > 0)
            {
                foreach (var unit in units)
                {
                    var unitIb = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unit.Unit_id);
                    if (!(unitIb.Direct_Paid == 0 && unitIb.PettyCash == 0))
                        selectedBudget += (decimal)unitIb.PettyCash;
                }
            }
            else
            {
                var subAccountInitialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
                if (subAccountInitialBudget.PettyCash == 0 && subAccountInitialBudget.Direct_Paid == 0)
                    selectedBudget = 0;
                else
                    selectedBudget = (decimal)subAccountInitialBudget.PettyCash;
            }
            return selectedBudget;
        }
        public static decimal GetSubAccountSelectedDirectCash(string accountNumber)
        {
            var db = new ISFMOCM_DBEntities();
            decimal selectedBudget = 0;
            var units = db.tbl_Unit.Where(u => u.Acc_no == accountNumber).ToList();
            if (units.Count > 0)
            {
                foreach (var unit in units)
                {
                    var unitIb = CommonReportFunctions.GetUnitInitialBudget(accountNumber, unit.Unit_id);
                    if (!(unitIb.Direct_Paid == 0 && unitIb.PettyCash == 0))
                        selectedBudget += (decimal)unitIb.Direct_Paid;
                }
            }
            else
            {
                var subAccountInitialBudget = CommonReportFunctions.GetSubAccountInitialBudget(accountNumber);
                if (subAccountInitialBudget.PettyCash == 0 && subAccountInitialBudget.Direct_Paid == 0)
                    selectedBudget = 0;
                else
                    selectedBudget = (decimal)subAccountInitialBudget.Direct_Paid;
            }
            return selectedBudget;
        }
        public static List<tbl_MandateDetail> GetAllPreviouseMandatesDetails(DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            var data = db.tbl_MandateDetail.Where(
                m =>
                    m.tbl_Mandate.status == false &&
                    m.tbl_Mandate.Mandate_date >= startDate &&
                    m.tbl_Mandate.Mandate_date < date
            );
            return data.ToList();
        }

        public static List<tbl_MandateDetail> GetAllMandatesDetails(DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_MandateDetail.Where(
               m =>
                m.tbl_Mandate.status == false &&
                m.tbl_Mandate.Mandate_date >= startDate &&
                m.tbl_Mandate.Mandate_date <= endDate
            );
            return data.ToList();
        }

        public static List<tbl_FCV> GetAllFvcs(DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f => 
                    f.FCV_date >= startDate &&
                    f.FCV_date <= endDate &&
                    f.status == false 
            );
            return data.ToList();
        }
        public static List<tbl_TransferDetail> GetPreviousTransferDetails(string accountNumber,int unitId, DateTime date)
        {
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
                var db = new ISFMOCM_DBEntities();
                //return db.tbl_TransferDetail.Where(
                //    t => 
                //        t.Acc_no == accountNumber &&
                //        t.tbl_Transfer.status == false &&
                //        t.Unit_id == unitId &&
                //        t.tbl_Transfer.Transfer_date >= startDate &&
                //        t.tbl_Transfer.Transfer_date < date 
                //).ToList();

            return (from trd in db.tbl_TransferDetail
                    join tr in db.tbl_Transfer on trd.Transfer_no equals tr.Transfer_no
                    where trd.Acc_no == accountNumber && 
                    tr.status == false &&
                    trd.Unit_id == unitId &&
                    tr.Transfer_date >=startDate && tr.Transfer_date<date
                    select trd).ToList();
        }

        public static List<tbl_TransferDetail> GetTransferDetails(string accountNumber, int unitId, DateTime startDate,DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            //return db.tbl_TransferDetail.Where(
            //    t =>
            //        t.Acc_no == accountNumber &&
            //        t.tbl_Transfer.status == false &&
            //        t.Unit_id == unitId &&
            //        t.tbl_Transfer.Transfer_date >= startDate &&
            //        t.tbl_Transfer.Transfer_date < endDate
            //).ToList();
            return (from trd in db.tbl_TransferDetail
                    join tr in db.tbl_Transfer on trd.Transfer_no equals tr.Transfer_no
                    where trd.Acc_no == accountNumber &&
                    tr.status == false &&
                    tr.Transfer_date >= startDate && tr.Transfer_date < endDate
                    select trd).ToList();
        }

        public static List<tbl_MandateDetail> GetUnitMandateDetails(string accountNumber, int unitId,DateTime startDate, DateTime endDate)
        {
            if (accountNumber == "62028" && unitId == 30) unitId = 2;
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_MandateDetail.Where(
                m =>
                    m.Acc_no == accountNumber &&
                    m.tbl_Mandate.Unit_id == unitId &&
                    m.tbl_Mandate.status == false &&
                    m.tbl_Mandate.Mandate_date >= startDate &&
                    m.tbl_Mandate.Mandate_date <= endDate
            ).Include(nameof(tbl_MandateDetail.tbl_Mandate));
            return data.ToList();
        }

        public static List<tbl_MandateDetail> GetPreviousUnitMandateDetails(string accountNumber,int unitId,DateTime date)
        {
            if (accountNumber == "62028" && unitId == 30) unitId = 2;
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            var data = db.tbl_MandateDetail.Where(
                m => 
                    m.Acc_no == accountNumber &&
                    m.tbl_Mandate.Unit_id == unitId &&
                    m.tbl_Mandate.status == false &&
                    m.tbl_Mandate.Mandate_date >= startDate &&
                    m.tbl_Mandate.Mandate_date < date
            );
            return data.ToList();
        }

        public static List<tbl_FCV> GetUnitPreviousFcvs(string accountNumber, int unitId, DateTime date)
        {
            if (accountNumber == "62028" && unitId == 30) unitId = 2;
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f => f.status == false &&
                f.Acc_no == accountNumber &&
                f.unit_id == unitId &&
                f.FCV_date >= startDate &&
                f.FCV_date < date
            );
            return data.ToList();
        }

        public static List<tbl_FCV> GetUnitFcvs(string accountNumber, int unitId, DateTime startDate,DateTime endDate)
        {
            if (accountNumber == "62028" && unitId == 30) unitId = 2;
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f => f.status == false &&
                     f.Acc_no == accountNumber &&
                     f.unit_id == unitId &&
                     f.FCV_date >= startDate &&
                     f.FCV_date <= endDate
            );
            return data.ToList();
        }



        public static List<tbl_FCV> GetUnitNormalFCV(string accountNumber,int unitId, DateTime startDate, DateTime endDate)
        {
            if (accountNumber == "62028" && unitId == 30)
                unitId = 2;
            var db = new ISFMOCM_DBEntities();
            if (db.tbl_Unit.Any(u => u.Unit_code == unitId))
            {
                var fcvs = from f in db.tbl_FCV
                    join unit in db.tbl_Unit on f.unit_id equals unit.Unit_id
                    where f.status == false &&
                          f.FCV_date >= startDate &&
                          f.FCV_date <= endDate &&
                          f.PettyCash == false &&
                          f.Acc_no == accountNumber &&
                          unit.Unit_code == unitId
                    select f;
                return fcvs.ToList();
            }
            else
            {
                var data = db.tbl_FCV.Where(
                    f =>
                        f.status == false &&
                        f.FCV_date >= startDate &&
                        f.FCV_date <= endDate &&
                        f.PettyCash == false &&
                        f.unit_id == unitId &&
                        f.Acc_no == accountNumber

                );
                return data.ToList();
            }
        }

        public static List<tbl_MandateDetail> GetUnitNormalMandate(string accountNumber,int unitId, DateTime startDate, DateTime endDate)
        {
            if (accountNumber == "62028" && unitId == 30)
                unitId = 2;
            var db = new ISFMOCM_DBEntities();
            if (db.tbl_Unit.Any(u => u.Unit_code == unitId))
            {

                //var data = db.tbl_MandateDetail.Where(
                //    m =>
                //        m.tbl_Mandate.Mandate_date >= startDate &&
                //        m.tbl_Mandate.Mandate_date <= endDate &&
                //        m.tbl_Mandate.status == false &&
                //        m.tbl_Mandate.PettyCash == false &&
                //        m.tbl_Mandate.tbl_Unit.Unit_code == unitId &&
                //        m.Acc_no == accountNumber
                //);
                var data = (from m in db.tbl_MandateDetail
                            join mm in db.tbl_Mandate on m.Mandate_id equals mm.Mandate_id
                            join res in db.tbl_Responsible_Unit on mm.Unit_id equals res.responsible_unit_id
                            join u in db.tbl_Unit on res.responsible_unit_id equals u.Responsible_Unit_Id
                            where mm.Mandate_date >= startDate &&
                            mm.Mandate_date <= endDate &&
                            mm.status == false &&
                            mm.PettyCash == false &&
                            m.Acc_no == accountNumber &&
                            u.Unit_code == unitId
                            select m);
                return data.ToList();
            }
            else
            {
                var data = db.tbl_MandateDetail.Where(
                    m =>
                        m.tbl_Mandate.Mandate_date >= startDate &&
                        m.tbl_Mandate.Mandate_date <= endDate &&
                        m.tbl_Mandate.status == false &&
                        m.tbl_Mandate.PettyCash == false &&
                        m.tbl_Mandate.Unit_id == unitId &&
                        m.Acc_no == accountNumber
                );
                return data.ToList();
            }
        }

        public static List<tbl_MandateDetail> GetUnitPettyMandate(string accountNumber,int unitId,DateTime startDate,DateTime endDate)
        {
            if (accountNumber == "62028" && unitId == 30) unitId = 2;

            var db = new ISFMOCM_DBEntities();

            var hasSubUnit = db.tbl_Unit.Any(u => u.Unit_code == unitId);
            if (hasSubUnit)
            {
                var unit = (from u in db.tbl_Unit
                            join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                            where u_res.responsible_unit_id == unitId
                            select new { u, u_res }).FirstOrDefault();

                //var data = db.tbl_MandateDetail.Where(
                //    m =>
                //        m.tbl_Mandate.Mandate_date >= startDate &&
                //        m.tbl_Mandate.Mandate_date <= endDate &&
                //        m.tbl_Mandate.status == false &&
                //        m.tbl_Mandate.PettyCash == true &&
                //        m.tbl_Mandate.tbl_Unit.Unit_code == unitId &&
                //        m.Acc_no == accountNumber
                //);
                var data = (from m in db.tbl_MandateDetail
                            join mm in db.tbl_Mandate on m.Mandate_id equals mm.Mandate_id
                            join res in db.tbl_Responsible_Unit on mm.Unit_id equals res.responsible_unit_id
                            join u in db.tbl_Unit on res.responsible_unit_id equals u.Responsible_Unit_Id
                            where mm.Mandate_date >= startDate &&
                            mm.Mandate_date <= endDate &&
                            mm.status == false &&
                            mm.PettyCash == true &&
                            m.Acc_no == accountNumber &&
                            u.Unit_code == unitId
                            select m);

                return data.ToList();
            }
            else
            {
                var data = db.tbl_MandateDetail.Where(
                    m =>
                        m.tbl_Mandate.Mandate_date >= startDate &&
                        m.tbl_Mandate.Mandate_date <= endDate &&
                        m.tbl_Mandate.status == false &&
                        m.tbl_Mandate.PettyCash == true &&
                        m.tbl_Mandate.Unit_id == unitId &&
                        m.Acc_no == accountNumber
                );
                return data.ToList();
            }

            
        }

        

        public static tbl_InitialBudget GetUnitInitialBudget(string accountNumber, int unitId,string IBYear=null)
        {
            var db = new ISFMOCM_DBEntities();

            // if the account is 62028 and unit is នាយកដ្ឋានផ្គត់ផ្គង់ និងហិរញ្ញកិច្ច we need to take the budget from 
            //​ចំណាយពិសេសរបស់រាជរដ្ឋាភិបាល
            if (accountNumber == "62028" && unitId == 2) unitId = 30;

            //Check if the Account has matched unit
            var result = db.tbl_Unit.Where(acc => acc.Unit_id == unitId && acc.Acc_no == accountNumber);
            if (result !=null && result.Any())
            {
                tbl_InitialBudget resutl = new tbl_InitialBudget();
                if (string.IsNullOrEmpty(IBYear))
                {
                    if(CommonClass.isCurrentYear())
                        resutl = db.tbl_InitialBudget.FirstOrDefault(
                            b =>
                                b.InitialBudget_date.Value.Year == DateTime.Now.Year &&
                                b.Acc_no == accountNumber &&
                                b.Unit_id == unitId &&
                                b.status == false
                        );
                    else
                    {

                        DateTime date = Convert.ToDateTime("1-1" + CommonClass.GetCurrentYear());
                        resutl = db.tbl_InitialBudget.FirstOrDefault(
                            b =>
                                b.InitialBudget_date.Value.Year == date.Year &&
                                b.Acc_no == accountNumber &&
                                b.Unit_id == unitId &&
                                b.status == false
                        );
                    }
                }
                else
                    resutl = db.tbl_InitialBudget.FirstOrDefault(
                        b =>
                            string.Compare(b.InitialBudget_date.Value.Year.ToString(),IBYear)==0 &&
                            b.Acc_no == accountNumber &&
                            b.Unit_id == unitId &&
                            b.status == false
                    );
                return resutl;
            }
            else
            {
                if(string.IsNullOrEmpty(IBYear))
                return db.tbl_InitialBudget.FirstOrDefault(
                    acc => 
                        acc.Acc_no == accountNumber &&
                        acc.status == false &&
                        acc.InitialBudget_date.Value.Year == DateTime.Now.Year);
                else
                    return db.tbl_InitialBudget.FirstOrDefault(
                    acc =>
                        acc.Acc_no == accountNumber &&
                        acc.status == false &&
                        string.Compare(acc.InitialBudget_date.Value.Year.ToString(),IBYear)==0);
            }
        }


        public static decimal GetTotalAccountInitialBudget(string accountNumber,string IBYear=null)
        {
            var db = new ISFMOCM_DBEntities();
            var subAccount = db.tbl_Account.Where(acc => acc.Acc_code == accountNumber);
            decimal ib = 0;
            foreach (var sub in subAccount)
            {
                if (GetSubAccountInitialBudget(sub.Acc_no,IBYear) != null)
                {
                    ib += (decimal)GetSubAccountInitialBudget(sub.Acc_no,IBYear).Budget;
                }
            }
            return ib;
        }

        public static decimal GetTotalAccountPettyCash(string accountNumber)
        {
            var db = new ISFMOCM_DBEntities();
            var subAccount = db.tbl_Account.Where(acc => acc.Acc_code == accountNumber);
            decimal ib = 0;
            foreach (var sub in subAccount)
            {
                ib += (decimal)GetSubAccountInitialBudget(sub.Acc_no).PettyCash;
            }
            return ib;
        }

        public static decimal GetTotalAccountDirectPaid(string accountNumber)
        {
            var db = new ISFMOCM_DBEntities();
            var subAccount = db.tbl_Account.Where(acc => acc.Acc_code == accountNumber);
            decimal ib = 0;
            foreach (var sub in subAccount)
            {
                var initialBudget = GetSubAccountInitialBudget(sub.Acc_no);
                if (initialBudget.PettyCash == 0 && initialBudget.Direct_Paid == 0)
                    ib += (decimal) initialBudget.Budget;
                else
                    ib += (decimal) initialBudget.Direct_Paid;

            }
            return ib;
        }

        public static tbl_InitialBudget GetSubAccountInitialBudget(string accountNumber,string IBYear=null)
        {
            var db = new ISFMOCM_DBEntities();
            if(string.IsNullOrEmpty(IBYear))
                return db.tbl_InitialBudget.FirstOrDefault(i => 
                        i.Acc_no == accountNumber &&
                        i.InitialBudget_date.Value.Year == DateTime.Now.Year &&
                        i.status == false
                );
            else
                return db.tbl_InitialBudget.FirstOrDefault(i =>
                        i.Acc_no == accountNumber &&
                        string.Compare(i.InitialBudget_date.Value.Year.ToString(),IBYear)==0 &&
                        i.status == false
                );
        }
        public static List<tbl_MandateDetail> GetMandateDetailByDateRange(string accountNumber,DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_MandateDetail.Where(
                f =>
                    f.tbl_Mandate.status == false &&
                    f.tbl_Mandate.Mandate_date >= startDate &&
                    f.tbl_Mandate.Mandate_date <= endDate &&
                    f.Acc_no == accountNumber
            ).Include(nameof(tbl_MandateDetail.tbl_Mandate)).ToList();

            return data;
        }

        public static List<tbl_MandateDetail> GetPreviousMandateDetailByAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_MandateDetail.Where(
                f =>
                    f.tbl_Mandate.status == false &&
                    f.tbl_Mandate.Mandate_date>= startDate &&
                    f.tbl_Mandate.Mandate_date < date &&
                    f.Acc_no == accountNumber
            ).ToList();

            return data;
        }

        public static List<tbl_MandateDetail> GetPreviousNormalMandateDetailBySubAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if(!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_MandateDetail.Where(
                f =>
                    f.tbl_Mandate.status == false &&
                    f.tbl_Mandate.Mandate_date >= startDate &&
                    f.tbl_Mandate.Mandate_date < date &&
                    f.Acc_no == accountNumber &&
                    f.tbl_Mandate.PettyCash == false
            ).ToList();

            return data;
        }

        public static List<tbl_MandateDetail> GetPreviousNormalMandateDetailByAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_MandateDetail.Where(
                f =>
                    f.tbl_Mandate.status == false &&
                    f.tbl_Mandate.Mandate_date >= startDate &&
                    f.tbl_Mandate.Mandate_date < date &&
                    f.tbl_Account.Acc_code == accountNumber &&
                    f.tbl_Mandate.PettyCash == false
            ).ToList();

            return data;
        }

        public static List<tbl_MandateDetail> GetPreviousPettyMandateDetailBySubAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if(!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_MandateDetail.Where(
                f =>
                    f.tbl_Mandate.status == false &&
                    f.tbl_Mandate.Mandate_date >= startDate &&
                    f.tbl_Mandate.Mandate_date < date &&
                    f.Acc_no == accountNumber &&
                    f.tbl_Mandate.PettyCash == true
            ).ToList();

            return data;
        }
        public static List<tbl_MandateDetail> GetPreviousPettyMandateDetailByAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_MandateDetail.Where(
                f =>
                    f.tbl_Mandate.status == false &&
                    f.tbl_Mandate.Mandate_date >= startDate &&
                    f.tbl_Mandate.Mandate_date < date &&
                    f.tbl_Account.Acc_code == accountNumber &&
                    f.tbl_Mandate.PettyCash == true
            ).ToList();

            return data;
        }
        public static List<tbl_FCV> GetPreviousFcvsByAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_FCV.Where(
                    f => 
                    f.status == false &&
                    f.FCV_date >= startDate &&
                    f.FCV_date < date &&
                    f.Acc_no == accountNumber
                ).ToList();

            return data;
        }

        public static List<tbl_TransferDetail> GetPreviousTransferDetailsByChapter(int chapterId, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            //var data = db.tbl_TransferDetail.Where(
            //    t =>
            //        t.tbl_Account.AccChapter_id == chapterId &&
            //        t.tbl_Transfer.Transfer_date >= startDate &&
            //        t.tbl_Transfer.Transfer_date < date &&
            //        t.tbl_Transfer.status == false
            //).ToList();
            var data = ( from td in db.tbl_TransferDetail
                        join t in db.tbl_Transfer on td.Transfer_no equals t.Transfer_no
                        //join acc in db.tbl_Account on td.a
                        where td.tbl_Account.AccChapter_id == chapterId &&
                    t.Transfer_date >= startDate &&
                    t.Transfer_date < date &&
                    t.status == false
                    select td
            ).ToList();
            return data;
        }

        public static List<tbl_MandateDetail> GetMandateDetialsByChapter(int chapterId, DateTime startDate,DateTime endDate,bool isDateNull=false)
        {
            var db  = new ISFMOCM_DBEntities();
            //var data = db.tbl_MandateDetail.Where(
            //    m => 
            //    m.tbl_Mandate.status == false &&
            //    m.tbl_Mandate.Mandate_date >= startDate &&
            //    m.tbl_Mandate.Mandate_date <= endDate &&
            //    m.tbl_Account.AccChapter_id == chapterId
            //    ).ToList();
            List<tbl_MandateDetail> data = new List<tbl_MandateDetail>();
            if (isDateNull)
            {
                data = (from md in db.tbl_MandateDetail
                        join m in db.tbl_Mandate on md.Mandate_id equals m.Mandate_id
                        join acc in db.tbl_Account on md.Acc_no equals acc.Acc_no
                        where m.status == false && m.Mandate_date == null && m.created_date >= startDate &&
                         m.created_date <= endDate &&
                            acc.AccChapter_id == chapterId
                        select md).ToList();
            }
            else
                data = (from md in db.tbl_MandateDetail
                        join m in db.tbl_Mandate on md.Mandate_id equals m.Mandate_id
                        join acc in db.tbl_Account on md.Acc_no equals acc.Acc_no
                        where m.status == false && m.Mandate_date >= startDate &&
                         m.Mandate_date <= endDate &&
                            acc.AccChapter_id == chapterId
                        select md).ToList();
            return data;
        }

        public static List<tbl_MandateDetail> GetPreviousMandateDetialsByChapter(int chapterId , DateTime date,bool isDateNull=false)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            //var data = db.tbl_MandateDetail.Where(
            //    m =>
            //        m.tbl_Mandate.status == false &&
            //        m.tbl_Mandate.Mandate_date >= startDate &&
            //        m.tbl_Mandate.Mandate_date <= date &&
            //        m.tbl_Account.AccChapter_id == chapterId
            //).ToList();
            List<tbl_MandateDetail> data = new List<tbl_MandateDetail>();
            if (isDateNull)
            {
                data = (from md in db.tbl_MandateDetail
                        join m in db.tbl_Mandate on md.Mandate_id equals m.Mandate_id
                        join acc in db.tbl_Account on md.Acc_no equals acc.Acc_no
                        where m.status == false && m.Mandate_date==null && m.created_date >= startDate &&
                         m.created_date <= date &&
                            acc.AccChapter_id == chapterId
                        select md).ToList();
            }
            else
                 data = (from md in db.tbl_MandateDetail
                        join m in db.tbl_Mandate on md.Mandate_id equals m.Mandate_id
                        join acc in db.tbl_Account on md.Acc_no equals acc.Acc_no
                        where m.status == false && m.Mandate_date >= startDate &&
                         m.Mandate_date <= date &&
                            acc.AccChapter_id == chapterId
                        select md).ToList();
            return data;
        }

        public static List<tbl_FCV> GetFCVByChapter(int chapterId, DateTime startDate,DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f =>
                    f.tbl_Account.AccChapter_id == chapterId &&
                    f.status == false &&
                    f.FCV_date >= startDate &&
                    f.FCV_date <= endDate
            ).ToList();
            return data;
        }

        public static List<tbl_FCV> GetPreviouseFCVByChapter(int chapterId, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            if (!CommonClass.isCurrentYear())
                startDate = Convert.ToDateTime("1-1-" + CommonClass.GetCurrentYear());
            var data = db.tbl_FCV.Where(
                f => 
                    f.tbl_Account.AccChapter_id == chapterId &&
                    f.status == false &&
                    f.FCV_date >= startDate &&
                    f.FCV_date <= date
                ).ToList();
            return data;
        }
        //public static List<tbl_TransferDetail> GetProviouseTransferDetails(string accNo, DateTime date)
        //{
        //    var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
        //    var db = new ISFMOCM_DBEntities();
        //    var data = db.tbl_TransferDetail.Where(
        //        t =>
        //            t.tbl_Transfer.status == false &&
        //            t.tbl_Transfer.Transfer_date >= startDate &&
        //            t.tbl_Transfer.Transfer_date < date &&
        //            t.Acc_no == accNo
        //    ).ToList();
        //    return data;
        //}

        public static List<tbl_TransferDetail> GetTransferDetailsByDateRange(string accNo, DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            //var data = db.tbl_TransferDetail.Where(
            //    t =>
            //        t.tbl_Transfer.status == false &&
            //        t.tbl_Transfer.Transfer_date >= startDate &&
            //        t.tbl_Transfer.Transfer_date <= endDate &&
            //        t.Acc_no == accNo
            //        ).ToList();
            var data = (from td in db.tbl_TransferDetail
                        join t in db.tbl_Transfer on td.Transfer_no equals t.Transfer_no
                        where t.status == false &&
                        t.Transfer_date >= startDate &&
                        t.Transfer_date <= endDate &&
                        td.Acc_no == accNo
                        select td).ToList();
            return data;
        }

        public static List<tbl_TransferDetail> GetTransferDetailsByDateRange(int chapterId,DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            //var data = db.tbl_TransferDetail.Where(
            //    t => 
            //        t.tbl_Transfer.status == false &&
            //        t.tbl_Transfer.Transfer_date >= startDate &&
            //        t.tbl_Transfer.Transfer_date <= endDate &&
            //        t.tbl_Account.AccChapter_id == chapterId
            //).ToList();
            var data = (from td in db.tbl_TransferDetail
                        join t in db.tbl_Transfer on td.Transfer_no equals t.Transfer_no
                        where t.status == false &&
                        t.Transfer_date >= startDate &&
                        t.Transfer_date <= endDate &&
                        td.tbl_Account.AccChapter_id == chapterId
                        select td).ToList();
            return data;
        }

        public static List<tbl_InitialBudget> GetInitaBudgetsByChpaterId(int chapterId,string IBYear=null)
        {
            var db = new ISFMOCM_DBEntities();
            List<tbl_InitialBudget> data = new List<tbl_InitialBudget>();
            //var data = db.tbl_InitialBudget.Where(
            //        ib => 
            //            ib.status == false &&
            //            ib.Unit_id == 0 &&
            //            ib.InitialBudget_date.Value.Year == DateTime.Now.Year &&
            //            ib.tbl_Account.AccChapter_id == chapterId                       
            //    ).ToList();
            if(string.IsNullOrEmpty(IBYear))
                data = db.tbl_InitialBudget.Where(
                    ib =>
                        ib.status == false &&
                        ib.Unit_id == 0 &&
                        ib.InitialBudget_date.Value.Year == DateTime.Now.Year &&
                        ib.tbl_Account.AccChapter_id == chapterId
                ).ToList();
            else
            {
                //data = db.tbl_InitialBudget.Where(
                //    ib =>
                //        ib.status == false &&
                //        ib.Unit_id == 0 &&
                //        string.Compare(ib.InitialBudget_date.Value.Year.ToString(), IBYear) == 0 &&
                //        ib.tbl_Account.AccChapter_id == chapterId
                //).ToList();
                data = (from ib in db.tbl_InitialBudget
                        join acc in db.tbl_Account on ib.Acc_no equals acc.Acc_no
                        where ib.status == false && ib.Unit_id == 0 && string.Compare(ib.InitialBudget_date.Value.Year.ToString(), IBYear) == 0 && acc.AccChapter_id == chapterId
                        select ib).ToList();

            }
                //data = db.tbl_InitialBudget.Where(
                //    ib =>
                //        ib.status == false &&
                //        ib.Unit_id == 0 &&
                //        string.Compare(ib.InitialBudget_date.Value.Year.ToString(),IBYear)==0 &&
                //        ib.tbl_Account.AccChapter_id == chapterId
                //).ToList();
            return data;
        }
        public static List<tbl_FCV> GetPreviouseFCVByDateRage(DateTime date)
        {
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f =>
                    f.status == false &&
                    f.FCV_date >= startDate &&
                    f.FCV_date < date 
            ).ToList();
            return data;
        }

        public static List<tbl_FCV> GetFcvByDateRange(string accountNumber,DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f =>
                    f.status == false &&
                    f.FCV_date >= startDate &&
                    f.FCV_date <= endDate &&
                    f.Acc_no == accountNumber
            ).ToList();
            return data;
        }

        /// <summary>
        /// Thsi funciton will return all the fcvs by the rang from Startdate to EndDate
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<tbl_FCV> GetFcvByDateRange(DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = db.tbl_FCV.Where(
                f =>
                    f.status == false &&
                    f.FCV_date >= startDate &&
                    f.FCV_date <= endDate
            ).ToList();
            return data;
        }

        /// <summary>
        /// Thsi function will return mandate which budget type is PettyCash
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<tbl_MandateDetail> GetPettyMandatesBySubAccount(string accountNumber,DateTime startDate,DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = (from mandate in db.tbl_MandateDetail
                where mandate.Acc_no == accountNumber &&
                      mandate.tbl_Mandate.status == false &&
                      mandate.tbl_Mandate.PettyCash == true &&
                      mandate.tbl_Mandate.Mandate_date.Value >= startDate &&
                      mandate.tbl_Mandate.Mandate_date.Value <= endDate
                select mandate).ToList();

            return data;
        }

        /// <summary>
        /// This function will return list of mandate whcih Pettycash equal true and Account Length = 4
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<tbl_MandateDetail> GetPettyMandatesByAccount(string accountNumber, DateTime startDate, DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = (from mandate in db.tbl_MandateDetail
                where mandate.tbl_Account.Acc_code == accountNumber &&
                      mandate.tbl_Mandate.status == false &&
                      mandate.tbl_Mandate.PettyCash == true &&
                      mandate.tbl_Mandate.Mandate_date.Value >= startDate &&
                      mandate.tbl_Mandate.Mandate_date.Value <= endDate
                select mandate).ToList();

            return data;
        }
        /// <summary>
        /// This function will return Initial Budget which select by date range
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static tbl_InitialBudget GetCurrentYearInitialBudget(string accountNumber)
        {
            var db = new ISFMOCM_DBEntities();
            var data = new tbl_InitialBudget();
            string active_year = db.tbl_year.OrderByDescending(s => s.year).Where(s => s.active == true).Select(s => s.year).FirstOrDefault().ToString();
            string current_year = DateTime.Now.Year.ToString();
            if (string.Compare(active_year, current_year) == 0)
            {
                data = db.tbl_InitialBudget.FirstOrDefault(
                    ib =>
                    ib.InitialBudget_date.Value.Year == DateTime.Now.Year &&
                    ib.Acc_no == accountNumber &&
                    ib.status == false
                );
            }
            else
            {
                var startDate = Convert.ToDateTime("1-1-" + active_year);
                data = db.tbl_InitialBudget.FirstOrDefault(
                    ib =>
                    ib.InitialBudget_date.Value.Year == startDate.Year &&
                    ib.Acc_no == accountNumber &&
                    ib.status == false
                );
            }
             
            return data;
        }
        public static List<tbl_FCV> GetPreviousNormalFcvsBySubAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            var data = new List<tbl_FCV>();
            data = db.tbl_FCV.Where(
                f =>
                    f.status == false &&
                    f.Acc_no == accountNumber &&
                    f.FCV_date.Value >= startDate &&
                    f.FCV_date.Value < date &&
                    f.PettyCash == false
            ).ToList();
            return data;
        }
        public static List<tbl_FCV> GetPreviousNormalFcvsByAccount(string accountNumber, DateTime date)
        {
            var db = new ISFMOCM_DBEntities();
            var startDate = Convert.ToDateTime("1-1-" + DateTime.Now.Year);
            var data = new List<tbl_FCV>();
            data = db.tbl_FCV.Where(
                f =>
                    f.status == false &&
                    f.tbl_Account.Acc_code == accountNumber &&
                    f.FCV_date.Value >= startDate &&
                    f.FCV_date.Value < date &&
                    f.PettyCash == false
            ).ToList();
            return data;
        }

        public static List<tbl_FCV> GetNormaFcvsBySubAccount(string accountNumber, DateTime startDate,
            DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = new List<tbl_FCV>();
            data = db.tbl_FCV.Where(
                f =>
                f.status == false &&
                f.Acc_no == accountNumber &&
                f.FCV_date.Value >= startDate &&
                f.FCV_date.Value <= endDate &&
                f.PettyCash == false
            ).ToList();
            return data;
        }
       
        public static List<tbl_FCV> GetNormalFcvsByAccount(string accountNumber, DateTime startDate,
            DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = new List<tbl_FCV>();
            data = db.tbl_FCV.Where(
                f =>
                    f.status == false &&
                    f.tbl_Account.Acc_code == accountNumber &&
                    f.FCV_date.Value >= startDate &&
                    f.FCV_date.Value <= endDate &&
                    f.PettyCash == false
            ).ToList();
            return data;
        }
        public static List<tbl_MandateDetail> GetNormalMandateDetailsBySubAccount(string accountNumber, DateTime startDate,
            DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = new List<tbl_MandateDetail>();
            data = db.tbl_MandateDetail.Where(
                m =>
                m.Acc_no == accountNumber &&
                m.tbl_Mandate.status == false &&
                m.tbl_Mandate.Mandate_date  >= startDate &&
                m.tbl_Mandate.Mandate_date  <= endDate &&
                m.tbl_Mandate.PettyCash == false 
            ).ToList();
            return data;
        }
        public static List<tbl_MandateDetail> GetNormalMandateDetailsByAccount(string accountNumber, DateTime startDate,
            DateTime endDate)
        {
            var db = new ISFMOCM_DBEntities();
            var data = new List<tbl_MandateDetail>();
            data = db.tbl_MandateDetail.Where(
                m =>
                    m.tbl_Account.Acc_code == accountNumber &&
                    m.tbl_Mandate.status == false &&
                    m.tbl_Mandate.Mandate_date >= startDate &&
                    m.tbl_Mandate.Mandate_date <= endDate &&
                    m.tbl_Mandate.PettyCash == false
            ).ToList();
            return data;
        }


        public static string GetTitleDate(DateTime startDate,DateTime endDate)
        {
            var st = "ចាប់ពីថ្ងៃទី " + CommonClass.GetKhmerNumber(startDate.Day.ToString())
                + " ខែ" + CommonClass.GetMonthName(startDate.Month)
                + " ឆ្នាំ" + CommonClass.GetKhmerNumber(startDate.Year.ToString())
                + " ដល់ "
                + " ថ្ងៃទី" + CommonClass.GetKhmerNumber(endDate.Day.ToString())
                + " ខែ" + CommonClass.GetMonthName(endDate.Month)
                + " ឆ្នាំ" + CommonClass.GetKhmerNumber(endDate.Year.ToString());

            return st;
        }
    }
}