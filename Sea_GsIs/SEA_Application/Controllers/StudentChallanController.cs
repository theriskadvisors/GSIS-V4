using Microsoft.AspNet.Identity;
using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class StudentChallanController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: StudentChallan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetChallan()
        {

            return View();
        }

        public ActionResult SaveChallanForm(int BranchId, int ClassId, int MonthId, string DueDate,string ImportantNotice)
        {
            var dbTransaction = db.Database.BeginTransaction();
            var errorMsg = "";
            var FeeExist = true;
            List<string> StudentIds = null;
            try
            {
                var userId = User.Identity.GetUserId();

                //  int BranchClassId = db.AspNetBranch_Class.Where(x => x.BranchId == BranchId && x.ClassId == ClassId).FirstOrDefault().Id;

                //  int BranchClassSectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BranchClassId && x.SectionId == SectionId).FirstOrDefault().Id;

                int BranchClassId = ClassId;


                AspNetBranch_Class BranchClass = db.AspNetBranch_Class.Where(x => x.Id == BranchClassId).FirstOrDefault();

                int BranchClassBranchId = BranchClass.BranchId;
                int BranchClassClassId = BranchClass.ClassId;
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(MonthId);

                List<int> AllStudentsIds = db.AspNetStudents.Where(x => x.BranchId == BranchClassBranchId && x.ClassId == BranchClassClassId).Select(x => x.Id).ToList();

                //  List<int> AllStudentsIds = db.AspNetStudent_Enrollments.Where(x => x.SectionId == SectionId).Select(x => x.StudentId).Distinct().ToList();

                // List<int> ids  = new List<int> {  1835,1757 };
                var BranchName = BranchClass.AspNetBranch.Name;
                var ClassName = BranchClass.AspNetClass.Name;

                var count = db.StudentFeeDetails.Where(x => AllStudentsIds.Contains(x.StudentFee.AspNetStudent.Id) && x.Month == MonthId).Count();

                if (count != 0)
                {
                   // errorMsg = "Selected challan is already created.";
                    errorMsg = "Challan for "+ ClassName +" is already created for "+monthName;
                }
                else
                {
                    if (AllStudentsIds.Count() != 0)
                    {

                        List<StudentFeeDetail> listStudentFeeDetails = new List<StudentFeeDetail>();
                        foreach (var StudentId in AllStudentsIds)
                        {
                            var StudentFee = db.StudentFees.Where(x => x.StudentID == StudentId).FirstOrDefault();
                            if (StudentFee == null)
                            {
                                FeeExist = false;

                                throw new System.ArgumentException("Parameter cannot be null", "original");
                            }
                        }

                        foreach (var StudentId in AllStudentsIds)
                        {
                            StudentFeeDetail studentfeeDetails = new StudentFeeDetail();

                            var StudentFee = db.StudentFees.Where(x => x.StudentID == StudentId).FirstOrDefault();

                            studentfeeDetails.StudentFeeID = StudentFee.Id;
                            studentfeeDetails.ChallanSubmissionDate = null;
                            studentfeeDetails.ChallanDueDate = Convert.ToDateTime(DueDate);
                            studentfeeDetails.ChallanIssueDate = DateTime.Now;
                            studentfeeDetails.Status = "Pending";
                            studentfeeDetails.FurtherDiscount = 0;
                            studentfeeDetails.DiscountComments = null;
                            studentfeeDetails.Month = MonthId;
                            studentfeeDetails.Notice = ImportantNotice;
                            int StudentTotalChallan = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).Count();
                            studentfeeDetails.InvoiceNo = StudentTotalChallan + 1;

                            double TotalWithoutAdminssion = StudentFee.TotalWithoutAdmission.Value;
                            var StudentChallanExist = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).ToList();

                            var NonRecurringFeeExist = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId).ToList();


                            //if (StudentChallanExist.Count() == 0)
                            //{
                            //    if (NonRecurringFeeExist.Count() == 0)
                            //    {
                            //        StudentNonRecurringFee studentNonRecurringFee = new StudentNonRecurringFee();

                            //        int AdmissionFeeTypeId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Admission Fee").FirstOrDefault().Id;


                            //        studentNonRecurringFee.Amount = Convert.ToInt32(StudentFee.DiscountAdmissionFeeAmount.Value);
                            //        studentNonRecurringFee.NonRecTypeID = AdmissionFeeTypeId;
                            //        studentNonRecurringFee.Month = MonthId;
                            //        studentNonRecurringFee.StudentFeeID = StudentId;
                            //        db.StudentNonRecurringFees.Add(studentNonRecurringFee);
                            //        db.SaveChanges();

                            //        //paidAmount = paidAmount + (StudentFee.DiscountAdmissionFeeAmount.Value);

                            //    }

                            //}



                            if (StudentChallanExist.Count() == 0)
                            {
                                if (NonRecurringFeeExist.Count() == 0)
                                {
                                    StudentNonRecurringFee studentNonRecurringFee = new StudentNonRecurringFee();

                                    int AdmissionFeeTypeId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Admission Fee").FirstOrDefault().Id;

                                    studentNonRecurringFee.Amount = Convert.ToInt32(StudentFee.DiscountAdmissionFeeAmount.Value);
                                    studentNonRecurringFee.NonRecTypeID = AdmissionFeeTypeId;
                                    studentNonRecurringFee.Month = MonthId;
                                    studentNonRecurringFee.StudentFeeID = StudentId;
                                    db.StudentNonRecurringFees.Add(studentNonRecurringFee);

                                    db.SaveChanges();

                                }
                                else
                                {
                                    var UpdateStudentNonRecurringFee = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == MonthId && x.NonRecurringType.ExpenseType == "Admission Fee").FirstOrDefault();
                                    UpdateStudentNonRecurringFee.Amount = Convert.ToInt32(StudentFee.DiscountAdmissionFeeAmount.Value);
                                    db.SaveChanges();
                                }
                            }

                            var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthId && x.StudentFeeID == StudentId).ToList();
                            int TotalNonRecurring = 0;
                            foreach (var nonRecurring in nonRecurringList)
                            {
                                TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                            }

                            var multiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == StudentId).FirstOrDefault();
                            double MonthMultiplier = 0;
                            if (MonthId == 1)
                                MonthMultiplier = multiplier.Jan_Multiplier.Value;
                            else if (MonthId == 2)
                                MonthMultiplier = multiplier.Feb_Multiplier.Value;
                            else if (MonthId == 3)
                                MonthMultiplier = multiplier.Mar_Multiplier.Value;
                            else if (MonthId == 4)
                                MonthMultiplier = multiplier.April__Multiplier.Value;
                            else if (MonthId == 5)
                                MonthMultiplier = multiplier.May_Multiplier.Value;
                            else if (MonthId == 6)
                                MonthMultiplier = multiplier.June_Multiplier.Value;
                            else if (MonthId == 7)
                                MonthMultiplier = multiplier.July__Multiplier.Value;
                            else if (MonthId == 8)
                                MonthMultiplier = multiplier.Aug_Multiplier.Value;
                            else if (MonthId == 9)
                                MonthMultiplier = multiplier.Sep_Multiplier.Value;
                            else if (MonthId == 10)
                                MonthMultiplier = multiplier.Oct_Multiplier.Value;
                            else if (MonthId == 11)
                                MonthMultiplier = multiplier.Nov_Multiplier.Value;
                            else if (MonthId == 12)
                                MonthMultiplier = multiplier.Dec__Multiplier.Value;
                            else
                            {

                            }
                            double paidAmount = (TotalWithoutAdminssion * MonthMultiplier) + TotalNonRecurring;

                            double? TotalAmount = paidAmount;

                            var TotalTutionFee = StudentFee.TutionFee * MonthMultiplier;

                            var TotalDiscountTutionFeeAmount = StudentFee.DiscountTutionFeeAmount * MonthMultiplier;
                            var DiscountWhileFeeCreation = TotalTutionFee - TotalDiscountTutionFeeAmount;

                            double? TotalAmountForStudentFee = TotalAmount;


                            if (StudentFee.DiscountTutionFee != 0)
                            {
                                TotalAmountForStudentFee = TotalAmount + (TotalTutionFee - TotalDiscountTutionFeeAmount);

                            }

                            studentfeeDetails.Multiplier = MonthMultiplier;
                            studentfeeDetails.PaidAmount = Convert.ToInt32(paidAmount);
                            listStudentFeeDetails.Add(studentfeeDetails);

                            var username = db.AspNetUsers.Where(x => x.Id == userId).Select(x => x.Name).FirstOrDefault();
                            Voucher voucher = new Voucher();
                            var SessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;
                            voucher.Name = "Student Fee Creation";
                            voucher.Notes = "";
                            voucher.Date = GetLocalDateTime.GetLocalDateTimeFunction();
                            voucher.CreatedBy = username;
                            int? VoucherObj = db.Vouchers.Max(x => x.VoucherNo);
                            voucher.VoucherNo = Convert.ToInt32(VoucherObj) + 1;
                            db.Vouchers.Add(voucher);
                            db.SaveChanges();


                            var Leadger = db.Ledgers.Where(x => x.Name == "Account Receivable").FirstOrDefault();
                            int AccountReceivableId = Leadger.Id;
                            decimal? CurrentBalance = Leadger.CurrentBalance;

                            VoucherRecord voucherRecord = new VoucherRecord();
                            decimal? AfterBalance = CurrentBalance + Convert.ToDecimal(TotalAmount);
                            voucherRecord.LedgerId = AccountReceivableId;
                            voucherRecord.Type = "Dr";
                            voucherRecord.Amount = Convert.ToDecimal(TotalAmount);
                            voucherRecord.CurrentBalance = CurrentBalance;
                            voucherRecord.AfterBalance = AfterBalance;
                            voucherRecord.VoucherId = voucher.Id;
                            voucherRecord.UserType = "Student";
                            voucherRecord.UserId = StudentId.ToString();
                            voucherRecord.Description = "Fee added of student";
                            Leadger.CurrentBalance = AfterBalance;
                            db.VoucherRecords.Add(voucherRecord);
                            db.SaveChanges();

                            VoucherRecord voucherRecord1 = new VoucherRecord();

                            var LedgerStudentFee = db.Ledgers.Where(x => x.Name == "Student Fee").FirstOrDefault();

                            decimal? CurrentBalanceOfStudentFee = LedgerStudentFee.CurrentBalance;
                            decimal? AfterBalanceOfNotes = CurrentBalanceOfStudentFee + Convert.ToDecimal(TotalAmountForStudentFee);
                            voucherRecord1.LedgerId = LedgerStudentFee.Id;
                            voucherRecord1.Type = "Cr";
                            voucherRecord1.Amount = Convert.ToDecimal(TotalAmountForStudentFee);
                            voucherRecord1.CurrentBalance = CurrentBalanceOfStudentFee;
                            voucherRecord1.AfterBalance = AfterBalanceOfNotes;
                            voucherRecord1.VoucherId = voucher.Id;
                            voucherRecord1.Description = "Fee Added of Student ";
                            voucherRecord1.UserType = "Student";
                            voucherRecord1.UserId = StudentId.ToString();
                            voucherRecord1.BranchId = BranchId;
                            LedgerStudentFee.CurrentBalance = AfterBalanceOfNotes;

                            db.VoucherRecords.Add(voucherRecord1);
                            db.SaveChanges();


                            if (StudentFee.DiscountTutionFee != 0)
                            {
                                VoucherRecord voucherRecord3 = new VoucherRecord();
                                var LeadgerDiscount = db.Ledgers.Where(x => x.Name == "Discount").FirstOrDefault();

                                decimal? CurrentBalanceOfDiscount = LeadgerDiscount.CurrentBalance;
                                decimal? AfterBalanceOfDiscount = CurrentBalanceOfDiscount + Convert.ToDecimal(DiscountWhileFeeCreation);
                                voucherRecord3.LedgerId = LeadgerDiscount.Id;
                                voucherRecord3.Type = "Dr";
                                voucherRecord3.Amount = Convert.ToDecimal(DiscountWhileFeeCreation);
                                voucherRecord3.CurrentBalance = CurrentBalanceOfDiscount;
                                voucherRecord3.AfterBalance = AfterBalanceOfDiscount;
                                voucherRecord3.VoucherId = voucher.Id;
                                voucherRecord3.UserType = "Student";
                                voucherRecord3.UserId = StudentId.ToString();
                                voucherRecord3.BranchId = BranchId;
                                voucherRecord3.Description = "Discount given to student while fee creation";
                                LeadgerDiscount.CurrentBalance = AfterBalanceOfDiscount;
                                db.VoucherRecords.Add(voucherRecord3);
                                db.SaveChanges();

                            }

                        } ///end loop

                        db.StudentFeeDetails.AddRange(listStudentFeeDetails);
                        db.SaveChanges();

                        StudentIds = listStudentFeeDetails.Select(x => x.Id.ToString()).ToList();
                     
                      //  TempData["ClassChallanFormCreated"] = "Created";
                        dbTransaction.Commit();
                         
                    }
                    
                    else
                    {
                        errorMsg = "There are no students enrolled. ";
                        dbTransaction.Dispose();
                    }
                }

                if (errorMsg == "")
                {
                    errorMsg = "Created";
                }
                

                return Json( new { errorMsg  = errorMsg, StudentIds = StudentIds }, JsonRequestBehavior.AllowGet);
            } //end of try

            catch (Exception ex)
            {

                var msg = ex.Message;

                dbTransaction.Dispose();

                if (FeeExist == true)
                {
                    errorMsg = "Exception";

                }
                else if (FeeExist == false)
                {
                    errorMsg = "Fee is not created for all students of the selected branch class";
                }
                else
                {
                    errorMsg = "Exception";
                }

                return Json(new { errorMsg = errorMsg, StudentIds = StudentIds }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult SaveChallanFormOfStudent(/*int BranchId1, int ClassId1, int SectionId1,*/ int StudentId, int Month1, string DueDate1, int FurtherDiscount, string DiscountComments,string ImportantNotice1)
        {
            string StudentFeeId  = null;
            string StatusMsg = null; 
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                // throw new System.ArgumentException("Parameter cannot be null", "original");
                var studentFeeDetailsExist = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId && x.Month == Month1).FirstOrDefault();

                int BranchId = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault().BranchId.Value;

                if (studentFeeDetailsExist == null)
                {
                    StudentFeeDetail studentfeeDetails = new StudentFeeDetail();

                    var StudentFee = db.StudentFees.Where(x => x.StudentID == StudentId).FirstOrDefault();
                    studentfeeDetails.StudentFeeID = StudentFee.Id;
                    studentfeeDetails.ChallanSubmissionDate = null;
                    studentfeeDetails.ChallanDueDate = Convert.ToDateTime(DueDate1);
                    studentfeeDetails.ChallanIssueDate = GetLocalDateTime.GetLocalDateTimeFunction();
                    studentfeeDetails.Status = "Pending";
                    studentfeeDetails.FurtherDiscount = FurtherDiscount;
                    studentfeeDetails.DiscountComments = DiscountComments;
                    studentfeeDetails.Month = Month1;
                    studentfeeDetails.Notice = ImportantNotice1;
                    int StudentTotalChallan = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).Count();

                    studentfeeDetails.InvoiceNo = StudentTotalChallan + 1;

                    //if (StudentTotalChallan ==0)
                    //{

                    //}
                    //else
                    //{
                    //    studentfeeDetails.InvoiceNo = StudentTotalChallan + 1;

                    //}

                    var StudentChallanExist = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).ToList();

                    var NonRecurringFeeExist = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == Month1).ToList();


                    if (StudentChallanExist.Count() == 0)
                    {
                        if (NonRecurringFeeExist.Count() == 0)
                        {
                            StudentNonRecurringFee studentNonRecurringFee = new StudentNonRecurringFee();

                            int AdmissionFeeTypeId = db.NonRecurringTypes.Where(x => x.ExpenseType == "Admission Fee").FirstOrDefault().Id;

                            studentNonRecurringFee.Amount = Convert.ToInt32(StudentFee.DiscountAdmissionFeeAmount.Value);
                            studentNonRecurringFee.NonRecTypeID = AdmissionFeeTypeId;
                            studentNonRecurringFee.Month = Month1;
                            studentNonRecurringFee.StudentFeeID = StudentId;
                            db.StudentNonRecurringFees.Add(studentNonRecurringFee);
                            db.SaveChanges();

                        }
                        else
                        {
                            var UpdateStudentNonRecurringFee = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == Month1 && x.NonRecurringType.ExpenseType == "Admission Fee").FirstOrDefault();
                            UpdateStudentNonRecurringFee.Amount = Convert.ToInt32(StudentFee.DiscountAdmissionFeeAmount.Value);
                            db.SaveChanges();
                        }
                    }

                    double TotalWithoutAdminssion = StudentFee.TotalWithoutAdmission.Value;

                    var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == Month1 && x.StudentFeeID == StudentId).ToList();

                    int TotalNonRecurring = 0;
                    foreach (var nonRecurring in nonRecurringList)
                    {
                        TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                    }

                    var multiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == StudentId).FirstOrDefault();

                    double MonthMultiplier = 0;
                    if (Month1 == 1)
                        MonthMultiplier = multiplier.Jan_Multiplier.Value;
                    else if (Month1 == 2)
                        MonthMultiplier = multiplier.Feb_Multiplier.Value;
                    else if (Month1 == 3)
                        MonthMultiplier = multiplier.Mar_Multiplier.Value;
                    else if (Month1 == 4)
                        MonthMultiplier = multiplier.April__Multiplier.Value;
                    else if (Month1 == 5)
                        MonthMultiplier = multiplier.May_Multiplier.Value;
                    else if (Month1 == 6)
                        MonthMultiplier = multiplier.June_Multiplier.Value;
                    else if (Month1 == 7)
                        MonthMultiplier = multiplier.July__Multiplier.Value;
                    else if (Month1 == 8)
                        MonthMultiplier = multiplier.Aug_Multiplier.Value;
                    else if (Month1 == 9)
                        MonthMultiplier = multiplier.Sep_Multiplier.Value;
                    else if (Month1 == 10)
                        MonthMultiplier = multiplier.Oct_Multiplier.Value;
                    else if (Month1 == 11)
                        MonthMultiplier = multiplier.Nov_Multiplier.Value;
                    else if (Month1 == 12)
                        MonthMultiplier = multiplier.Dec__Multiplier.Value;
                    else
                    {

                    }
                    //var TotalWithoutAdmissionAfterDiscount = TotalWithoutAdminssion - (FurtherDiscount);

                    double paidAmount = (TotalWithoutAdminssion * MonthMultiplier) + TotalNonRecurring;

                    //if (StudentChallanExist.Count() == 0)
                    //{
                    //    paidAmount = paidAmount + (StudentFee.DiscountAdmissionFeeAmount.Value);
                    //}

                    double paidAmountWithoutDiscount = paidAmount;
                    double? TotalAmount = paidAmount;

                    paidAmount = paidAmount - FurtherDiscount;


                    if (FurtherDiscount != 0)
                    {
                        paidAmount = paidAmount;
                    }

                    var TotalTutionFee = StudentFee.TutionFee * MonthMultiplier;
                    var TotalDiscountTutionFeeAmount = StudentFee.DiscountTutionFeeAmount * MonthMultiplier;

                    var DiscountWhileFeeCreation = TotalTutionFee - TotalDiscountTutionFeeAmount;
                    //    var TotalDiscount = FurtherDiscount + (TotalTutionFee - TotalDiscountTutionFeeAmount);

                    double? TotalAmountForStudentFee = TotalAmount;

                    if (StudentFee.DiscountTutionFee != 0)
                    {
                        TotalAmountForStudentFee = TotalAmount + (TotalTutionFee - TotalDiscountTutionFeeAmount);

                    }
                    // var TotalDiscount = FurtherDiscount + (StudentFee.DiscountTutionFeeAmount * MonthMultiplier);

                    studentfeeDetails.Multiplier = MonthMultiplier;
                    studentfeeDetails.PaidAmount = Convert.ToInt32(paidAmount);

                    db.StudentFeeDetails.Add(studentfeeDetails);
                    db.SaveChanges();
                    StudentFeeId = studentfeeDetails.Id.ToString();


                    var id = User.Identity.GetUserId();
                    var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
                    Voucher voucher = new Voucher();
                    var SessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;
                    voucher.Name = "Student Fee Creation of student ";
                    voucher.Notes = "";
                    voucher.Date = GetLocalDateTime.GetLocalDateTimeFunction();
                    voucher.CreatedBy = username;
                    int? VoucherObj = db.Vouchers.Max(x => x.VoucherNo);
                    voucher.VoucherNo = Convert.ToInt32(VoucherObj) + 1;
                    db.Vouchers.Add(voucher);
                    db.SaveChanges();

                    var Leadger = db.Ledgers.Where(x => x.Name == "Account Receivable").FirstOrDefault();
                    int AccountReceivableId = Leadger.Id;
                    decimal? CurrentBalance = Leadger.CurrentBalance;

                    VoucherRecord voucherRecord = new VoucherRecord();
                    decimal? AfterBalance = CurrentBalance + Convert.ToDecimal(TotalAmount);
                    voucherRecord.LedgerId = AccountReceivableId;
                    voucherRecord.Type = "Dr";
                    voucherRecord.Amount = Convert.ToDecimal(TotalAmount);
                    voucherRecord.CurrentBalance = CurrentBalance;
                    voucherRecord.AfterBalance = AfterBalance;
                    voucherRecord.VoucherId = voucher.Id;
                    voucherRecord.UserType = "Student";
                    voucherRecord.UserId = StudentId.ToString();
                    voucherRecord.BranchId = BranchId;


                    voucherRecord.Description = "Fee added of student";
                    Leadger.CurrentBalance = AfterBalance;
                    db.VoucherRecords.Add(voucherRecord);
                    db.SaveChanges();


                    VoucherRecord voucherRecord1 = new VoucherRecord();

                    var LedgerStudentFee = db.Ledgers.Where(x => x.Name == "Student Fee").FirstOrDefault();

                    decimal? CurrentBalanceOfStudentFee = LedgerStudentFee.CurrentBalance;
                    decimal? AfterBalanceOfNotes = CurrentBalanceOfStudentFee + Convert.ToDecimal(TotalAmountForStudentFee);
                    voucherRecord1.LedgerId = LedgerStudentFee.Id;
                    voucherRecord1.Type = "Cr";
                    voucherRecord1.Amount = Convert.ToDecimal(TotalAmountForStudentFee);
                    voucherRecord1.CurrentBalance = CurrentBalanceOfStudentFee;
                    voucherRecord1.AfterBalance = AfterBalanceOfNotes;
                    voucherRecord1.VoucherId = voucher.Id;
                    voucherRecord1.Description = "Fee Added of Student ";
                    voucherRecord1.UserType = "Student";

                    voucherRecord1.UserId = StudentId.ToString();
                    voucherRecord1.BranchId = BranchId;
                    LedgerStudentFee.CurrentBalance = AfterBalanceOfNotes;

                    db.VoucherRecords.Add(voucherRecord1);
                    db.SaveChanges();


                    if (StudentFee.DiscountTutionFee != 0)
                    {

                        var DiscountAmount = FurtherDiscount;
                        VoucherRecord voucherRecord3 = new VoucherRecord();
                        var LeadgerDiscount = db.Ledgers.Where(x => x.Name == "Discount").FirstOrDefault();

                        decimal? CurrentBalanceOfDiscount = LeadgerDiscount.CurrentBalance;
                        decimal? AfterBalanceOfDiscount = CurrentBalanceOfDiscount + Convert.ToDecimal(DiscountWhileFeeCreation);
                        voucherRecord3.LedgerId = LeadgerDiscount.Id;
                        voucherRecord3.Type = "Dr";
                        voucherRecord3.Amount = Convert.ToDecimal(DiscountWhileFeeCreation);
                        voucherRecord3.CurrentBalance = CurrentBalanceOfDiscount;
                        voucherRecord3.AfterBalance = AfterBalanceOfDiscount;
                        voucherRecord3.VoucherId = voucher.Id;
                        voucherRecord3.UserType = "Student";
                        voucherRecord3.UserId = StudentId.ToString();
                        voucherRecord3.BranchId = BranchId;
                        voucherRecord3.Description = "Discount given to student while fee creation";
                        LeadgerDiscount.CurrentBalance = AfterBalanceOfDiscount;
                        db.VoucherRecords.Add(voucherRecord3);
                        db.SaveChanges();

                    }

                    if (FurtherDiscount != 0)
                    {

                        Voucher voucher1 = new Voucher();
                        voucher1.Name = "Student Discount";
                        voucher1.Notes = "";
                        voucher1.Date = GetLocalDateTime.GetLocalDateTimeFunction();
                        voucher1.CreatedBy = username;
                        int? VoucherObj1 = db.Vouchers.Max(x => x.VoucherNo);
                        voucher1.VoucherNo = Convert.ToInt32(VoucherObj1) + 1;
                        db.Vouchers.Add(voucher1);
                        db.SaveChanges();


                        var DiscountAmount = FurtherDiscount;
                        VoucherRecord voucherRecord3 = new VoucherRecord();
                        var LeadgerDiscount = db.Ledgers.Where(x => x.Name == "Discount").FirstOrDefault();

                        decimal? CurrentBalanceOfDiscount = LeadgerDiscount.CurrentBalance;
                        decimal? AfterBalanceOfDiscount = CurrentBalanceOfDiscount + Convert.ToDecimal(FurtherDiscount);
                        voucherRecord3.LedgerId = LeadgerDiscount.Id;
                        voucherRecord3.Type = "Dr";
                        voucherRecord3.Amount = Convert.ToDecimal(FurtherDiscount);
                        voucherRecord3.CurrentBalance = CurrentBalanceOfDiscount;
                        voucherRecord3.AfterBalance = AfterBalanceOfDiscount;
                        voucherRecord3.VoucherId = voucher1.Id;
                        voucherRecord3.UserType = "Student";
                        voucherRecord3.UserId = StudentId.ToString();
                        voucherRecord3.BranchId = BranchId;
                        voucherRecord3.Description = "Discount given to student ";
                        LeadgerDiscount.CurrentBalance = AfterBalanceOfDiscount;
                        db.VoucherRecords.Add(voucherRecord3);
                        db.SaveChanges();


                        VoucherRecord voucherRecord4 = new VoucherRecord();
                        var LedgerAccountReceivable = db.Ledgers.Where(x => x.Name == "Account Receivable").FirstOrDefault();

                        decimal? CurrentBalanceOfAccountReceivable = LedgerAccountReceivable.CurrentBalance;
                        decimal? AfterBalanceOfAccountReceivable = CurrentBalanceOfAccountReceivable - Convert.ToDecimal(FurtherDiscount);
                        voucherRecord4.LedgerId = LedgerAccountReceivable.Id;
                        voucherRecord4.Type = "Cr";
                        voucherRecord4.Amount = Convert.ToDecimal(FurtherDiscount);
                        voucherRecord4.CurrentBalance = CurrentBalanceOfAccountReceivable;
                        voucherRecord4.AfterBalance = AfterBalanceOfAccountReceivable;
                        voucherRecord4.VoucherId = voucher1.Id;
                        voucherRecord4.UserType = "Student";
                        voucherRecord4.UserId = StudentId.ToString();
                        voucherRecord4.BranchId = BranchId;
                        voucherRecord4.Description = "";
                        LedgerAccountReceivable.CurrentBalance = AfterBalanceOfAccountReceivable;

                        db.VoucherRecords.Add(voucherRecord4);
                        db.SaveChanges();
                    }

                }
                else
                {

                    studentFeeDetailsExist.PaidAmount = studentFeeDetailsExist.PaidAmount - FurtherDiscount;
                    studentFeeDetailsExist.ChallanDueDate = Convert.ToDateTime(DueDate1);
                    studentFeeDetailsExist.FurtherDiscount = studentFeeDetailsExist.FurtherDiscount + FurtherDiscount;
                    studentFeeDetailsExist.DiscountComments = DiscountComments;
                    studentFeeDetailsExist.Notice = ImportantNotice1;
                    db.SaveChanges();

                    StudentFeeId = studentFeeDetailsExist.Id.ToString();
                    var id = User.Identity.GetUserId();
                    var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();

                    if (FurtherDiscount != 0)
                    {

                        Voucher voucher1 = new Voucher();
                        voucher1.Name = "Student Discount";
                        voucher1.Notes = "";
                        voucher1.Date = GetLocalDateTime.GetLocalDateTimeFunction();
                        voucher1.CreatedBy = username;
                        int? VoucherObj1 = db.Vouchers.Max(x => x.VoucherNo);
                        voucher1.VoucherNo = Convert.ToInt32(VoucherObj1) + 1;
                        db.Vouchers.Add(voucher1);
                        db.SaveChanges();


                        var DiscountAmount = FurtherDiscount;
                        VoucherRecord voucherRecord3 = new VoucherRecord();
                        var LeadgerDiscount = db.Ledgers.Where(x => x.Name == "Discount").FirstOrDefault();

                        decimal? CurrentBalanceOfDiscount = LeadgerDiscount.CurrentBalance;
                        decimal? AfterBalanceOfDiscount = CurrentBalanceOfDiscount + Convert.ToDecimal(FurtherDiscount);
                        voucherRecord3.LedgerId = LeadgerDiscount.Id;
                        voucherRecord3.Type = "Dr";
                        voucherRecord3.Amount = Convert.ToDecimal(FurtherDiscount);
                        voucherRecord3.CurrentBalance = CurrentBalanceOfDiscount;
                        voucherRecord3.AfterBalance = AfterBalanceOfDiscount;
                        voucherRecord3.VoucherId = voucher1.Id;
                        voucherRecord3.UserType = "Student";
                        voucherRecord3.UserId = StudentId.ToString();
                        voucherRecord3.BranchId = BranchId;
                        voucherRecord3.Description = "Discount given to student ";
                        LeadgerDiscount.CurrentBalance = AfterBalanceOfDiscount;
                        db.VoucherRecords.Add(voucherRecord3);
                        db.SaveChanges();


                        VoucherRecord voucherRecord4 = new VoucherRecord();
                        var LedgerAccountReceivable = db.Ledgers.Where(x => x.Name == "Account Receivable").FirstOrDefault();

                        decimal? CurrentBalanceOfAccountReceivable = LedgerAccountReceivable.CurrentBalance;
                        decimal? AfterBalanceOfAccountReceivable = CurrentBalanceOfAccountReceivable - Convert.ToDecimal(FurtherDiscount);
                        voucherRecord4.LedgerId = LedgerAccountReceivable.Id;
                        voucherRecord4.Type = "Cr";
                        voucherRecord4.Amount = Convert.ToDecimal(FurtherDiscount);
                        voucherRecord4.CurrentBalance = CurrentBalanceOfAccountReceivable;
                        voucherRecord4.AfterBalance = AfterBalanceOfAccountReceivable;
                        voucherRecord4.VoucherId = voucher1.Id;
                        voucherRecord4.UserType = "Student";
                        voucherRecord4.UserId = StudentId.ToString();
                        voucherRecord4.BranchId = BranchId;
                        voucherRecord4.Description = "";
                        LedgerAccountReceivable.CurrentBalance = AfterBalanceOfAccountReceivable;

                        db.VoucherRecords.Add(voucherRecord4);
                        db.SaveChanges();
                    }

                }

             //  TempData["StudentChallanFormCreated"] = "Created";
                dbTransaction.Commit();
                StatusMsg = "Created";
                return Json( new { StatusMsg = StatusMsg, StudentFeeId = StudentFeeId }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                dbTransaction.Dispose();
                StatusMsg = "Error";
                return Json(new { StatusMsg = StatusMsg, StudentFeeId = StudentFeeId }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetTotalFee(int MonthId, int StudentId)
        {

            StudentFeeDetail studentfeeDetails = new StudentFeeDetail();
            var DiscountComments = "";
            DateTime? duedate = null;
            var Msg = "";
            double? tutionFee = 0;
            double paidAmount = 0;
            double MonthMultiplier = 0;
            string ImportantNotice = "";
            var StudentFee = db.StudentFees.Where(x => x.StudentID == StudentId).FirstOrDefault();
            if (StudentFee != null)
            {

                double TotalWithoutAdminssion = StudentFee.TotalWithoutAdmission.Value;

                var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthId && x.StudentFeeID == StudentId).ToList();

                int TotalNonRecurring = 0;
                foreach (var nonRecurring in nonRecurringList)
                {
                    TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                }

                var multiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == StudentId).FirstOrDefault();
                MonthMultiplier = 0;
                if (MonthId == 1)
                    MonthMultiplier = multiplier.Jan_Multiplier.Value;
                else if (MonthId == 2)
                    MonthMultiplier = multiplier.Feb_Multiplier.Value;
                else if (MonthId == 3)
                    MonthMultiplier = multiplier.Mar_Multiplier.Value;
                else if (MonthId == 4)
                    MonthMultiplier = multiplier.April__Multiplier.Value;
                else if (MonthId == 5)
                    MonthMultiplier = multiplier.May_Multiplier.Value;
                else if (MonthId == 6)
                    MonthMultiplier = multiplier.June_Multiplier.Value;
                else if (MonthId == 7)
                    MonthMultiplier = multiplier.July__Multiplier.Value;
                else if (MonthId == 8)
                    MonthMultiplier = multiplier.Aug_Multiplier.Value;
                else if (MonthId == 9)
                    MonthMultiplier = multiplier.Sep_Multiplier.Value;
                else if (MonthId == 10)
                    MonthMultiplier = multiplier.Oct_Multiplier.Value;
                else if (MonthId == 11)
                    MonthMultiplier = multiplier.Nov_Multiplier.Value;
                else if (MonthId == 12)
                    MonthMultiplier = multiplier.Dec__Multiplier.Value;
                else { }

                paidAmount = (TotalWithoutAdminssion * MonthMultiplier) + TotalNonRecurring;

                var StudentChallanExist = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).ToList();

                if (StudentChallanExist.Count() == 0)
                {
                    paidAmount = paidAmount + (StudentFee.DiscountAdmissionFeeAmount.Value);
                }

                tutionFee = StudentFee.DiscountTutionFeeAmount * MonthMultiplier;

                var sfd = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId && x.Month == MonthId).FirstOrDefault();

                //   int? paidAmount1 =0;


                if (sfd != null)
                {
                    duedate = sfd.ChallanDueDate;
                    paidAmount = sfd.PaidAmount.Value;
                    tutionFee = tutionFee - sfd.FurtherDiscount;
                    DiscountComments = sfd.DiscountComments;
                    ImportantNotice = sfd.Notice;
                }

            }
            else
            {
                Msg = "Fee  is not created of selected Student";
            }


            return Json(new { paidAmount = paidAmount, MonthMultiplier = MonthMultiplier, tutionFee = tutionFee, Msg = Msg, duedate = duedate, DiscountComments = DiscountComments, ImportantNotice= ImportantNotice }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult checkStudentChallan(int StudentId, int MonthId)
        {
            var studentFeeDetails = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId && x.Month == MonthId).FirstOrDefault();

            var Msg = "";

            if (studentFeeDetails == null)
            {
                Msg = "";
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
            else if (studentFeeDetails.Status == "Paid")
            {
                Msg = "Selected student challan is paid";
                return Json(Msg, JsonRequestBehavior.AllowGet);

            }
            else
            {
                Msg = "Challan is already created of selected student and month";

                return Json(Msg, JsonRequestBehavior.AllowGet);

            }

        }

    }
}