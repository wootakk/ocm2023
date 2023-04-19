using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Function
{
    public class Constants
    {
        public enum MessageParameter
        {
            Successfull,
            Error,
            Waring,

            SaveSuccessful,
            SaveError,
            SaveWarning,

            DeleteSuccessful,
            DeleteError,
            DeleteWarning,

            UpdateSuccessful,
            UpdateError,
            UpdateWarning
        }

        public const string UserSanset = "Sanset";
        public const string OtherBurdenAccountNumber = "6498";
    }
}