using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tutioncloud.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        //public ActionResult News()
        //{
        //    return View();
        //}
        // for wordutopia pages
        public ActionResult Home_Wordutopia()
        {
            return View();
        }
        
         public ActionResult About_Wordutopia()
        {
            return View();
        }

         public ActionResult Demo_Wordutopia()
        {
            return View();
        }

         // for quester pages
         public ActionResult Home_Quester()
         {
             return View();
         }

         public ActionResult About_Quester()
         {
             return View();
         }

         public ActionResult Demo_Quester()
         {
             return View();
         }

         // for choicer pages
         public ActionResult Home_Choicer()
         {
             return View();
         }

         public ActionResult About_Choicer()
         {
             return View();
         }

         public ActionResult Demo_Choicer()
         {
             return View();
         }

        
    }
}