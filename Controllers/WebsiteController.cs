using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tutioncloud.Models;

namespace tutioncloud.Controllers
{
    public class WebsiteController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: Website
        public ActionResult Index()
        {
            return View(db.Websites.ToList());
        }

        // GET: Website/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Website website = db.Websites.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }
            return View(website);
        }

        // GET: Website/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Website/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WebID,WebName")] Website website)
        {
            if (ModelState.IsValid)
            {
                db.Websites.Add(website);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(website);
        }

        // GET: Website/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Website website = db.Websites.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }
            return View(website);
        }

        // POST: Website/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WebID,WebName")] Website website)
        {
            if (ModelState.IsValid)
            {
                db.Entry(website).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(website);
        }

        // GET: Website/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Website website = db.Websites.Find(id);
            if (website == null)
            {
                return HttpNotFound();
            }
            return View(website);
        }

        // POST: Website/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Website website = db.Websites.Find(id);
            db.Websites.Remove(website);
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
