using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using MailAPIApplication.Models;
using MailAPIApplication.Models.EntityData;

namespace MailAPIApplication.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IRepository _repo;

        public ValuesController(IRepository repo)
        {
            _repo = repo;
        }

        // GET api/values     
        public IEnumerable<MailMaster> Get()
        {
            return _repo.MyValues;
        }

        // GET api/values/5     
        public MailMaster Get(int id)
        {
            var list = _repo.MyValues.ToList();

            if (id >= 0)
            {
                return list.Where(x=>x.ID == id).FirstOrDefault();
            }

            return null;
        }
    }
}