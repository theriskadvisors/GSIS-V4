using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEA_Application.Models
{
    public class StudentRegistrationViewModel
    {

        public string Name { get; set; }
        public string RollNo { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> NationalityId { get; set; }
        public Nullable<int> ReligionId { get; set; }
        public Nullable<int> GenderId { get; set; }
        public Nullable<int> Status { get; set; }

        public string Email { get; set; }

        public string FName { get; set; }

        public string MName { get; set; }
        public string LName { get; set; }
        public string Password { get; set; }

        public string Address { get; set; }
        public string Birthdate { get; set; }
        public string CellNo { get; set; }
        public string File { get; set; }
        public string UserId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public double TutionFee { get; set; }
        public double ComputerFee { get; set; }
        public double LabCharges { get; set; }
        public double OtherServices { get; set; }
        public double AdmissionFee { get; set; }
        public double TotalFee { get; set; }
        public double DiscountTutionFee { get; set; }
        public double DiscountLabCharges { get; set; }
        public double DiscountOtherServices { get; set; }
        public double DiscountAdmissionFee { get; set; }
        public double DiscountComputerFee { get; set; }
        public double DiscountTotal { get; set; }
        public double DiscountTutionFeeAmount { get; set; }
        public double DiscountComputerFeeAmount { get; set; }
        public double DiscountLabChargesAmount { get; set; }
        public double DiscountOtherServicesAmount { get; set; }
        public double DiscountAdmissionFeeAmount { get; set; }
        public double DiscountTotalAmount { get; set; }
        public int SessionID { get; set; }
        public double TotalWithoutAdmission { get; set; }
        public DateTime CreationDate { get; set; }


        public double Jan_Multiplier { get; set; }
        public double Feb_Multiplier { get; set; }
        public double Mar_Multiplier { get; set; }
        public double April__Multiplier { get; set; }
        public double May_Multiplier { get; set; }
        public double June_Multiplier { get; set; }
        public double July__Multiplier { get; set; }
        public double Aug_Multiplier { get; set; }
        public double Sep_Multiplier { get; set; }
        public double Oct_Multiplier { get; set; }
        public double Nov_Multiplier { get; set; }
        public double Dec__Multiplier { get; set; }
        public double StudentID { get; set; }

        public int Jan_StatusPaid { get; set; }
        public int Feb_StatusPaid { get; set; }
        public int Mar_StatusPaid { get; set; }
        public int April_StatusPaid { get; set; }
        public int May_StatusPaid { get; set; }
        public int June_StatusPaid { get; set; }
        public int July_StatusPaid { get; set; }
        public int Aug_StatusPaid { get; set; }
        public int Sep_StatusPaid { get; set; }
        public int Oct_StatusPaid { get; set; }
        public int Nov_StatusPaid { get; set; }
        public int Dec_StatusPaid { get; set; }



    }
}