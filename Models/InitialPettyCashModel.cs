using ISFMOCM_Project.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class InitialPettyCashModel
    {
        public int InitialPettyCash_id { get; set; }
        [DisplayName("អង្គភាព ឬ អាជ្ញាធរ")]
        public int Unit_id { get; set; }
        [DisplayName("អង្គភាព ឬ អាជ្ញាធរ")]
        public virtual UnitModel Unit { get; set; }
        public virtual ICollection<InitialPettyCashDetailModel> InitialPettyCashDetail { get; set; }
    }

}