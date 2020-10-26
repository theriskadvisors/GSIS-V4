using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEA_Application.Controllers
{
    public class ChartOfAccountsController : Controller
    {
        Sea_Entities db = new Sea_Entities();
        // GET: ChartOfAccounts
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChartsOf_Accounts()
        {
            return View();
        }
        public ActionResult BalanceSheet()
        {
            return View();
        }

        public JsonResult GetChart()
        {
            var headlist = db.LedgerHeads.OrderBy(x => x.Name).ToList();
            List<Ledger_Head> ledgerheadlist = new List<Ledger_Head>();
            foreach (var h_item in headlist)
            {
                Ledger_Head ledgerhead = new Ledger_Head();
                ledgerhead.HeadName = h_item.Name;
                ledgerhead.HeadId = h_item.Id;

                var grouplist = db.LedgerGroups.Where(x => x.LedgerHeadID == h_item.Id).OrderBy(x => x.Name).ToList();
                var _ledgerlist = db.Ledgers.Where(x => x.LedgerHeadId == h_item.Id).OrderBy(x => x.Name).ToList();

                ledgerhead.ledgerGroup = new List<Ledger_Group>();
                ledgerhead.ledger = new List<_Ledger>();

                foreach (var g_item in grouplist)
                {
                    Ledger_Group lg = new Ledger_Group();
                    lg.GroupId = g_item.Id;
                    lg.GroupName = g_item.Name;
                    lg.ledger = new List<_Ledger>();

                    var ledgerlist = db.Ledgers.Where(x => x.Id == lg.GroupId).OrderBy(x => x.Name).ToList();
                    foreach (var l_item in ledgerlist)
                    {
                        _Ledger l = new _Ledger();
                        l.LedgerId = l_item.Id;
                        l.LedgerName = l_item.Name;
                        l.Balance = Convert.ToDouble(l_item.CurrentBalance);
                        lg.ledger.Add(l);
                    }
                    ledgerhead.ledgerGroup.Add(lg);
                }
                foreach (var item in _ledgerlist)
                {
                    _Ledger l = new _Ledger();
                    l.LedgerId = item.Id;
                    l.LedgerName = item.Name;
                    l.Balance = Convert.ToDouble(item.CurrentBalance);
                    ledgerhead.ledger.Add(l);
                }
                ledgerheadlist.Add(ledgerhead);
            }
            return Json(ledgerheadlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChart1()
        {
            var headlist = db.LedgerHeads.OrderBy(x => x.Name).ToList();
            List<Ledger_Head> ledgerheadlist = new List<Ledger_Head>();
            foreach (var h_item in headlist)

            {
                Ledger_Head ledgerhead = new Ledger_Head();
                ledgerhead.HeadName = h_item.Name;
                ledgerhead.HeadId = h_item.Id;

                var grouplist = db.LedgerGroups.Where(x => x.LedgerHeadID == h_item.Id).OrderBy(x => x.Name).ToList();
                var _ledgerlist = db.Ledgers.Where(x => x.LedgerHeadId == h_item.Id).OrderBy(x => x.Name).ToList();

                ledgerhead.ledgerGroup = new List<Ledger_Group>();
                ledgerhead.ledger = new List<_Ledger>();

                foreach (var g_item in grouplist)
                {
                    Ledger_Group lg = new Ledger_Group();
                    lg.GroupId = g_item.Id;
                    lg.GroupName = g_item.Name;
                    lg.ledger = new List<_Ledger>();

                    var ledgerlist = db.Ledgers.Where(x => x.LedgerGroupId == lg.GroupId).OrderBy(x => x.Name).ToList();
                    foreach (var l_item in ledgerlist)
                    {
                        _Ledger l = new _Ledger();
                        l.LedgerId = l_item.Id;
                        l.LedgerName = l_item.Name;
                        l.Balance = Convert.ToDouble(l_item.CurrentBalance);
                        lg.ledger.Add(l);
                    }
                    ledgerhead.ledgerGroup.Add(lg);
                }
                List<_Ledger> AllLedgersList = new List<_Ledger>();
                foreach (var item in _ledgerlist)
                {
                    _Ledger l = new _Ledger();
                    l.LedgerId = item.Id;
                    l.LedgerName = item.Name;
                    l.Balance = Convert.ToDouble(item.CurrentBalance);

                    SEA_Application.Controllers.ChartOfAccountsController.Ledger_Group lGroup = new SEA_Application.Controllers.ChartOfAccountsController.Ledger_Group();
                    //lGroup.Id = 0;

                    if (item.LedgerGroup != null)
                    {
                        lGroup.GroupName = item.LedgerGroup.Name;
                    }
                    else
                    {

                        lGroup.GroupName = null;
                    }

                    l.ledgerGroup = lGroup;

                    AllLedgersList.Add(l);
                }

                AllLedgersList = AllLedgersList.OrderByDescending(x => x.ledgerGroup.GroupName).ToList();

                ledgerhead.ledger.AddRange(AllLedgersList);

                ledgerheadlist.Add(ledgerhead);
            }
            return Json(ledgerheadlist, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetBalanceSheet(string FromDate, string ToDate)
        {

            DateTime dateTimeTo = Convert.ToDateTime(ToDate);

            dateTimeTo = dateTimeTo.AddDays(1);

            string toDateInString = dateTimeTo.ToString();

            List<_Ledger> BalanceSheetList = new List<_Ledger>();

            var BalanceSheetListStoredProcedure = db.LedgersByDate(FromDate, toDateInString).ToList();

            foreach (var BalanceSheet in BalanceSheetListStoredProcedure)
            {
                _Ledger ledger = new _Ledger();
                Ledger_Head lh = new Ledger_Head();

                ledger.LedgerId = BalanceSheet.LedgerId;
                ledger.LedgerName = BalanceSheet.LedgerName;
                // ledger.LedgerHead.HeadName = BalanceSheet.LedgerType;
                lh.HeadName = BalanceSheet.LedgerType;
                ledger.Balance = Convert.ToDouble(BalanceSheet.LedgerAmount);
                ledger.LedgerHead = lh;
                BalanceSheetList.Add(ledger);
            }

            List<Ledger_Head> ledgerheadlist = new List<Ledger_Head>();

            //if (BalanceSheetList.Count() != 0)
            //{

            ledgerheadlist = GetLedgers(BalanceSheetList);

            // }

            return Json(ledgerheadlist, JsonRequestBehavior.AllowGet);
            //var ledgers = from voucher in db.Vouchers
            //              join voucherRecord in db.VoucherRecords on voucher.Id equals  voucherRecord.VoucherId
            //              join ledger in db.Ledgers on voucherRecord.LedgerId equals ledger.Id
            //              where voucher.Date >= dateTimeFrom && voucher.Date <= dateTimeTo
            //              select voucher;

        }

        public ActionResult GetProfitAndLoss(int BranchId, string FromDate, string ToDate)
        {
            List<_Ledger> BalanceSheetList = new List<_Ledger>();

            if (FromDate != "" && ToDate != "" && BranchId != 0)
            {
                DateTime dateTimeTo = Convert.ToDateTime(ToDate);

                dateTimeTo = dateTimeTo.AddDays(1);

                string toDateInString = dateTimeTo.ToString();

                var ProfitAndLossByBranchAndDate = db.ProfitAndLossByBranchAndDate(FromDate, ToDate, BranchId).ToList();

                foreach (var BalanceSheet in ProfitAndLossByBranchAndDate)
                {
                    _Ledger ledger = new _Ledger();
                    Ledger_Head lh = new Ledger_Head();

                    ledger.LedgerId = BalanceSheet.LedgerId;
                    ledger.LedgerName = BalanceSheet.LedgerName;
                    //   ledger.LedgerHead.HeadName = BalanceSheet.LedgerType;
                    lh.HeadName = BalanceSheet.LedgerType;
                    ledger.Balance = Convert.ToDouble(BalanceSheet.LedgerAmount);

                    ledger.LedgerHead = lh;

                    BalanceSheetList.Add(ledger);

                }

            }
            else
            {
                var ProfitAndLossByBranch = db.ProfitAndLossByBranch(BranchId).ToList();

                foreach (var BalanceSheet in ProfitAndLossByBranch)
                {
                    _Ledger ledger = new _Ledger();
                    Ledger_Head lh = new Ledger_Head();

                    ledger.LedgerId = BalanceSheet.LedgerId;
                    ledger.LedgerName = BalanceSheet.LedgerName;
                    // ledger.LedgerHead.HeadName = BalanceSheet.LedgerType;
                    lh.HeadName = BalanceSheet.LedgerType;
                    ledger.Balance = Convert.ToDouble(BalanceSheet.LedgerAmount);

                    ledger.LedgerHead = lh;

                    BalanceSheetList.Add(ledger);

                }

            }
            List<Ledger_Head> ledgerheadlist = new List<Ledger_Head>();

            ////  if (BalanceSheetList.Count() != 0)
            //  {

            ledgerheadlist = GetLedgers(BalanceSheetList);

            // }

            return Json(ledgerheadlist, JsonRequestBehavior.AllowGet);


        }
        public ActionResult TrialBalance()
        {

            return View();
        }
        public ActionResult GetTrialBalanceList()
        {
            List<TrialBalanceClass> TrialBalanceList = new List<TrialBalanceClass>();

            var AllLedgersList = db.Ledgers.ToList();

            var AllDebitCreditLedgerList = db.LedgersDebitCreditList().ToList();

            foreach (var item in AllLedgersList)
            {
                TrialBalanceClass trialBalance = new TrialBalanceClass();
                trialBalance.LedgerId = item.Id;
                trialBalance.LedgerName = item.Name;

                if (item.LedgerGroup != null)
                {
                    trialBalance.GroupName = item.LedgerGroup.Name;
                }
                else
                {
                    trialBalance.GroupName = null;
                }

                //  trialBalance.GroupName = item.LedgerGroup.Name;

                trialBalance.HeadName = item.LedgerHead.Name;
                trialBalance.HeadId = item.LedgerHead.Id;

                //if (trialBalance.HeadName == "Assets" || trialBalance.HeadName == "Expense")
                //{

                //    trialBalance.DebitAmount = "0";
                //    trialBalance.CreditAmount = "-";
                //}
                //else
                //{
                //    trialBalance.DebitAmount = "-";
                //    trialBalance.CreditAmount = "0";

                //}
                trialBalance.CreditAmount = "0";
                trialBalance.DebitAmount = "0";

                TrialBalanceList.Add(trialBalance);
            }
            var TotalDebit = 0.0;
            var TotalCredit = 0.0;


            foreach (var item in AllDebitCreditLedgerList)
            {
                var TrialBalanceToUpdate = TrialBalanceList.Where(x => x.LedgerId == item.LedgerId).FirstOrDefault();

                if (TrialBalanceToUpdate != null)
                {

                    if (item.LedgerHead == "Assets" || item.LedgerHead == "Expense")
                    {
                        if (item.LedgerType == "Dr")
                        {
                            TrialBalanceToUpdate.DebitAmount = item.LedgerAmount.ToString();
                           // TotalDebit = TotalDebit + Convert.ToDouble(item.LedgerAmount);
                        }
                        else
                        {
                            TrialBalanceToUpdate.CreditAmount = item.LedgerAmount.ToString();
                          //  TotalCredit = TotalCredit + Convert.ToDouble(item.LedgerAmount);
                        }
                    }
                    else
                    {
                        if (item.LedgerType == "Cr")
                        {
                            TrialBalanceToUpdate.CreditAmount = item.LedgerAmount.ToString();
                          //  TotalCredit = TotalCredit + Convert.ToDouble(item.LedgerAmount);
                        }
                        else
                        {
                            TrialBalanceToUpdate.DebitAmount = item.LedgerAmount.ToString();
                           // TotalCredit = TotalCredit + Convert.ToDouble(item.LedgerAmount);
                        }
                    }


                }
            }


            foreach (var TrialBalance1 in TrialBalanceList)
            {
                if (TrialBalance1.HeadName == "Assets" || TrialBalance1.HeadName == "Expense")
                {
                    TrialBalance1.DebitAmount = ( Convert.ToInt32( TrialBalance1.DebitAmount) - Convert.ToInt32( TrialBalance1.CreditAmount)).ToString();
                    TotalDebit = TotalDebit + Convert.ToDouble(TrialBalance1.DebitAmount);
                    TrialBalance1.CreditAmount = "-";
                }
                else
                {
                    TrialBalance1.CreditAmount = ( Convert.ToInt32( TrialBalance1.CreditAmount )- Convert.ToInt32( TrialBalance1.DebitAmount)).ToString();
                    TotalCredit = TotalCredit + Convert.ToDouble(TrialBalance1.CreditAmount);
                    TrialBalance1.DebitAmount = "-";
                }

            }

            //  TrialBalanceTotal.CreditTotal = TotalCredit.ToString();
            //  TrialBalanceTotal.DebitTotal = TotalDebit.ToString();
            //  TrialBalanceTotal.TrailBalanceList = TrialBalanceList;

            List<TrialBalanceTotal> TrialBalanceTotalList = new List<TrialBalanceTotal>();

            var headlist = db.LedgerHeads.OrderBy(x => x.Name).ToList();

            TrialBalanceSheet TrialBalance = new TrialBalanceSheet();

            foreach (var h_item in headlist)
            {
                TrialBalanceTotal ledgerhead = new TrialBalanceTotal();
                ledgerhead.HeadName = h_item.Name;
                ledgerhead.HeadId = h_item.Id;

                var _ledgerlist = TrialBalanceList.Where(x => x.HeadId == h_item.Id).OrderBy(x => x.LedgerName).ToList();

                ledgerhead.TrailBalanceList = new List<TrialBalanceClass>();


                List<TrialBalanceClass> TrialBalanceClassList = new List<TrialBalanceClass>();

                foreach (var item in _ledgerlist)
                {
                    TrialBalanceClass obj = new TrialBalanceClass();

                    obj.HeadId = item.HeadId;
                    obj.HeadName = item.HeadName;
                    obj.LedgerId = item.LedgerId;
                    obj.LedgerName = item.LedgerName;
                    obj.DebitAmount = item.DebitAmount;
                    obj.CreditAmount = item.CreditAmount;
                    if (item.GroupName != null)
                    {

                        obj.GroupName = item.GroupName;
                    }
                    else
                    {
                        obj.GroupName = null;

                    }

                    TrialBalanceClassList.Add(obj);

                }

                TrialBalanceClassList = TrialBalanceClassList.OrderByDescending(x => x.GroupName).ToList();
                ledgerhead.TrailBalanceList.AddRange(TrialBalanceClassList);

                TrialBalanceTotalList.Add(ledgerhead);

                TrialBalance.DebitTotal = TotalDebit.ToString();
                TrialBalance.CreditTotal = TotalCredit.ToString();
                TrialBalance.TrialBalanceTotal = TrialBalanceTotalList;
            }

            return Json(TrialBalance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTrialBalanceByDate(string ToDate)
        {

            if (ToDate != "")
            {
                DateTime dateTimeTo = Convert.ToDateTime(ToDate);

                dateTimeTo = dateTimeTo.AddDays(1);

                string toDateInString = dateTimeTo.ToString();

                List<TrialBalanceClass> TrialBalanceList = new List<TrialBalanceClass>();

                var AllLedgersList = db.Ledgers.ToList();

                var AllDebitCreditLedgerList = db.LedgersDebitCreditListByDate(toDateInString).ToList();

                foreach (var item in AllLedgersList)
                {
                    TrialBalanceClass trialBalance = new TrialBalanceClass();
                    trialBalance.LedgerId = item.Id;
                    trialBalance.LedgerName = item.Name;

                    if (item.LedgerGroup != null)
                    {
                        trialBalance.GroupName = item.LedgerGroup.Name;
                    }
                    else
                    {
                        trialBalance.GroupName = null;
                    }

                    //  trialBalance.GroupName = item.LedgerGroup.Name;

                    trialBalance.HeadName = item.LedgerHead.Name;
                    trialBalance.HeadId = item.LedgerHead.Id;
                    trialBalance.DebitAmount = "0";
                    trialBalance.CreditAmount = "0";


                    TrialBalanceList.Add(trialBalance);
                }
                var TotalDebit = 0.0;
                var TotalCredit = 0.0;


                foreach (var item in AllDebitCreditLedgerList)
                {
                    var TrialBalanceToUpdate = TrialBalanceList.Where(x => x.LedgerId == item.LedgerId).FirstOrDefault();

                    if (TrialBalanceToUpdate != null)
                    {


                        if (item.LedgerHead == "Assets" || item.LedgerHead == "Expense")
                        {
                            if (item.LedgerType == "Dr")
                            {
                                TrialBalanceToUpdate.DebitAmount = item.LedgerAmount.ToString();
                            
                            }
                            else
                            {
                                TrialBalanceToUpdate.CreditAmount = item.LedgerAmount.ToString();
                             
                            }


                        }
                        else
                        {

                            if (item.LedgerType == "Cr")
                            {
                                TrialBalanceToUpdate.CreditAmount = item.LedgerAmount.ToString();
                              
                            }
                            else
                            {
                                TrialBalanceToUpdate.DebitAmount = item.LedgerAmount.ToString();
                             
                            }

                        }


                    }
                }

                foreach (var TrialBalance1 in TrialBalanceList)
                {
                    if (TrialBalance1.HeadName == "Assets" || TrialBalance1.HeadName == "Expense")
                    {
                        TrialBalance1.DebitAmount = (Convert.ToInt32(TrialBalance1.DebitAmount) - Convert.ToInt32(TrialBalance1.CreditAmount)).ToString();
                        TotalDebit = TotalDebit + Convert.ToDouble(TrialBalance1.DebitAmount);
                        TrialBalance1.CreditAmount = "-";
                    }
                    else
                    {
                        TrialBalance1.CreditAmount = (Convert.ToInt32(TrialBalance1.CreditAmount) - Convert.ToInt32(TrialBalance1.DebitAmount)).ToString();
                        TotalCredit = TotalCredit + Convert.ToDouble(TrialBalance1.CreditAmount);
                        TrialBalance1.DebitAmount = "-";
                    }

                }

                List<TrialBalanceTotal> TrialBalanceTotalList = new List<TrialBalanceTotal>();

                var headlist = db.LedgerHeads.OrderBy(x => x.Name).ToList();

                TrialBalanceSheet TrialBalance = new TrialBalanceSheet();

                foreach (var h_item in headlist)
                {
                    TrialBalanceTotal ledgerhead = new TrialBalanceTotal();
                    ledgerhead.HeadName = h_item.Name;
                    ledgerhead.HeadId = h_item.Id;

                    var _ledgerlist = TrialBalanceList.Where(x => x.HeadId == h_item.Id).OrderBy(x => x.LedgerName).ToList();

                    ledgerhead.TrailBalanceList = new List<TrialBalanceClass>();


                    List<TrialBalanceClass> TrialBalanceClassList = new List<TrialBalanceClass>();

                    foreach (var item in _ledgerlist)
                    {
                        TrialBalanceClass obj = new TrialBalanceClass();

                        obj.HeadId = item.HeadId;
                        obj.HeadName = item.HeadName;
                        obj.LedgerId = item.LedgerId;
                        obj.LedgerName = item.LedgerName;
                        obj.DebitAmount = item.DebitAmount;
                        obj.CreditAmount = item.CreditAmount;
                        if (item.GroupName != null)
                        {

                            obj.GroupName = item.GroupName;
                        }
                        else
                        {
                            obj.GroupName = null;

                        }

                        TrialBalanceClassList.Add(obj);

                    }

                    TrialBalanceClassList = TrialBalanceClassList.OrderByDescending(x => x.GroupName).ToList();
                    ledgerhead.TrailBalanceList.AddRange(TrialBalanceClassList);

                    TrialBalanceTotalList.Add(ledgerhead);

                    TrialBalance.DebitTotal = TotalDebit.ToString();
                    TrialBalance.CreditTotal = TotalCredit.ToString();
                    TrialBalance.TrialBalanceTotal = TrialBalanceTotalList;
                }

                return Json(TrialBalance, JsonRequestBehavior.AllowGet);

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public List<Ledger_Head> GetLedgers(List<_Ledger> BalanceSheetList)
        {
            // DateTime dateTimeFrom = Convert.ToDateTime(FromDate);
            //DateTime dateTimeTo = Convert.ToDateTime(ToDate);

            //dateTimeTo = dateTimeTo.AddDays(1);
            //string toDateInString = dateTimeTo.ToString();
            //var AllLedgersByDate = db.LedgersByDate(FromDate, toDateInString).ToList();

            var AllLedgers = db.Ledgers.ToList();

            List<LedgerSearch> LedgerSearchList = new List<LedgerSearch>();

            foreach (var ledger in AllLedgers)
            {
                LedgerSearch ledgerSearch = new LedgerSearch();

                if (ledger.LedgerGroup != null)
                {
                    ledgerSearch.GroupName = ledger.LedgerGroup.Name;
                }
                else
                {
                    ledgerSearch.GroupName = null;
                }

                ledgerSearch.HeadName = ledger.LedgerHead.Name;
                ledgerSearch.HeadId = ledger.LedgerHead.Id;
                ledgerSearch.LedgerName = ledger.Name;
                ledgerSearch.LedgerType = null;
                ledgerSearch.LedgerId = ledger.Id;
                ledgerSearch.DebitAmount = 0;
                ledgerSearch.CreditAmount = 0;
                ledgerSearch.CalculatedAmount = 0;
                // ledgerSearch.Amount = 0;
                LedgerSearchList.Add(ledgerSearch);

            }

            foreach (var ledgerSearch in LedgerSearchList)
            {
                foreach (var ledgerByDate in BalanceSheetList)
                {

                    if (ledgerSearch.LedgerId == ledgerByDate.LedgerId && ledgerByDate.LedgerHead.HeadName == "Dr")
                    {

                        ledgerSearch.DebitAmount = Convert.ToDouble(ledgerByDate.Balance);

                    }
                    else if (ledgerSearch.LedgerId == ledgerByDate.LedgerId && ledgerByDate.LedgerHead.HeadName == "Cr")
                    {

                        ledgerSearch.CreditAmount = Convert.ToDouble(ledgerByDate.Balance);
                    }
                    else
                    {
                    }
                }

            }

            foreach (var ledgerSearch in LedgerSearchList)
            {
                if (ledgerSearch.HeadName == "Assets" || ledgerSearch.HeadName == "Expense")
                {
                    ledgerSearch.CalculatedAmount = ledgerSearch.DebitAmount - ledgerSearch.CreditAmount;
                }
                else
                {
                    ledgerSearch.CalculatedAmount = ledgerSearch.CreditAmount - ledgerSearch.DebitAmount;
                }

            }


            List<Ledger_Head> ledgerheadlist = new List<Ledger_Head>();

            var headlist = db.LedgerHeads.OrderBy(x => x.Name).ToList();
            foreach (var h_item in headlist)

            {
                Ledger_Head ledgerhead = new Ledger_Head();
                ledgerhead.HeadName = h_item.Name;
                ledgerhead.HeadId = h_item.Id;

                var _ledgerlist = LedgerSearchList.Where(x => x.HeadId == h_item.Id).OrderBy(x => x.LedgerName).ToList();


                ledgerhead.ledger = new List<_Ledger>();


                List<_Ledger> AllLedgersList = new List<_Ledger>();
                foreach (var item in _ledgerlist)
                {
                    _Ledger l = new _Ledger();
                    l.LedgerId = item.LedgerId;
                    l.LedgerName = item.LedgerName;
                    l.Balance = Convert.ToDouble(item.CalculatedAmount);

                    SEA_Application.Controllers.ChartOfAccountsController.Ledger_Group lGroup = new SEA_Application.Controllers.ChartOfAccountsController.Ledger_Group();
                    //lGroup.Id = 0;
                    lGroup.GroupName = item.GroupName;
                    l.ledgerGroup = lGroup;

                    AllLedgersList.Add(l);
                }

                AllLedgersList = AllLedgersList.OrderByDescending(x => x.ledgerGroup.GroupName).ToList();

                ledgerhead.ledger.AddRange(AllLedgersList);

                ledgerheadlist.Add(ledgerhead);
            }

            return ledgerheadlist;


        }

        public class Ledger_Head
        {
            public string HeadName { get; set; }
            public int HeadId { get; set; }
            public List<Ledger_Group> ledgerGroup { get; set; }
            public List<_Ledger> ledger { get; set; }

        }
        public class Ledger_Group
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public List<_Ledger> ledger { get; set; }
        }
        //public class LedgerDebitCredit
        //{
        //    public int LedgerId { get; set; }
        //    public string LedgerName { get; set; }
        //    public double Amount { get; set; }
        //    public int HeadId { get; set; }
        //    public string HeadName { get; set; }
        //    public string LedgerType { get; set; }
        //    public string GroupName { get; set; }

        //}
        public class TrialBalanceClass
        {
            public int LedgerId { get; set; }
            public string LedgerName { get; set; }
            public string GroupName { get; set; }
            public string HeadName { get; set; }
            public int HeadId { get; set; }
            public string DebitAmount { get; set; }
            public string CreditAmount { get; set; }

        }
        public class TrialBalanceTotal
        {

            public string HeadName { get; set; }
            public int HeadId { get; set; }
            public List<TrialBalanceClass> TrailBalanceList { get; set; }


        }
        public class TrialBalanceSheet
        {
            public List<TrialBalanceTotal> TrialBalanceTotal = new List<TrialBalanceTotal>();
            public string DebitTotal { get; set; }
            public string CreditTotal { get; set; }

        }


        public class LedgerSearch
        {
            public int LedgerId { get; set; }
            public string LedgerName { get; set; }
            //   public double Amount { get; set; }
            public double Balance { get; set; }
            public int HeadId { get; set; }
            public string HeadName { get; set; }
            public string LedgerType { get; set; }
            public string GroupName { get; set; }

            public double DebitAmount { get; set; }
            public double CreditAmount { get; set; }

            public double CalculatedAmount { get; set; }

            public Ledger_Group ledgerGroup { get; set; }

        }

        public class _Ledger
        {
            public int LedgerId { get; set; }
            public string LedgerName { get; set; }
            public double Balance { get; set; }

            //added by shahzad
            public Ledger_Head LedgerHead { get; set; }

            public Ledger_Group ledgerGroup { get; set; }


        }

    }
}