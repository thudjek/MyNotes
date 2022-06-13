using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;
public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailService> _logger;
    public SendGridEmailService(ISendGridClient sendGridClient, IConfiguration configuration, ILogger<SendGridEmailService> logger)
    {
        _sendGridClient = sendGridClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendConfirmationEmail(string email, string token)
    {
        var url = _configuration["SendGrid:EmailConfirmationUrl"].Replace("*email*", email).Replace("*token*", token);

        var sendGridMessage = new SendGridMessage();
        sendGridMessage.SetFrom(_configuration["SendGrid:From"], _configuration["SendGrid:FromDisplayName"]);
        sendGridMessage.AddTo(email);
        sendGridMessage.SetSubject("MyNotes Email Confirmation");
        sendGridMessage.SetTemplateId(_configuration["SendGrid:EmailConfirmationTemplateId"]);
        sendGridMessage.SetTemplateData(new { confirmationUrl = url });

        var emailResponse = await _sendGridClient.SendEmailAsync(sendGridMessage);

        if (!emailResponse.IsSuccessStatusCode)
        {
            var message = await emailResponse.Body.ReadAsStringAsync();
            _logger.LogError("Error while trying to send confirmation email. Message: {message}", message);
        }
    }

    public async Task SendPasswordResetEmail(string email, string token)
    {
        var url = _configuration["SendGrid:PasswordResetUrl"].Replace("*email*", email).Replace("*token*", token);

        var sendGridMessage = new SendGridMessage();
        sendGridMessage.SetFrom(_configuration["SendGrid:From"], _configuration["SendGrid:FromDisplayName"]);
        sendGridMessage.AddTo(email);
        sendGridMessage.SetSubject("MyNotes Password Reset");
        sendGridMessage.SetTemplateId(_configuration["SendGrid:PasswordResetTemplateId"]);
        sendGridMessage.SetTemplateData(new { passwordResetUrl = url });

        var emailResponse = await _sendGridClient.SendEmailAsync(sendGridMessage);

        if (!emailResponse.IsSuccessStatusCode)
        {
            var message = await emailResponse.Body.ReadAsStringAsync();
            _logger.LogError("Error while trying to send password reset email. Message: {message}", message);
        }
    }
}