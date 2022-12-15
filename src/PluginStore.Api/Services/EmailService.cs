using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using PluginStore.Api.Services.Interfaces;

namespace PluginStore.Api.Services;

public class EmailService : IEmailService
{
    public void SendPasswordToEmail(string to, string name, string password)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("billy.herrington2022@outlook.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = "Одноразовый пароль";
        email.Body = new TextPart(TextFormat.Html)
        {
            Text =
                "<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">" +
                "<div style=\"margin:50px auto;width:70%;padding:20px 0\">" +
                $"<p style=\"font-size:1.1em\">Hi, {name}</p>" +
                "<p>Use the following OTP to complete your Sign Up procedures</p>" +
                $"<h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">{password}</h2>" +
                "</div></div>"
        };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp-mail.outlook.com", 587);
        smtp.Authenticate("billy.herrington2022@outlook.com", "k2IIe9\\#)YgV%MX<WZ8]");
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}