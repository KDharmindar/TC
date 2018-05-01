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
    public class QuestTestController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: QuestTest
        public ActionResult Index()
        {
            var questTests = db.QuestTests.Include(q => q.QTest).Include(q => q.Quester);
            return View(questTests.ToList());
        }

        // GET: QuestTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestTest questTest = db.QuestTests.Find(id);
            if (questTest == null)
            {
                return HttpNotFound();
            }
            return View(questTest);
        }

        // GET: QuestTest/Create
        public ActionResult Create()
        {
            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote");
            ViewBag.FQuesterID = new SelectList(db.Questers, "QuesterID", "Question");
            return View();
        }

        // POST: QuestTest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestTestID,FQTestID,FQuesterID,Result,TImeSkip,TestAppearance")] QuestTest questTest)
        {
            if (ModelState.IsValid)
            {
                db.QuestTests.Add(questTest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote", questTest.FQTestID);
            ViewBag.FQuesterID = new SelectList(db.Questers, "QuesterID", "Question", questTest.FQuesterID);
            return View(questTest);
        }

        // GET: QuestTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestTest questTest = db.QuestTests.Find(id);
            if (questTest == null)
            {
                return HttpNotFound();
            }
            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote", questTest.FQTestID);
            ViewBag.FQuesterID = new SelectList(db.Questers, "QuesterID", "Question", questTest.FQuesterID);
            return View(questTest);
        }

        // POST: QuestTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestTestID,FQTestID,FQuesterID,Result,TImeSkip,TestAppearance")] QuestTest questTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FQTestID = new SelectList(db.QTests, "QTestID", "QTNote", questTest.FQTestID);
            ViewBag.FQuesterID = new SelectList(db.Questers, "QuesterID", "Question", questTest.FQuesterID);
            return View(questTest);
        }

        // GET: QuestTest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestTest questTest = db.QuestTests.Find(id);
            if (questTest == null)
            {
                return HttpNotFound();
            }
            return View(questTest);
        }

        // POST: QuestTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestTest questTest = db.QuestTests.Find(id);
            db.QuestTests.Remove(questTest);
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
