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
    
    public partial class Student_Quiz_Scoring
    {
        public int Id { get; set; }
        public Nullable<int> QuizId { get; set; }
        public Nullable<int> QuestionId { get; set; }
        public string Score { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<int> SelectedOptionID { get; set; }
    
        public virtual AspnetQuestion AspnetQuestion { get; set; }
        public virtual AspnetQuiz AspnetQuiz { get; set; }
        public virtual AspNetStudent AspNetStudent { get; set; }
        public virtual AspnetOption AspnetOption { get; set; }
    }
}
