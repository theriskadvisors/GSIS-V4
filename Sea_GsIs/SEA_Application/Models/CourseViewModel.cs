using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class CourseViewModel
    {
        [Required]
        [Display(Name = "Course Title")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Is Mandatory")]
        public bool IsMandatory { get; set; }

    }
}