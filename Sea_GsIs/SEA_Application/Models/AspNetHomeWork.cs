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
    
    public partial class AspNetHomeWork
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetHomeWork()
        {
            this.AspNetStudentHomeWorks = new HashSet<AspNetStudentHomeWork>();
            this.AspNetSubjectHomeWorks = new HashSet<AspNetSubjectHomeWork>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Principal_Approved_Status { get; set; }
        public string Attachment { get; set; }
    
        public virtual AspNetClass AspNetClass { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetStudentHomeWork> AspNetStudentHomeWorks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetSubjectHomeWork> AspNetSubjectHomeWorks { get; set; }
    }
}
