using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;
using System.Threading;
using ISFMOCM_Project.Models;
using System.Text;

namespace ISFMOCM_Project.Function
{
    public class CommonClass
    {
        /*======================================
         *              Account                 
         *======================================*/
         /// <summary>
         /// Thsi Function will return List of AccountModel
         /// </summary>
         /// <returns></returns>
        public static List<AccountModel> GetAllAccountModels()
        {
            var db = new ISFMOCM_DBEntities();

            var accounts = db.tbl_Account;
            var list = new List<AccountModel>();
            if (accounts.Any())
            {
                foreach (var acc in accounts)
                {
                    list.Add(AccountModel(acc));
                }
            }
            return list;
        }

        /// <summary>
        /// This Function will return the Account as Select list
        /// </summary>
        /// <param name="acc_no"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetAccount(string acc_no)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            List<SelectListItem> AccList = new List<SelectListItem>();
            try
            {
                var tbl_acc = from tbl in db.tbl_Account select tbl;

                //AccList.Add(new SelectListItem { Text = "ជ្រើសរើសគណនី", Value = "" });
                if (tbl_acc.Any())
                {
                    foreach (var _acc in tbl_acc)
                    {

                        if (_acc.Acc_no == acc_no)
                        {
                            AccList.Add(new SelectListItem { Text = _acc.Acc_no + " - " + _acc.Acc_name, Value = _acc.Acc_no, Selected = true });
                        }
                        else
                        {
                            AccList.Add(new SelectListItem { Text = _acc.Acc_no + " - " + _acc.Acc_name, Value = _acc.Acc_no });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return AccList;
        }

        /// <summary>
        /// This function will return the accoutn name
        /// </summary>
        /// <param name="acc_no"></param>
        /// <returns></returns>
        public static string GetAccountName(string acc_no)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            string account = string.Empty;
            try
            {
                var acc_namelist = db.tbl_Account.FirstOrDefault(ID => ID.Acc_no == acc_no);
                if (acc_namelist != null)
                {
                    account = acc_namelist.Acc_name;
                }
            }
            catch (Exception ex)
            {
            }
            return account;
        }
        public static List<AccountModel> GetAllSubAccounts()
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            List<AccountModel> accounts = new List<AccountModel>();
            foreach (var acc in db.tbl_Account)
            {
                if (acc.Acc_no.Length == 5)
                {
                    accounts.Add(CommonClass.AccountModel(acc));
                }
            }
            return accounts;
        }
        internal static AccountModel AccountModel(tbl_Account account)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            AccountModel acc = new AccountModel();
            acc.Acc_no = account.Acc_no;
            acc.Acc_id = account.Acc_id;
            acc.AccChapter_id = account.AccChapter_id;
            acc.Acc_code = account.Acc_code;
            acc.Acc_name = account.Acc_name;
            acc.Acc_desc = account.Acc_desc;
            tbl_AccountChapter chapter = db.tbl_AccountChapter.SingleOrDefault(a => a.AccChapter_id == account.AccChapter_id);
            acc.AccChapter = chapter;
            acc.Units = account.tbl_Unit;
            return acc;
        }

        /*======================================
         *              Initial Budget
         *=====================================*/
         /// <summary>
         /// This function will return the current Year Budget
         /// </summary>
         /// <param name="accountNumber"></param>
         /// <returns></returns>
        public static List<tbl_InitialBudget> GetInitialBudget(string accountNumber,string year=null)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            if(string.IsNullOrEmpty(year))
                return db.tbl_InitialBudget.Where(i => i.Acc_no == accountNumber &&
                                              i.status == false &&  
                                              i.InitialBudget_date.Value.Year == DateTime.Now.Year).ToList();
            else
                return db.tbl_InitialBudget.Where(i => i.Acc_no == accountNumber &&
                                              i.status == false &&
                                              string.Compare(i.InitialBudget_date.Value.Year.ToString(),year)==0
                                              ).ToList();
        }

        public static List<tbl_InitialBudget> GetUnitInitialBudget(string accountNumber,int unitId)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            return db.tbl_InitialBudget.Where(i => i.Acc_no == accountNumber &&
                                                    i.Unit_id == unitId &&
                                                   i.status == false &&
                                                   i.InitialBudget_date.Value.Year == DateTime.Now.Year).ToList();
        }

        public static List<InitialBudgetModel> InitialBudgetModel(List<tbl_InitialBudget> budgets)
        {
            List<InitialBudgetModel> ListBudget = new List<Models.InitialBudgetModel>();
            if (budgets.Count == 0) return null;
            foreach (var b in budgets)
            {
                ListBudget.Add(new Models.InitialBudgetModel() { acc_no = b.Acc_no, IB_budget = b.Budget, IB_id = b.InitialBudget_id , Unit_id = (int) b.Unit_id ,Direct_Paid =(decimal) b.Direct_Paid,PettyCash = (decimal) b.PettyCash});
            }

            return ListBudget;
        }
        /*======================================
        *              Transfer
        *======================================*/

        public static List<tbl_TransferDetail> GetTransferDetail(string accountNumber,string year=null)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            if (db.tbl_TransferDetail.Any())
            {
                if (string.IsNullOrEmpty(year))
                {
                    //return db.tbl_TransferDetail.Where(t => t.Acc_no == accountNumber &&
                    //                           t.tbl_Transfer.status == false &&
                    //                           string.Compare(t.tbl_Transfer.Transfer_date.Value.Year.ToString(), DateTime.Now.Year.ToString()) == 0).ToList();
                    return (from trd in db.tbl_TransferDetail
                                //join tr in db.tbl_Transfer on trd.Transfer_no equals tr.Transfer_no
                            join tr in db.tbl_Transfer on trd.Transfer_id equals tr.Transfer_id
                            where trd.Acc_no == accountNumber && tr.status == false && string.Compare(tr.Transfer_date.Value.Year.ToString(), DateTime.Now.Year.ToString()) == 0
                            select trd).ToList();

                }
                else
                {
                    //return db.tbl_TransferDetail.Where(t => t.Acc_no == accountNumber &&
                    //                           t.tbl_Transfer.status == false &&
                    //                           string.Compare(t.tbl_Transfer.Transfer_date.Value.Year.ToString(), year) == 0).ToList();
                    return (from trd in db.tbl_TransferDetail
                            join tr in db.tbl_Transfer on trd.Transfer_id equals tr.Transfer_id
                            where trd.Acc_no == accountNumber && tr.status == false && string.Compare(tr.Transfer_date.Value.Year.ToString(), year) == 0
                            select trd).ToList();
                }
                
            }
            return new List<tbl_TransferDetail>();
        }
        public static List<tbl_TransferDetail> GetTransferDetail(string accountNumber,int unitId)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();

            var tbUnit = db.tbl_Unit.Where(
                acc =>
                    acc.Acc_no == accountNumber &&
                    acc.tbl_Account.Acc_no == accountNumber &&
                    acc.Unit_id == unitId);

            //var data = db.tbl_TransferDetail.Where(t => t.Acc_no == accountNumber &&
            //                                            t.tbl_Transfer.status == false &&
            //                                            t.tbl_Transfer.Transfer_date.Value.Year ==
            //                                            DateTime.Now.Year).ToList();
            var data= (from trd in db.tbl_TransferDetail
                    //join tr in db.tbl_Transfer on trd.Transfer_no equals tr.Transfer_no
                       join tr in db.tbl_Transfer on trd.Transfer_id equals tr.Transfer_id
                       where trd.Acc_no == accountNumber && tr.status == false && string.Compare(tr.Transfer_date.Value.Year.ToString(), DateTime.Now.Year.ToString()) == 0
                    select trd).ToList();
            data = data.Where(d => d.tbl_Account.tbl_Unit == tbUnit).ToList();
            return data;
        }

        /*======================================
         *              FCV
         * ===================================*/

        ///// <summary>
        ///// This function will return FCV which FCV Number is not equals zero
        ///// </summary>
        ///// <param name="accountNumber"></param>
        ///// <returns></returns>
        //public static IQueryable<tbl_FCV> GetFcvByAccount(string accountNumber)
        //{
        //    ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
        //    return db.tbl_FCV.Where(a => a.Acc_no == accountNumber &&
        //                            a.FCV_no != "0" && a.status == false &&
        //                            a.FCV_date.Value.Year == DateTime.Now.Year);
        //}

        /// <summary>
        /// This function will return FCV as return 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>

        public static List<tbl_FCV> GetFcvByAccount(string accountNumber,string year=null)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            if(string.IsNullOrEmpty(year))
                return db.tbl_FCV.Where(a => a.Acc_no == accountNumber &&
                                    a.status == false &&
                                    a.FCV_date.Value.Year == DateTime.Now.Year).ToList();
            else
                return db.tbl_FCV.Where(a => a.Acc_no == accountNumber &&
                                    a.status == false &&
                                    string.Compare(a.FCV_date.Value.Year.ToString(), year)==0).ToList();
        }
        public static List<tbl_FCV> GetFcvByAccount(string accountNumber,int unitId)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            return db.tbl_FCV.Where(a => a.Acc_no == accountNumber &&
                                         a.status == false &&
                                         a.unit_id == unitId &&
                                         a.FCV_date.Value.Year == DateTime.Now.Year).ToList();
        }
        /// <summary>
        /// This Function Convert from FCV Enity to FCVModel
        /// </summary>
        /// <param name="FCV"></param>
        /// <returns></returns>
        public static FCVModel ConvertEntityModelToModel(tbl_FCV FCV)
        {
            FCVModel FCVM = new FCVModel();
            FCVM.FCV_id = FCV.FCV_id;
            FCVM.FCV_no = FCV.FCV_no;
            FCVM.Acc_no = FCV.Acc_no;
            FCVM.FCV_amount = FCV.FCV_amount;
            FCVM.FCVDate = FCV.FCV_date;
            FCVM.Letter_no = FCV.Letter_no;
            FCVM.Letter_date = FCV.Letter_date;
            FCVM.MEF_date = FCV.MEF_date;
            FCVM.MEF_amount = FCV.MEF_amount;
            FCVM.AmountAfterProcurement = FCV.AmountAfterProcurement;
            FCVM.Commitment_desc = FCV.Commitment_desc;
            FCVM.Dep_of_commitment = FCV.Dep_of_commitment;
            FCVM.Documentation = FCV.Documentation;
            FCVM.AmountInLetter = FCV.AmountInLetter;
            FCVM.Rate = FCV.rate;
            FCVM.Status = (bool)FCV.status;
            FCVM.created_date = (DateTime)FCV.created_date;
            FCVM.Salary = (Boolean)FCV.Salary;
            FCVM.Electricity = (Boolean)FCV.Electricity;
            FCVM.Water = (Boolean)FCV.Water;
            FCVM.Advance = (Boolean)FCV.Advance;
            FCVM.Contribution = (Boolean)FCV.Contribution;
            FCVM.PettyCash = (Boolean)FCV.PettyCash;
            FCVM.Allowance = (Boolean)FCV.Allowance;
            FCVM.Other = (Boolean)FCV.Other;
            FCVM.Telecom = (Boolean) FCV.Telecom;
            FCVM.FCV_Identity = (int) FCV.FCV_Identity;
            return FCVM;
        }

        /*======================================
         *              Mandate
         * ===================================*/

        /// <summary>
        /// This function will Convert from tbl_Mandate to MandateModel
        /// </summary>
        /// <param name="Mandate"></param>
        /// <returns></returns>
        public static MandateModel ConvertEntityModelToModel(tbl_Mandate Mandate)
        {
            MandateModel MD = new MandateModel();
            if (Mandate != null)
            {
                MD.Mandate_id = Mandate.Mandate_id;
                MD.Mandate_no = Mandate.Mandate_no;
                MD.Unit_id = Mandate.Unit_id;
                MD.Mandate_date = Mandate.Mandate_date;
                MD.Mandate_desc = Mandate.Mandate_desc;
                MD.Reciever = Mandate.Reciever;
                MD.BankAcc_no = Mandate.BankAcc_no;
                MD.BankAcc_address = Mandate.BankAcc_address;
                MD.AmountInWord = Mandate.AmountInWord;
                MD.USD = Mandate.USD;
                MD.MEF_date = Mandate.MEF_date;
                MD.Salary =Mandate.Salary==null?false:(bool)Mandate.Salary;
                MD.Electricity =Mandate.Electricity==null?false: (bool)Mandate.Electricity;
                MD.Water =Mandate.Water==null?false: (bool)Mandate.Water;
                MD.Contribution =Mandate.Contribution==null?false:(bool)Mandate.Contribution;
                MD.PettyCash =Mandate.PettyCash==null?false: (bool)Mandate.PettyCash;
                MD.Advance =Mandate.Advance==null?false: (bool)Mandate.Advance;
                MD.AdvanceNumber = Mandate.AdvanceNumber;
                MD.Advance_date = Mandate.Advance_date;
                MD.Allowance =Mandate.Allowance==null?false: (bool)Mandate.Allowance;
                MD.Other =Mandate.Other==null?false: (bool)Mandate.Other;
                MD.Telecom =Mandate.Telecom==null?false: (bool)Mandate.Telecom;
                MD.created_date =Mandate.created_date==null?DateTime.Now: (DateTime)Mandate.created_date;
                MD.CurrencyType = Mandate.CurrencyType;
                if (Mandate.Unit_id != null)
                {
                    //MD.tbl_Unit = Mandate.tbl_Unit;
                    ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
                    MD.tbl_Unit = (from u in db.tbl_Unit
                                   join u_res in db.tbl_Responsible_Unit on u.Responsible_Unit_Id equals u_res.responsible_unit_id
                                   where u_res.responsible_unit_id == Mandate.Unit_id
                                   select u).FirstOrDefault();
                }
            }
            
            return MD;
        }

        /*======================================
         *              Unit
         * ===================================*/

        /// <summary>
        /// This function will return the Unit as select list item
        /// </summary>
        /// <param name="unit_id"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetUnit(int unit_id)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            List<SelectListItem> AccList = new List<SelectListItem>();
            try
            {
                var tbl_unit = from tbl in db.tbl_Unit select tbl;

                AccList.Add(new SelectListItem { Text = "Please Unit", Value = "" });
                if (tbl_unit.Any())
                {
                    foreach (var _acc in tbl_unit)
                    {

                        if (_acc.Unit_id == unit_id)
                        {
                            AccList.Add(new SelectListItem { Text = _acc.Unit_name, Value = _acc.Unit_id.ToString(), Selected = true });
                        }
                        else
                        {
                            AccList.Add(new SelectListItem { Text = _acc.Unit_name, Value = _acc.Unit_id.ToString() });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return AccList;
        }

        /*======================================
         *              Other
         * ===================================*/
        /// <summary>
        /// Get The Budget Summmay of the AccountNumber
        /// </summary>
        /// <returns></returns>
        public static Array GetAccountsBudget()
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            List<AccountBudget> AccountsBudget = new List<AccountBudget>();
            foreach (var acc in db.tbl_Account)
            {
                decimal Budget = 0, Transfer = 0, Ref = 0, AvailableBudget = 0;
                var IB = CommonClass.GetInitialBudget(acc.Acc_no);
                var TransferDetail = CommonClass.GetTransferDetail(acc.Acc_no);
                var FCV = CommonClass.GetFcvByAccount(acc.Acc_no);

                Budget = (decimal)(IB != null && IB.Any() ? IB.Sum(b => b.Budget) : 0);
                Transfer = (decimal)((TransferDetail != null && TransferDetail.Any()
                                                                ? TransferDetail.Sum(t => t.Transfer_add)
                                                                + TransferDetail.Sum(t => t.Transfer_increase) : 0)
                                                                -
                                        (TransferDetail != null && TransferDetail.Any() ? TransferDetail.Sum(t => t.Transfer_decrease) : 0));
                Ref = (decimal)(FCV != null && FCV.Any() ? FCV.Sum(f => f.FCV_amount) : 0);
                AvailableBudget = Budget - Ref;

                AccountsBudget.Add(new AccountBudget() { AccountNO = acc.Acc_no, Initial_Budget = Budget, Total_Budget = Budget + Transfer, Transfer_Budget = Transfer, Available_Budget = AvailableBudget, Reference_Budget = Ref });
            }
            return AccountsBudget.ToArray();
        }


        public static List<InitialBudgetDropdownModel> GetSubAccountWithUnit()
        {
            var db = new ISFMOCM_DBEntities();
            var account = db.tbl_Account.Where(acc => acc.Acc_no.Length == 5);
            var accountList = new List<InitialBudgetDropdownModel>();
            foreach (var acc in account)
            {
                int index = 1;
                accountList.Add(new InitialBudgetDropdownModel() { Acc_no = acc.Acc_no, Name = acc.Acc_name, Unit_id = 0 });
                if (acc.tbl_Unit != null)
                    foreach (var unit in acc.tbl_Unit)
                    {
                        accountList.Add(new InitialBudgetDropdownModel() { Acc_no = acc.Acc_no, Name = index + " - " + unit.Unit_name, Unit_id = unit.Unit_id });
                        index++;
                    }
                index = 0;
            }
            return accountList.OrderBy(a => a.Acc_no).ToList();
        }
        // 

        /// <summary>
        /// The function will return the Khmer Month's name in Khmer
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetMonthName(int number)
        {
            switch (number)
            {
                case 1: return "មករា";
                case 2: return "កុម្ភះ";
                case 3: return "មិនា";
                case 4: return "មេសា";
                case 5: return "ឧសភា";
                case 6: return "មិថុនា";
                case 7: return "កក្កដា";
                case 8: return "សីហា";
                case 9: return "កញ្ញា";
                case 10: return "តុលា";
                case 11: return "វិច្ឆការ";
                case 12: return "ធ្នូ";
                default: return "";
            }
        }

        /// <summary>
        /// This function will return Khmer Number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetKhmerNumber(string number)
        {
            if (string.IsNullOrEmpty(number)) return "";
            string result = "";

            char[] arr = number.ToCharArray();
            foreach (var num in arr)
            {
                switch (num)
                {
                    case '0': result += "០"; break;
                    case '1': result += "១"; break;
                    case '2': result += "២"; break;
                    case '3': result += "៣"; break;
                    case '4': result += "៤"; break;
                    case '5': result += "៥"; break;
                    case '6': result += "៦"; break;
                    case '7': result += "៧"; break;
                    case '8': result += "៨"; break;
                    case '9': result += "៩"; break;

                }
            }
            return result;
        }
             

        public static decimal GetTransferTotalAmountbyAccount(string accountNumber,string year)
        {
            var transfer = CommonClass.GetTransferDetail(accountNumber, year);
            var Transfer = transfer != null && transfer.Any()
                ? transfer.Sum(t => t.Transfer_add) +
                  transfer.Sum(t => t.Transfer_increase) -
                  transfer.Sum(t => t.Transfer_decrease)
                : 0;
            return (decimal)Transfer;
        }

        public static bool isCurrentYear()
        {
            using(ISFMOCM_DBEntities db=new ISFMOCM_DBEntities())
            {
                string active_year = db.tbl_year.OrderByDescending(s => s.year).Where(s => s.active == true).Select(s => s.year).FirstOrDefault().ToString();
                string current_year = DateTime.Now.Year.ToString();
                return string.Compare(active_year, current_year)==0;
            }
        }
        public static string GetCurrentYear()
        {
            using(ISFMOCM_DBEntities db=new ISFMOCM_DBEntities())
            {
                return db.tbl_year.OrderByDescending(s => s.year).Where(s => s.active == true).Select(s => s.year).FirstOrDefault().ToString();
            }
        }
    }
}