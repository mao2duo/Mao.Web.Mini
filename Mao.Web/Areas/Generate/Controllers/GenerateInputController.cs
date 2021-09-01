using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Areas.Generate.Controllers
{
    [Authorize]
    public class GenerateInputController : GenerateAreaController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}