using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            MailDBEntities r = new MailDBEntities();
            var data = r.MailMasters.ToList();
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