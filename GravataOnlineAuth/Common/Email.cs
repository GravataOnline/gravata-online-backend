using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Common.Email
{
    public class Mail
    {
        public static string SENDER { get; set; }
        public static string USER { get; set; }
        public static string SENDERNAME { get; set; }
        public static string MAILPORT { get; set; }
        public static string MAILSERVER { get; set; }
        public static string PASSWORD { get; set; }
        public static string USESSL { get; set; }
        public static string USEAUTHENTICATION { get; set; }
    }
    public class MailServices
    {
        public static async Task<string> SendMailAsync(string email, string assunto, string message)
        {
            try
            {

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(Mail.SENDER, Mail.SENDERNAME)
                };

                mail.To.Add(new MailAddress(email));

                mail.Subject = assunto;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;


                using (SmtpClient smtp = new SmtpClient(Mail.MAILSERVER, Int32.Parse(Mail.MAILPORT)))
                {
                    smtp.Credentials = new NetworkCredential(Mail.USER, Mail.PASSWORD);
                     if(Mail.USESSL == "True") smtp.EnableSsl = true;
                     else smtp.EnableSsl = false;

                    await smtp.SendMailAsync(mail);
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return $"Erro ao enviar email: {ex.Message}";
            }
        }
    }
}