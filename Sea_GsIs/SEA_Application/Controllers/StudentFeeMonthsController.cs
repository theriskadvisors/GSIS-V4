using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            var studentFeeMonths = db.StudentFeeMonths.Include(s => s.AspNetStudent);
            return View(studentFeeMonths.ToList());
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
                                  select new { fee.Id, fee.IssueDate, std.Name, fee.Months, fee.Status, fee.FeePayable, fee.InstalmentAmount, fee.Multiplier, fee.StudentId}).ToList();
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



        public ActionResult ChallanForm()
        {
            return View();
        }
        public class challanform
        {
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
            public List<String> Notes { get; set; }
            public DateTime PrintedDate { get; set; }
            public double? PayableFee { get; set; }
            public double? TotalAmount { get; set; }
            public double? Penalty { get; set; }
            public string ValidDate { get; set; }
            public int? Arrears { get; set; } 
            public string FeeMonth { get; set; }
            public double? TripCharges { get; set; }
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
            //recurring
            public int Lab_Charges = 0;
            public int Computer_Fee = 0;
            public int TutionFee = 0;
            public int Others = 0;
            public int TotalRecurring = 0;
            public int Rec_NonRec_Total = 0;
           
           
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
        public ActionResult ChallanFormView( string Month, string idlist)
        {
            ViewBag.Month = Month.ToString();
            ViewBag.list = idlist;
            return View();
        }
        public JsonResult GetChallanForm(string month, string idlist)
        {

            var userid = idlist.Split(',');
            int ClassId ;
        //    var userid = idlist;
            var Nextmonth = "";
            var dates = db.FeeDateSettings.FirstOrDefault();
            List<challanform> ChallanList = new List<challanform>();
            try
            {
                foreach (var item in userid)
                {
                    var Feemonthid = Int32.Parse(item);
                    var intid =  db.StudentFeeMonths.Where(x => x.Id == Feemonthid).FirstOrDefault().StudentId;
                    var student = db.AspNetStudents.Where(x => x.Id == intid).FirstOrDefault();
                    StudentFeeMonth Student_FeeMonth = db.StudentFeeMonths.Where(x => x.StudentId == student.Id && x.Months == month).FirstOrDefault();
                    Student_FeeMonth.ValidityDate = dates.ValidityDate;
                    Student_FeeMonth.DueDate = dates.DueDate;
                    Student_FeeMonth.Status = "Printed";
                    ClassId = Student_FeeMonth.AspNetStudent.AspNetClass.Id;
                    db.SaveChanges();

                    int recordid = Student_FeeMonth.Id;
                    recordid++;
                    var Studentdata = db.StudentFeeMonths.Where(x => x.Id == recordid).FirstOrDefault();
                    if (Studentdata.Multiplier == 0)
                    {
                        Nextmonth = month + '-' + Studentdata.Months;

                    }
                    double? trip = 0;
                 
                    AspNetSession Session = db.AspNetSessions.OrderByDescending(x => x.Id).Select(x => x).FirstOrDefault();
                    decimal? No = 0;
                    var Student_Penalty = db.StudentPenalties.Where(x => x.StudentId == student.Id && x.Status == "Pending").Select(x => x.PenaltyFee.Amount).FirstOrDefault();

                    if (Student_Penalty == null)
                    {

                        Student_Penalty = 0;
                    }
                    var Student_discount = db.StudentDiscounts.Where(x => x.StudentId == student.Id).Select(x => x.FeeDiscount.Amount).FirstOrDefault();

                    if (Student_discount == null)
                    {
                        Student_discount = 0;
                    }
                    ///////////////arrears////////////
                    var _SessionId = Student_FeeMonth.SessionId;
                    int _Id = Student_FeeMonth.Id - 1;
                    bool Flag = true;
                    double? Arrear = 0;
                    while (Flag)
                    {
                        try
                        {
                            double? payableamount = db.StudentFeeMonths.Where(x => x.StudentId == student.Id  && x.Id == _Id   && (x.Status == "Pending" || x.Status == "Printed") ).FirstOrDefault().FeePayable;

                            Arrear += payableamount;
                            _Id--;
                        }
                        catch (Exception ex)
                        {
                            Flag = false;
                        }
                    }
                    /////////End/////////

                    var nonrecurringfee = db.NonRecurringFeeMultipliers.Where(x => x.StudentId == student.Id && x.Month == Student_FeeMonth.Months).ToList();

                    challanform Challan = new challanform();
                    Challan.AcademicSessionStart = Session.StartDate;
                    Challan.AcademicSessionEnd = Session.EndDate;
                    Challan.StudentName = student.AspNetUser.Name;
                    Challan.StudentUserName = student.AspNetUser.UserName;
                    Challan.StudentClass = student.AspNetClass.Name;
                    Challan.SchoolName = "NGS-IPC PRESCHOOL";
                    Challan.BranchName = "Canal Branch";
                    ///////Non-RecurringFee/////////////
                    foreach (var itemo in nonrecurringfee)
                    {
                        string expensetype = db.NonRecurringCharges.Where(x=>x.Id == itemo.DescriptionId).FirstOrDefault().ExpenseType;
                        var totalfee =  itemo.TutionFee;
                        if (expensetype == "Books & Stationary")
                        {
                            Challan.Books_Stationary = Convert.ToInt32(totalfee);
                           
                        }
                        else if (expensetype == "SLC Charges")
                        {
                            Challan.SLC_Charges = Convert.ToInt32(totalfee);
                        }
                        else if (expensetype == "Sports & Recreation")
                        {
                            Challan.Sports_Recreation = Convert.ToInt32(totalfee);
                        }
                        else if (expensetype == "Annual Fund")
                        {
                            Challan.Annual_Fund = Convert.ToInt32(totalfee);
                        }
                        else if (expensetype == "Security (Ref)")
                        {
                            Challan.Security_Ref = Convert.ToInt32(totalfee);
                        }
                        else if (expensetype == "Admission Fee (Non-Ref)")
                        {
                            Challan.AdmissionFee_NonRef = Convert.ToInt32(totalfee);
                        }
                        else if (expensetype == "Exam Charges")
                        {
                            Challan.Exam_Charges =Convert.ToInt32(totalfee);
                        }
                      }
                 int nonrecurringcharges =  Challan.Books_Stationary + Challan.SLC_Charges + Challan.Sports_Recreation + Challan.Annual_Fund + Challan.Security_Ref + Challan.AdmissionFee_NonRef + Challan.Exam_Charges;
                       
                    //Challan.ChallanCopy = new List<string>();
                    //Challan.ChallanCopy.Add("Parent Copy");
                    //Challan.ChallanCopy.Add("Bank Copy");
                    //Challan.ChallanCopy.Add("School Copy");

                    //recurringfee
                  var recurringfee =   db.StudentRecurringFees.Where(x => x.ClassId == ClassId).FirstOrDefault();
                  Challan.Lab_Charges = Convert.ToInt32(recurringfee.LabCharges/12);
                  Challan.Others = Convert.ToInt32(recurringfee.Other/12);
                  Challan.Computer_Fee = Convert.ToInt32(recurringfee.ComputerFee/12);
                  Challan.TutionFee = Convert.ToInt32(recurringfee.TutionFee / 12);
                  Challan.TotalRecurring = Challan.Lab_Charges + Challan.TutionFee + Challan.Computer_Fee + Challan.Others;

                    if (Nextmonth == "")
                    {
                        Challan.FeeMonth = Student_FeeMonth.Months;
                    }
                    else
                    {
                        Challan.FeeMonth = Nextmonth;
                    }
                    var duedate = Student_FeeMonth.DueDate.ToString().Split(' ');
                    Challan.DueDate = duedate[0];
                    var validdate = Student_FeeMonth.ValidityDate.ToString().Split(' ');
                    Challan.ValidDate = validdate[0];
                    Challan.PayableFee = Student_FeeMonth.FeePayable - Convert.ToDouble(Student_discount);
                    Challan.Arrears =Convert.ToInt32(Arrear);
                    var totalpayable = Arrear + Convert.ToDouble(Student_Penalty) + Student_FeeMonth.FeePayable;
                    Challan.TripCharges = trip;
                    Challan.TotalAmount =  Convert.ToInt32(totalpayable + trip);



                    Challan.Rec_NonRec_Total = Challan.TotalRecurring + nonrecurringcharges + Convert.ToInt32(Challan.Arrears);

                    /////////////Challan No/////////////
                    var challan = db.StudentChallanForms.Where(x => x.StudentId == student.Id && x.StudentFeeMonthId == Student_FeeMonth.Id).FirstOrDefault();
                    if (challan == null)
                    {
                        try
                        {
                            No = (int)db.StudentChallanForms.Select(x => x.ChallanNo).Max();
                            No++;
                            StudentChallanForm stdchallan = new StudentChallanForm();
                            stdchallan.StudentId = student.Id;
                            stdchallan.ChallanNo = No;
                            stdchallan.StudentFeeMonthId = Student_FeeMonth.Id;
                            stdchallan.Status = "Created";
                            db.StudentChallanForms.Add(stdchallan);
                            db.SaveChanges();
                            ////////////// Accounting System///////////////
                            Ledger ledger_Assets = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                            ledger_Assets.CurrentBalance += Convert.ToDecimal(Challan.TotalAmount);
                            db.SaveChanges();

                            Ledger ledger_Income = db.Ledgers.Where(x => x.Name == "Student Fee").FirstOrDefault();
                            ledger_Income.CurrentBalance += Convert.ToDecimal(Challan.TotalAmount);
                            db.SaveChanges();

                        }
                        catch
                        {
                            No = 5050100;
                            StudentChallanForm stdchallan = new StudentChallanForm();
                            stdchallan.StudentId = student.Id;
                            stdchallan.ChallanNo = No;
                            stdchallan.StudentFeeMonthId = Student_FeeMonth.Id;
                            stdchallan.Status = "Created";
                            db.StudentChallanForms.Add(stdchallan);
                            db.SaveChanges();
                            /////////// Accounting System///////////
                            Ledger ledger_Assets = db.Ledgers.Where(x => x.Name == "Student Receivables").FirstOrDefault();
                            ledger_Assets.CurrentBalance += Convert.ToDecimal(Challan.TotalAmount);
                            db.SaveChanges();

                            Ledger ledger_Income = db.Ledgers.Where(x => x.Name == "Student Fee").FirstOrDefault();
                            ledger_Income.CurrentBalance += Convert.ToDecimal(Challan.TotalAmount);
                            db.SaveChanges();
                            ////////////////End///////////////
                        }
                    }
                    else
                    {
                        No = challan.ChallanNo;
                    }
                    Challan.ChallanID = No;
                    ChallanList.Add(Challan);
                }
                return Json(ChallanList, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var Error = "Error";
                return Json(Error, JsonRequestBehavior.AllowGet);
            }
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
        public ActionResult GetStudentInstalment(int stdid)
        {
           var amount= db.StudentFeeMultipliers.Where(x => x.StudentId == stdid).Select(x => x.SharePerInstalment).FirstOrDefault();
            return Json(amount,JsonRequestBehavior.AllowGet);
        }
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
            float multiplier =float.Parse(Request.Form["multiplier"]);
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
