using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEA_Application.Models
{
    public class EmployeeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Post { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public Nullable<int> Position { get; set; }
        public string DateAvailable { get; set; }
        public string BirthDate { get; set; }
        public int NationalityId { get; set; }
        public int ReligionId { get; set; }
        public int GenderId { get; set; }
        public string CellNo { get; set; }
        public string Landline { get; set; }

        public Nullable<int> BranchId { get; set; }
        public string JoiningDate { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }

        public Nullable<int> VirtualRoleId { get; set; }
        public string Cnic { get; set; }
        public Nullable<bool> IsApplicationUser { get; set; }

        public decimal Salary { get; set;  }

    




    }
}