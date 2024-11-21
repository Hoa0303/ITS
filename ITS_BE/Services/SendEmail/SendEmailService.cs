using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MailKit.Security;

namespace ITS_BE.Services.SendEmail
{
    public class EmailSetting
    {
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
    public class SendEmailService : ISendEmailService
    {
        private readonly EmailSetting _setting;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SendEmailService(IOptions<EmailSetting> settings, IWebHostEnvironment webHostEnvironment)
        {
            _setting = settings.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMess)
        {
            var Message = new MimeMessage();
            Message.Sender = new MailboxAddress(_setting.DisplayName, _setting.Email);
            Message.From.Add(new MailboxAddress(_setting.DisplayName, _setting.Email));
            Message.To.Add(MailboxAddress.Parse(email));
            Message.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMess
            };

            Message.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_setting.Host, _setting.Port, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync(_setting.Email, _setting.Password);
                    await smtp.SendAsync(Message);
                }
                catch (Exception ex)
                {
                    Directory.CreateDirectory("MailsSave");
                    var emailsavefile = string.Format(@"MailsSave/{0}.txt", email + Guid.NewGuid());
                    await Message.WriteToAsync(emailsavefile);
                    await File.AppendAllTextAsync(emailsavefile, ex.Message);
                }
                await smtp.DisconnectAsync(true);
            }
        }
        public string GetPathOrderConfirm => Path.Combine(_webHostEnvironment.ContentRootPath, "Template", "confirm_email.txt");
        public string GetPathProductList => Path.Combine(_webHostEnvironment.ContentRootPath, "Template", "list_product.txt");
        //public string GetPathOrderConfirm()
        //{
        //    var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Template", "confirm_email.txt");
        //    return path;
        //}
        //public string GetPathProductList()
        //{
        //    var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Template", "list_product.txt");
        //    return path;
        //}
    }
}
