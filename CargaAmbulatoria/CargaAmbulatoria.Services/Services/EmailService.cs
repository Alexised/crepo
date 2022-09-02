using CargaAmbulatoria.EntityFramework.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace CargaAmbulatoria.Services.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendResetPasswordMail(string emailTo, string url, string token)
        { 
            this.SendMail(emailTo, "Restablecimiento de Contraseña - Coosalud",
                " <p>¡Hola! <br /><br />¿Necesitas reestablecer tu contraseña?<br /><br />" +
                "¡Es muy fácil! Simplemente ingresa al código en pantalla y lo tendremos listo y funcionando en muy poco tiempo: <br /><br />"
                + token
                + "<br /><br />O bien, puedes ingresar al siguiente link y seguir los pasos correspondientes: <a href='" + url + "'>" + url + "</a><br /> <br />" +
                "Si no realizaste esta solicitud, ignora este correo electrónico. <br /> <br />¡Gracias!</p>");
        }

        public void SendWelcomeMail(string emailTo, string url, string email, string password)
        {
            this.SendMail(emailTo, "Usuario Creado - Carga Ambulatoria de Documentos  - Coosalud",
                " <p>¡Hola! <br /><br />Bienvenido al sistema de carga ambulatoria de documentos de CooSalud<br /><br />" +
                "¡Su usuario ha sido creado! Para ingresar simplemente presión clic <a href='" + url + "'> aquí</a>: <br /><br />" +
                "Las credenciales asignadas han sido: <br />" +
                "Usuario: " + email + "<br />" +
                "Contraseña: " + password + "<br /><br />" +
                "Atentamente COOSALUD");
        }


        public void SendMail(string emailTo, string subject, string body)
        {
            string emailFrom = _configuration["Mail:Email"];
            body = $"<p><img src='{_configuration["Application:Url"]}/assets/images/banner.png' alt='Coosalud'/></p>" + body;
            MailMessage oMailMessage = new MailMessage(emailFrom, emailTo, subject, body);

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient(_configuration["Mail:SmtpServer"]);
            oSmtpClient.EnableSsl = bool.Parse(_configuration["Mail:UseSSL"]);
            oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Port = int.Parse(_configuration["Mail:SmtpPort"]);
            oSmtpClient.Credentials = new System.Net.NetworkCredential(emailFrom, _configuration["Mail:Password"]);

            oSmtpClient.Send(oMailMessage);

            oSmtpClient.Dispose();
        }
    }
}
