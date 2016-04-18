using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using AarhusWebDevelopers.ViewModels;
using System.Net.Mail;
using Umbraco.Core.Models;

namespace AarhusWebDevelopers.Controllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // GET: ContactFormSurface
        public ActionResult Index()
        {
            return PartialView("ContactForm", new ContactForm());
        }

        [HttpPost]
        public ActionResult HandleFormSubmit(ContactForm model)
        {
            if (!ModelState.IsValid) { return CurrentUmbracoPage(); }

            IContent comment = Services.ContentService.CreateContent(model.Subject, CurrentPage.Id, "message");

            comment.SetValue("names", model.Name);
            comment.SetValue("email", model.Email);
            comment.SetValue("subject", model.Subject);
            comment.SetValue("message", model.Message);

            //Save
            Services.ContentService.Save(comment);

            MailMessage message = new MailMessage();
            message.To.Add("turekb@gmail.com");
            message.Subject = model.Subject;
            message.From = new MailAddress(model.Email, model.Name);
            message.Body = model.Message;

            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("turekb.umbraco@gmail.com", "Umbraco123");
                smtp.EnableSsl = true;
                // send mail
                smtp.Send(message);
                TempData["success"] = true;
            }

            return RedirectToCurrentUmbracoPage();
        }
    }
}