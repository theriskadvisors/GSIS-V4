using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEA_Application.CustomModel
{
    public static  class EmailDesign
    {
        public static string StudentAssignementSubmittedTemplate(string TeacherName, string SubjectName, string ClassName, string SectionName, string LessonName, string StudentName)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi  " + TeacherName + ", </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> Assignment Submitted </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> Your Assignment has been submitted by  '" + StudentName + "'<br>  Subject: " + SubjectName + " <br>  Class: " + ClassName + " <br> Section: " + SectionName + " <br> Lesson: " + LessonName + " <br><br> Thanks </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'> Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }
        public static string TeacherAssignmentCheckedTemplate(string TeacherName, string StudentName, string LessonName, string SubjectName)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi  " + StudentName + ", </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> Assignment Checked </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> Miss '" + TeacherName + "'has checked your assignment and provided her feedback for '" + SubjectName + " , " + LessonName + " '.Kindly check it on VLEP.'<br/>   <br/>Thanks </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'> Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }

        public static string LessonEmailTemplate(string name)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi  Branch Admin, </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> Lesson Created </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> Miss Irum has created a lesson and sent you for your approval. Your immediate action is required.  <br/>   <br/> Thanks. </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'> Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }
        public static string SignupEmailTemplate(string name, string username, string password)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi " + name + ", </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> Your registration has been completed </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> Below is your login information. Please click on the button below to access your GSIS VLEP.  <br/> <br/> Login ID : " + username + " <br/> Password: " + password + "  <br/> For security reasons, you are advised to change your password. </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'>Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }
        public static string CommentsEmailTemplate(string TeacherName, string StudentName, string Comment)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi " + TeacherName + ", </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> Question Post </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> " + StudentName + " has asked the below question. Your immediate action is required.  <br/>   <br/> " + Comment + " </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'> Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }

        public static string TecherLessonTemplate(string TeacherName, string BranchHeadName, string LessonName)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi  " + TeacherName + ", </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> Lesson Published </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> Your Lesson '" + LessonName + "' has been published by your branch head '" + BranchHeadName + "'<br/>   <br/>Thanks </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'> Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }
        public static string StudentLessonTemplate(string StudentName, string LessonName, string LessonDate)
        {
            string template = "";


            template += "<div style='direction:ltr;padding-top:44px;background-color:#f7fafe'>";

            template += "<div style='direction:ltr;padding-top:30px;padding-right:50px;padding-bottom:30px;padding-left:50px;background:#ffffff;background-color:#ffffff;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;background:#ffffff;background-color:#ffffff;width:100%;border-radius:2px' width='100%' bgcolor='#ffffff'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";
            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:left;vertical-align:top' align='left' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:collapse;border-spacing:0px'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;width:36px' width='36'>";
            template += "<img alt='person' height='36' src='http://www.sea-gsis.com//Content/Images/gsis%20logo.png' style='direction:ltr;border:0;border-radius:0px;display:block;outline:none;text-decoration:none;height:36px;width:100%' width='36' class='CToWUd'>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-left:16px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:24px;font-weight:500;line-height:36px;text-align:left;color:#000000'> GSIS sent you a notification </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='padding-top:20px;padding-bottom:20px;font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<p style='direction:ltr;border-top:solid 1px #e0e5e8;font-size:1;margin:0px auto;width:100%'>";
            template += "</p>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:7px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> Hi  " + StudentName + ", </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:300;line-height:30px;text-align:left;color:#000000'> </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:22px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;Margin:0px auto;border-radius:2px;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%;border-radius:2px' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:solid 1px #dadfe4;direction:ltr;font-size:0px;padding:19px 30px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;padding-bottom:10px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:500;line-height:30px;text-align:left;color:#000000'> " + LessonName + " </div>";
            template += "</td>";
            template += "</tr>";
            template += "<tr style='direction:ltr'>";
            template += "<td align='left' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<div style='direction:ltr;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:16px;font-weight:300;line-height:150%;text-align:left;color:#333333'> You have the following Lesson scheduled for tomorrow  <br/>   <br/> " + LessonName + " -- " + LessonDate + "<br/>   <br/>Please attend on time.<br/>   <br/>Thanks  </div>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:36px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' style='direction:ltr;font-size:0px;padding:0px;word-break:break-word'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr;border-collapse:separate;line-height:100%'>";
            template += "<tbody><tr style='direction:ltr'>";
            template += "<td align='center' bgcolor='#459fed' style='direction:ltr;border:0;border-radius:100px;height:42px;padding:0px;background:#459fed' valign='middle' height='42'>";
            template += "<a href='http://www.sea-gsis.com/' style='direction:ltr;padding:10px 36px;border-radius:100px;background:#459fed;color:#ffffff;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-size:18px;font-weight:normal;line-height:120%;Margin:0;text-decoration:none;text-transform:none' target='_blank' data-saferedirecturl='http://www.sea-gsis.com/'> Visit GSIS VLEP </a>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr style='direction:ltr'>";

            template += "</tr>";
            template += "<tr style='direction:ltr'>";

            template += "</tr>";
            template += "</tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "<div style='direction:ltr;padding-top:14px;padding-bottom:60px;Margin:0px auto;max-width:600px'>";
            template += "<table align='center' border='0' cellpadding='0' cellspacing='0' style='direction:ltr;width:100%' width='100%'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='border:0;direction:ltr;font-size:0px;padding:0px;text-align:center;vertical-align:top' align='center' valign='top'>";

            template += "<div style='font-size:13px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='direction:ltr'>";
            template += "<tbody style='direction:ltr'>";
            template += "<tr style='direction:ltr'>";
            template += "<td style='direction:ltr;border:0;vertical-align:top;padding:0px' valign='top'>";
            template += "<table border='0' cellpadding='0' cellspacing='0' style='direction:ltr' width='100%'>";
            template += "<tbody><tr><td><div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> ${uniqueIdentifier} </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> <a href='http://www.sea-gsis.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.cssofficersonline.com/account/settings&amp;source=gmail&amp;ust=1588314992974000&amp;usg=AFQjCNE0_-U4ajnOLOVDW6YooATKb2vdbQ'>https://www.cssofficersonline.<wbr>com/account/settings</a> </div>";
            template += "<div style='direction:ltr;display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden'> en </div>";
            template += "</td></tr></tbody></table>";
            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</td>";
            template += "</tr>";
            template += "</tbody>";
            template += "</table>";
            template += "</div>";

            template += "</div>";
            template += "</body>";


            return template;
        }
    }
}