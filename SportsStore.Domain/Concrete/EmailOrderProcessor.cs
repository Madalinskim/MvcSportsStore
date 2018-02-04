using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "zamowienia@sklep.pl";
        public string MailFromAddress = "sklepsportowy@sklep.pl";
        public bool UseSsl = true;
        public string Username = "UżytkownikSmtp";
        public string Password = "HasłoSmtp";
        public string ServerName = "smtp@sklep.pl";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"C:\SportsStoreEmails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippinginfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder().AppendLine("Nowe zamówienie").AppendLine("---").AppendLine("Produkty:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (wartość: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Wartość całkowita {0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Wysyłka dla:")
                    .AppendLine(shippinginfo.Name)
                    .AppendLine(shippinginfo.Line1)
                    .AppendLine(shippinginfo.Line2 ?? "")
                    .AppendLine(shippinginfo.Line3 ?? "")
                    .AppendLine(shippinginfo.City)
                    .AppendLine(shippinginfo.State ?? "")
                    .AppendLine(shippinginfo.Country ?? "")
                    .AppendLine(shippinginfo.Zip)
                    .AppendLine("---")
                    .AppendFormat("Pakowanie prezentu: {0}", shippinginfo.GiftWrap ? "Tak" : "Nie");

                MailMessage mailMessage = new MailMessage(
                    emailSettings.MailFromAddress,
                    emailSettings.MailToAddress,
                    "Otrzymano nowe zamówienie!",
                    body.ToString());

                if (emailSettings.WriteAsFile)
                {
                    if (!Directory.Exists(emailSettings.FileLocation))
                        Directory.CreateDirectory(emailSettings.FileLocation);

                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }
}
