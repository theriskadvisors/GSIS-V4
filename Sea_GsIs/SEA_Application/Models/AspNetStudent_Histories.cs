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
    
    public partial class AspNetStudent_Histories
    {
        public int Id { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<int> SessionId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public Nullable<bool> AdmissionStatusId { get; set; }
        public Nullable<int> CourseId { get; set; }
    
        public virtual AspNetBranchClass_Sections AspNetBranchClass_Sections { get; set; }
        public virtual AspNetCours AspNetCours { get; set; }
        public virtual AspNetSession AspNetSession { get; set; }
        public virtual AspNetStudent AspNetStudent { get; set; }
    }
}
