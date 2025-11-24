using System.Net.Mail;
using System.Net;

namespace Data_From_Html
{
    public class SendExcelByEmail
    {
        public static void SendExcel(string excelFilePath)
        {
            var fromAddress = new MailAddress("estimesikamz@gmail.com", "Mail");
            var toAddress = new MailAddress("estimasika1@gmail.com");
            const string fromPassword = "dpcnupaylluqkrhk"; // לא הסיסמה הרגילה!
            string yesterday = DateTime.Now/*.AddDays(-1)*/.ToString("dd/MM/yyyy");
            string subject = $" Tochat - סיכום שיחות נציגים תאריך: {yesterday}";
           //
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
                message.Attachments.Add(new Attachment(excelFilePath));
                smtp.Send(message);
               

            }
        }
    }
}
