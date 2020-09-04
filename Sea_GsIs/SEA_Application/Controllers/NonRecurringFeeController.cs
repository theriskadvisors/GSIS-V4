using Microsoft.AspNet.Identity;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class NonRecurringFeeController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NonRecurringFeeList()
        {

            //var AllNonRecurringFee = from nonRecurringFee in db.StudentNonRecurringFees
            //                         join student in db.AspNetStudents on nonRecurringFee.StudentFeeID equals student.Id
            //                         join feeType in db.NonRecurringTypes on nonRecurringFee.NonRecTypeID equals feeType.Id
            //                         select new
            //                         {
            //                             student.Name,
            //                             nonRecurringFee.Id,
            //                             feeType.ExpenseType,
            //                             nonRecurringFee.Amount,
            //                             nonRecurringFee.Month
            //                         };
            //return Json(AllNonRecurringFee, JsonRequestBehavior.AllowGet);

            var StudentNonRecurringFees = db.StudentNonRecurringFeesList().ToList();

            return Json(StudentNonRecurringFees, JsonRequestBehavior.AllowGet);



        }

        //public class NonRecurringFee
        //{
        //    public int Id { get; set; }

        //    public string StudentName { get; set; }

        //    public string MonthName { get; set; }

        //    public List<NoRecurringFeeType> TypeList = new List<NoRecurringFeeType>();
        //}
        //public class NoRecurringFeeType
        //{
        //    public int TypeName { get; set; }
        //    public int TypeAmount { get; set; }
        //}
        [HttpGet]
        public ActionResult Create()
        {

            ViewBag.FeeType = db.NonRecurringTypes.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult SaveClassNonRecurringFee(_NonRecurringFee NonRecurringFee, List<FeeTypeAmount> FeeTypeAmount)
        {
            var ErrorMsg = "";
            try
            {
                //int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == NonRecurringFee.BranchId && x.ClassId == NonRecurringFee.ClassId).FirstOrDefault().Id;

                // int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == NonRecurringFee.SectionId).FirstOrDefault().Id;
                // var ErrorMsg = "";

                int BId = NonRecurringFee.BranchId;

                int BranchClassId = NonRecurringFee.ClassId;


                AspNetBranch_Class BranchClass = db.AspNetBranch_Class.Where(x => x.Id == BranchClassId).FirstOrDefault();

                int BranchId = BranchClass.BranchId;
                int ClassId = BranchClass.ClassId;

                var AllStudentsIds = db.AspNetStudents.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).Select(x => x.Id).ToList();


                //  var AllStudentsIds = db.AspNetStudent_Enrollments.Where(x => x.SectionId == NonRecurringFee.SectionId).Select(x => x.StudentId).Distinct().ToList();

                var count = db.StudentNonRecurringFees.Where(x => AllStudentsIds.Contains(x.StudentFeeID.Value) && x.Month == NonRecurringFee.MonthId).Count();


                var countStudentFeeDetails = db.StudentFeeDetails.Where(x => AllStudentsIds.Contains(x.StudentFee.AspNetStudent.Id) && x.Month == NonRecurringFee.MonthId).Count();

                if (countStudentFeeDetails != 0)
                {
                    ErrorMsg = "Selected challan is already created ";
                }
                else if (count != 0)
                {
                    ErrorMsg = "Fee is already created of selected branch class students";
                }
                else
                {

                    List<StudentNonRecurringFee> studentNonRecurringFeeList = new List<StudentNonRecurringFee>();

                    if (AllStudentsIds.Count() != 0)
                    {

                        foreach (var stuId in AllStudentsIds)
                        {

                            foreach (var feeTypeAmount in FeeTypeAmount)
                            {

                                StudentNonRecurringFee studentNonRecurringFee = new StudentNonRecurringFee();
                                studentNonRecurringFee.NonRecTypeID = feeTypeAmount.TypeId;
                                studentNonRecurringFee.Amount = feeTypeAmount.Amount;
                                studentNonRecurringFee.StudentFeeID = stuId;
                                studentNonRecurringFee.Month = NonRecurringFee.MonthId;
                                studentNonRecurringFeeList.Add(studentNonRecurringFee);

                            }

                        }

                        db.StudentNonRecurringFees.AddRange(studentNonRecurringFeeList);
                        db.SaveChanges();
                        TempData["ClassNonRecurringFeeCreated"] = "Created";
                    }
                    else
                    {
                        ErrorMsg = "There are no students entrolled";
                    }


                }

                if (ErrorMsg == "")
                {
                    ErrorMsg = "Created";
                }

                return Json(ErrorMsg, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {

                ErrorMsg = "Exception";
                return Json(ErrorMsg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AllBranches()
        {
            var Branches = (from branch in db.AspNetBranches
                                //join branchclasssubject in db.AspnetGenericBranchClassSubjects on branch.Id equals branchclasssubject.BranchId

                            select new
                            {
                                branch.Id,
                                branch.Name,
                            }).Distinct();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Branches);
            return Content(status);
        }

        public ActionResult checkStudentFee(int StudentId, int MonthId)
        {
            var studentNonRecurringFee = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == MonthId).FirstOrDefault();

            var studentFeeDetails = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId && x.Month == MonthId).FirstOrDefault();


            var ErrorMsg = "";
            var ErrorMsg1 = "";


            var NonRecurringFeeList = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == MonthId).ToList();

            var NonRecurringFeeTypeList = db.NonRecurringTypes.ToList();

            List<FeeTypeAmount> FeeTypeAmontList = new List<FeeTypeAmount>();

            foreach (var item in NonRecurringFeeTypeList)
            {

                foreach (var item1 in NonRecurringFeeList)
                {

                    if (item.Id == item1.NonRecTypeID)
                    {
                        FeeTypeAmount fee = new FeeTypeAmount();

                        fee.TypeId = item.Id;
                        fee.Amount = item1.Amount.Value;
                        fee.TypeName = item.ExpenseType;

                        FeeTypeAmontList.Add(fee);

                        break;
                    }

                }


            }


            if (studentFeeDetails != null)
            {
                ErrorMsg1 = "ChallanCreated";
                ErrorMsg = "Challan is already created of selected Student and Month ";
                return Json(new { ErrorMsg = ErrorMsg, ErrorMsg1 = ErrorMsg1, FeeTypeAmontList = FeeTypeAmontList }, JsonRequestBehavior.AllowGet);
            }

            else if (studentNonRecurringFee != null)
            {
                ErrorMsg1 = "FeeCreated";
                ErrorMsg = "Fee is already created of selected Student and month but Editable";
                return Json(new { ErrorMsg = ErrorMsg, ErrorMsg1 = ErrorMsg1, FeeTypeAmontList = FeeTypeAmontList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ErrorMsg = ErrorMsg, ErrorMsg1 = ErrorMsg1, FeeTypeAmontList = "" }, JsonRequestBehavior.AllowGet);

            }

        }
        [HttpGet]
        public ActionResult Edit(int StudentId, int MonthId)
        {
            _NonRecurringFee FeeObj = new _NonRecurringFee();


            var Enrollment = db.AspNetStudent_Enrollments.Where(x => x.StudentId == StudentId).FirstOrDefault();
            var SectionId = Enrollment.SectionId;
            var Section = db.AspNetBranchClass_Sections.Where(x => x.Id == SectionId).FirstOrDefault();

            var BranchClass = db.AspNetBranch_Class.Where(x => x.Id == Section.BranchClassId).FirstOrDefault();

            var BranchClassId = Section.BranchClassId;
            var BranchId = BranchClass.BranchId;
            var ClassId = BranchClass.ClassId;


            FeeObj.MonthId = MonthId;
            FeeObj.BranchId = BranchId;
            FeeObj.ClassId = BranchClassId;
            FeeObj.SectionId = SectionId;
            FeeObj.StudentId = StudentId;

            var NonRecurringFeeList = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == MonthId).ToList();

            var NonRecurringFeeTypeList = db.NonRecurringTypes.ToList();

            List<FeeTypeAmount> FeeTypeAmontList = new List<FeeTypeAmount>();




            foreach (var item in NonRecurringFeeTypeList)
            {

                foreach (var item1 in NonRecurringFeeList)
                {

                    if (item.Id == item1.NonRecTypeID)
                    {
                        FeeTypeAmount fee = new FeeTypeAmount();

                        fee.TypeId = item1.Id;
                        fee.Amount = item1.Amount.Value;
                        fee.TypeName = item.ExpenseType;

                        FeeTypeAmontList.Add(fee);

                        break;
                    }

                }


            }

            ViewBag.FeeType = FeeTypeAmontList;



            return View(FeeObj);
        }

        public ActionResult ClassesByBranch(int BranchId)
        {
            var BranchClasses = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.Id, x.AspNetClass.Name });
            //var ID = User.Identity.GetUserId();
            //var Classes = (from classs in db.AspNetClasses
            //                join branchClass in db.AspNetBranch_Class on 

            //               select new
            //               {
            //                   classs.Id,
            //                   classs.Name,
            //               }).Distinct();
            string status = Newtonsoft.Json.JsonConvert.SerializeObject(BranchClasses);
            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);
        }


        public ActionResult SectionByClasses(int ClassId)
        {
            var ID = User.Identity.GetUserId();

            //var Sections = db.AspNetTeacher_Enrollments.Where(x => x.AspNetEmployee.UserId == ID).Select(x => new { x.AspNetBranchClass_Sections.AspNetSection.Id, x.AspNetBranchClass_Sections.AspNetSection.Name }).Distinct();
            // var BranchClasses = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId).ToList().Select(x => new { x.Id, x.AspNetClass.Name });

            var Sections = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == ClassId).Select(x => new { x.Id, x.AspNetSection.Name });

            //var Sections = (from section in db.AspNetSections
            //                select new
            //                {
            //                    section.Id,
            //                    section.Name,

            //                }).Distinct();


            string status = Newtonsoft.Json.JsonConvert.SerializeObject(Sections);
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudents(/*int SectionId*/)
        {
            //int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;

            // int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.Id == SectionId).FirstOrDefault().Id;

            //var AllStudents = (from enrollment in db.AspNetStudent_Enrollments.Where(x => x.SectionId == SectionId)
            //                   join student in db.AspNetStudents on enrollment.StudentId equals student.Id
            //                   select new
            //                   {
            //                       student.Id,
            //                       student.Name,

            //                   }).Distinct();


            var students = (from stdnt in db.AspNetStudents
                            join usr in db.AspNetUsers
                            on stdnt.UserId equals usr.Id
                            select new { stdnt.Id, Name = stdnt.Name + "(" + usr.UserName + ")", stdnt.RollNo, stdnt.CellNo, usr.Image }).OrderBy(x => x.Name).ToList();


            string status = Newtonsoft.Json.JsonConvert.SerializeObject(students);
            // return Json(SubjectsByClass, JsonRequestBehavior.AllowGet);
            return Content(status);



        }

        public ActionResult SaveStudentNonRecurringFee(_NonRecurringFee NonRecurringFee, List<FeeTypeAmount> FeeTypeAmount, int StudentId)
        {
            try
            {
                // throw new System.ArgumentException("Parameter cannot be null", "original");

                StudentNonRecurringFee checkStudentFeeExist = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == NonRecurringFee.MonthId).FirstOrDefault();

                if (checkStudentFeeExist == null)
                {
                    foreach (var feeTypeAmount in FeeTypeAmount)
                    {

                        StudentNonRecurringFee studentNonRecurringFee = new StudentNonRecurringFee();
                        studentNonRecurringFee.NonRecTypeID = feeTypeAmount.TypeId;
                        studentNonRecurringFee.Amount = feeTypeAmount.Amount;
                        studentNonRecurringFee.StudentFeeID = StudentId;
                        studentNonRecurringFee.Month = NonRecurringFee.MonthId;

                        db.StudentNonRecurringFees.Add(studentNonRecurringFee);
                        db.SaveChanges();

                    }
                }

                else
                {

                    List<StudentNonRecurringFee> deleteStudentNonRecurringFee = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == NonRecurringFee.MonthId).ToList();

                    db.StudentNonRecurringFees.RemoveRange(deleteStudentNonRecurringFee);
                    db.SaveChanges();

                    foreach (var feeTypeAmount in FeeTypeAmount)
                    {

                        StudentNonRecurringFee studentNonRecurringFee = new StudentNonRecurringFee();
                        studentNonRecurringFee.NonRecTypeID = feeTypeAmount.TypeId;
                        studentNonRecurringFee.Amount = feeTypeAmount.Amount;
                        studentNonRecurringFee.StudentFeeID = StudentId;
                        studentNonRecurringFee.Month = NonRecurringFee.MonthId;
                        db.StudentNonRecurringFees.Add(studentNonRecurringFee);
                        db.SaveChanges();

                    }


                }

                TempData["StudentNonRecurringFeeCreated"] = "Created";
                return Json("", JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);

            }

        }

        public class NonRecurringFeeEdit
        {
            public int BranchId { get; set; }
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int MonthId { get; set; }
        }



        public class _NonRecurringFee
        {
            public int BranchId { get; set; }
            public int ClassId { get; set; }
            public int SectionId { get; set; }
            public int StudentId { get; set; }
            public int MonthId { get; set; }

            public List<FeeTypeAmount> FeeTypeAmountList = new List<FeeTypeAmount>();
        }
        public class FeeTypeAmount
        {
            public int TypeId { get; set; }
            public string TypeName { get; set; }
            public int Amount { get; set; }

        }
    }
}