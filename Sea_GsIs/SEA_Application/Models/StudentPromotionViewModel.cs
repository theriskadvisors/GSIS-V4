using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SEA_Application.Models
{
    public class StudentPromotionViewModel
    {
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public int NextClassId { get; set; }

        [Required]
        public int NextSessionId { get; set; }

        [Required]
        public int BranchClassSectionId { get; set; }
    }
}