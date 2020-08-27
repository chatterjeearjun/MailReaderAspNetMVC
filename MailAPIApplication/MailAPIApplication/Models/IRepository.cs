using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailAPIApplication.Models.EntityData;

namespace MailAPIApplication.Models
{
    public interface IRepository
    {
        IEnumerable<MailMaster> MyValues { get; set; }
    }
}