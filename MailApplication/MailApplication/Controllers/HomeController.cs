using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using MailApplication.Models.EntityData;
using MailApplication.Repository;

namespace MailApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            // Uesd Web.API to retrive result
            List<MailMaster> data = new List<MailMaster>(); 
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:51908/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Values");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MailMaster>>();
                    readTask.Wait();

                    data = readTask.Result.ToList();
                }
                else //web api sent error response 
                {
                    //fallback to connect to Entities..
                    MailDBEntities r = new MailDBEntities();
                    data = r.MailMasters.ToList();
                    
                    // data = Enumerable.Empty<MailMaster>().ToList();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            //MailDBEntities r = new MailDBEntities();
            //var data = r.MailMasters.ToList();
            //ViewBag.userdetails = data;
            Session["userdetails"] = data;
            return View(data);
        }

        [HttpPost]
        public JsonResult PullLatestMail()
        {
            List<MailMaster> SessionData = (List<MailMaster>)Session["userdetails"];

            var mailRepository = new MailRepository(ConfigurationManager.AppSettings["MailServerDetails"], 993, true, ConfigurationManager.AppSettings["LogIn"],
                ConfigurationManager.AppSettings["Password"]);

            var allEmails = mailRepository.GetAllMails();

            using (MailDBEntities entities = new MailDBEntities())
            {
                foreach (var email in allEmails)
                {
                    if (SessionData != null && SessionData.Where(x => x.ReceivedDateTime == email.ReceivedDateTime && x.Subject == email.Subject && x.Body == email.Body).Count() == 0)
                    {
                        entities.MailMasters.Add(email);
                    }
                }
                entities.SaveChanges();
            }

            MailDBEntities r = new MailDBEntities();
            var data = r.MailMasters.ToList();
            //ViewBag.userdetails = data;
            Session["userdetails"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string contentType = MimeMapping.GetMimeMapping(fileName);
                return File(fileName, contentType);
            }
            else
            {
                return null;
            }
        }
    }
}