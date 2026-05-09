using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace VehicleParts.API.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendInvoiceEmail(
        string toEmail,
        string customerName,
        int invoiceId,
        decimal finalAmount
    )
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(
            _configuration["EmailSettings:FromEmail"]
        ));

        email.To.Add(MailboxAddress.Parse(toEmail));

        email.Subject = $"Vehicle Parts Invoice INV-{invoiceId}";

        email.Body = new TextPart("html")
        {
            Text = $@"
                <h2>Vehicle Parts Invoice</h2>
                <p>Dear {customerName},</p>
                <p>Your sales invoice has been generated successfully.</p>
                <p><b>Invoice ID:</b> INV-{invoiceId}</p>
                <p><b>Total Amount:</b> Rs. {finalAmount}</p>
                <p>Thank you for choosing VehicleParts.</p>
            "
        };

        using var smtp = new SmtpClient();

        smtp.Connect(
            _configuration["EmailSettings:SmtpHost"],
            int.Parse(_configuration["EmailSettings:SmtpPort"]!),
            SecureSocketOptions.StartTls
        );

        smtp.Authenticate(
            _configuration["EmailSettings:FromEmail"],
            _configuration["EmailSettings:AppPassword"]
        );

        smtp.Send(email);
        smtp.Disconnect(true);
    }
}