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
    
    public partial class AspNetAssessment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetAssessment()
        {
            this.AspNetStudentAssessments = new HashSet<AspNetStudentAssessment>();
        }
    
        public int Id { get; set; }
        public int AssessmentTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
        public int TermId { get; set; }
        public decimal Weightage { get; set; }
        public decimal Total { get; set; }
        public System.DateTime DueDate { get; set; }
        public System.DateTime PostingDate { get; set; }
    
        public virtual AspNetAssessmentType AspNetAssessmentType { get; set; }
        public virtual AspNetTerm AspNetTerm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetStudentAssessment> AspNetStudentAssessments { get; set; }
    }
}
