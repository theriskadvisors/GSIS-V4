using SEA_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
namespace SEA_Application.Controllers
{
    public class ExcelLoaderController : Controller
    {

        private Sea_Entities db = new Sea_Entities();
        //
        // GET: /ExcelLoader/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UploadFile(RegisterViewModel model)
        {

            HttpPostedFileBase file = Request.Files["students"];
            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
            }

     //       Excel.Application excelApp = new Excel.Application();
       //     Excel.Workbook workBook = excelApp.Workbooks.Open("C:/Users/Hello/Desktop/file1.xlsx");
      //      //C, E, F
         //   int[] Cols = { 1,2};
         //   string dataa = "";

          //  int i = 0;
           // int sheetno = 1;
      //      foreach (Excel.Worksheet sheet in workBook.Worksheets)
        //    {
           
          //     Excel.Sheets sheets = workBook.Worksheets;
            //   Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(sheetno);//Get the reference of second worksheet
              // string strWorksheetName = worksheet.Name;//Get the name of worksheet.
            //   sheetno++;
            //dataa += "******* " + strWorksheetName + "   ********\n";

         //       foreach (Excel.Range row in sheet.UsedRange.Rows)
           //     {
                   // foreach (int c in Cols) //changed here to loop through columns
                   // {

                    var teacherList = new List<RegisterViewModel>();
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                         //   var workSheet = currentSheet.First();
                            foreach (var workSheet in currentSheet)
                            {
                                if (workSheet.Name == "Branch_Class_Section")
                                {
                                    //  var x = currentSheet.Last();
                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;
                                    ApplicationDbContext context = new ApplicationDbContext();
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        AspNetBranch_Class classcourse = new AspNetBranch_Class();

                                        var Branch = workSheet.Cells[rowIterator, 1].Value.ToString();
                                        classcourse.BranchId = db.AspNetBranches.Where(x => x.Name == Branch).Select(x => x.Id).FirstOrDefault();
                                        var classname = workSheet.Cells[rowIterator, 2].Value.ToString();
                                        classcourse.ClassId = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
                                        classcourse.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();
                                        classcourse.IsActive = true;
                                        //db.AspNetBranch_Class.Add(classcourse);
                                        //db.SaveChanges();

                                        AspNetBranchClass_Sections branchclasssection = new AspNetBranchClass_Sections();
                                        branchclasssection.BranchClassId = classcourse.Id;
                                        var section = workSheet.Cells[rowIterator, 3].Value.ToString();
                                        branchclasssection.SectionId = db.AspNetSections.Where(x => x.Name == section).Select(x => x.Id).FirstOrDefault();
                                        branchclasssection.IsActive = true;
                                        //  db.AspNetBranchClass_Sections.Add(branchclasssection);
                                        // db.SaveChanges();
                                    }
                                }
                                else if (workSheet.Name == "Course_Package")
                                {
                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;
                                    ApplicationDbContext context = new ApplicationDbContext();
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        AspNetCoursePackage coursepkg = new AspNetCoursePackage();

                                        var course = workSheet.Cells[rowIterator, 1].Value.ToString();
                                        coursepkg.CourseId = db.AspNetCourses.Where(x => x.Name == course).Select(x => x.Id).FirstOrDefault();
                                        var pkg = workSheet.Cells[rowIterator, 2].Value.ToString();
                                        coursepkg.PackageId = db.AspNetPackages.Where(x => x.Title == pkg).Select(x => x.Id).FirstOrDefault();
                                        db.AspNetCoursePackages.Add(coursepkg);
                                        db.SaveChanges();
                                    }
                                }
                                else if (workSheet.Name == "Class_Course")
                                {
                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;
                                    ApplicationDbContext context = new ApplicationDbContext();
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        AspNetClass_Courses classcourse = new AspNetClass_Courses();

                                        var Class = workSheet.Cells[rowIterator, 1].Value.ToString();
                                        classcourse.ClassId = db.AspNetClasses.Where(x => x.Name == Class).Select(x => x.Id).FirstOrDefault();
                                        var course = workSheet.Cells[rowIterator, 2].Value.ToString();
                                        classcourse.CourseId = db.AspNetCourses.Where(x => x.Name == course).Select(x => x.Id).FirstOrDefault();

                                      //  db.AspNetClass_Courses.Add(classcourse);
                                     //   db.SaveChanges();
                                    }
                                }
                                else if (workSheet.Name == "Employees")
                                {


                                }
                                else if (workSheet.Name == "Students")
                                {

                                }
                                else if (workSheet.Name == "Teacher_Class_Subject")
                                {
                                    var noOfCol = workSheet.Dimension.End.Column;
                                    var noOfRow = workSheet.Dimension.End.Row;
                                    ApplicationDbContext context = new ApplicationDbContext();
                                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                                    {
                                        AspNetTeacher_Enrollments classcourse = new AspNetTeacher_Enrollments();

                                        var teacher = workSheet.Cells[rowIterator, 1].Value.ToString();
                                        classcourse.TeacherId = db.AspNetEmployees.Where(x => x.Name == teacher).Select(x => x.Id).FirstOrDefault();

                                        var course = workSheet.Cells[rowIterator, 2].Value.ToString();
                                        var classname = workSheet.Cells[rowIterator, 3].Value.ToString();
                                        var courseid = db.AspNetCourses.Where(x => x.Name == course).Select(x => x.Id).FirstOrDefault();
                                        var classid = db.AspNetClasses.Where(x => x.Name == classname).Select(x => x.Id).FirstOrDefault();
                                        classcourse.CourseId = db.AspNetClass_Courses.Where(x => x.ClassId == classid && x.CourseId == courseid).Select(x => x.Id).FirstOrDefault();
                                        classcourse.SessionId = db.AspNetSessions.Where(x => x.StatusId == 1).Select(x => x.Id).FirstOrDefault();

                                        var section = workSheet.Cells[rowIterator, 4].Value.ToString();
                                        classcourse.SectionId = db.AspNetSections.Where(x => x.Name == section).Select(x => x.Id).FirstOrDefault();

                                        var branch = workSheet.Cells[rowIterator, 5].Value.ToString();
                                        var branchid = db.AspNetBranches.Where(x => x.Name == branch).Select(x => x.Id).FirstOrDefault();
                                        var BCid = db.AspNetBranch_Class.Where(x => x.BranchId == branchid && x.ClassId == classid).Select(x => x.Id).FirstOrDefault();
                                        classcourse.SectionId = db.AspNetBranchClass_Sections.Where(x => x.BranchClassId == BCid).Select(x => x.Id).FirstOrDefault();

                                        db.AspNetTeacher_Enrollments.Add(classcourse);
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {


                                }
                            }
                         //   dbTransaction.Commit();
                            //return RedirectToAction("BranchClassSection", "SubjectGroups");
                        }
                  //  }
              //      dataa += "\n";
           //     }
          //  }

            //excelApp.Quit();

            return View();
        }
	}
}