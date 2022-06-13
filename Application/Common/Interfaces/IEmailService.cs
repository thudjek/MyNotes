namespace Application.Common.Interfaces;
public interface IEmailService
{
    Task SendConfirmationEmail(string email, string token);
    Task SendPasswordResetEmail(string email, string token);
}