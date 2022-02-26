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
    [Authorize(Roles = "Accountant,Accounting_Head")]
    public class FinanceSummaryController : Controller
    {
        Sea_Entities db = new Sea_Entities();
        // GET: FinanceSummary

        public ActionResult CalendarNotification()
        {
            var id = User.Identity.GetUserId();
            var fullname = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            var namelist = fullname.Split(' ');
            var name = namelist[0];
            var result = new { name };
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AllStudents()
        {



            return View();
        }

        public ActionResult BillingMonth(string userName)
        {
            List<BilingMonth> BiliingMonthList = new List<BilingMonth>();

            var UserId = db.AspNetUsers.Where(x => x.UserName == userName).FirstOrDefault().Id;
            var StudentId = db.AspNetStudents.Where(x => x.UserId == UserId).FirstOrDefault().Id;

            var StudentFee = db.StudentFees.Where(x => x.StudentID == StudentId).FirstOrDefault();
            var StudentFeeMultiplier = db.StudentFeeMultipliers.Where(x => x.StudentId == StudentId).FirstOrDefault();
            string[] MonthNames = new string[12] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", };

            if (StudentFee == null)
            {
                foreach (var MonthName in MonthNames)
                {
                    BilingMonth BillingMonth = new BilingMonth();
                    BillingMonth.MonthName = MonthName;
                    BillingMonth.Status = "UnPaid";
                    BillingMonth.TotalRecurringFee = 0;
                    BillingMonth.TotalNonRecurringFee = 0;
                    BillingMonth.Total = 0;

                    BiliingMonthList.Add(BillingMonth);

                }
            }
            else
            {
                foreach (var MonthName in MonthNames)
                {
                    BilingMonth BillingMonth = new BilingMonth();
                    BillingMonth.MonthName = MonthName;
                    BillingMonth.Status = "";
                    BillingMonth.TotalRecurringFee = 0;
                    BillingMonth.TotalNonRecurringFee = 0;
                    BillingMonth.Total = 0;

                    if (MonthName == "January")
                    {
                        var MonthStatus = StudentFeeMultiplier.Jan_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Jan_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Jan_Multiplier;
                        var MonthNumber = 1;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);


                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;

                        if (MonthStatus == true)
                        {
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);
                            BillingMonth.Status = "Paid";

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;
                            BillingMonth.Total = Convert.ToInt32(GrandTotal);

                        }
                        BiliingMonthList.Add(BillingMonth);

                    }
                    else if (MonthName == "February")
                    {
                        var MonthStatus = StudentFeeMultiplier.Feb_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Feb_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Feb_Multiplier;
                        var MonthNumber = 2;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;



                        if (MonthStatus == true)
                        {
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                            BillingMonth.Status = "Paid";

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";
                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;
                            BillingMonth.Total = Convert.ToInt32(GrandTotal);

                        }

                        BiliingMonthList.Add(BillingMonth);

                    }
                    else if (MonthName == "March")
                    {
                        var MonthStatus = StudentFeeMultiplier.Mar_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Mar_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Mar_Multiplier;

                        var MonthNumber = 3;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;


                        if (MonthStatus == true)
                        {
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                            BillingMonth.Status = "Paid";
                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;
                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);

                    }

                    else if (MonthName == "April")
                    {
                        var MonthStatus = StudentFeeMultiplier.April_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Apr_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.April__Multiplier;

                        var MonthNumber = 4;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;


                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);



                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);


                        }

                        BiliingMonthList.Add(BillingMonth);

                    }

                    else if (MonthName == "May")
                    {
                        var MonthStatus = StudentFeeMultiplier.May_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.May_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.May_Multiplier;
                        var MonthNumber = 5;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;



                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);



                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);

                    }

                    else if (MonthName == "June")
                    {
                        var MonthStatus = StudentFeeMultiplier.June_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Jun_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.June_Multiplier;
                        var MonthNumber = 6;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }

                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);
                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;

                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);
                    }

                    else if (MonthName == "July")
                    {
                        var MonthStatus = StudentFeeMultiplier.July_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Jul_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.July__Multiplier;
                        var MonthNumber = 7;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;

                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);
                    }
                    else if (MonthName == "August")
                    {
                        var MonthStatus = StudentFeeMultiplier.Aug_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Aug_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Aug_Multiplier;

                        var MonthNumber = 8;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }

                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);
                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;

                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);
                    }

                    else if (MonthName == "September")
                    {
                        var MonthStatus = StudentFeeMultiplier.Sep_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Sep_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Sep_Multiplier;

                        var MonthNumber = 9;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }
                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;


                        if (MonthStatus == true)
                        {

                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);
                    }

                    else if (MonthName == "October")
                    {
                        var MonthStatus = StudentFeeMultiplier.Oct_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Oct_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Oct_Multiplier;

                        var MonthNumber = 10;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }

                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);
                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;


                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);

                        }
                        else
                        {
                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);
                    }

                    else if (MonthName == "November")
                    {
                        var MonthStatus = StudentFeeMultiplier.Nov_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Nov_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Nov_Multiplier;

                        var MonthNumber = 11;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }

                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);
                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;

                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);


                        }
                        else
                        {

                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);
                    }

                    else
                    {
                        var MonthStatus = StudentFeeMultiplier.Dec_StatusPaid;
                        var MonthPaidAmount = StudentFeeMultiplier.Dec_PaidAmount;
                        var MonthMultiplier = StudentFeeMultiplier.Dec__Multiplier;

                        var MonthNumber = 12;

                        var nonRecurringList = db.StudentNonRecurringFees.Where(x => x.Month == MonthNumber && x.StudentFeeID == StudentId).ToList();
                        int TotalNonRecurring = 0;
                        foreach (var nonRecurring in nonRecurringList)
                        {
                            TotalNonRecurring = TotalNonRecurring + nonRecurring.Amount.Value;
                        }

                        var TotalRecurringFee = StudentFee.TotalWithoutAdmission * MonthMultiplier;
                        BillingMonth.TotalRecurringFee = Convert.ToInt32(TotalRecurringFee);

                        BillingMonth.TotalNonRecurringFee = TotalNonRecurring;

                        if (MonthStatus == true)
                        {
                            BillingMonth.Status = "Paid";
                            BillingMonth.Total = Convert.ToInt32(MonthPaidAmount);


                        }
                        else
                        {

                            BillingMonth.Status = "UnPaid";

                            var GrandTotal = TotalRecurringFee + TotalNonRecurring;

                            BillingMonth.Total = Convert.ToInt32(GrandTotal);
                        }

                        BiliingMonthList.Add(BillingMonth);


                    }


                }
            }

            return View(BiliingMonthList);
        }
        public class BilingMonth
        {
            public string MonthName { get; set; }
            public int TotalRecurringFee { get; set; }
            public int TotalNonRecurringFee { get; set; }
            public int Total { get; set; }
            public string Status { get; set; }
        }
        public JsonResult GetStudents(DataTablesParam param)
        {
            var loggedInUserId = User.Identity.GetUserId();
            try
            {


                if (User.IsInRole("Accountant"))
                {
                    int pageNo = 1;

                    if (param.iDisplayStart >= param.iDisplayLength)
                    {

                        pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;

                    }

                    int totalCount = 0;

                    if (param.sSearch != null)
                    {
                        // totalCount = db.AllStudentsList().Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Count();

                        // var studentList = db.AllStudentsList().Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();

                        totalCount = (from stdnt in db.AspNetStudents
                                      join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                      join stufee in db.StudentFees
                                      on stdnt.Id equals stufee.StudentID into egroup
                                      from stufee in egroup.DefaultIfEmpty()
                                      join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                      where stdnt.UserId == usr.Id && usr.StatusId != 2
                                      select new { stdnt.Name, BranchName = stdnt.AspNetBranch.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Distinct().Count();

                        var studentList = (from stdnt in db.AspNetStudents
                                           join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                           join stufee in db.StudentFees
                                           on stdnt.Id equals stufee.StudentID into egroup
                                           from stufee in egroup.DefaultIfEmpty()
                                           join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                           where stdnt.UserId == usr.Id && usr.StatusId != 2
                                           select new { stdnt.Name, BranchName = stdnt.AspNetBranch.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Where(x => x.RollNo.ToLower().Contains(param.sSearch.ToLower()) || x.Name.ToLower().Contains(param.sSearch.ToLower()) || x.ClassName.ToLower().Contains(param.sSearch.ToLower()) || x.CellNo.Contains(param.sSearch)).Distinct().OrderBy(x => x.Name).Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();


                        return Json(new
                        {
                            aaData = studentList,
                            sEcho = param.sEcho,
                            iTotalDisplayRecords = totalCount,
                            iTotalRecords = totalCount

                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {


                        totalCount = (from stdnt in db.AspNetStudents
                                      join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                      join stufee in db.StudentFees
                                      on stdnt.Id equals stufee.StudentID into egroup
                                      from stufee in egroup.DefaultIfEmpty()
                                      join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                      where stdnt.UserId == usr.Id && usr.StatusId != 2
                                      select new { stdnt.Name, BranchName = stdnt.AspNetBranch.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Distinct().Count();
                        //var studentList = db.AllStudentsList().Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();


                        var studentList = (from stdnt in db.AspNetStudents
                                           join usr in db.AspNetUsers on stdnt.UserId equals usr.Id
                                           join stufee in db.StudentFees
                                           on stdnt.Id equals stufee.StudentID into egroup
                                           from stufee in egroup.DefaultIfEmpty()
                                           join enrollment in db.AspNetStudent_Enrollments on stdnt.Id equals enrollment.StudentId
                                           where stdnt.UserId == usr.Id && usr.StatusId != 2
                                           select new { stdnt.Name, BranchName = stdnt.AspNetBranch.Name, stufee.TotalWithoutAdmission, stdnt.RollNo, stdnt.CellNo, usr.Image, JoiningDate = stdnt.AspNetUser.CreationDate, ClassName = stdnt.AspNetClass.Name }).Distinct().OrderBy(x => x.Name).Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength).ToList();


                        return Json(new
                        {
                            aaData = studentList,
                            sEcho = param.sEcho,
                            iTotalDisplayRecords = totalCount,
                            iTotalRecords = totalCount

                        }, JsonRequestBehavior.AllowGet);
                    }

                }//accountant if condition
            }
            catch (Exception ex)
            {
                var exmsg = ex.Message;
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserName()
        {
            var id = User.Identity.GetUserId();
            var name = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            var date = DateTime.Now;
            var result = new { date, name };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InterTransaction()
        {
            return View();
        }
        public JsonResult BankCashLedgers()
        {
            var headlist = db.LedgerGroups.ToList();

            List<HeadList> Head_list = new List<HeadList>();
            foreach (var item in headlist)
            {
                HeadList hl = new HeadList();
                hl.HeadId = item.Id;
                hl.HeadName = item.Name;
                var ledger = db.Ledgers.Where(x => (x.LedgerGroup.Name == "Cash" || x.LedgerGroup.Name == "Bank") && (x.LedgerGroupId == item.Id)).ToList();
                hl.accountlist = new List<AccountsList>();
                foreach (var l_item in ledger)
                {
                    AccountsList Ledger = new AccountsList();
                    Ledger.Id = l_item.Id;
                    Ledger.Name = l_item.Name;
                    hl.accountlist.Add(Ledger);
                }
                Head_list.Add(hl);
            }
            return Json(Head_list, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AddCashBankVoucher(CashBank_Voucher CBVoucher)
        {

            string[] D4 = CBVoucher.Date.Split('-');
            CBVoucher.Date = D4[2] + "/" + D4[1] + "/" + D4[0];
            Voucher v = new Voucher();

            var localtime = DateTime.Now.ToLocalTime().ToString();
            var time = localtime.Split(' ');
            var timestamps = time[1].Split(':');
            if (timestamps[0].Count() == 1)
            {
                timestamps[0] = "0" + timestamps[0];
            }
            if (timestamps[1].Count() == 1)
            {
                timestamps[1] = "0" + timestamps[1];
            }
            if (timestamps[2].Count() == 1)
            {
                timestamps[2] = "0" + timestamps[2];
            }
            time[1] = timestamps[0] + ":" + timestamps[1] + ":" + timestamps[2];

            var date = CBVoucher.Date + " " + time[1] + " " + time[2];

            DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture).Add(DateTime.Now.TimeOfDay);
            v.Date = dt;
            v.Notes = CBVoucher.Description;
            v.VoucherNo = CBVoucher.VoucherNo;
            v.Name = "Cash And Bank";
            var id = User.Identity.GetUserId();
            var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            v.CreatedBy = username;
            db.Vouchers.Add(v);
            db.SaveChanges();
            //////////////////////CREDIT//////////////////////////////
            VoucherRecord voucherrecord = new VoucherRecord();
            string[] fdfd = CBVoucher.PaymentFrom.Split(new char[] { '-' }, 2);
            var ledgerid = Convert.ToInt32(fdfd[0]);
            voucherrecord.LedgerId = ledgerid;
            voucherrecord.VoucherId = db.Vouchers.Select(x => x.Id).Max();
            var type = fdfd[1];
            var amount = 0;

            if (type == "Cr")
            {
                voucherrecord.Type = "Cr";
                amount = CBVoucher.Amount;

                voucherrecord.Amount = amount;
                voucherrecord.Description = CBVoucher.Description;
                var ledgerrecord = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                voucherrecord.CurrentBalance = ledgerrecord.CurrentBalance;
                /////////////////Game/////////////////////
                var groupid = ledgerrecord.LedgerGroupId;
                var ledgerHead = "";

                var ledgerheadId = db.LedgerGroups.Where(x => x.Id == groupid).Select(x => x.LedgerHeadID).FirstOrDefault();
                ledgerHead = db.LedgerHeads.Where(x => x.Id == ledgerheadId).Select(x => x.Name).FirstOrDefault();

                if (ledgerHead == "Assets")
                {
                    voucherrecord.AfterBalance = ledgerrecord.CurrentBalance - amount;
                    Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                    ledger.CurrentBalance = voucherrecord.AfterBalance;
                    db.SaveChanges();
                }

                db.VoucherRecords.Add(voucherrecord);
                db.SaveChanges();
            }
            ////////////////////////////////DEBIT/////////////////////////
            VoucherRecord voucher_record = new VoucherRecord();

            string[] rece_Array = CBVoucher.ReceivedIn.Split(new char[] { '-' }, 2);
            ledgerid = Convert.ToInt32(rece_Array[0]);
            voucher_record.LedgerId = ledgerid;
            voucher_record.VoucherId = db.Vouchers.Select(x => x.Id).Max();
            type = rece_Array[1];
            amount = 0;

            if (type == "Dr")
            {
                voucher_record.Type = "Dr";
                amount = CBVoucher.Amount;

                voucher_record.Amount = amount;
                voucher_record.Description = CBVoucher.Description;
                var ledgerrecord = db.Ledgers.Where(x => x.Id == voucher_record.LedgerId).FirstOrDefault();
                voucher_record.CurrentBalance = ledgerrecord.CurrentBalance;
                ////////////////Game/////////////////
                var groupid = ledgerrecord.LedgerGroupId;
                var ledgerHead = "";
                var ledgerheadId = db.LedgerGroups.Where(x => x.Id == groupid).Select(x => x.LedgerHeadID).FirstOrDefault();
                ledgerHead = db.LedgerHeads.Where(x => x.Id == ledgerheadId).Select(x => x.Name).FirstOrDefault();
                if (ledgerHead == "Assets")
                {
                    voucher_record.AfterBalance = ledgerrecord.CurrentBalance + amount;
                    Ledger ledger = db.Ledgers.Where(x => x.Id == voucher_record.LedgerId).FirstOrDefault();
                    ledger.CurrentBalance = voucher_record.AfterBalance;
                    db.SaveChanges();
                }

                db.VoucherRecords.Add(voucher_record);
                db.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            var ID = User.Identity.GetUserId();
            var branchID = db.AspNetBranch_Admins.Where(x => x.AdminId == ID).Select(x => x.BranchId).FirstOrDefault();
            var Classes1 = db.AspNetBranch_Class.Where(x => x.BranchId == branchID).Select(x => x.AspNetClass.Name).Distinct().ToList();

            List<string> classes = new List<string>();

            foreach (var clas in Classes1)
            {
                classes.Add(clas);

            }
            classes.Add("In Process");


            ViewBag.AllClasses = classes;

            return View();
        }
        public JsonResult GetEvents()
        {
            using (Sea_Entities dc = new Sea_Entities())
            {
                var id = User.Identity.GetUserId();
                var events = dc.Events.Where(x => x.UserId == id || x.IsPublic == true).Select(x => new { x.Description, x.End, x.EventID, x.IsFullDay, x.Subject, x.ThemeColor, x.Start, x.IsPublic }).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }




        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            e.UserId = User.Identity.GetUserId();
            var status = false;
            using (Sea_Entities dc = new Sea_Entities())
            {
                if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    dc.Events.Add(e);
                }

                dc.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (Sea_Entities dc = new Sea_Entities())
            {
                var v = dc.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
        public ActionResult Summary()
        {
            return View();
        }
        public JsonResult GetSumamryReport()
        {
            var headlist = db.LedgerHeads.ToList();
            List<Ledger_Head> ledgerheadlist = new List<Ledger_Head>();

            foreach (var h_item in headlist)
            {
                Ledger_Head ledgerhead = new Ledger_Head();
                ledgerhead.HeadId = h_item.Id;
                ledgerhead.HeadName = h_item.Name;
                decimal? amount = 0;
                var grouplist = db.LedgerGroups.Where(x => x.LedgerHeadID == h_item.Id).ToList();
                var _ledgerlist = db.Ledgers.Where(x => x.LedgerHeadId == h_item.Id && x.LedgerGroupId == null).ToList();
                ledgerhead.ledgerGroup = new List<Ledger_Group>();
                ledgerhead.ledger = new List<_Ledger>();
                foreach (var g_item in grouplist)
                {
                    Ledger_Group lg = new Ledger_Group();
                    lg.GroupId = g_item.Id;
                    lg.GroupName = g_item.Name;
                    lg.ledger = new List<_Ledger>();
                    decimal? groupAmount = 0;
                    var ledgerlist = db.Ledgers.Where(x => x.LedgerGroupId == lg.GroupId).ToList();
                    foreach (var l_item in ledgerlist)
                    {
                        _Ledger l = new _Ledger();
                        l.LedgerId = l_item.Id;
                        l.LedgerName = l_item.Name;
                        l.LedgerAmount = l_item.CurrentBalance;
                        if (l_item.CurrentBalance == null)
                        {
                            groupAmount = groupAmount + 0;
                            amount = amount + 0;
                        }
                        else
                        {
                            groupAmount = groupAmount + l_item.CurrentBalance;
                            amount = amount + l_item.CurrentBalance;
                        }

                        lg.ledger.Add(l);
                    }
                    lg.GroupTotal = groupAmount;
                    ledgerhead.ledgerGroup.Add(lg);
                }
                foreach (var item in _ledgerlist)
                {
                    _Ledger l = new _Ledger();
                    l.LedgerId = item.Id;
                    l.LedgerName = item.Name;
                    l.LedgerAmount = item.CurrentBalance;
                    if (item.CurrentBalance == null)
                    {
                        amount = amount + 0;
                    }
                    else
                    {
                        amount = amount + item.CurrentBalance;
                    }
                    ledgerhead.ledger.Add(l);
                }
                ledgerhead.TotalAmount = amount;
                ledgerheadlist.Add(ledgerhead);
            }

            return Json(ledgerheadlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cash()
        {
            return View();
        }
        public ActionResult CashIndex()
        {

            return View();
        }
        public JsonResult CashList()
        {
            var cash = (from ledger in db.Ledgers
                        join grp in db.LedgerGroups on ledger.LedgerGroupId equals grp.Id
                        where grp.Name == "Cash"
                        select new { ledger.Id, ledger.Name, ledger.StartingBalance, ledger.CurrentBalance, h_name = ledger.LedgerHead.Name, g_name = ledger.LedgerGroup.Name }).ToList();
            return Json(cash, JsonRequestBehavior.AllowGet);
        }
        ///////////////////////////////////////BANK///////////////////////////
        public ActionResult Bank()
        {

            return View();
        }
        public ActionResult BankIndex()
        {
            return View();
        }
        public JsonResult BankList()
        {
            var bank = (from ledger in db.Ledgers
                        join grp in db.LedgerGroups on ledger.LedgerGroupId equals grp.Id
                        where grp.Name == "Bank"
                        select new { ledger.Id, ledger.Name, ledger.StartingBalance, ledger.CurrentBalance, h_name = ledger.LedgerHead.Name, g_name = ledger.LedgerGroup.Name }).ToList();
            return Json(bank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JournalEntryList(int id)
        {

            ViewBag.LedgerId = id;
            return View();
        }
        public JsonResult LedgerRecord(int id)
        {
            var vouchers = db.VoucherRecords.Where(x => x.LedgerId == id).ToList();
            List<JournalEntry_List> entry = new List<JournalEntry_List>();

            foreach (var item in vouchers)
            {
                JournalEntry_List j = new JournalEntry_List();
                j.ID = item.Id;
                j.Name = item.Voucher.Name;
                j.LedgerName = item.Ledger.Name;
                j.Notes = item.Description;
                j.Date = item.Voucher.Date;
                j.Amount = item.Amount;
                j.type = item.Type;
                j.BeforeBalance = item.CurrentBalance.ToString();
                j.Afterbalance = item.AfterBalance.ToString();
                entry.Add(j);
            }
            return Json(entry, JsonRequestBehavior.AllowGet);
        }
        /////////////////////////////////////////////////////////////////
        public ActionResult AddBankVoucher(_Voucher Vouchers)
        {
            try
            {
                string[] D4 = Vouchers.Time.Split('-');
                Vouchers.Time = D4[2] + "/" + D4[1] + "/" + D4[0];
                Voucher v = new Voucher();

                var localtime = DateTime.Now.ToLocalTime().ToString();
                var time = localtime.Split(' ');
                var timestamps = time[1].Split(':');
                if (timestamps[0].Count() == 1)
                {
                    timestamps[0] = "0" + timestamps[0];
                }
                if (timestamps[1].Count() == 1)
                {
                    timestamps[1] = "0" + timestamps[1];
                }
                if (timestamps[2].Count() == 1)
                {
                    timestamps[2] = "0" + timestamps[2];
                }
                time[1] = timestamps[0] + ":" + timestamps[1] + ":" + timestamps[2];

                var date = Vouchers.Time + " " + time[1] + " " + time[2];

                DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                v.Date = dt;
                v.Notes = Vouchers.Narration;
                v.VoucherNo = Vouchers.VoucherNo;
                v.Name = Vouchers.VoucherName;
                var id = User.Identity.GetUserId();
                var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
                v.CreatedBy = username;
                db.Vouchers.Add(v);
                db.SaveChanges();

                VoucherRecord voucherrcd = new VoucherRecord();

                voucherrcd.LedgerId = Vouchers.uppercode;
                if (Vouchers.accounttype == "Dr")
                {
                    voucherrcd.Type = "Dr";
                }
                else if (Vouchers.accounttype == "Cr")
                {
                    voucherrcd.Type = "Cr";
                }
                voucherrcd.VoucherId = db.Vouchers.Select(x => x.Id).Max();
                voucherrcd.Description = Vouchers.upperdesc;
                var record = db.Ledgers.Where(x => x.Id == voucherrcd.LedgerId).FirstOrDefault();
                var currentbalance = record.CurrentBalance;
                if (currentbalance == null)
                {
                    voucherrcd.CurrentBalance = 0;
                }
                else
                {
                    voucherrcd.CurrentBalance = currentbalance;
                }

                var total = decimal.Parse(Vouchers.uppertotal);
                voucherrcd.Amount = total;
                if (voucherrcd.Type == "Cr")
                {

                    voucherrcd.AfterBalance = voucherrcd.CurrentBalance - total;
                    Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrcd.LedgerId).FirstOrDefault();
                    ledger.CurrentBalance = voucherrcd.AfterBalance;
                }
                else if (voucherrcd.Type == "Dr")
                {
                    voucherrcd.AfterBalance = voucherrcd.CurrentBalance + total;
                    Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrcd.LedgerId).FirstOrDefault();
                    ledger.CurrentBalance = voucherrcd.AfterBalance;
                }
                db.VoucherRecords.Add(voucherrcd);
                db.SaveChanges();

                foreach (var item in Vouchers.VoucherDetail)
                {
                    VoucherRecord voucherrecord = new VoucherRecord();

                    var ledgerid = Int32.Parse(item.Code);
                    voucherrecord.LedgerId = ledgerid;
                    voucherrecord.VoucherId = db.Vouchers.Select(x => x.Id).Max();
                    var amount = Int32.Parse(item.Credit);
                    voucherrecord.Amount = amount;

                    if (item.BranchId == 0)
                    {
                        voucherrecord.BranchId = null;
                    }
                    else
                    {
                        voucherrecord.BranchId = item.BranchId;
                    }
                    //studentid
                    if (item.StudentId != null)
                    {

                        string[] EmployeeStudentId = item.StudentId.Split('-');


                        if (EmployeeStudentId[0] == "Student")
                        {
                            voucherrecord.UserType = "Student";
                            voucherrecord.UserId = EmployeeStudentId[1];

                        }
                        else // if (EmployeeStudentId[0] == "Employee")
                        {
                            voucherrecord.UserType = "Employee";
                            voucherrecord.UserId = EmployeeStudentId[1];
                        }

                    }
                    else
                    {
                        voucherrecord.UserType = null;
                        voucherrecord.UserId = null;

                    }
                    //studentId

                    if (voucherrcd.Type == "Dr")
                    {
                        voucherrecord.Type = "Cr";
                    }
                    else if (voucherrcd.Type == "Cr")
                    {
                        voucherrecord.Type = "Dr";
                    }
                    voucherrecord.Description = item.Transaction;

                    var ledgerrecord = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                    voucherrecord.CurrentBalance = ledgerrecord.CurrentBalance;
                    //////////////////////////Game/////////////////////
                    var groupid = ledgerrecord.LedgerGroupId;
                    var ledgerHead = "";
                    if (groupid != null)
                    {
                        var ledgerheadId = db.LedgerGroups.Where(x => x.Id == groupid).Select(x => x.LedgerHeadID).FirstOrDefault();
                        ledgerHead = db.LedgerHeads.Where(x => x.Id == ledgerheadId).Select(x => x.Name).FirstOrDefault();
                    }
                    else
                    {
                        var headid = ledgerrecord.LedgerHeadId;
                        ledgerHead = db.LedgerHeads.Where(x => x.Id == headid).Select(x => x.Name).FirstOrDefault();

                    }
                    //var groupid = ledgerrecord.LedgerGroupId;
                    //var ledgerheadId = db.LedgerGroups.Where(x => x.Id == groupid).Select(x => x.LedgerHeadID).FirstOrDefault();
                    //var ledgerHead = db.LedgerHeads.Where(x => x.Id == ledgerheadId).Select(x => x.Name).FirstOrDefault();
                    if (ledgerHead == "Assets" || ledgerHead == "Expense")
                    {
                        if (voucherrecord.Type == "Cr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance - amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;

                        }
                        else if (voucherrecord.Type == "Dr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance + amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;
                        }
                    }
                    else if (ledgerHead == "Equity" || ledgerHead == "Liabilities" || ledgerHead == "Income")
                    {
                        if (voucherrecord.Type == "Cr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance + amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;
                        }
                        else if (voucherrecord.Type == "Dr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance - amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;
                        }
                    }
                    db.VoucherRecords.Add(voucherrecord);
                    db.SaveChanges();
                }


                var result = "yes";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Bank");
            }
        }

        public ActionResult AddCashVoucher(_Voucher Vouchers)
        {
            try
            {
                string[] D4 = Vouchers.Time.Split('-');
                Vouchers.Time = D4[2] + "/" + D4[1] + "/" + D4[0];
                Voucher v = new Voucher();

                var localtime = DateTime.Now.ToLocalTime().ToString();
                var time = localtime.Split(' ');
                var timestamps = time[1].Split(':');
                if (timestamps[0].Count() == 1)
                {
                    timestamps[0] = "0" + timestamps[0];
                }
                if (timestamps[1].Count() == 1)
                {
                    timestamps[1] = "0" + timestamps[1];
                }
                if (timestamps[2].Count() == 1)
                {
                    timestamps[2] = "0" + timestamps[2];
                }
                time[1] = timestamps[0] + ":" + timestamps[1] + ":" + timestamps[2];

                var date = Vouchers.Time + " " + time[1] + " " + time[2];

                DateTime dt = DateTime.ParseExact(date, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                v.Date = dt;
                v.Notes = Vouchers.Narration;
                v.VoucherNo = Vouchers.VoucherNo;
                v.Name = Vouchers.VoucherName;
                var id = User.Identity.GetUserId();
                var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
                v.CreatedBy = username;
                db.Vouchers.Add(v);
                db.SaveChanges();

                VoucherRecord voucherrcd = new VoucherRecord();

                voucherrcd.LedgerId = Vouchers.uppercode;
                if (Vouchers.accounttype == "Dr")
                {
                    voucherrcd.Type = "Dr";
                }
                else if (Vouchers.accounttype == "Cr")
                {
                    voucherrcd.Type = "Cr";
                }
                voucherrcd.VoucherId = db.Vouchers.Select(x => x.Id).Max();
                voucherrcd.Description = Vouchers.upperdesc;
                var record = db.Ledgers.Where(x => x.Id == voucherrcd.LedgerId).FirstOrDefault();
                var currentbalance = record.CurrentBalance;
                if (currentbalance == null)
                {
                    voucherrcd.CurrentBalance = 0;
                }
                else
                {
                    voucherrcd.CurrentBalance = currentbalance;
                }

                var total = decimal.Parse(Vouchers.uppertotal);
                voucherrcd.Amount = total;
                if (voucherrcd.Type == "Cr")
                {

                    voucherrcd.AfterBalance = voucherrcd.CurrentBalance - total;
                    Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrcd.LedgerId).FirstOrDefault();
                    ledger.CurrentBalance = voucherrcd.AfterBalance;
                }
                else if (voucherrcd.Type == "Dr")
                {
                    voucherrcd.AfterBalance = voucherrcd.CurrentBalance + total;
                    Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrcd.LedgerId).FirstOrDefault();
                    ledger.CurrentBalance = voucherrcd.AfterBalance;
                }
                db.VoucherRecords.Add(voucherrcd);
                db.SaveChanges();

                foreach (var item in Vouchers.VoucherDetail)
                {
                    VoucherRecord voucherrecord = new VoucherRecord();

                    var ledgerid = Int32.Parse(item.Code);
                    voucherrecord.LedgerId = ledgerid;
                    voucherrecord.VoucherId = db.Vouchers.Select(x => x.Id).Max();
                    var amount = Int32.Parse(item.Credit);
                    voucherrecord.Amount = amount;
                    if (item.BranchId == 0)
                    {
                        voucherrecord.BranchId = null;
                    }
                    else
                    {
                        voucherrecord.BranchId = item.BranchId;
                    }
                    //studentid
                    if (item.StudentId != null)
                    {

                        string[] EmployeeStudentId = item.StudentId.Split('-');


                        if (EmployeeStudentId[0] == "Student")
                        {
                            voucherrecord.UserType = "Student";
                            voucherrecord.UserId = EmployeeStudentId[1];

                        }
                        else // if (EmployeeStudentId[0] == "Employee")
                        {
                            voucherrecord.UserType = "Employee";
                            voucherrecord.UserId = EmployeeStudentId[1];
                        }

                    }
                    else
                    {
                        voucherrecord.UserType = null;
                        voucherrecord.UserId = null;

                    }
                    //studentId

                    if (voucherrcd.Type == "Dr")
                    {
                        voucherrecord.Type = "Cr";
                    }
                    else if (voucherrcd.Type == "Cr")
                    {
                        voucherrecord.Type = "Dr";
                    }
                    voucherrecord.Description = item.Transaction;

                    var ledgerrecord = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                    voucherrecord.CurrentBalance = ledgerrecord.CurrentBalance;
                    //////////////////////////Game/////////////////////
                    var groupid = ledgerrecord.LedgerGroupId;
                    var ledgerHead = "";
                    if (groupid != null)
                    {
                        var ledgerheadId = db.LedgerGroups.Where(x => x.Id == groupid).Select(x => x.LedgerHeadID).FirstOrDefault();
                        ledgerHead = db.LedgerHeads.Where(x => x.Id == ledgerheadId).Select(x => x.Name).FirstOrDefault();
                    }
                    else
                    {
                        var headid = ledgerrecord.LedgerHeadId;
                        ledgerHead = db.LedgerHeads.Where(x => x.Id == headid).Select(x => x.Name).FirstOrDefault();

                    }
                    //var groupid = ledgerrecord.LedgerGroupId;
                    //var ledgerheadId = db.LedgerGroups.Where(x => x.Id == groupid).Select(x => x.LedgerHeadID).FirstOrDefault();
                    //var ledgerHead = db.LedgerHeads.Where(x => x.Id == ledgerheadId).Select(x => x.Name).FirstOrDefault();
                    if (ledgerHead == "Assets" || ledgerHead == "Expense")
                    {
                        if (voucherrecord.Type == "Cr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance - amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;

                        }
                        else if (voucherrecord.Type == "Dr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance + amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;
                        }
                    }
                    else if (ledgerHead == "Equity" || ledgerHead == "Liabilities" || ledgerHead == "Income")
                    {
                        if (voucherrecord.Type == "Cr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance + amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;
                        }
                        else if (voucherrecord.Type == "Dr")
                        {
                            voucherrecord.AfterBalance = ledgerrecord.CurrentBalance - amount;

                            Ledger ledger = db.Ledgers.Where(x => x.Id == voucherrecord.LedgerId).FirstOrDefault();
                            ledger.CurrentBalance = voucherrecord.AfterBalance;
                        }
                    }
                    db.VoucherRecords.Add(voucherrecord);
                    db.SaveChanges();
                }


                var result = "yes";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Cash");
            }

        }

        //public ActionResult CashIndex()
        //{
        //    return View();
        //}
        //public ActionResult GetCash()
        //{
        //    var result = db.Vouchers.ToList();
        //    List<Voucher_list> list = new List<Voucher_list>();
        //    foreach (var item in result)
        //    {
        //        Voucher_list vl = new Voucher_list();
        //        vl.Name = item.Name;
        //        vl.Notes = item.Notes;
        //        vl.Date = item.Date;
        //        vl.CreatedBy = item.CreatedBy;
        //        list.Add(vl);
        //    }
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult FindVoucherNo()
        {
            int No;
            try
            {
                No = (int)db.Vouchers.Select(x => x.VoucherNo).Max();
                No++;

            }
            catch
            {
                No = 1;
            }

            return Json(No, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetLedgerAmount(int LedgerId)
        {
            var CurrentBalance = db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault().CurrentBalance;

            return Json(CurrentBalance, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectListLedgersBank()
        {

            var ledger = db.Ledgers.Where(x => x.LedgerGroup.Name == "Bank").ToList();
            List<Ledger> List = new List<Ledger>();

            foreach (var item in ledger)
            {
                Ledger Ledger = new Ledger();
                Ledger.Id = item.Id;
                Ledger.Name = item.Name;
                List.Add(Ledger);
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectListLedgers()
        {

            var ledger = db.Ledgers.Where(x => x.LedgerGroup.Name == "Cash").ToList();
            List<Ledger> List = new List<Ledger>();

            foreach (var item in ledger)
            {
                Ledger Ledger = new Ledger();
                Ledger.Id = item.Id;
                Ledger.Name = item.Name;
                List.Add(Ledger);
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectAllLedgers()
        {

            var headlist = db.LedgerHeads.Where(x => x.Name == "Assets" || x.Name == "Expense").ToList();

            List<HeadList> Head_list = new List<HeadList>();
            foreach (var item in headlist)
            {
                HeadList hl = new HeadList();
                hl.HeadId = item.Id;
                hl.HeadName = item.Name;

                var ledger = db.Ledgers.Where(x => x.LedgerGroup.Name != "Cash" && x.LedgerGroup.Name != "Bank" && x.LedgerHeadId == item.Id).ToList();
                hl.accountlist = new List<AccountsList>();
                foreach (var l_item in ledger)
                {
                    AccountsList Ledger = new AccountsList();
                    Ledger.Id = l_item.Id;
                    Ledger.Name = l_item.Name;
                    hl.accountlist.Add(Ledger);
                }
                Head_list.Add(hl);
            }
            return Json(Head_list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeFinanceList()
        {
            return View();
        }
        public ActionResult EmployeeFinances()
        {
            var EmployeeFinances = db.EmployeeFinances.Select(x => new { x.Id, x.AspNetEmployee.Name, x.Bank_Chq, x.DueDate, x.Total_SalaryPayable ,x.Status }).ToList();

            return Json(EmployeeFinances, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateFinance()
        {

            return View();
        }
        [HttpGet]
        public ActionResult CreateEmployeeFinance()
        {
            var AllEmployeesList = db.AspNetEmployees.Select(x => new { x.Id, x.Name }).ToList();
            ViewBag.EmployeeList = new SelectList(AllEmployeesList, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult CreateEmployeeFinance(EmployeeVM employeeFinance)
        {
            var IsFinanceCreated = "No";
            var dbTransaction = db.Database.BeginTransaction();
            try
            {
                var Employee = db.AspNetEmployees.Where(x => x.Id == employeeFinance.EmployeeId).FirstOrDefault();

                EmployeeFinance finance = new EmployeeFinance();
                finance.EmployeeId = employeeFinance.EmployeeId;
                //finance.Date = employeeFinance.Date;//this date is as Due Date
                finance.DueDate = employeeFinance.DueDate;//this date is as Due Date
                finance.Bank_Chq = employeeFinance.Bank_Chq;
                finance.Add_Arrears = employeeFinance.Add_Arrears;
                finance.Add_LoanAdvance = employeeFinance.Add_LoanAdvance;
                finance.Add_Inrmnt = employeeFinance.Add_Inrmnt;
                finance.Add_Other = employeeFinance.Add_Other;
                finance.Add_Total_AddInSalary = employeeFinance.Add_Total_AddInSalary;
                finance.Add_GrossSalary = employeeFinance.Add_GrossSalary;


                finance.Less_TaxDed = employeeFinance.Less_TaxDed;
                finance.Less_LoanAdvance = employeeFinance.Less_LoanAdvance;
                finance.Less_SecurityDed = employeeFinance.Less_SecurityDed;
                finance.Less_AbsentDed = employeeFinance.Less_AbsentDed;
                finance.Less_Others = employeeFinance.Less_Others;
                finance.Less_TotalLessInSalary = employeeFinance.Less_TotalLessInSalary;
                finance.Total_SalaryPayable = employeeFinance.Total_SalaryPayable;

                finance.CreationDate = GetLocalDateTime.GetLocalDateTimeFunction();
                finance.Status = "Created";
                db.EmployeeFinances.Add(finance);
                db.SaveChanges();



                var id = User.Identity.GetUserId();
                var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
                Voucher voucher = new Voucher();
                var SessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;
                voucher.Name = "Salary Created of Employee " + Employee.Name;
                voucher.Notes = "";
                voucher.Date = GetLocalDateTime.GetLocalDateTimeFunction();
                voucher.CreatedBy = username;
                voucher.ChallanId = null;
                int? VoucherObj = db.Vouchers.Max(x => x.VoucherNo);
                voucher.VoucherNo = Convert.ToInt32(VoucherObj) + 1;
                db.Vouchers.Add(voucher);
                db.SaveChanges();

                var EmployeeFinanceToUpdate = db.EmployeeFinances.Where(x => x.Id == finance.Id).FirstOrDefault();
                EmployeeFinanceToUpdate.CreatedVoucherId = voucher.Id;
                db.SaveChanges();


                var Leadger = db.Ledgers.Where(x => x.Name == "Salary Payable").FirstOrDefault();
                int AccountReceivableId = Leadger.Id;
                decimal? CurrentBalance = Leadger.CurrentBalance;

                VoucherRecord voucherRecord = new VoucherRecord();
                decimal? AfterBalance = CurrentBalance + Convert.ToDecimal(employeeFinance.Total_SalaryPayable);
                voucherRecord.LedgerId = AccountReceivableId;
                voucherRecord.Type = "Cr";
                voucherRecord.Amount = Convert.ToDecimal(employeeFinance.Total_SalaryPayable);
                voucherRecord.CurrentBalance = CurrentBalance;
                voucherRecord.AfterBalance = AfterBalance;
                voucherRecord.VoucherId = voucher.Id;
                voucherRecord.UserType = "Employee";
                voucherRecord.UserId = Employee.Id.ToString();
                voucherRecord.BranchId = Employee.BranchId;


                voucherRecord.Description = "Salary Created of Employee " + Employee.Name;
                Leadger.CurrentBalance = AfterBalance;
                db.VoucherRecords.Add(voucherRecord);
                db.SaveChanges();


                VoucherRecord voucherRecord1 = new VoucherRecord();

                var LedgerStudentFee = db.Ledgers.Where(x => x.Name == "Staff Salary").FirstOrDefault();

                decimal? CurrentBalanceOfStudentFee = LedgerStudentFee.CurrentBalance;
                decimal? AfterBalanceOfNotes = CurrentBalanceOfStudentFee + Convert.ToDecimal(employeeFinance.Total_SalaryPayable);
                voucherRecord1.LedgerId = LedgerStudentFee.Id;
                voucherRecord1.Type = "Dr";
                voucherRecord1.Amount = Convert.ToDecimal(employeeFinance.Total_SalaryPayable);
                voucherRecord1.CurrentBalance = CurrentBalanceOfStudentFee;
                voucherRecord1.AfterBalance = AfterBalanceOfNotes;
                voucherRecord1.VoucherId = voucher.Id;
                voucherRecord1.Description = "Staff Salary Created of Employee " + Employee.Name;
                voucherRecord1.UserType = "Employee";
                voucherRecord1.UserId = Employee.Id.ToString();
                voucherRecord1.BranchId = Employee.BranchId;
                LedgerStudentFee.CurrentBalance = AfterBalanceOfNotes;

                db.VoucherRecords.Add(voucherRecord1);
                db.SaveChanges();

                IsFinanceCreated = "Yes";

                dbTransaction.Commit();

                return Json(new { IsFinanceCreated = IsFinanceCreated }, JsonRequestBehavior.AllowGet);


            }

            catch (Exception ex)
            {
                var msg = ex.Message;
                dbTransaction.Dispose();
                return Json(new { IsFinanceCreated = IsFinanceCreated }, JsonRequestBehavior.AllowGet);


            }

        }

        public ActionResult PaySalary(string idlist, int LedgerId, string PaidDate)
        {
            var dbTransaction = db.Database.BeginTransaction();
            try
            {

                var EmployeeFinanceIds = idlist.Split(',');

                foreach (var listitem in EmployeeFinanceIds)
                {
                    int item = Convert.ToInt32(listitem);

                   EmployeeFinance employeeFinancesToUpdate = db.EmployeeFinances.Where(x => x.Id == item).FirstOrDefault();

                    if(employeeFinancesToUpdate.Status =="Paid")
                    {

                    }
                    else
                    {
                        employeeFinancesToUpdate.PaidDate = Convert.ToDateTime(PaidDate);
                        employeeFinancesToUpdate.Status = "Paid";
                        db.SaveChanges();
                    }

                    var TotalSalaryPayable = employeeFinancesToUpdate.Total_SalaryPayable;

                    var id = User.Identity.GetUserId();
                    var username = db.AspNetUsers.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
                    Voucher voucher = new Voucher();
                    var SessionId = db.AspNetSessions.Where(x => x.IsActive == true).FirstOrDefault().Id;
                    voucher.Name = "Employee Salary Paid "+ employeeFinancesToUpdate.AspNetEmployee.Name;
                    voucher.Notes = "";
                    voucher.Date = GetLocalDateTime.GetLocalDateTimeFunction();
                    voucher.CreatedBy = username;
                    voucher.ChallanId = null;
                    int? VoucherObj = db.Vouchers.Max(x => x.VoucherNo);
                    voucher.VoucherNo = Convert.ToInt32(VoucherObj) + 1;
                    db.Vouchers.Add(voucher);
                    db.SaveChanges();

                    var EmployeeFinances = db.EmployeeFinances.Where(x => x.Id == employeeFinancesToUpdate.Id).FirstOrDefault();
                    EmployeeFinances.PaidVoucherId = voucher.Id;
                    db.SaveChanges();

                    //Salary Payable
                    var Leadger = db.Ledgers.Where(x => x.Name == "Salary Payable").FirstOrDefault();
                    int AccountReceivableId = Leadger.Id;
                    decimal? CurrentBalance = Leadger.CurrentBalance;
                    VoucherRecord voucherRecord = new VoucherRecord();
                    decimal? AfterBalance = CurrentBalance - Convert.ToDecimal(TotalSalaryPayable);
                    voucherRecord.LedgerId = AccountReceivableId;
                    voucherRecord.Type = "Dr";
                    voucherRecord.Amount = Convert.ToDecimal(TotalSalaryPayable);
                    voucherRecord.CurrentBalance = CurrentBalance;
                    voucherRecord.AfterBalance = AfterBalance;
                    voucherRecord.VoucherId = voucher.Id;
                    voucherRecord.UserType = "Employee";
                    voucherRecord.UserId = EmployeeFinances.EmployeeId.ToString();
                    voucherRecord.BranchId = EmployeeFinances.AspNetEmployee.BranchId;
                    voucherRecord.Description = "Employee Salary Paid "+ EmployeeFinances.AspNetEmployee.Name;
                    Leadger.CurrentBalance = AfterBalance;
                    db.VoucherRecords.Add(voucherRecord);
                    db.SaveChanges();



                    VoucherRecord voucherRecord1 = new VoucherRecord();
                    var LedgerCash = db.Ledgers.Where(x => x.Id == LedgerId).FirstOrDefault();
                    decimal? CurrentBalanceOfCash = LedgerCash.CurrentBalance;
                    decimal? AfterBalanceOfCash = CurrentBalanceOfCash - Convert.ToDecimal(TotalSalaryPayable);
                    voucherRecord1.LedgerId = LedgerCash.Id;
                    voucherRecord1.Type = "Cr";
                    voucherRecord1.Amount = Convert.ToDecimal(TotalSalaryPayable);
                    voucherRecord1.CurrentBalance = CurrentBalanceOfCash;
                    voucherRecord1.AfterBalance = AfterBalanceOfCash;
                    voucherRecord1.VoucherId = voucher.Id;
                    voucherRecord1.Description = "Employee Salary Paid " + EmployeeFinances.AspNetEmployee.Name;
                    voucherRecord1.UserType = "Employee";
                    voucherRecord1.UserId = EmployeeFinances.EmployeeId.ToString();
                    voucherRecord1.BranchId = EmployeeFinances.AspNetEmployee.BranchId;
                    LedgerCash.CurrentBalance = AfterBalanceOfCash;

                    db.VoucherRecords.Add(voucherRecord1);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                dbTransaction.Dispose();
                ViewBag.Error = "Something went wrong";
            }

            dbTransaction.Commit();
            return RedirectToAction("EmployeeFinanceList");


        }
        public ActionResult CheckSalaryCreated(int EmployeeId, string Date)
        {
            var DateTimevalue = Convert.ToDateTime(Date);

            //var StringDate = DateTime.ToString();

          //  var SplitDate = StringDate.Split('/').ToList();
            string day = "", month = "", year = "";

            year = DateTimevalue.Year.ToString() ;
            month = DateTimevalue.Month.ToString();
           // day = SplitDate[2];

            var Msg = "";

           var EmployeeFinance =  db.EmployeeFinances.Where(x => x.EmployeeId == EmployeeId && (x.DueDate.Value.Month.ToString() == month && x.DueDate.Value.Year.ToString() == year)).FirstOrDefault();

            if(EmployeeFinance == null)
            {
                Msg = "";
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
            else if (EmployeeFinance.Status == "Paid")
            {
                Msg = "Selected employee salary is paid";
                return Json(Msg, JsonRequestBehavior.AllowGet);
             }
            else
            {
                Msg = "Salary is already created of selected employee for the selected date.";
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }

           // return Json(EmployeeFinance, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetEmployeeDetails(int EmployeeId)
        {

            var EmployeeDetails = db.AspNetEmployees.Where(x => x.Id == EmployeeId).Select(x => new { x.Id, x.Name, BranchName = x.AspNetBranch.Name, x.JoiningDate, x.Post, x.Cnic, x.Salary }).FirstOrDefault();

            return Json(EmployeeDetails, JsonRequestBehavior.AllowGet);
        }
        public class EmployeeVM
        {
            public Nullable<int> EmployeeId { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
            public Nullable<System.DateTime> DueDate { get; set; }
            public Nullable<System.DateTime> PaidDate { get; set; }

            public string Bank_Chq { get; set; }

            public Nullable<double> Add_Arrears { get; set; }
            public Nullable<double> Add_LoanAdvance { get; set; }
            public Nullable<double> Add_Inrmnt { get; set; }
            public Nullable<double> Add_Other { get; set; }
            public Nullable<double> Add_Total_AddInSalary { get; set; }
            public Nullable<double> Add_GrossSalary { get; set; }
            public Nullable<double> Less_TaxDed { get; set; }
            public Nullable<double> Less_LoanAdvance { get; set; }
            public Nullable<double> Less_SecurityDed { get; set; }
            public Nullable<double> Less_AbsentDed { get; set; }
            public Nullable<double> Less_Others { get; set; }
            public Nullable<double> Less_TotalLessInSalary { get; set; }
            public Nullable<double> Total_SalaryPayable { get; set; }
            public string Remarks { get; set; }



        }


        public class _Voucher
        {
            public string VoucherName { get; set; }
            public string VoucherDescription { get; set; }
            public string Narration { get; set; }
            public string Time { get; set; }
            public string upperdesc { get; set; }
            public int uppercode { get; set; }
            public string accounttype { get; set; }
            public string uppertotal { get; set; }
            public int VoucherNo { get; set; }
            public List<Voucher_Detail> VoucherDetail { set; get; }
        }
        public class JournalEntry_List
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Notes { get; set; }
            public DateTime? Date { get; set; }
            public decimal? Amount { get; set; }
            public string LedgerName { get; set; }
            public string BeforeBalance { get; set; }
            public string Afterbalance { get; set; }
            public string type { get; set; }

        }
        public class Voucher_Detail
        {
            public string Type { get; set; }
            public string VoucherNo { get; set; }
            public string Time { get; set; }
            public string Code { get; set; }
            public string Transaction { get; set; }
            public string Credit { get; set; }
            public string Debit { get; set; }
            public double balance { get; set; }
            public int BranchId { get; set; }
            public string StudentId { get; set; }


        }
        public class Ledger_Head
        {
            public string HeadName { get; set; }
            public int HeadId { get; set; }
            public decimal? TotalAmount { get; set; }
            public List<Ledger_Group> ledgerGroup { get; set; }
            public List<_Ledger> ledger { get; set; }

        }
        public class Voucher_list
        {
            public string Name { get; set; }
            public string Notes { get; set; }
            public DateTime? Date { get; set; }
            public string CreatedBy { get; set; }
        }
        public class Ledger_Group
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public decimal? GroupTotal { get; set; }
            public List<_Ledger> ledger { get; set; }
        }
        public class CashBank_Voucher
        {
            public string Date { get; set; }
            public int VoucherNo { get; set; }
            public string Description { get; set; }
            public int Amount { get; set; }
            public string PaymentFrom { get; set; }
            public string ReceivedIn { get; set; }
        }
        public class _Ledger
        {
            public int LedgerId { get; set; }
            public string LedgerName { get; set; }
            public decimal? LedgerAmount { get; set; }
        }
        public class AccountsList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class HeadList
        {
            public int HeadId { get; set; }
            public string HeadName { get; set; }
            public List<AccountsList> accountlist { get; set; }
        }
    }
}