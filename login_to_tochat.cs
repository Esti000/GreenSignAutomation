using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Data_From_Html
{
    class login_to_tochat
    {
        public static string GetRepresentativesTableHtml(string username, string password)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // אם לא רוצים לפתוח דפדפן

            using (var driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl("https://app.tochat.co.il/login");

                // מילוי שדות התחברות
                driver.FindElement(By.Name("email")).SendKeys(username);
                driver.FindElement(By.Name("password")).SendKeys(password);

                // לחיצה על התחברות
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                // מחכים שהאתר יטען
                Thread.Sleep(3000);

                // מעבר למסך הנציגים
                driver.Navigate().GoToUrl("https://app.tochat.co.il/representatives");

                Thread.Sleep(3000); // המתנה שהטבלה תיטען

                // שליפת אלמנט הטבלה
                var table = driver.FindElement(By.CssSelector("table"));

                // החזרת ה-HTML של הטבלה כמחרוזת
                return table.GetAttribute("outerHTML");
            }
        }
        public static void ConvertAndSend(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");

            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet("Data");
                ws.RightToLeft = true;
                ws.Cell(1, 1).Value = "שם מלא";
                ws.Cell(1, 2).Value = "שיחות יוצאות";
                ws.Cell(1, 3).Value = "נוהלו";
                ws.Cell(1, 4).Value = "סגורות";
                ws.Cell(1, 5).Value = "סה\"כ זמן";
                ws.Range("A1:E1").Style.Font.Bold = true;

                int rowIndex = 2;
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var tds = row.SelectNodes(".//td");
                        if (tds == null || tds.Count < 7)
                            continue;

                        var name = row.SelectSingleNode(".//th//div")?.InnerText.Trim();

                        ws.Cell(rowIndex, 1).Value = name;
                        ws.Cell(rowIndex, 2).Value = tds[1].InnerText.Trim();
                        ws.Cell(rowIndex, 3).Value = tds[2].InnerText.Trim();
                        ws.Cell(rowIndex, 4).Value = tds[3].InnerText.Trim();
                        ws.Cell(rowIndex, 5).Value = tds[6].InnerText.Trim();

                        rowIndex++;
                    }
                }

                var tableRange = ws.Range(1, 1, rowIndex - 1, 5);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // שמירה בזיכרון במקום בדיסק
                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0; // מחזירים את המצב ההתחלתי כדי שה־Attachment יקרא את כל הקובץ

                    SendExcelFromStream(ms, "output.xlsx");
                }
            }
        }

        public static void SendExcelFromStream(Stream excelStream, string fileName)
        {
            var fromAddress = new MailAddress("estimesikamz@gmail.com", "Mail");
            var toAddress = new MailAddress("estimasika1@gmail.com");
            const string fromPassword = "dpcnupaylluqkrhk";
            string yesterday = DateTime.Now.ToString("dd/MM/yyyy");
            string subject = $"Tochat - סיכום שיחות נציגים תאריך: {yesterday}";
            const string body = "מצורף אקסל סיכום שיחות";

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                // יוצרים attachment ישירות מה־MemoryStream
                message.Attachments.Add(new Attachment(excelStream, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
                smtp.Send(message);
            }
        }

        static void Main(string[] args)
        {
            
            string username = "kfir-pitaron+magen@outlook.co.il";

            
            string password = "leahf1234";

            try
            {
                string tableHtml = GetRepresentativesTableHtml(username, password);
                string folderPath = "C:\\Users\\User1\\Documents\\פרויקט תזמון לילי שליחת אקסל\\Data_From_Green_Sign";
                string yesterday = DateTime.Now/*.AddDays(-1)*/.ToString("dd-MM-yyyy");
                string filename = $"אקסל סטטוס Tochat : {yesterday}.xlsx";
                ConvertAndSend(tableHtml);
                /*HtmlToExcelConverter.Convert(tableHtml, folderPath, filename);
                string fullPath = Path.Combine(folderPath, filename);
                SendExcelByEmail.SendExcel(fullPath);*/
                /*Console.WriteLine("HTML of the table:");
                Console.WriteLine(tableHtml);*/
                /*try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    // לוג אם יש שגיאה במחיקה
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("The end...");
            //Console.ReadKey();
        
        }
    }

}
