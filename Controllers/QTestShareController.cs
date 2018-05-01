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
    public class QTestShareController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: QTestShare
        public ActionResult Index()
        {
            var qTestShares = db.QTestShares.Include(q => q.QTest);
            return View(qTestShares.ToList());
        }

        // GET: QTestShare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QTestShare qTestShare = db.QTestShares.Find(id);
            if (qTestShare == null)
            {
                return HttpNotFound();
            }
            return View(qTestShare);
        }

        // GET: QTestShare/Create
        public ActionResult Create()
        {
            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote");
            return View();
        }

        // POST: QTestShare/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestShareID,FQTestID,ShareBy,ShareTo,Password,ShareDate")] QTestShare qTestShare)
        {
            if (ModelState.IsValid)
            {
                db.QTestShares.Add(qTestShare);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote", qTestShare.FQTestID);
            return View(qTestShare);
        }

        // GET: QTestShare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QTestShare qTestShare = db.QTestShares.Find(id);
            if (qTestShare == null)
            {
                return HttpNotFound();
            }
            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote", qTestShare.FQTestID);
            return View(qTestShare);
        }

        // POST: QTestShare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestShareID,FQTestID,ShareBy,ShareTo,Password,ShareDate")] QTestShare qTestShare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(qTestShare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote", qTestShare.FQTestID);
            return View(qTestShare);
        }

        // GET: QTestShare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QTestShare qTestShare = db.QTestShares.Find(id);
            if (qTestShare == null)
            {
                return HttpNotFound();
            }
            return View(qTestShare);
        }

        // POST: QTestShare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QTestShare qTestShare = db.QTestShares.Find(id);
            db.QTestShares.Remove(qTestShare);
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
