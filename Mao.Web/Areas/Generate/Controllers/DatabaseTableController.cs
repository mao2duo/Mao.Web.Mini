using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Areas.Generate.Controllers
{
    [Authorize]
    public class DatabaseTableController : GenerateAreaController
    {
        public ActionResult List()
        {
            return View();
        }
        
        public ActionResult Add()
        {
            return View();
        }
        
        public ActionResult Update()
        {
            return View();
        }
    }
}