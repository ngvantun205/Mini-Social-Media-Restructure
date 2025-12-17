using System.Net;
using System.Net.Mail;

namespace Mini_Social_Media.AppService {
    public class EmailService : IEmailService {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config) {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message) {
            try {
                var mailSettings = _config.GetSection("EmailSettings");

                string mailServer = mailSettings["MailServer"];
                int mailPort = int.Parse(mailSettings["MailPort"]);
                string senderEmail = mailSettings["SenderEmail"];
                string password = mailSettings["Password"];
                string senderName = mailSettings["SenderName"];
                var client = new SmtpClient(mailServer, mailPort) {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mailMessage = new MailMessage {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true 
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex) {
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                throw;
            }
        }
    }
}