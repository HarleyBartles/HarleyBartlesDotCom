using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HBDotCom.Models;
using HBDotCom.Models.ViewModels;
using HBDotCom.Data;
using HBDotCom.Services;

namespace HBDotCom.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        private EmailAddress _fromAndToEmailAddress;

        public HomeController(ApplicationDbContext context, IEmailService emailService, EmailAddress fromAddress)
        {
            _context = context;
            _emailService = emailService;
            _fromAndToEmailAddress = fromAddress;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "What would you like to know?";

            return View();
        }

        public IActionResult AboutSite()
        {
            ViewData["Message"] = "So what is it?";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Reach out and touch me.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact([FromBody]ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            EmailMessage msgToSend = new EmailMessage
            {
                FromAddresses = new List<EmailAddress> { _fromAndToEmailAddress },
                ToAddresses = new List<EmailAddress> { _fromAndToEmailAddress },
                Content = $"Here is your message: Name: {model.Name}, " +
                        $"Email: {model.Email}, Message: {model.Message}",
                Subject = "Contact Form - BasicContactForm App"
            };

            _emailService.Send(msgToSend);
            return RedirectToAction("Contact");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
