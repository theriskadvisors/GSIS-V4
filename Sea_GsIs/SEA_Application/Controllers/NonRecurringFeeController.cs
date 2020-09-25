using Microsoft.AspNet.Identity;
using OfficeOpenXml;
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

        public ActionResult NonRecurringFeeLoader()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult NonRecurringFeeFromFile(RegisterViewModel model)
        {

            var dbTransaction = db.Database.BeginTransaction();
            int? RowNumber = null;
            int? ColumnNumber = null;
            var StudentFeeMsg = "";

            try
            {
                HttpPostedFileBase file = Request.Files["StudentLoaderFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                }
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    ApplicationDbContext context = new ApplicationDbContext();

                    List<int> NonRecurringFeeTypeIds = new List<int>();

                    int AdmissionFee = 0;
                    int StationeryFundId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Stationery Fund").FirstOrDefault().Id;
                    int AnnualFundId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Annual Fund").FirstOrDefault().Id;
                    int AdmissionFeeId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Admission Fee").FirstOrDefault().Id;
                    int SecurityRefId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Security (Ref)").FirstOrDefault().Id;
                    int RegistrationId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Registration").FirstOrDefault().Id;
                    int AdvanceTaxId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Advance Tax").FirstOrDefault().Id;
                    int AdjustmentId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Adjustment").FirstOrDefault().Id;
                    int SLCChargesId = db.NonRecurringTypes.Where(x => x.ExpenseType == "SLC Charges").FirstOrDefault().Id;
                    int ArrearsId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Arrears").FirstOrDefault().Id;
                    int ExamChargesId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Exam Charges").FirstOrDefault().Id;
                    int NonPaymentFineId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Non-Payment Fine").FirstOrDefault().Id;
                    int WaiverId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Waiver (If Any)/ VLEP Sofware charges").FirstOrDefault().Id;
                    int DefferedId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Deffered (If Any)").FirstOrDefault().Id;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        //  RowNumber = rowIterator - 1;
                        //  RowNumber = rowIterator;

                        var Name = workSheet.Cells[rowIterator, 1].Value.ToString();
                        var UserName = workSheet.Cells[rowIterator, 2].Value.ToString();
                        var UserNameExist = (from user in db.AspNetUsers //db.AspNetUsers.Where(x=>x.AspNetUser.UserName ==UserName).FirstOrDefault();
                                             join student in db.AspNetStudents on user.Id equals student.UserId
                                             where user.UserName == UserName
                                             select user).FirstOrDefault();

                        
                        if (UserNameExist != null)
                        {

                            var Month = workSheet.Cells[rowIterator, 3].Value;

                            if (Month == null)
                            {
                                StudentFeeMsg = ":Please Enter Billing Month";

                                throw new System.ArgumentException(":Please Enter Billing Month.");

                                //   Month = 0;

                            }
                            var MonthToInterger = Convert.ToInt32(Month);

                            if (MonthToInterger < 0 || MonthToInterger == 0)
                            {
                                StudentFeeMsg = ":Please Enter Corrent Month No";

                                throw new System.ArgumentException(":Please Enter Corrent Month No");
                            }


                            var StationeryFund = workSheet.Cells[rowIterator, 4].Value;

                            if (StationeryFund == null)
                            {
                                StationeryFund = 0;
                            }
                            var AnnualFund = workSheet.Cells[rowIterator, 5].Value;

                            if (AnnualFund == null)
                            {
                                AnnualFund = 0;
                            }
                            var SecurityRef = workSheet.Cells[rowIterator, 6].Value;

                            if (SecurityRef == null)
                            {
                                SecurityRef = 0;
                            }
                            var Registration = workSheet.Cells[rowIterator, 7].Value;

                            if (Registration == null)
                            {
                                Registration = 0;
                            }

                            var AdvanceTax = workSheet.Cells[rowIterator, 8].Value;

                            if (AdvanceTax == null)
                            {
                                AdvanceTax = 0;
                            }

                            var Adjustment = workSheet.Cells[rowIterator, 9].Value;

                            if (Adjustment == null)
                            {
                                Adjustment = 0;
                            }

                            var SLCCharges = workSheet.Cells[rowIterator, 10].Value;

                            if (SLCCharges == null)
                            {
                                SLCCharges = 0;
                            }

                            var Arrears = workSheet.Cells[rowIterator, 11].Value;

                            if (Arrears == null)
                            {
                                Arrears = 0;
                            }

                            var ExamCharges = workSheet.Cells[rowIterator, 12].Value;

                            if (ExamCharges == null)
                            {
                                ExamCharges = 0;
                            }

                            var NonPaymentFine = workSheet.Cells[rowIterator, 13].Value;

                            if (NonPaymentFine == null)
                            {
                                NonPaymentFine = 0;
                            }

                            var Waiver = workSheet.Cells[rowIterator, 14].Value;
                            if (Waiver == null)
                            {
                                Waiver = 0;
                            }

                            var Deffered = workSheet.Cells[rowIterator, 15].Value;

                            if (Deffered == null)
                            {
                                Deffered = 0;
                            }


                            //   int MonthInt = Convert.ToInt32(Month);
                            var StudentExist = db.AspNetStudents.Where(x => x.UserId == UserNameExist.Id).FirstOrDefault();

                            if (StudentExist != null)
                            {

                                StudentNonRecurringFee checkStudentFeeExist = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentExist.Id && x.Month == MonthToInterger).FirstOrDefault();

                                if (checkStudentFeeExist == null)
                                {

                                    StudentNonRecurringFee studentNonRecurringFee1 = new StudentNonRecurringFee();
                                    studentNonRecurringFee1.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee1.Amount = Convert.ToInt32(StationeryFund);
                                    studentNonRecurringFee1.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee1.NonRecTypeID = StationeryFundId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee1);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee2 = new StudentNonRecurringFee();
                                    studentNonRecurringFee2.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee2.Amount = Convert.ToInt32(AnnualFund);
                                    studentNonRecurringFee2.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee2.NonRecTypeID = AnnualFundId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee2);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee3 = new StudentNonRecurringFee();
                                    studentNonRecurringFee3.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee3.Amount = AdmissionFee;
                                    studentNonRecurringFee3.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee3.NonRecTypeID = AdmissionFeeId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee3);
                                    db.SaveChanges();


                                    StudentNonRecurringFee studentNonRecurringFee4 = new StudentNonRecurringFee();
                                    studentNonRecurringFee4.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee4.Amount = Convert.ToInt32(SecurityRef);
                                    studentNonRecurringFee4.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee4.NonRecTypeID = SecurityRefId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee4);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee5 = new StudentNonRecurringFee();
                                    studentNonRecurringFee5.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee5.Amount = Convert.ToInt32(Registration);
                                    studentNonRecurringFee5.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee5.NonRecTypeID = RegistrationId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee5);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee6 = new StudentNonRecurringFee();
                                    studentNonRecurringFee6.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee6.Amount = Convert.ToInt32(AdvanceTax);
                                    studentNonRecurringFee6.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee6.NonRecTypeID = AdvanceTaxId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee6);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee7 = new StudentNonRecurringFee();
                                    studentNonRecurringFee7.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee7.Amount = Convert.ToInt32(Adjustment);
                                    studentNonRecurringFee7.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee7.NonRecTypeID = AdjustmentId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee7);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee8 = new StudentNonRecurringFee();
                                    studentNonRecurringFee8.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee8.Amount = Convert.ToInt32(SLCCharges);
                                    studentNonRecurringFee8.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee8.NonRecTypeID = SLCChargesId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee8);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee9 = new StudentNonRecurringFee();
                                    studentNonRecurringFee9.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee9.Amount = Convert.ToInt32(Arrears);
                                    studentNonRecurringFee9.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee9.NonRecTypeID = ArrearsId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee9);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee10 = new StudentNonRecurringFee();
                                    studentNonRecurringFee10.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee10.Amount = Convert.ToInt32(ExamCharges);
                                    studentNonRecurringFee10.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee10.NonRecTypeID = ExamChargesId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee10);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee11 = new StudentNonRecurringFee();
                                    studentNonRecurringFee11.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee11.Amount = Convert.ToInt32(NonPaymentFine);
                                    studentNonRecurringFee11.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee11.NonRecTypeID = NonPaymentFineId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee11);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee12 = new StudentNonRecurringFee();
                                    studentNonRecurringFee12.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee12.Amount = Convert.ToInt32(Waiver);
                                    studentNonRecurringFee12.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee12.NonRecTypeID = WaiverId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee12);
                                    db.SaveChanges();

                                    StudentNonRecurringFee studentNonRecurringFee13 = new StudentNonRecurringFee();
                                    studentNonRecurringFee13.Month = Convert.ToInt32(Month);
                                    studentNonRecurringFee13.Amount = Convert.ToInt32(Deffered);
                                    studentNonRecurringFee13.StudentFeeID = StudentExist.Id;
                                    studentNonRecurringFee13.NonRecTypeID = DefferedId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee13);
                                    db.SaveChanges();


                                }
                                else
                                {
                                    StudentFeeMsg = ":Fee is Already created of selected student and month";

                                    throw new System.ArgumentException(":Fee is Already created of this student");
                                }

                            }

                        }

                        else
                        {
                            ViewBag.Error = "Error in Row " + RowNumber + " Please Enter Valid UserName";
                            dbTransaction.Dispose();

                            return View("NonRecurringFeeLoader");
                        }


                    }
                    dbTransaction.Commit();
                    return RedirectToAction("Index","NonRecurringFee");
                }
            }//try bracket

            catch (Exception e)
            {
                //   ModelState.AddModelError("Error", e.InnerException);
                dbTransaction.Dispose();

                ViewBag.Error = "Error in Row " + RowNumber + " " + StudentFeeMsg;

                return View("NonRecurringFeeLoader");
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