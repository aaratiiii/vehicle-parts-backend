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
        decimal finalAmount,
        byte[] pdfBytes
    )
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(
            _configuration["EmailSettings:FromEmail"]
        ));

        email.To.Add(MailboxAddress.Parse(toEmail));

        email.Subject = $"Vehicle Parts Invoice INV-{invoiceId}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; background:#f4f6f8; padding:30px;'>
                <div style='max-width:650px; margin:auto; background:#ffffff; border-radius:12px; overflow:hidden; border:1px solid #e5e7eb;'>
                    <div style='background:#0f766e; padding:25px; color:white; text-align:center;'>
                        <h1 style='margin:0;'>VehicleParts</h1>
                        <p style='margin:8px 0 0;'>Official Sales Invoice</p>
                    </div>

                    <div style='padding:30px;'>
                        <h2 style='color:#111827;'>Dear {customerName},</h2>

                        <p style='color:#4b5563; font-size:15px; line-height:1.6;'>
                            Your sales invoice has been generated successfully.
                            Please find the official PDF invoice attached with this email.
                        </p>

                        <table style='width:100%; border-collapse:collapse; margin-top:25px;'>
                            <tr>
                                <td style='padding:12px; border:1px solid #e5e7eb;'>Invoice ID</td>
                                <td style='padding:12px; border:1px solid #e5e7eb; text-align:right; font-weight:bold;'>INV-{invoiceId}</td>
                            </tr>
                            <tr>
                                <td style='padding:12px; border:1px solid #e5e7eb;'>Final Amount</td>
                                <td style='padding:12px; border:1px solid #e5e7eb; text-align:right; font-weight:bold; color:#0f766e;'>Rs. {finalAmount}</td>
                            </tr>
                        </table>

                        <p style='margin-top:25px; color:#6b7280;'>
                            Thank you for choosing VehicleParts.
                        </p>
                    </div>

                    <div style='background:#111827; color:#d1d5db; padding:15px; text-align:center; font-size:13px;'>
                        © 2026 VehicleParts Management System
                    </div>
                </div>
            </div>"
        };

        bodyBuilder.Attachments.Add(
            $"Invoice-INV-{invoiceId}.pdf",
            pdfBytes,
            ContentType.Parse("application/pdf")
        );

        email.Body = bodyBuilder.ToMessageBody();

        SendEmail(email);
    }

    public void SendCreditReminderEmail(
        string toEmail,
        string customerName,
        int invoiceId,
        decimal finalAmount,
        DateTime invoiceDate
    )
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(
            _configuration["EmailSettings:FromEmail"]
        ));

        email.To.Add(MailboxAddress.Parse(toEmail));

        email.Subject = $"Payment Reminder for Invoice INV-{invoiceId}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; background:#f4f6f8; padding:30px;'>
                <div style='max-width:650px; margin:auto; background:#ffffff; border-radius:12px; overflow:hidden; border:1px solid #e5e7eb;'>

                    <div style='background:#b45309; padding:25px; color:white; text-align:center;'>
                        <h1 style='margin:0;'>VehicleParts</h1>
                        <p style='margin:8px 0 0;'>Pending Payment Reminder</p>
                    </div>

                    <div style='padding:30px;'>
                        <h2 style='color:#111827;'>Dear {customerName},</h2>

                        <p style='color:#4b5563; font-size:15px; line-height:1.6;'>
                            This is a friendly reminder that your invoice payment is still pending for more than one month.
                        </p>

                        <table style='width:100%; border-collapse:collapse; margin-top:25px;'>
                            <tr>
                                <td style='padding:12px; border:1px solid #e5e7eb;'>Invoice ID</td>
                                <td style='padding:12px; border:1px solid #e5e7eb; text-align:right; font-weight:bold;'>INV-{invoiceId}</td>
                            </tr>
                            <tr>
                                <td style='padding:12px; border:1px solid #e5e7eb;'>Invoice Date</td>
                                <td style='padding:12px; border:1px solid #e5e7eb; text-align:right; font-weight:bold;'>{invoiceDate:yyyy-MM-dd}</td>
                            </tr>
                            <tr>
                                <td style='padding:12px; border:1px solid #e5e7eb;'>Pending Amount</td>
                                <td style='padding:12px; border:1px solid #e5e7eb; text-align:right; font-weight:bold; color:#b45309;'>Rs. {finalAmount}</td>
                            </tr>
                        </table>

                        <p style='margin-top:25px; color:#6b7280;'>
                            Please complete your payment as soon as possible. Thank you.
                        </p>
                    </div>

                    <div style='background:#111827; color:#d1d5db; padding:15px; text-align:center; font-size:13px;'>
                        © 2026 VehicleParts Management System
                    </div>
                </div>
            </div>"
        };

        email.Body = bodyBuilder.ToMessageBody();

        SendEmail(email);
    }

    private void SendEmail(MimeMessage email)
    {
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