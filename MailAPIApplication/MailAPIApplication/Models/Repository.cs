using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailAPIApplication.Models.EntityData;

namespace MailAPIApplication.Models
{
    public class Repository : IRepository
    {
        public IEnumerable<MailMaster> MyValues { get; set; }

        public Repository()
        {
            //MyValues = new List<string> { "Value1", "Value2", "Value3", "Value4" };
            using (var context = new MailDBEntities())
            {
                MyValues = context.MailMasters.ToList();                
            }
        }
    }
}