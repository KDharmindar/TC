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
    public class TestShareController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: TestShare
        public ActionResult Index()
        {
            var testShares = db.TestShares.Include(t => t.WTest);
            return View(testShares.ToList());
        }

        // GET: TestShare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestShare testShare = db.TestShares.Find(id);
            if (testShare == null)
            {
                return HttpNotFound();
            }
            return View(testShare);
        }

        // GET: TestShare/Create
        public ActionResult Create()
        {
            ViewBag.FTestID = new SelectList(db.WTests, "WTestID", "TNote");
            return View();
        }

        // POST: TestShare/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestShareID,FTestID,ShareBy,ShareTo,Password,ShareDate")] TestShare testShare)
        {
            if (ModelState.IsValid)
            {
                db.TestShares.Add(testShare);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FTestID = new SelectList(db.WTests, "WTestID", "TNote", testShare.FTestID);
            return View(testShare);
        }

        // GET: TestShare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestShare testShare = db.TestShares.Find(id);
            if (testShare == null)
            {
                return HttpNotFound();
            }
            ViewBag.FTestID = new SelectList(db.WTests, "WTestID", "TNote", testShare.FTestID);
            return View(testShare);
        }

        // POST: TestShare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestShareID,FTestID,ShareBy,ShareTo,Password,ShareDate")] TestShare testShare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testShare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FTestID = new SelectList(db.WTests, "WTestID", "TNote", testShare.FTestID);
            return View(testShare);
        }

        // GET: TestShare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestShare testShare = db.TestShares.Find(id);
            if (testShare == null)
            {
                return HttpNotFound();
            }
            return View(testShare);
        }

        // POST: TestShare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestShare testShare = db.TestShares.Find(id);
            db.TestShares.Remove(testShare);
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
