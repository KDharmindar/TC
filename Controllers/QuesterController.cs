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
    public class QuesterController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

        // GET: Quester
        //public ActionResult Index()
        //{
        //    var questers = db.Questers.Include(q => q.Category).Include(q => q.User);
        //    return View(questers.ToList());
        //}
        public ActionResult Welcome()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            IQueryable<Quester> questers = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                questers = from w in db.Questers
                              where w.FQUserID == userid
                              select w;
            }
            else if (usertype == "admin")
            {
                questers = from w in db.Questers
                              select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                questers = questers.Where(w => w.Question.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    questers = questers.OrderByDescending(w => w.Question);
                    break;
                default:
                    questers = questers.OrderBy(w => w.Question);
                    break;
            }

            
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(questers.ToPagedList(pageNumber, pageSize));
        }

        // Index  [HttpPost] for favourite and skip
        [HttpPost]
        public ActionResult Index(int[] Parameters, int[] SkipParameters)
        {
            // for favourite
            //  int l = Parameters.Length;
            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    var k = Parameters[i];
                    var q = (from word in db.Questers
                             where word.QuesterID == k
                             select word).SingleOrDefault();
                    q.IsFavourite = true;
                    db.SaveChanges();

                }
            }

            // for skip


            if (SkipParameters != null)
            {
                for (int i = 0; i < SkipParameters.Length; i++)
                {
                    var k = SkipParameters[i];
                    var q = (from word in db.Questers
                             where word.QuesterID == k
                             select word).SingleOrDefault();
                    q.IsSkip = true;
                    db.SaveChanges();

                }
            }

            // for add test words 
            //if (TestWordsParameters != null)
            //{
            //    for (int i = 0; i < TestWordsParameters.Length; i++)
            //    {
            //        var r = TestWordsParameters[i];
            //        var h = (from word in db.tblWords
            //                 where word.WordId == r
            //                 select word).SingleOrDefault();
            //        h.IsTestWord = "1";
            //        db.SaveChanges();
            //        //return RedirectToAction("Test","Index");
            //    }
            //}
            //else{ 
            //    // NO WORD HAS SELECTED .. PLZ SELECTED WORDS FOR TEST ????
            //}
            return RedirectToAction("Index");
            //  int j = int(Parameters[1]);

        }

        // for Home View
        public ActionResult Home()
        {
            var firstdata = (from s in db.Questers where s.FQUserID == userid select s).FirstOrDefault();
          
            if (firstdata == null)
            {
                return RedirectToAction("Welcome", "Quester");
            }
            else
            {
                return View(firstdata);
            }
           // return View(firstdata);
        }
        // for Home next
        public ActionResult next(int? id)
        {
            int value = int.Parse(db.Questers.Where(s => s.FQUserID == userid)
                            .OrderByDescending(p => p.QuesterID)
                            .Select(r => r.QuesterID)
                            .First().ToString());
            if (id != value)
            {

                var next = db.Questers.Where(s => s.FQUserID == userid)
                    .OrderBy(item => item.QuesterID).First(item => item.QuesterID > id);
                return View("Home", next);
            }
            else
            {
                var firstdata = (from s in db.Questers where s.FQUserID == userid select s).FirstOrDefault();
                return View("Home", firstdata);
                // write this is last word of database ... plz select previous
            }

        }
        // for Home previous
        public ActionResult prev(int? id)
        {
            int value = int.Parse(db.Questers.Where(s => s.FQUserID == userid)
                            .OrderBy(p => p.QuesterID)
                            .Select(r => r.QuesterID)
                            .First().ToString());
            if (id != value)
            {
                var previous = db.Questers.Where(s => s.FQUserID == userid).OrderByDescending(item => item.QuesterID).First(item => item.QuesterID < id);
                return View("Home", previous);
            }
            else
            {
                var firstdata = (from s in db.Questers where s.FQUserID == userid select s).FirstOrDefault();
                return View("Home", firstdata);
            }

        }
        // For Reverse Test
        public ActionResult ReverseTest()
        {
            var reverseData = (from s in db.Questers where s.FQUserID == userid select s).FirstOrDefault();
            if (reverseData == null)
            {
                return RedirectToAction("Welcome", "Quester");
            }
            else
            {
                return View(reverseData);
            }
           // return View(reverseData);
        }
        // For Reverse Test next
        public ActionResult nextR(int? id)
        {
            int value = int.Parse(db.Questers.Where(s => s.FQUserID == userid)
                            .OrderByDescending(p => p.QuesterID)
                            .Select(r => r.QuesterID)
                            .First().ToString());
            if (id != value)
            {

                var next = db.Questers.Where(s => s.FQUserID == userid).OrderBy(item => item.QuesterID).First(item => item.QuesterID > id);
                return View("ReverseTest", next);
            }
            else
            {
                var reverseData = (from s in db.Questers where s.FQUserID == userid select s).FirstOrDefault();
                return View("ReverseTest", reverseData);
                // write this is last word of database ... plz select previous
            }

        }
        // For Reverse Test previous
        public ActionResult prevR(int? id)
        {
            int value = int.Parse(db.Questers.Where(s => s.FQUserID == userid)
                            .OrderBy(p => p.QuesterID)
                            .Select(r => r.QuesterID)
                            .First().ToString());
            if (id != value)
            {
                var previous = db.Questers.Where(s => s.FQUserID == userid).OrderByDescending(item => item.QuesterID).First(item => item.QuesterID < id);
                return View("ReverseTest", previous);
            }
            else
            {
                var reverseData = (from s in db.Questers where s.FQUserID == userid select s).FirstOrDefault();
                return View("ReverseTest", reverseData);
            }

        }


        // add favourite 
        public ActionResult Favourite(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var questers = db.Questers.Where(x => (x.IsFavourite == true && x.FQUserID == userid));

            //for sortOrder
            switch (sortOrder)
            {
                case "name_desc":
                    questers = questers.OrderByDescending(s => s.Question);
                    break;
                case "Date":
                    questers = questers.OrderBy(s => s.Score);
                    break;
                case "date_desc":
                    questers = questers.OrderByDescending(s => s.Score);
                    break;
                default:
                    questers = questers.OrderBy(s => s.Question);
                    break;
            }
            //for searchString

            if (!String.IsNullOrEmpty(searchString))
            {
                questers = questers.Where(s => s.Question.Contains(searchString));
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);


            return View(questers.ToPagedList(pageNumber, pageSize));

        }


        // add SkipList
        public ActionResult SkipList(string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            ViewBag.CurrentFilter = searchString;

            //var wordutopias = from w in db.Wordutopias
            //                  select w;
            var questers = db.Questers.Where(x => (x.IsSkip == true && x.FQUserID == userid));

            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                questers = questers.Where(w => w.Question.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    questers = questers.OrderByDescending(w => w.Question);
                    break;
                default:
                    questers = questers.OrderBy(w => w.Question);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(questers.ToPagedList(pageNumber, pageSize));
        }
        // ShareOnline
        public ActionResult ShareOnline(string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            ViewBag.CurrentFilter = searchString;

            //var wordutopias = from w in db.Wordutopias
            //                  select w;
            var questers = db.Questers.Where(x => (x.IsShareOnline == true && x.FQUserID == userid));

            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                questers = questers.Where(w => w.Question.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    questers = questers.OrderByDescending(w => w.Question);
                    break;
                default:
                    questers = questers.OrderBy(w => w.Question);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(questers.ToPagedList(pageNumber, pageSize));
        }

        // GET: Quester/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quester quester = db.Questers.Find(id);
            if (quester == null)
            {
                return HttpNotFound();
            }
            return View(quester);
        }

        // GET: Quester/Create
        public ActionResult Create()
        {
            ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name");

            //ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            //ViewBag.FQUserID = new SelectList(db.Users, "UserID", "Forename");
            return View();
        }

        // POST: Quester/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuesterID,FCategoryID,FQUserID,Question,Answer,Hint,AdditionalInfo,IsShareOnline,IsFavourite,IsSkip,Appearance,WAppearance,Score")] Quester quester, bool checkResp = false)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    if (checkResp == true)
                    {
                        quester.IsShareOnline = true;
                    }
                    else
                    {
                        quester.IsShareOnline = false;

                    }
                    quester.FQUserID = userid;

                    db.Questers.Add(quester);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name", quester.FCategoryID);
            //ViewBag.FQUserID = new SelectList(db.Users, "UserID", "Forename", quester.FQUserID);
            return View(quester);
        }

        // GET: Quester/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quester quester = db.Questers.Find(id);
            if (quester.IsShareOnline == true)
            {
                ViewBag.chkShare = true;
            }
            else
            {
                ViewBag.chkShare = false;
            }
            if (quester == null)
            {
                return HttpNotFound();
            }
            ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name", quester.FCategoryID);
            //ViewBag.FQUserID = new SelectList(db.Users, "UserID", "Forename", quester.FQUserID);
            // ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name");

            return View(quester);
        }

        // POST: Quester/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuesterID,FCategoryID,FQUserID,Question,Answer,Hint,AdditionalInfo,IsShareOnline,IsFavourite,IsSkip,Appearance,WAppearance,Score")] Quester quester,  bool chkShare)
        {
            if (ModelState.IsValid)
            {
                ViewBag.chkShare = chkShare;
                if (chkShare)
                {
                    quester.IsShareOnline = true;
                }
                else
                {
                    quester.IsShareOnline = false;
                }
                quester.FQUserID = userid;

                ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name", quester.FCategoryID);

                db.Entry(quester).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.FQUserID = new SelectList(db.Users, "UserID", "Forename", quester.FQUserID);
           // ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name");


            return View(quester);
        }

        // GET: Quester/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quester quester = db.Questers.Find(id);
            if (quester == null)
            {
                return HttpNotFound();
            }
            return View(quester);
        }

        // POST: Quester/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quester quester = db.Questers.Find(id);
            db.Questers.Remove(quester);
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
