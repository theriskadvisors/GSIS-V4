using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class GrandTotalController : Controller
    {
        private Sea_Entities db = new Sea_Entities();
      
        //
        // GET: /GrandTotal/
        public ActionResult Index()
        {
         ViewBag.ClassID = new SelectList(db.AspNetClasses, "Id", "Name");
         ViewBag.BranchID = new SelectList(db.AspNetBranches, "Id", "Name");

            return View();
        }


        public ActionResult ListAllStudents()
        {
                List<ListTotalStudents_Result> list = new List<ListTotalStudents_Result>();
                list = db.ListTotalStudents().ToList();
                string status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return Content(status);
        }




        public ActionResult StudentListWithBranchClass(int BranchID , int ClassId)
        {
            
             string BranchName= "TRA";
           var result = (from BC in db.AspNetBranch_Class
                        join B in db.AspNetBranches on BC.BranchId equals B.Id
                        where BC.BranchId == BranchID &&  BC.ClassId == ClassId
                        select new { B.Name }).FirstOrDefault();
           if (result != null)
           {
               BranchName = result.Name;

               List<ListTotalStudents_BranchClass_Result> list = new List<ListTotalStudents_BranchClass_Result>();
               list = db.ListTotalStudents_BranchClass(ClassId.ToString(), BranchName).ToList();
               string status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
               return Content(status);
           }
           return Content("0");
        }

        public ActionResult ListAllStudentClass(string Class)
        {
            string status;
            if(Class != "0"){
            List<ListTotalStudentClass_Result> list = new List<ListTotalStudentClass_Result>();
            list = db.ListTotalStudentClass(Class).ToList();
             status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
          }
            else
            {

                List<ListTotalStudents_Result> list = new List<ListTotalStudents_Result>();
                list = db.ListTotalStudents().ToList();
                status = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                return Content(status);
            }
            return Content(status);
        }
        public ActionResult ClassList()
        {
            List<AspNetClass> list = new List<AspNetClass>();
            list = db.AspNetClasses.ToList();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            return Content(status);
        }

        public ActionResult StudentFeeDetails(int StudentID)
        {
            ViewBag.StudentID = StudentID;
           AspNetStudent student =  db.AspNetStudents.Where(x => x.Id == StudentID).FirstOrDefault();
           return View(student);
        }

   
        public ActionResult AllResults(string Month, int StudentID)
        {

            GrandTotal GT = new GrandTotal();
            if (Month != "" && StudentID > 0)
            {
                //Recurrence Fee
                var fee = db.StudentFeeMonths.Where(x => x.StudentId == StudentID && x.Months == Month && x.Status == "Pending").Select(x => x.FeePayable).FirstOrDefault();
                if (fee == null)
                {
                    fee = 0;
                }
                GT.RecurrenceFee = fee.ToString();


                //NonRecurrence Fee
                List<NonRecurringFeeMultiplier> result = db.NonRecurringFeeMultipliers.Where(x => x.StudentId == StudentID && x.Month == Month && x.Status == "Pending").ToList();
                double? totalfee = 0;

                for (int i = 0; i < result.Count(); i++)
                {
                    totalfee += result[i].TutionFee;
                }
                GT.NonRecurrenceFee = totalfee.ToString();

                //Discount 
                var result1 = (from std_dis in db.StudentDiscounts
                               join fee_dis in db.FeeDiscounts on std_dis.FeeDiscountId equals fee_dis.Id
                               where std_dis.StudentId == StudentID && std_dis.Month == Month
                               select new { fee_dis.Amount }).ToList();
                decimal? totalfee1 = 0;
                for (int i = 0; i < result1.Count(); i++)
                {
                    totalfee1 += result1[i].Amount;
                }
                GT.Discount = totalfee1.ToString();


                //plentyfee

                var result3 = (from stdplenty in db.StudentPenalties
                               join plenty in db.PenaltyFees on stdplenty.PenaltyId equals plenty.Id
                               where stdplenty.StudentId == StudentID
                               select new { plenty.Amount }).ToList();
                decimal? totalfee3 = 0;
                for (int i = 0; i < result3.Count(); i++)
                {
                    totalfee3 += result3[i].Amount;
                }

                GT.PlentyFee = totalfee3.ToString();

                string Result = Newtonsoft.Json.JsonConvert.SerializeObject(GT);

                return Content(Result);
            }
            return Content("0");
           
        }
	}
}