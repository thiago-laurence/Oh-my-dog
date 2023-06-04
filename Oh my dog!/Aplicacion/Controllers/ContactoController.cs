using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using System.Net.Mail;
using Org.BouncyCastle.Crypto.Macs;

namespace Aplicacion.Controllers
{
    public class ContactoController : Controller
    {
        [HttpPost]
        public IActionResult EnviarCorreo(string remitente, string asunto, string contenido)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("",remitente)); // Correo de origen, tine que estar configurado en el metodo client.Authenticate()
                message.To.Add(new MailboxAddress("","ohmydoggonet@outlook.es")); // Correo de destino
                message.Subject = asunto;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = contenido;
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("ohmydoggonet@outlook.es", "@ohmydog123");
                    client.Send(message);
                    client.Disconnect(true);
                }

                return RedirectToAction("Index", "Home"); // o redirige a la página que desees después de enviar el correo electrónico
            }
            catch (Exception ex)
            {
                // Manejo de errores aquí
                Console.WriteLine(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
