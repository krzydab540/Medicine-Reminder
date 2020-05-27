using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManualAuth.Models;
using MimeKit;
using MailKit.Net.Smtp;

namespace ManualAuth.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Privacy()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Patient's Mailbox", "test.app.kd@gmail.com"));
            message.To.Add(new MailboxAddress("To Add", "krzysztofwozniak1234@gmail.com"));
            message.Subject = "Medicine Reminder";
            message.Body = new TextPart("plain")
            {
                Text = "This is the message from your application."
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("test.app.kd@gmail.com", "Application_1");
                client.Send(message);
                client.Disconnect(true);
            }


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
