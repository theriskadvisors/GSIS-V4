using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEA_Application.Models
{
    public class AccountantUserEmployeeViewModel
    {
        [Required]
        [Display(Name = "Registration No")]
        public string RegistrationNo { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }

        [Required]
        [Display(Name = "Nationality")]
        public int NationalityId { get; set; }

        [Required]
        [Display(Name = "Religion")]
        public int ReligionId { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public int GenderId { get; set; }

        [Required]
        public string Address { get; set; }

        public string Landline { get; set; }

        [Required]
        [Display(Name = "Cell No")]
        public string CellNo { get; set; }

        [Required]
        [Display(Name = "Gross Salary")]
        public decimal? GrossSalary { get; set; }

        [Required]
        [Display(Name = "Basic Salary")]
        public decimal? BasicSalary { get; set; }

        [Required]
        [Display(Name = "Medical Allowance")]
        public decimal? MedicalAllowance { get; set; }

        [Required]
        [Display(Name = "Provided Fund")]
        public decimal? ProvidedFund { get; set; }

        [Required]
        public decimal? EOP { get; set; }

        [Required]
        public decimal? Tax { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
    }
}