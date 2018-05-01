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
    public class CTestShareController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: CTestShare
        public ActionResult Index()
        {
            var cTestShares = db.CTestShares.Include(c => c.CTest);
            return View(cTestShares.ToList());
        }

        // GET: CTestShare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTestShare cTestShare = db.CTestShares.Find(id);
            if (cTestShare == null)
            {
                return HttpNotFound();
            }
            return View(cTestShare);
        }

        // GET: CTestShare/Create
        public ActionResult Create()
        {
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote");
            return View();
        }

        // POST: CTestShare/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CTestShareID,FCTestID,ShareBy,ShareTo,Password,ShareDate")] CTestShare cTestShare)
        {
            if (ModelState.IsValid)
            {
                db.CTestShares.Add(cTestShare);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote", cTestShare.FCTestID);
            return View(cTestShare);
        }

        // GET: CTestShare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTestShare cTestShare = db.CTestShares.Find(id);
            if (cTestShare == null)
            {
                return HttpNotFound();
            }
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote", cTestShare.FCTestID);
            return View(cTestShare);
        }

        // POST: CTestShare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CTestShareID,FCTestID,ShareBy,ShareTo,Password,ShareDate")] CTestShare cTestShare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cTestShare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote", cTestShare.FCTestID);
            return View(cTestShare);
        }

        // GET: CTestShare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTestShare cTestShare = db.CTestShares.Find(id);
            if (cTestShare == null)
            {
                return HttpNotFound();
            }
            return View(cTestShare);
        }

        // POST: CTestShare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CTestShare cTestShare = db.CTestShares.Find(id);
            db.CTestShares.Remove(cTestShare);
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
