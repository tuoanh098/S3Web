using Identity.Application.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

public sealed class SmtpOptions
{
    public string Host { get; init; } = "smtp.gmail.com";
    public int Port { get; init; } = 587;
    public string User { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FromEmail { get; init; } = default!;
    public string FromName { get; init; } = "RAS Support";
    public bool UseStartTls { get; init; } = true;
}

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _o;
    private readonly ILogger<SmtpEmailSender> _log;
    public SmtpEmailSender(IOptions<SmtpOptions> opt, ILogger<SmtpEmailSender> log)
    {
        _o = opt.Value;
        _log = log;
    }

    public async Task SendAsync(string to, string subject, string html, CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        try
        {
            msg.From.Add(new MailboxAddress(_o.FromName, _o.FromEmail));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = html }.ToMessageBody();

            using var client = new SmtpClient();
            // connect with appropriate SSL/TLS option
            var sockOpt = _o.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;
            _log.LogInformation("Connecting to SMTP {Host}:{Port} StartTls={StartTls}", _o.Host, _o.Port, _o.UseStartTls);
            await client.ConnectAsync(_o.Host, _o.Port, sockOpt, ct);

            if (!string.IsNullOrWhiteSpace(_o.User))
            {
                _log.LogInformation("Authenticating SMTP user {User}", _o.User);
                await client.AuthenticateAsync(_o.User, _o.Password, ct);
            }

            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);
            _log.LogInformation("Email sent to {To}", to);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to send email to {To} using SMTP host {Host}. Error: {Message}", to, _o.Host, ex.Message);
            // rethrow so caller knows it failed, or swallow & return if you prefer non-blocking behavior
            throw;
        }
    }
}
