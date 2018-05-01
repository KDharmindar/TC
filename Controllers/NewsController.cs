using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tutioncloud.Models;
using PagedList;

namespace tutioncloud.Controllers
{
   // [Authorize(Roles = "Admin")]
   //  [Authorize(Users = "stewart.gemmill")]
    public class NewsController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();
        // for user view
        // [AllowAnonymous]
        // GET: News
        //public ActionResult UserIndex()
        //{
        //    var news = db.News.Include(n => n.User);
        //    return View(news.ToList());
        //}

        public ActionResult UserIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var news = db.News.Include(n => n.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                news = news.Where(s => s.Title.Contains(searchString)
                                       || s.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    news = news.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    news = news.OrderBy(s => s.Title);
                    break;
                case "date_desc":
                    news = news.OrderByDescending(s => s.Title);
                    break;
                default:  // Name ascending 
                    news = news.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(news.ToPagedList(pageNumber, pageSize));
        }

      //  [AllowAnonymous]
        // GET: News
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var news = db.News.Include(n => n.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                news = news.Where(s => s.Title.Contains(searchString)
                                       || s.Title.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    news = news.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    news = news.OrderBy(s => s.Title);
                    break;
                case "date_desc":
                    news = news.OrderByDescending(s => s.Title);
                    break;
                default:  // Name ascending 
                    news = news.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(news.ToPagedList(pageNumber, pageSize));
        }


        // GET: News/Details/5
      //  [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "NewsID,Title,Description,UserID")] News news)
        {
            if (ModelState.IsValid)
            {
                news.Date = DateTime.Now;
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename", news.UserID);
            return View(news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename", news.UserID);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "NewsID,Title,Description,UserID")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename", news.UserID);
            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
