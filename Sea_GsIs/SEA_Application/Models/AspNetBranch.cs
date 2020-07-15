//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SEA_Application.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetBranch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetBranch()
        {
            this.AspNetAnnouncements = new HashSet<AspNetAnnouncement>();
            this.AspNetBranch_Admins = new HashSet<AspNetBranch_Admins>();
            this.AspNetBranch_Class = new HashSet<AspNetBranch_Class>();
            this.AspNetEmployees = new HashSet<AspNetEmployee>();
            this.AspnetGenericBranchClassSubjects = new HashSet<AspnetGenericBranchClassSubject>();
            this.AspNetStudents = new HashSet<AspNetStudent>();
            this.VoucherRecords = new HashSet<VoucherRecord>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string BranchPrincipalId { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetAnnouncement> AspNetAnnouncements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetBranch_Admins> AspNetBranch_Admins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetBranch_Class> AspNetBranch_Class { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetEmployee> AspNetEmployees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspnetGenericBranchClassSubject> AspnetGenericBranchClassSubjects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetStudent> AspNetStudents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VoucherRecord> VoucherRecords { get; set; }
    }
}
