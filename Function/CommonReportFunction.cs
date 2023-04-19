using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Function
{
    public class CommonReportFunction
    {


        /// <summary>
        /// This function will return InitialBudget of Specific Account 
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static List<tbl_InitialBudget> GetInititalBudgetList(string accountNo,int? Year) 
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            int year = DateTime.Now.Year;
            if (Year != null) year =(int) Year;

            var IB = (from ib in db.tbl_InitialBudget
                     where ib.status == false && ib.InitialBudget_date.Value.Year == Year && ib.tbl_Account.Acc_no == accountNo
                     select ib).ToList();
            return IB;
        }
        
        /// <summary>
        /// This Function Will return trnasfer from 1st Jan to params Month - 1 of the current year
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static List<tbl_TransferDetail> GetTransferPreviousMonthList(string AccountNumber,int Month)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            //var TransferLM =( from trans in db.tbl_TransferDetail
            //                 where trans.tbl_Transfer.status == false &&
            //                 trans.Acc_no == AccountNumber &&
            //                 trans.tbl_Transfer.Transfer_date.Value.Month < Month &&
            //                 trans.tbl_Transfer.Transfer_date.Value.Year == DateTime.Now.Year
            //                 select trans).ToList();
            var TransferLM = (from trans in db.tbl_TransferDetail
                              join tran in db.tbl_Transfer on trans.Transfer_no equals tran.Transfer_no
                              where tran.status == false &&
                              trans.Acc_no == AccountNumber &&
                              tran.Transfer_date.Value.Month < Month &&
                              tran.Transfer_date.Value.Year == DateTime.Now.Year
                              select trans).ToList();
            return TransferLM;
        }

        /// <summary>
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static List<tbl_TransferDetail> GetTransferList(string AccountNumber, int Month)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            //var Transfer =( from trans in db.tbl_TransferDetail
            //               where trans.tbl_Transfer.status == false &&
            //               trans.Acc_no == AccountNumber &&
            //               trans.tbl_Transfer.Transfer_date.Value.Month == Month &&
            //               trans.tbl_Transfer.Transfer_date.Value.Year == DateTime.Now.Year
            //               select trans).ToList();
            var Transfer = (from trans in db.tbl_TransferDetail
                            join tran in db.tbl_Transfer on trans.Transfer_no equals tran.Transfer_no
                            where tran.status == false &&
                            trans.Acc_no == AccountNumber &&
                            tran.Transfer_date.Value.Month == Month &&
                            tran.Transfer_date.Value.Year == DateTime.Now.Year
                            select trans).ToList();

            return Transfer;
        }


        /// <summary>
        /// This Function Will return FCV from 1st Jan to params Month - 1
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static List<tbl_FCV> GetFCVPreviousMonthList(string AccountNumber,int Month)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            var FCV = (from fcv in db.tbl_FCV
                      where fcv.status == false &&
                      fcv.Acc_no == AccountNumber && fcv.FCV_date.Value.Month < Month && fcv.FCV_date.Value.Year == DateTime.Now.Year
                      select fcv).ToList();
            return FCV;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static List<tbl_FCV> GetFCVList(string AccountNumber,int Month)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            var FCV = (from fcv in db.tbl_FCV
                      where fcv.status == false &&
                      fcv.Acc_no == AccountNumber &&
                      fcv.FCV_date.Value.Month == Month &&
                      fcv.FCV_date.Value.Year == DateTime.Now.Year
                      select fcv).ToList();

            return FCV;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static List<tbl_MandateDetail> GetMandatePreviousMonthList(string AccountNumber,int Month)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            var Mandate = (from mandate in db.tbl_MandateDetail
                          where mandate.tbl_Mandate.status == false &&
                          mandate.tbl_Mandate.Mandate_date.Value.Month < Month &&
                          mandate.tbl_Mandate.Mandate_date.Value.Year == DateTime.Now.Year &&
                          mandate.Acc_no == AccountNumber
                          select mandate).ToList();
            return Mandate;
        }

        /// <summary>
        /// This function will return the all mandate by month in the current year
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static List<tbl_MandateDetail> GetMandateList(string AccountNumber,int Month)
        {
            ISFMOCM_DBEntities db = new ISFMOCM_DBEntities();
            var Mandate = (from mandate in db.tbl_MandateDetail
                           where mandate.tbl_Mandate.status == false &&
                           mandate.tbl_Mandate.Mandate_date.Value.Month == Month &&
                           mandate.tbl_Mandate.Mandate_date.Value.Year == DateTime.Now.Year &&
                           mandate.Acc_no == AccountNumber
                           select mandate).ToList();
            return Mandate;
        }


    }
}