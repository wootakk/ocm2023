//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ISFMOCM_Project.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Responsible_Unit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_Responsible_Unit()
        {
            this.tbl_Account = new HashSet<tbl_Account>();
            this.tbl_Unit = new HashSet<tbl_Unit>();
        }
    
        public int responsible_unit_id { get; set; }
        public string responsible_unit_code { get; set; }
        public string responsible_unit_name { get; set; }
        public Nullable<bool> active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Account> tbl_Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_Unit> tbl_Unit { get; set; }
    }
}
