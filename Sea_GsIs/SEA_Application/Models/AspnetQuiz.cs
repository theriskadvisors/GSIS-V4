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
    
    public partial class AspnetQuiz
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspnetQuiz()
        {
            this.Quiz_Topic_Questions = new HashSet<Quiz_Topic_Questions>();
            this.Student_Quiz_Scoring = new HashSet<Student_Quiz_Scoring>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Start_Date { get; set; }
        public Nullable<System.DateTime> Due_Date { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> QuizTime { get; set; }
        public string MeetingLink { get; set; }
        public Nullable<bool> IsPublished { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quiz_Topic_Questions> Quiz_Topic_Questions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Quiz_Scoring> Student_Quiz_Scoring { get; set; }
    }
}
