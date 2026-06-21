using System.Net;
using System.Net.Mail;

namespace KozossegAPI.Model
{
    public class EmailVerification
    {
        public void SendVerificationEmail(string toEmail, string token)
        {
            var link = $"https://localhost:7259/api/Auth/verify?token={token}";
            var mail = new MailMessage("otvenora.info@gmail.com", toEmail);
            mail.Subject = "Regisztráció visszaigazolása";
            mail.Body = $"Kérlek kattints a linkre a fiókod aktiválásához: {link}";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("otvenora.info@gmail.com", "lpmo ufcl ndxw ktef"),
                EnableSsl = true
            };
            smtp.Send(mail);
        }
    }
}
