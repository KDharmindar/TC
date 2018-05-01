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
    public class WordTestController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: WordTest
        public ActionResult Index()
        {
            var wordTests = db.WordTests.Include(w => w.Wordutopia).Include(w => w.WTest);
            return View(wordTests.ToList());
        }

        // GET: WordTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordTest wordTest = db.WordTests.Find(id);
            if (wordTest == null)
            {
                return HttpNotFound();
            }
            return View(wordTest);
        }

        // GET: WordTest/Create
        public ActionResult Create()
        {
            ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word");
            ViewBag.FWtestID = new SelectList(db.WTests, "WTestID", "TNote");
            return View();
        }

        // POST: WordTest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WordTestID,FWtestID,FWordID,Result,TImeSkip,TestAppearance")] WordTest wordTest)
        {
            if (ModelState.IsValid)
            {
                db.WordTests.Add(wordTest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word", wordTest.FWordID);
            ViewBag.FWtestID = new SelectList(db.WTests, "WTestID", "TNote", wordTest.FWtestID);
            return View(wordTest);
        }

        // GET: WordTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordTest wordTest = db.WordTests.Find(id);
            if (wordTest == null)
            {
                return HttpNotFound();
            }
            ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word", wordTest.FWordID);
            ViewBag.FWtestID = new SelectList(db.WTests, "WTestID", "TNote", wordTest.FWtestID);
            return View(wordTest);
        }

        // POST: WordTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WordTestID,FWtestID,FWordID,Result,TImeSkip,TestAppearance")] WordTest wordTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wordTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word", wordTest.FWordID);
            ViewBag.FWtestID = new SelectList(db.WTests, "WTestID", "TNote", wordTest.FWtestID);
            return View(wordTest);
        }

        // GET: WordTest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordTest wordTest = db.WordTests.Find(id);
            if (wordTest == null)
            {
                return HttpNotFound();
            }
            return View(wordTest);
        }

        // POST: WordTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WordTest wordTest = db.WordTests.Find(id);
            db.WordTests.Remove(wordTest);
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
