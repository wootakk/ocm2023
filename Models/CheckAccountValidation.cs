using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class CheckAccountValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ISFMOCM_DBEntities context = new ISFMOCM_DBEntities();
            var foundAcc = context.tbl_Account.Find(value);
            if (foundAcc == null)
            {
                return ValidationResult.Success;
            }else
            {
                return new ValidationResult("The Account Already Exist");
            }
        }
    }
}