using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ISFMOCM_Project.Entity;
using Microsoft.SqlServer.Server;

namespace ISFMOCM_Project.Models
{
    public class ExpenseModel
    {
        public int ExpenseId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Account { get; set; }
        public string Description { set; get; }
        public decimal Amount { set; get; }
        public virtual ICollection<ExpenseDetail> ExpenseDetail { get; set; }
      
    }

    public class ExpenseDetail
    {
        public int Id { set; get; }
        public int ExpenseId { set; get; }
        public string Account { get; set; }
        public string Description { set; get; }
        public decimal Amount { set; get; }
        public DateTime ExpenseDate { set; get; }
    }

    public class ExpenseViewModel
    {
        public int id { get; set; }
        public string []Accounts { get; set; }
        public string []Description { set; get; }
        public decimal []Amount { set; get; }
        public DateTime ExpenseDate { set; get; }
        public string ExpenseYear { get; set; }
    }


}