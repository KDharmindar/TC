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
    public class NoteController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();
        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

        // GET: Note
        //public ActionResult Index()
        //{
        //    var notes = db.Notes.Include(n => n.User);
        //    return View(notes.ToList());
        //}
     

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.webname = Session["WebName"];

            //for paging
            ViewBag.CurrentSort = sortOrder;
            // for sorting
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            IQueryable<Note> notes = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                notes = from w in db.Notes
                              where w.FUserID == userid
                              select w;
            }
            else if (usertype == "admin")
            {
                notes = from w in db.Notes
                              select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                notes = notes.Where(w => w.Title.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    notes = notes.OrderByDescending(w => w.Title);
                    break;
                default:
                    notes = notes.OrderBy(w => w.Title);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(notes.ToPagedList(pageNumber, pageSize));

        }

        // GET: Note/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.webname = Session["WebName"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // GET: Note/Create
        public ActionResult Create()
        {

            ViewBag.webname =Session["WebName"];
            //ViewBag.FUserID = new SelectList(db.Users, "UserID", "Forename");
            //ViewBag.Gender = new SelectList(DDLHelper.GetGender(), "Value", "Text");
            var exemploList = new SelectList(new[] { "Exemplo1:", "Exemplo2", "Exemplo3" });
            ViewBag.ExemploList = exemploList;

            return View();
        }

        // POST: Note/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NoteID,Title,Description,FUserID")] Note note)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    note.FUserID = userid;

                    db.Notes.Add(note);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                // ViewBag.FUserID = new SelectList(db.Users, "UserID", "Forename", note.FUserID);
                //ViewBag.Gender = new SelectList(DDLHelper.GetGender(), "Value", "Text");
                var exemploList = new SelectList(new[] { "Exemplo1:", "Exemplo2", "Exemplo3" });
                ViewBag.ExemploList = exemploList;

               
            }
            return View(note);
        }

        // GET: Note/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
           // ViewBag.FUserID = new SelectList(db.Users, "UserID", "Forename", note.FUserID);
            ViewBag.webname = Session["WebName"];

            return View(note);
        }

        // POST: Note/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NoteID,Title,Description,FUserID")] Note note)
        {
            if (ModelState.IsValid)
            {
                note.FUserID = userid;
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           // ViewBag.FUserID = new SelectList(db.Users, "UserID", "Forename", note.FUserID);
            ViewBag.webname = Session["WebName"];

            return View(note);
        }

        // GET: Note/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.webname = Session["WebName"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = db.Notes.Find(id);
            db.Notes.Remove(note);
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
