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
    
    public partial class AspnetQuestion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspnetQuestion()
        {
            this.Quiz_Topic_Questions = new HashSet<Quiz_Topic_Questions>();
            this.Student_Quiz_Scoring = new HashSet<Student_Quiz_Scoring>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> AnswerId { get; set; }
        public Nullable<int> LessonId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> Is_Quiz { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> TopicID { get; set; }
        public string Photo { get; set; }
    
        public virtual AspnetLesson AspnetLesson { get; set; }
        public virtual AspnetOption AspnetOption { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quiz_Topic_Questions> Quiz_Topic_Questions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Quiz_Scoring> Student_Quiz_Scoring { get; set; }
        public virtual AspnetSubjectTopic AspnetSubjectTopic { get; set; }
    }
}
