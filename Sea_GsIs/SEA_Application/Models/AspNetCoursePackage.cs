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
    
    public partial class AspNetCoursePackage
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int PackageId { get; set; }
    
        public virtual AspNetCours AspNetCours { get; set; }
        public virtual AspNetPackage AspNetPackage { get; set; }
    }
}
