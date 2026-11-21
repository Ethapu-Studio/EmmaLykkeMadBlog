using System.Globalization;
using System.Net;
using EmmaLykkeMadBlog.DTOs;
using EmmaLykkeMadBlog.Infrastructure.Models;
using EmmaLykkeMadBlog.Services.Interfaces;

namespace EmmaLykkeMadBlog.Services;

// Decides what each account email says. Sending itself is IEmailSender's job.
// Everything a user typed (like the name) is HTML-encoded before it goes into a mail.
public class AccountEmailService(
    IEmailSender sender,
    IHostEnvironment env,
    ILogger<AccountEmailService> logger) : IAccountEmailService
{
    public Task SendConfirmationAsync(AppUser user, string link) =>
        Send(user,
            subject: "Bekræft din e-mail hos Emma Lykke",
            heading: "Bekræft din e-mail",
            intro: "Tak fordi du har oprettet en konto. Klik på knappen for at bekræfte din e-mail. Linket virker i 2 timer.",
            buttonText: "Bekræft e-mail",
            link: link,
            footnote: "Hvis du ikke har oprettet en konto, kan du bare ignorere denne mail.");

    public Task SendPasswordResetAsync(AppUser user, string link) =>
        Send(user,
            subject: "Nulstil din adgangskode",
            heading: "Nulstil din adgangskode",
            intro: "Vi har modtaget en anmodning om at nulstille din adgangskode. Klik på knappen for at vælge en ny. Linket virker i 2 timer.",
            buttonText: "Vælg ny adgangskode",
            link: link,
            footnote: "Hvis du ikke har bedt om dette, kan du bare ignorere denne mail. Din adgangskode er uændret.");

    public Task SendAlreadyRegisteredAsync(AppUser user) =>
        Send(user,
            subject: "Du har allerede en konto",
            heading: "Du har allerede en konto",
            intro: "Der blev forsøgt oprettet en konto med din e-mail, men du har allerede en. Du kan logge ind på siden, eller bruge Glemt adgangskode, hvis du ikke kan huske den.",
            buttonText: null,
            link: null,
            footnote: "Hvis det ikke var dig, kan du bare ignorere denne mail.");

    public Task SendAdminLoginNoticeAsync(AppUser user, string clientIp, DateTimeOffset when) =>
        Send(user,
            subject: "Ny login på administratorkontoen",
            heading: "Ny login",
            intro: $"Der blev logget ind på administratorkontoen {when.UtcDateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)} UTC fra IP-adressen {clientIp}.",
            buttonText: null,
            link: null,
            footnote: "Hvis det ikke var dig, skal du nulstille adgangskoden med det samme.");

    private Task Send(AppUser user, string subject, string heading, string intro,
        string? buttonText, string? link, string footnote)
    {
        // Development convenience only: lets you test the flow without a working mail server.
        if (env.IsDevelopment() && link is not null)
            logger.LogWarning("DEV ONLY link for '{Subject}': {Link}", subject, link);

        var name = WebUtility.HtmlEncode(user.DisplayName);

        var button = buttonText is null || link is null
            ? string.Empty
            : $"""<p style="margin:24px 0;"><a href="{WebUtility.HtmlEncode(link)}" style="display:inline-block;padding:14px 28px;background:#2f4a3a;color:#ffffff;text-decoration:none;font-size:13px;font-weight:bold;letter-spacing:1px;text-transform:uppercase;border-radius:4px;">{WebUtility.HtmlEncode(buttonText)}</a></p>""";

        var body = $"""
            <p style="margin:0 0 12px;font-size:15px;line-height:1.6;">Hej {name},</p>
            <p style="margin:0 0 12px;font-size:15px;line-height:1.6;">{WebUtility.HtmlEncode(intro)}</p>
            {button}
            """;

        var html = Layout(WebUtility.HtmlEncode(heading), body, WebUtility.HtmlEncode(footnote));
        var linkText = link is null ? string.Empty : link + "\n\n";
        var text = $"Hej {user.DisplayName},\n\n{intro}\n\n{linkText}{footnote}";

        return sender.SendAsync(new EmailMessage(user.Email!, user.DisplayName, subject, html, text));
    }

    private static string Layout(string heading, string bodyHtml, string footnote) => $"""
        <!DOCTYPE html>
        <html lang="da">
        <head><meta charset="UTF-8"><title>{heading}</title></head>
        <body style="margin:0;padding:0;background:#faf7f2;font-family:Arial,Helvetica,sans-serif;color:#2b2b28;">
          <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background:#faf7f2;padding:32px 16px;">
            <tr><td align="center">
              <table width="560" cellpadding="0" cellspacing="0" border="0" style="max-width:560px;width:100%;background:#ffffff;border:1px solid #e3ddd1;border-radius:4px;">
                <tr><td style="padding:32px 36px 8px;">
                  <p style="margin:0;font-family:Georgia,'Times New Roman',serif;font-size:26px;color:#2f4a3a;">Emma Lykke</p>
                  <p style="margin:4px 0 0;font-size:11px;letter-spacing:2px;text-transform:uppercase;color:#6b6a63;">Aut. klinisk diætist</p>
                </td></tr>
                <tr><td style="padding:24px 36px 8px;">
                  <h1 style="margin:0 0 16px;font-family:Georgia,'Times New Roman',serif;font-size:22px;font-weight:normal;color:#2f4a3a;">{heading}</h1>
                  {bodyHtml}
                </td></tr>
                <tr><td style="padding:8px 36px 32px;">
                  <p style="margin:16px 0 0;padding-top:16px;border-top:1px solid #e3ddd1;font-size:13px;line-height:1.6;color:#6b6a63;">{footnote}</p>
                </td></tr>
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;
}
