using ElateService.Common;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ElateService.BLL.Services
{
    public static class EmailService
    {
        static public async Task SendConfirmationCode(string emailAddress, Role role, string confirmationCode, int? id, string messageLanguage)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.EnableSsl = true;
            NetworkCredential credential = (NetworkCredential)smtpClient.Credentials;
            
            MailMessage mailMessage = new MailMessage(credential.UserName, emailAddress);
            StringBuilder sbEmailBody = new StringBuilder();
            if(messageLanguage == "ru")
            {
                sbEmailBody.Append("Спасибо, что используете сервисы ElateService.<br>");
                sbEmailBody.Append("Для активации аккаунта перейдите по ссылке: <br>");
            }
            else
            {
                sbEmailBody.Append("Thank you for using ElateService services.<br>");
                sbEmailBody.Append("To activate your account, please follow this link:<br>");
            }
            sbEmailBody.Append("<br>");
            sbEmailBody.Append("http://localhost:55333/");
            sbEmailBody.Append(role.ToString());
            sbEmailBody.Append("/VerifyConfirmationCode/");
            sbEmailBody.Append(id.ToString() + "/");
            sbEmailBody.Append(confirmationCode);
            sbEmailBody.Append("<br>");
            if(messageLanguage == "ru")
            {
                sbEmailBody.Append("<br>Администрация ElateService<br>");
            }
            else
            {
                sbEmailBody.Append("<br>ElateService Administration<br>");
            }

            mailMessage.IsBodyHtml = true;

            mailMessage.Body = sbEmailBody.ToString();
            if(messageLanguage == "ru")
            {
                mailMessage.Subject = "Активация аккаунта ElateService";
            }
            else
            {
                mailMessage.Subject = "ElateService account activation";
            }
            
            await smtpClient.SendMailAsync(mailMessage);
        }


        static public async Task SendNewConfirmationCode(string emailAddress, Role role, string confirmationCode, int id, string messageLanguage)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.EnableSsl = true;
            NetworkCredential credential = (NetworkCredential)smtpClient.Credentials;

            MailMessage mailMessage = new MailMessage(credential.UserName, emailAddress);
            StringBuilder sbEmailBody = new StringBuilder();
            if (messageLanguage == "ru")
            {
                sbEmailBody.Append("<br>Ссылка для воссановления пароля ElateService.<br>");
            }
            else
            {
                sbEmailBody.Append("<br>Link for password recovery ElateService.<br>");
            }
            sbEmailBody.Append("<br>");
            sbEmailBody.Append("http://localhost:55333/");
            sbEmailBody.Append(role.ToString());
            sbEmailBody.Append("/EnterNewPassword/");
            sbEmailBody.Append(id.ToString() + "/");
            sbEmailBody.Append(confirmationCode);
            sbEmailBody.Append("<br>");
            if (messageLanguage == "ru")
            {
                sbEmailBody.Append("<br>Администрация ElateService<br>");
            }
            else
            {
                sbEmailBody.Append("<br>ElateService Administration<br>");
            }

            mailMessage.IsBodyHtml = true;

            mailMessage.Body = sbEmailBody.ToString();
            if (messageLanguage == "ru")
            {
                mailMessage.Subject = "Восстановление пароля ElateService";
            }
            else
            {
                mailMessage.Subject = "ElateService password recovery";
            }

            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
