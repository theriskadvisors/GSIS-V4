using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SEA_Application.Models;

namespace SEA_Application.Controllers
{
    public class StudentFeeMonthsController : Controller
    {
        private Sea_Entities db = new Sea_Entities();

        // GET: StudentFeeMonths
        public ActionResult Index()
        {
            // no forign key found
            var studentfeeDetails = db.StudentFeeDetails.Include(x => x.StudentFee.AspNetStudent);


            return View(studentfeeDetails.ToList());
        }

        // GET: StudentFeeMonths/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentFeeMonth studentFeeMonth = db.StudentFeeMonths.Find(id);
            if (studentFeeMonth == null)
            {
                return HttpNotFound();
            }
            return View(studentFeeMonth);
        }

        // GET: StudentFeeMonths/Create
        public ActionResult Create()
        {
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name");
            return View();
        }
        public ActionResult GetStudentDetailDDL(string Month, string Status)
        {
            if (Month == "All" && Status == null)
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }
            else if (Month != null && Status == null)
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  where fee.Months == Month
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }

            else if (Month == "All" && Status == "All")
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }
            else if (Month == "All" && Status != null)
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  where fee.Status == Status
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }
            else if (Month != null && Status == "All")
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  where fee.Months == Month
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }
            else if (Month != null && Status == "Printed")
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  where fee.Months == Month && fee.Status == "Printed"
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }
            else if (Month != null && Status != null)
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  where fee.Months == Month && fee.Status == Status
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var monthlyfee = (from fee in db.StudentFeeMonths
                                  join std in db.AspNetStudents on fee.StudentId equals std.Id
                                  where fee.Status == Status
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId }).ToList();
                return Json(monthlyfee, JsonRequestBehavior.AllowGet);
            }

        }
        public class StudentMonthlyFee
        {
            public int Id { get; set; }
            public string StudentId { get; set; }
            public string Date { get; set; }
            public string Name { get; set; }
            public string Month { get; set; }
            public string Status { get; set; }
            public double? MonthlyFee { get; set; }
            public double? PayableFee { get; set; }
            public double? Multiplier { get; set; }

        }
        public ActionResult GetStatusRecord(string StatusRecord)
        {
            var dbTransaction = db.Database.BeginTransaction();
            string status = "error";
            if (StatusRecord != "")
            {
                var RecordId = StatusRecord.Split(',');
                for (int i = 0; i < RecordId.Count(); i++)
                {
                    int? Arrear = 0;
                    var sfmid = Int32.Parse(RecordId[i]);
                    StudentFeeMonth data = db.StudentFeeMonths.Where(x => x.Id == sfmid).FirstOrDefault();
                    if (data.Status == "Printed")
                    {
                        data.Status = "Clear";
                        //}
                        ///////////////arrears////////////
                        //var _SessionId = data.SessionId;
                        //int _Id = data.Id - 1;
                        //bool Flag = true;

                        //while (Flag)
                        //{
                        //    try
                        //    {
                        //        double? payableamount = db.StudentFeeMonths.Where(x => x.StudentId == data.StudentId && x.SessionId == _SessionId && x.Status == "Pending" && x.Id == _Id).FirstOrDefault().FeePayable;

                        //        Arrear += payableamount;
                        //        _Id--;
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Flag = false;
                        //    }
                        //}
                        var totalreceivable = Arrear + data.FeePayable;
                        ///////////End/////////
                        try
                        {
                            Ledger ledger_Assets = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                            ledger_Assets.CurrentBalance -= Convert.ToDecimal(totalreceivable);
                            db.SaveChanges();

                            Ledger ledger_bank = db.Ledgers.Where(x => x.Name == "Meezan Bank" && x.LedgerGroup.Name == "Bank").FirstOrDefault();
                            ledger_bank.CurrentBalance += Convert.ToDecimal(totalreceivable);
                            db.SaveChanges();
                            status = "success";
                        }
                        catch
                        {
                            TempData["ErrorMessage"] = "No Cash in Student Receivables Account.";
                            return RedirectToAction("Edit", data);
                            //   return RedirectToAction("Edit?="+studentFeeMonth.StudentId);

                        }
                    }
                    else if (data.Status == "Clear")
                    {
                        data.Status = "Printed";
                        var totalreceivable = Arrear + data.FeePayable;

                        //try
                        //{
                        //    Ledger ledger_Assets = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                        //    ledger_Assets.CurrentBalance += Convert.ToDecimal(totalreceivable);
                        //    db.SaveChanges();

                        //    Ledger ledger_bank = db.Ledgers.Where(x => x.Name == "Meezan Bank" && x.LedgerGroup.Name == "Bank").FirstOrDefault();
                        //    ledger_bank.CurrentBalance -= Convert.ToDecimal(totalreceivable);
                        //    db.SaveChanges();
                        //    status = "success";
                        //}
                        //catch
                        //{
                        //    TempData["ErrorMessage"] = "No Cash in Meezan Bank Account.";
                        //    return RedirectToAction("Edit", data);
                        //    //   return RedirectToAction("Edit?="+studentFeeMonth.StudentId);

                        //}

                    }

                    if (db.SaveChanges() > 0)
                    {
                        status = "success";
                    }

                }
            }
            dbTransaction.Commit();
            return Content(status);

        }

        public ActionResult ChangeChallanStatus(string idlist, int LedgerId, string PaidDate)

        {
            var StudentFeeDetailIds = idlist.Split(',');
            List<challanform> listChallanForm = new List<challanform>();
            foreach (var listitem in StudentFeeDetailIds)
            {
                int item = Convert.ToInt32(listitem);

                StudentFeeDetail studentFeeDetailToUpdate = db.StudentFeeDetails.Where(x => x.Id == item).FirstOrDefault();


                if (studentFeeDetailToUpdate.Status == "Paid")
                {
                }
                else
                {
                    studentFeeDetailToUpdate.PaidDate = Convert.ToDateTime(PaidDate);
                    studentFeeDetailToUpdate.Status = "Paid";
                    db.SaveChanges();

                    int Month = studentFeeDetailToUpdate.Month.Value;

                    StudentFee studentFeeToUpdate = db.StudentFees.Where(x => x.Id == studentFeeDetailToUpdate.StudentFeeID).FirstOrDefault();

                    StudentFeeMultiplier studentFeeMultiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == studentFeeToUpdate.StudentID).FirstOrDefault();

                    if (Month == 1)
                    {
                        studentFeeMultiplier.Jan_StatusPaid = true;
                        studentFeeMultiplier.Jan_PaidAmount = studentFeeDetailToUpdate.PaidAmount;
                    }
                    else if (Month == 2)
                    {
                        studentFeeMultiplier.Feb_StatusPaid = true;
                        studentFeeMultiplier.Feb_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 3)
                    {
                        studentFeeMultiplier.Mar_StatusPaid = true;
                        studentFeeMultiplier.Mar_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 4)
                    {
                        studentFeeMultiplier.April_StatusPaid = true;
                        studentFeeMultiplier.Apr_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 5)
                    {
                        studentFeeMultiplier.May_StatusPaid = true;
                        studentFeeMultiplier.May_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 6)
                    {
                        studentFeeMultiplier.June_StatusPaid = true;
                        studentFeeMultiplier.Jun_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 7)
                    {
                        studentFeeMultiplier.July_StatusPaid = true;
                        studentFeeMultiplier.Jul_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 8)
                    {
                        studentFeeMultiplier.Aug_StatusPaid = true;
                        studentFeeMultiplier.Aug_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 9)
                    {
                        studentFeeMultiplier.Sep_StatusPaid = true;
                        studentFeeMultiplier.Sep_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 10)
                    {
                        studentFeeMultiplier.Oct_StatusPaid = true;
                        studentFeeMultiplier.Oct_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 11)
                    {
                        studentFeeMultiplier.Nov_StatusPaid = true;
                        studentFeeMultiplier.Nov_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else if (Month == 12)
                    {
                        studentFeeMultiplier.Dec_StatusPaid = true;
                        studentFeeMultiplier.Dec_PaidAmount = studentFeeDetailToUpdate.PaidAmount;

                    }
                    else { }

                    db.SaveChanges();


                    var paidAmount = studentFeeDetailToUpdate.PaidAmount;
                    var id = User.Identity.GetUserId();
                    var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
                    Voucher voucher = new Voucher();
                    var SessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;
                    voucher.Name = "Student Fee Paid";
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
                    decimal? AfterBalance = CurrentBalance - Convert.ToDecimal(paidAmount);
                    voucherRecord.LedgerId = AccountReceivableId;
                    voucherRecord.Type = "Cr";
                    voucherRecord.Amount = Convert.ToDecimal(paidAmount);
                    voucherRecord.CurrentBalance = CurrentBalance;
                    voucherRecord.AfterBalance = AfterBalance;
                    voucherRecord.VoucherId = voucher.Id;
                    voucherRecord.UserType = "Student";
                    voucherRecord.UserId = studentFeeDetailToUpdate.StudentFee.AspNetStudent.Id.ToString();
                    voucherRecord.BranchId = studentFeeDetailToUpdate.StudentFee.AspNetStudent.BranchId;
                    voucherRecord.Description = "Fee added of student";
                    Leadger.CurrentBalance = AfterBalance;
                    db.VoucherRecords.Add(voucherRecord);
                    db.SaveChanges();


                    VoucherRecord voucherRecord1 = new VoucherRecord();
                    var LedgerCash = db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault();
                    decimal? CurrentBalanceOfCash = LedgerCash.CurrentBalance;
                    decimal? AfterBalanceOfCash = CurrentBalanceOfCash + Convert.ToDecimal(paidAmount);
                    voucherRecord1.LedgerId = LedgerCash.Id;
                    voucherRecord1.Type = "Dr";
                    voucherRecord1.Amount = Convert.ToDecimal(paidAmount);
                    voucherRecord1.CurrentBalance = CurrentBalanceOfCash;
                    voucherRecord1.AfterBalance = AfterBalanceOfCash;
                    voucherRecord1.VoucherId = voucher.Id;
                    voucherRecord1.Description = "";
                    voucherRecord1.UserType = null;
                    voucherRecord1.UserId = null;
                    voucherRecord1.BranchId = studentFeeDetailToUpdate.StudentFee.AspNetStudent.BranchId;
                    LedgerCash.CurrentBalance = AfterBalanceOfCash;

                    db.VoucherRecords.Add(voucherRecord1);
                    db.SaveChanges();

                }
            }
            return RedirectToAction("Index", "StudentFeeMonths");

        }

        public ActionResult ChallanForm()
        {
            return View();
        }


        public class FeeTypes
        {
            public double? Amount { get; set; }
            public string Name { get; set; }
        }
        public ActionResult AddChallanDate(FeeDateSetting FDS)
        {
            string status = "error";
            if (FDS.ValidityDate != null && FDS.DueDate != null)
            {
                if (db.FeeDateSettings.ToList().Count > 0)
                {
                    FeeDateSetting settings = db.FeeDateSettings.Where(x => x.Id == 1).FirstOrDefault();
                    settings.DueDate = FDS.DueDate;
                    settings.ValidityDate = FDS.ValidityDate;

                }
                else
                {
                    db.FeeDateSettings.Add(FDS);
                }
                if (db.SaveChanges() > 0)
                {
                    status = "success";

                }
            }

            return Content(status);
        }
        public ActionResult ChallanFormView(string idlist)
        {

            ViewBag.list = idlist;
            return View();
        }


        public JsonResult GetChallanForm(string idlist)
        {
            var StudentFeeDetailIds = idlist.Split(',');
            List<challanform> listChallanForm = new List<challanform>();


            foreach (var listitem in StudentFeeDetailIds)
            {
                int? countTotalMultipliers = 0;
                int item = Convert.ToInt32(listitem);
                StudentFeeDetail studentFeeDetail = db.StudentFeeDetails.Where(x => x.Id == item).FirstOrDefault();
                int? StudentFeeId = db.StudentFeeDetails.Where(x => x.Id == item).FirstOrDefault().StudentFeeID;
                StudentFee studentFee = db.StudentFees.Where(x => x.Id == StudentFeeId).FirstOrDefault();
                int? StudentId = studentFee.StudentID;
                AspNetStudent Student = db.AspNetStudents.Where(x => x.Id == StudentId).FirstOrDefault();

                challanform challan = new challanform();
                List<NonRecurringFee> NonRecurringFeeList = new List<NonRecurringFee>();

                int Month1 = studentFeeDetail.Month.Value;
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
                { }

                if (multiplier.Jan_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;
                }

                if (multiplier.Feb_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.Mar_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.April__Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.May_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.June_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.July__Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.Aug_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.Sep_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.Oct_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.Nov_Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                if (multiplier.Dec__Multiplier.Value != 0)
                {
                    countTotalMultipliers = countTotalMultipliers + 1;

                }
                else
                {

                }
                int ChallanFormNumber = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).Count();

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Month1);
                double? TutionFeeDiscounted = ((studentFee.DiscountTutionFeeAmount * MonthMultiplier) - (studentFeeDetail.FurtherDiscount));

                double? ComputerFee = studentFee.DiscountComputerFeeAmount * MonthMultiplier;
                double? LabCharges = studentFee.DiscountLabChargesAmount * MonthMultiplier;
                double? OtherServices = studentFee.DiscountOtherServicesAmount * MonthMultiplier;

                challan.TutionFee = Convert.ToInt32(studentFee.TutionFee * MonthMultiplier);

                challan.Computer_Fee = Convert.ToInt32(ComputerFee);
                challan.Lab_Charges = Convert.ToInt32(LabCharges);
                challan.Others = Convert.ToInt32(OtherServices);
                challan.DiscountTutionFee = Convert.ToInt32(TutionFeeDiscounted); //+ (studentFeeDetail.FurtherDiscount.Value)* MonthMultiplier;

                if (MonthMultiplier == 0)
                {
                    challan.MonthlyTutionFee = 0;
                    challan.MonthlyTutionFeeDiscounted = 0;
                    challan.MonthlyComputerFee = 0;
                    challan.MonthlyLabCharges = 0;
                    challan.MonthlyOthers = 0;
                }
                else
                {
                    challan.MonthlyTutionFee = Convert.ToInt32(challan.TutionFee / MonthMultiplier);
                    challan.MonthlyTutionFeeDiscounted = Convert.ToInt32(TutionFeeDiscounted / MonthMultiplier);
                    challan.MonthlyComputerFee = Convert.ToInt32(ComputerFee / MonthMultiplier);
                    challan.MonthlyLabCharges = Convert.ToInt32(LabCharges / MonthMultiplier);
                    challan.MonthlyOthers = Convert.ToInt32(OtherServices / MonthMultiplier);
                }


                challan.TotalMonthlyFee = challan.MonthlyTutionFeeDiscounted + challan.MonthlyComputerFee + challan.MonthlyLabCharges + challan.MonthlyOthers;

                challan.SchoolName = "NGS-IPC PRESCHOOL";
                challan.BranchName = "Canal Branch";
                challan.StudentName = Student.Name;
                challan.StudentUserName = Student.AspNetUser.UserName;
                challan.StudentClass = Student.AspNetClass.Name;
                challan.DueDate = studentFeeDetail.ChallanDueDate.ToString();
                challan.IssueDate = studentFeeDetail.ChallanIssueDate.ToString();
                challan.ImportantNotice = studentFeeDetail.Notice;
                //   challan.FeeMonth = MonthMultiplier.ToString();
                challan.InvoiceNumber = "Invoice no." + studentFeeDetail.InvoiceNo + " of " + countTotalMultipliers;

                Class_Session classSession = db.Class_Session.Where(x => x.ClassId == Student.AspNetClass.Id).FirstOrDefault();


                if (classSession == null)
                {
                    challan.ClassSession = "";
                }
                else
                {
                    challan.ClassSession = classSession.ClassSessionName;

                }

                challan.FeeMonth = monthName;
                challan.BillingMonth = monthName + " " + studentFeeDetail.ChallanDueDate.Value.Year.ToString();


                challan.Branch = Student.AspNetBranch.Name;
                challan.TotalRecurring = Convert.ToInt32(TutionFeeDiscounted) + challan.Computer_Fee + challan.Lab_Charges + challan.Others;

                challan.TotalAmount = Convert.ToInt32(studentFeeDetail.PaidAmount);
                var PaidAmount = challan.TotalAmount;

                var StudentChallanExist = db.StudentFeeDetails.Where(x => x.StudentFee.AspNetStudent.Id == StudentId).ToList();

                //if (StudentChallanExist.Count() == 0)
                //{
                //    challan.TotalAmount = PaidAmount + (studentFee.DiscountAdmissionFeeAmount.Value);
                //}

                var YearNumber = 0;

                var ChallanIssueDate = studentFeeDetail.ChallanIssueDate;
                var Month = ChallanIssueDate?.ToString("MM");
                var Year = ChallanIssueDate?.ToString("yy");

                var monthint = Convert.ToInt32(Month);

                if (monthint == 1 || monthint == 2 || monthint == 3 || monthint == 4 || monthint == 5 || monthint == 6)
                {
                    YearNumber = Convert.ToInt32(Year);

                    challan.JanMonth = "Jan-" + YearNumber;
                    challan.FebMonth = "Feb-" + YearNumber;
                    challan.MarMonth = "Mar-" + YearNumber;
                    challan.AprMonth = "Apr-" + YearNumber;
                    challan.MayMonth = "May-" + YearNumber;
                    challan.JunMonth = "jun-" + YearNumber;

                    YearNumber = YearNumber - 1;

                    challan.JulMonth = "Jul-" + YearNumber;
                    challan.AugMonth = "Aug-" + YearNumber;
                    challan.SepMonth = "Sep-" + YearNumber;
                    challan.OctMonth = "Oct-" + YearNumber;
                    challan.NovMonth = "Nov-" + YearNumber;
                    challan.DecMonth = "Dec-" + YearNumber;
                }
                else
                {
                    YearNumber = Convert.ToInt32(Year);

                    challan.JulMonth = "Jul-" + YearNumber;
                    challan.AugMonth = "Aug-" + YearNumber;
                    challan.SepMonth = "Sep-" + YearNumber;
                    challan.OctMonth = "Oct-" + YearNumber;
                    challan.NovMonth = "Nov-" + YearNumber;
                    challan.DecMonth = "Dec-" + YearNumber;

                    YearNumber = YearNumber + 1;

                    challan.JanMonth = "Jan-" + YearNumber;
                    challan.FebMonth = "Feb-" + YearNumber;
                    challan.MarMonth = "Mar-" + YearNumber;
                    challan.AprMonth = "Apr-" + YearNumber;
                    challan.MayMonth = "May-" + YearNumber;
                    challan.JunMonth = "jun-" + YearNumber;

                }

                challan.Jan_PaidAmount = Convert.ToInt32(multiplier.Jan_PaidAmount);
                challan.Feb_PaidAmount = Convert.ToInt32(multiplier.Feb_PaidAmount);
                challan.Mar_PaidAmount = Convert.ToInt32(multiplier.Mar_PaidAmount);
                challan.Apr_PaidAmount = Convert.ToInt32(multiplier.Apr_PaidAmount);
                challan.May_PaidAmount = Convert.ToInt32(multiplier.May_PaidAmount);
                challan.Jun_PaidAmount = Convert.ToInt32(multiplier.Jun_PaidAmount);
                challan.Jul_PaidAmount = Convert.ToInt32(multiplier.Jul_PaidAmount);
                challan.Aug_PaidAmount = Convert.ToInt32(multiplier.Aug_PaidAmount);
                challan.Sep_PaidAmount = Convert.ToInt32(multiplier.Sep_PaidAmount);
                challan.Oct_PaidAmount = Convert.ToInt32(multiplier.Oct_PaidAmount);
                challan.Nov_PaidAmount = Convert.ToInt32(multiplier.Nov_PaidAmount);
                challan.Dec_PaidAmount = Convert.ToInt32(multiplier.Dec_PaidAmount);

                var TotalNonRecurringFee = 0;

                List<StudentNonRecurringFee> listStudentNonRecurringFee = db.StudentNonRecurringFees.Where(x => x.StudentFeeID == StudentId && x.Month == studentFeeDetail.Month).ToList();

                if (listStudentNonRecurringFee.Count != 0)
                {

                    foreach (var nonRecurring in listStudentNonRecurringFee)
                    {
                        var type = nonRecurring.NonRecurringType.ExpenseType;

                        if (type == "Stationery Fund")
                        {
                            challan.Annual_Stationary_Fund = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }
                        else if (type == "Annual Fund")
                        {
                            challan.Annual_Fund = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }
                        else if (type == "Admission Fee")
                        {
                            challan.AdmissionFee_NonRef = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                            //   challan.AdmissionFee_NonRef = 0;
                            // TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(0);

                        }
                        else if (type == "Security (Ref)")
                        {
                            challan.Security_Ref = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);


                        }
                        else if (type == "Registration")
                        {
                            challan.Registration_Charges = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);


                        }
                        else if (type == "SLC Charges")
                        {
                            challan.SLC_Charges = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);

                        }
                        else if (type == "Arrears")
                        {
                            challan.Arrears = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);

                        }
                        else if (type == "Exam Charges")
                        {
                            challan.Exam_Charges = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);


                        }
                        //else if (type == "Books and Stationary")
                        //{
                        //    challan.Books_Stationary = nonRecurring.Amount.Value;
                        //    TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);


                        //}
                        //else if (type == "Sports & Recreation")
                        //{
                        //    challan.Sports_Recreation = nonRecurring.Amount.Value;
                        //    TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        //}

                        else if (type == "Advance Tax")
                        {
                            challan.Advance_Tax = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }
                        else if (type == "Adjustment")
                        {
                            challan.Adjustment = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }
                        else if (type == "Non-Payment Fine")
                        {
                            challan.NonPaymentFine = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }
                        else if (type == "Waiver (If Any)/ VLEP Sofware charges")
                        {
                            challan.Waiver = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }
                        else if (type == "Deffered (If Any)")
                        {
                            challan.Deffered = nonRecurring.Amount.Value;
                            TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(nonRecurring.Amount);
                        }

                        else
                        {
                        }
                    }
                }

                //if (StudentChallanExist.Count() == 1)
                //{
                //    // challan.TotalAmount = PaidAmount + (studentFee.DiscountAdmissionFeeAmount.Value);
                //    challan.AdmissionFee_NonRef = Convert.ToInt32(studentFee.DiscountAdmissionFeeAmount.Value);
                //    TotalNonRecurringFee = TotalNonRecurringFee + Convert.ToInt32(studentFee.DiscountAdmissionFeeAmount.Value);
                //}

                challan.Rec_NonRec_Total = TotalNonRecurringFee;

                listChallanForm.Add(challan);
            }

            return Json(listChallanForm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatusChallan(string idlist)
        {
            var StudentFeeDetailIds = idlist.Split(',');

            foreach (var listitem in StudentFeeDetailIds)
            {
                int item = Convert.ToInt32(listitem);

                StudentFeeDetail studentFeeDetailToUpdate = db.StudentFeeDetails.Where(x => x.Id == item).FirstOrDefault();

                if (studentFeeDetailToUpdate.Status == "Pending")
                {
                    studentFeeDetailToUpdate.Status = "Printed";
                    db.SaveChanges();
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllBankCashLedgers()
        {

            var AllBankCashLedgers = db.Ledgers.Where(x => x.LedgerGroup.Name == "Bank" || x.LedgerGroup.Name == "Cash").Select(x=> new { x.Id, x.Name, x.LedgerGroupId}).ToList();

            return Json(AllBankCashLedgers, JsonRequestBehavior.AllowGet);
        }

        public class challanform
        {
            public string InvoiceNumber { get; set; }
            public string SchoolName { get; set; }
            public string BranchName { get; set; }
            public List<string> ChallanCopy { get; set; }
            public DateTime? AcademicSessionStart { get; set; }
            public DateTime? AcademicSessionEnd { get; set; }
            public decimal? ChallanID { get; set; }
            public string UserID { get; set; }
            public string StudentName { get; set; }
            public string StudentUserName { get; set; }
            public string StudentClass { get; set; }
            public List<String> DiscountNotes { get; set; }
            public string DueDate { get; set; }
            public string IssueDate { get; set; }
            public List<String> Notes { get; set; }
            public DateTime PrintedDate { get; set; }
            public double? PayableFee { get; set; }
            public double? TotalAmount { get; set; }
            public double? Penalty { get; set; }
            public string ValidDate { get; set; }
            public int Arrears { get; set; }
            public string FeeMonth { get; set; }
            public string BillingMonth { get; set; }
            public double? TripCharges { get; set; }

            public string ClassSession { get; set; }
            public string JanMonth { get; set; }
            public string FebMonth { get; set; }
            public string MarMonth { get; set; }
            public string AprMonth { get; set; }
            public string MayMonth { get; set; }

            public string Branch { get; set; }
            public string JunMonth { get; set; }
            public string JulMonth { get; set; }
            public string AugMonth { get; set; }
            public string SepMonth { get; set; }
            public string OctMonth { get; set; }
            public string NovMonth { get; set; }
            public string DecMonth { get; set; }

            public string ImportantNotice { get; set; }
            //  public List<NonRecurringFee> NonRecurringFeeList { get; set; }

            //non recuring
            public int AdmissionFee_NonRef = 0;
            public int Security_Ref = 0;
            public int Annual_Fund = 0;
            public int Annual_Stationary_Fund = 0;
            public int Exam_Charges = 0;
            public int Registration_Charges = 0;
            public int Books_Stationary = 0;
            public int ICT_ICT_Library_Resources = 0;
            public int Sports_Recreation = 0;
            public int SLC_Charges = 0;

            public int Advance_Tax = 0;

            public int Adjustment = 0;

            public int NonPaymentFine = 0;

            public int Waiver = 0;

            public int Deffered = 0;


            //recurring
            public int Lab_Charges = 0;
            public int DiscountTutionFee = 0;
            public int Computer_Fee = 0;
            public int TutionFee = 0;
            public int Others = 0;
            public int TotalRecurring = 0;
            public int Rec_NonRec_Total = 0;


            public int MonthlyTutionFee = 0;
            public int MonthlyTutionFeeDiscounted = 0;
            public int MonthlyComputerFee = 0;
            public int MonthlyLabCharges = 0;
            public int MonthlyOthers = 0;
            public int TotalMonthlyFee = 0;

            public int Jan_PaidAmount = 0;
            public int Feb_PaidAmount = 0;
            public int Mar_PaidAmount = 0;
            public int Apr_PaidAmount = 0;
            public int May_PaidAmount = 0;
            public int Jun_PaidAmount = 0;
            public int Jul_PaidAmount = 0;
            public int Aug_PaidAmount = 0;
            public int Sep_PaidAmount = 0;
            public int Oct_PaidAmount = 0;
            public int Nov_PaidAmount = 0;
            public int Dec_PaidAmount = 0;
        }

        public class NonRecurringFee
        {
            public string Type { get; set; }
            public int? Amount { get; set; }

        }

        //POST: StudentFeeMonths/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StudentId,Months,Status,InstalmentAmount,Date")] StudentFeeMonth studentFeeMonth)
        {
            try
            {
                var checkbox = Request.Form["ingredients"];
                List<string> names = new List<string>(checkbox.Split(','));
                if (ModelState.IsValid)
                {
                    foreach (var item in names)
                    {
                        studentFeeMonth.Months = item;
                        db.StudentFeeMonths.Add(studentFeeMonth);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                return View(studentFeeMonth);
            }
            catch
            {
                return RedirectToAction("Create");

            }

        }
        //public ActionResult GetStudentInstalment(int stdid)
        //{
        ////   var amount= db.StudentFeeMultipliers.Where(x => x.StudentId == stdid).Select(x => x.SharePerInstalment).FirstOrDefault();
        //  //  return Json(amount,JsonRequestBehavior.AllowGet);
        //}
        // GET: StudentFeeMonths/Edit/5
        public ActionResult Edit(int? id)
        {
            //  ViewBag.Error = TempData["ErrorMessage"] as string;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentFeeMonth studentFeeMonth = db.StudentFeeMonths.Find(id);
            if (studentFeeMonth == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
            return View(studentFeeMonth);
        }

        // POST: StudentFeeMonths/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StudentId,Months,Status,InstalmentAmount,IssueDate,Date")] StudentFeeMonth studentFeeMonth)
        {

            var checkbox1 = Request.Form["counter"];
            bool Flag = true;
            double? installmentamount = studentFeeMonth.InstalmentAmount;
            float multiplier = float.Parse(Request.Form["multiplier"]);
            studentFeeMonth.Multiplier = Math.Round(multiplier, 2, MidpointRounding.AwayFromZero);

            double decimalvalues = multiplier - Math.Truncate(multiplier);
            double roundvalues = Math.Round(decimalvalues, 2, MidpointRounding.AwayFromZero);
            var total = float.Parse(Request.Form["total"]);
            studentFeeMonth.FeePayable = total;
            var round = Math.Ceiling(multiplier);
            var checkbox = Request.Form["counter"];
            if (checkbox == null)
            {
                ViewBag.Error = "Please Select Months.";
                ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                return View(studentFeeMonth);
            }
            List<string> names = new List<string>(checkbox.Split(','));
            int months = names.Count();

            if (!names.Contains(studentFeeMonth.Months))
            {
                if (round == months + 1)
                {
                    if (roundvalues == 0)  // for int multiplier
                    {
                        double? value = (total - installmentamount) / months;

                        for (var i = 0; i < names.Count; i++)
                        {
                            string monthname = names[i];
                            StudentFeeMonth student = db.StudentFeeMonths.Where(x => x.Months == monthname && x.StudentId == studentFeeMonth.StudentId).FirstOrDefault();
                            student.FeePayable = student.FeePayable - value;
                            if (student.FeePayable >= 0)
                            {
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewBag.Error = "Invalid Transaction.";
                                ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                                return View(studentFeeMonth);
                            }
                        }
                    }
                    else
                    {   //for float multiplier

                        var round2 = Math.Floor(multiplier);
                        var evenprice = installmentamount * round2;
                        double? value2 = (evenprice) / months;
                        string Lastmonth = names.Last();

                        for (var i = 0; i < names.Count - 1; i++)
                        {
                            string monthname = names[i];
                            StudentFeeMonth student = db.StudentFeeMonths.Where(x => x.Months == monthname && x.StudentId == studentFeeMonth.StudentId).FirstOrDefault();
                            student.FeePayable = student.FeePayable - value2;
                            if (student.FeePayable >= 0)
                            {
                                db.SaveChanges();
                            }

                            else
                            {
                                ViewBag.Error = "Invalid Transaction.";
                                ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                                return View(studentFeeMonth);
                            }
                        }
                        //last month
                        if (Lastmonth != null)
                        {
                            var oddprice = total - evenprice;
                            StudentFeeMonth studentdata = db.StudentFeeMonths.Where(x => x.Months == Lastmonth && x.StudentId == studentFeeMonth.StudentId).FirstOrDefault();
                            studentdata.FeePayable = studentdata.FeePayable - oddprice;
                            if (studentdata.FeePayable >= 0)
                            {
                                db.SaveChanges();
                            }
                            else
                            {
                                ViewBag.Error = "Invalid Transaction.";
                                ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                                return View(studentFeeMonth);
                            }
                        }

                    }

                }
                else
                {
                    Flag = false;
                    // months extends or decends
                    ViewBag.Error = "Months Exceeds.";
                    ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                    return View(studentFeeMonth);

                }
            }
            else
            {
                Flag = false;
                //invalid month selected
                ViewBag.Error = "Invalid Month Selected.";
                ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
                return View(studentFeeMonth);
            }

            //if (checkbox == null)
            //{
            //    ViewBag.ErrorMessage = "No Month Selected, Please Select Month";
            //}
            if (ModelState.IsValid && Flag == true)
            {
                studentFeeMonth.Status = "Pending";
                studentFeeMonth.DueDate = studentFeeMonth.DueDate;
                studentFeeMonth.IssueDate = studentFeeMonth.IssueDate;
                db.Entry(studentFeeMonth).State = EntityState.Modified;
                db.SaveChanges();


                Month_Multiplier Multi = new Month_Multiplier();
                for (var i = 0; i < names.Count; i++)
                {
                    string monthname = names[i];
                    Multi.Month = monthname;
                    Multi.StudentId = studentFeeMonth.StudentId;
                    //    student.FeePayable = student.InstalmentAmount - value;
                    db.Month_Multiplier.Add(Multi);
                    db.SaveChanges();

                }

                //if(studentFeeMonth.Status=="Clear")
                //{
                //    StudentFeeMultiplier stdfee = db.StudentFeeMultipliers.Where(x => x.StudentId == studentFeeMonth.StudentId).Select(x => x).FirstOrDefault();
                //    stdfee.PaidInstalments += 1;
                //    stdfee.PaidAmount += studentFeeMonth.InstalmentAmount;
                //    stdfee.RemainingInstalments -= 1;
                //    stdfee.RemainingAmount -= studentFeeMonth.InstalmentAmount;
                //    db.SaveChanges();

                //    Ledger ledger = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                //    ledger.CurrentBalance -= Convert.ToDecimal(studentFeeMonth.InstalmentAmount);
                //    db.SaveChanges();                    
                //}


                return RedirectToAction("Index");
            }
            ViewBag.StudentId = new SelectList(db.AspNetStudents, "Id", "Name", studentFeeMonth.StudentId);
            return View(studentFeeMonth);
        }

        // GET: StudentFeeMonths/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentFeeMonth studentFeeMonth = db.StudentFeeMonths.Find(id);
            if (studentFeeMonth == null)
            {
                return HttpNotFound();
            }
            return View(studentFeeMonth);
        }

        // POST: StudentFeeMonths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentFeeMonth studentFeeMonth = db.StudentFeeMonths.Find(id);
            db.StudentFeeMonths.Remove(studentFeeMonth);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
