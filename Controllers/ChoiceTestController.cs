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
    public class ChoiceTestController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: ChoiceTest
        public ActionResult Index()
        {
            var choiceTests = db.ChoiceTests.Include(c => c.Choicer).Include(c => c.CTest);
            return View(choiceTests.ToList());
        }

        // GET: ChoiceTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChoiceTest choiceTest = db.ChoiceTests.Find(id);
            if (choiceTest == null)
            {
                return HttpNotFound();
            }
            return View(choiceTest);
        }

        // GET: ChoiceTest/Create
        public ActionResult Create()
        {
            ViewBag.FChoicerID = new SelectList(db.Choicers, "ChoicerID", "CQuestion");
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote");
            return View();
        }

        // POST: ChoiceTest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChoiceTestID,FCTestID,FChoicerID,Result,TimeSkip,TestAppearance")] ChoiceTest choiceTest)
        {
            if (ModelState.IsValid)
            {
                db.ChoiceTests.Add(choiceTest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FChoicerID = new SelectList(db.Choicers, "ChoicerID", "CQuestion", choiceTest.FChoicerID);
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote", choiceTest.FCTestID);
            return View(choiceTest);
        }

        // GET: ChoiceTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChoiceTest choiceTest = db.ChoiceTests.Find(id);
            if (choiceTest == null)
            {
                return HttpNotFound();
            }
            ViewBag.FChoicerID = new SelectList(db.Choicers, "ChoicerID", "CQuestion", choiceTest.FChoicerID);
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote", choiceTest.FCTestID);
            return View(choiceTest);
        }

        // POST: ChoiceTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChoiceTestID,FCTestID,FChoicerID,Result,TimeSkip,TestAppearance")] ChoiceTest choiceTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(choiceTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FChoicerID = new SelectList(db.Choicers, "ChoicerID", "CQuestion", choiceTest.FChoicerID);
            ViewBag.FCTestID = new SelectList(db.CTests, "CTestID", "CTNote", choiceTest.FCTestID);
            return View(choiceTest);
        }

        // GET: ChoiceTest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChoiceTest choiceTest = db.ChoiceTests.Find(id);
            if (choiceTest == null)
            {
                return HttpNotFound();
            }
            return View(choiceTest);
        }

        // POST: ChoiceTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChoiceTest choiceTest = db.ChoiceTests.Find(id);
            db.ChoiceTests.Remove(choiceTest);
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
