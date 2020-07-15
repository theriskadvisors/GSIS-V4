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
    
    public partial class AspnetSubjectTopic
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspnetSubjectTopic()
        {
            this.AspnetLessons = new HashSet<AspnetLesson>();
            this.AspnetStudentAssignmentSubmissions = new HashSet<AspnetStudentAssignmentSubmission>();
            this.Quiz_Topic_Questions = new HashSet<Quiz_Topic_Questions>();
            this.AspnetQuestions = new HashSet<AspnetQuestion>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public string FAQ { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> GenericBranchClassSubjectId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
    
        public virtual AspnetGenericBranchClassSubject AspnetGenericBranchClassSubject { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspnetLesson> AspnetLessons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspnetStudentAssignmentSubmission> AspnetStudentAssignmentSubmissions { get; set; }
        public virtual GenericSubject GenericSubject { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quiz_Topic_Questions> Quiz_Topic_Questions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspnetQuestion> AspnetQuestions { get; set; }
    }
}
