using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Controllers
{
    public class DevelopController : Controller
    {
        // GET: Debug
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SqlKataQuery()
        {
            return View();
        }

        public ActionResult UserClaims()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                if (claimsIdentity.Claims != null)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        stringBuilder.AppendLine($"{claim.Type}: {claim.Value}");
                    }
                }
            }
            return Content(stringBuilder.ToString());
        }
        public ActionResult ApiUserClaims()
        {
            return View();
        }


        public ActionResult NewGuid()
        {
            return Content(Guid.NewGuid().ToString());
        }

        public ActionResult Exception()
        {
            throw new Exception("Test");
        }
    }
}