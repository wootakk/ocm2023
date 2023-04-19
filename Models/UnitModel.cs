using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISFMOCM_Project.Models
{
    public class UnitModel
    {
        [Key]
        public int unit_id { get; set; }
        [DisplayName("ឈ្មោះអង្គភាព ឬអាជ្ញាធរ")]

        [Required(ErrorMessage ="សូមបញ្ចូលទិន្នន័យ")]
        
        public string unit_name { get; set; }
        [DisplayName("ពណ៌នា")]
        public string unit_desc { get; set; }
        public string responsible_unit_id { get; set; }
        public string responsible_unit_name { get; set; }
        public string unit_number { get; set; }
    }
    public class YearModel
    {
        public string year { get; set; }
        public List<YearModel> years { get; set; }
        public static List<YearModel> GetAllYears()
        {
            List<YearModel> years = new List<YearModel>();
            for(int i = 2018; i <= 2028; i++)
            {
                years.Add(new YearModel() { year = i.ToString() });
            }
            return years;
        }
    }
}