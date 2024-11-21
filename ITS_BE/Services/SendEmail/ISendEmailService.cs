namespace ITS_BE.Services.SendEmail
{
    public interface ISendEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMess);
        string GetPathOrderConfirm { get; }
        string GetPathProductList { get; }
    }
}
