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
    public class ChoicerController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

        // GET: Choicer
        //public ActionResult Index()
        //{
        //    var choicers = db.Choicers.Include(c => c.Category).Include(c => c.User);
        //    return View(choicers.ToList());
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

            IQueryable<Choicer> choicers = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                choicers = from w in db.Choicers
                           where w.FCUserID == userid
                           select w;
            }
            else if (usertype == "admin")
            {
                choicers = from w in db.Choicers
                           select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                choicers = choicers.Where(w => w.CQuestion.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    choicers = choicers.OrderByDescending(w => w.CQuestion);
                    break;
                default:
                    choicers = choicers.OrderBy(w => w.CQuestion);
                    break;
            }


            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(choicers.ToPagedList(pageNumber, pageSize));
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
                    var q = (from word in db.Choicers
                             where word.ChoicerID == k
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
                    var q = (from word in db.Choicers
                             where word.ChoicerID == k
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
            var firstdata = (from s in db.Choicers where s.FCUserID == userid select s).FirstOrDefault();
            if (firstdata == null)
            {
                return RedirectToAction("Welcome", "Choicer");
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
            int value = int.Parse(db.Choicers.Where(s => s.FCUserID == userid)
                            .OrderByDescending(p => p.ChoicerID)
                            .Select(r => r.ChoicerID)
                            .First().ToString());
            if (id != value)
            {

                var next = db.Choicers.Where(s => s.FCUserID == userid)
                    .OrderBy(item => item.ChoicerID).First(item => item.ChoicerID > id);
                return View("Home", next);
            }
            else
            {
                var firstdata = (from s in db.Choicers where s.FCUserID == userid select s).FirstOrDefault();
                return View("Home", firstdata);
                // write this is last word of database ... plz select previous
            }

        }
        // for Home previous
        public ActionResult prev(int? id)
        {
            int value = int.Parse(db.Choicers.Where(s => s.FCUserID == userid)
                            .OrderBy(p => p.ChoicerID)
                            .Select(r => r.ChoicerID)
                            .First().ToString());
            if (id != value)
            {
                var previous = db.Choicers.Where(s => s.FCUserID == userid).OrderByDescending(item => item.ChoicerID).First(item => item.ChoicerID < id);
                return View("Home", previous);
            }
            else
            {
                var firstdata = (from s in db.Choicers where s.FCUserID == userid select s).FirstOrDefault();
                return View("Home", firstdata);
            }

        }
        //// For Reverse Test
        //public ActionResult ReverseTest()
        //{
        //    var reverseData = (from s in db.Choicers select s).FirstOrDefault();
        //    return View(reverseData);
        //}
        //// For Reverse Test next
        //public ActionResult nextR(int? id)
        //{
        //    int value = int.Parse(db.Choicers
        //                    .OrderByDescending(p => p.ChoicerID)
        //                    .Select(r => r.ChoicerID)
        //                    .First().ToString());
        //    if (id != value)
        //    {

        //        var next = db.Choicers.OrderBy(item => item.ChoicerID).First(item => item.ChoicerID > id);
        //        return View("ReverseTest", next);
        //    }
        //    else
        //    {
        //        var reverseData = (from s in db.Choicers select s).FirstOrDefault();
        //        return View("ReverseTest", reverseData);
        //        // write this is last word of database ... plz select previous
        //    }

        //}
        //// For Reverse Test previous
        //public ActionResult prevR(int? id)
        //{
        //    int value = int.Parse(db.Choicers
        //                    .OrderBy(p => p.ChoicerID)
        //                    .Select(r => r.ChoicerID)
        //                    .First().ToString());
        //    if (id != value)
        //    {
        //        var previous = db.Choicers.OrderByDescending(item => item.ChoicerID).First(item => item.ChoicerID < id);
        //        return View("ReverseTest", previous);
        //    }
        //    else
        //    {
        //        var reverseData = (from s in db.Choicers select s).FirstOrDefault();
        //        return View("ReverseTest", reverseData);
        //    }

        //}


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

            var choicers = db.Choicers.Where(x => (x.IsFavourite == true && x.FCUserID == userid));

            //for sortOrder
            switch (sortOrder)
            {
                case "name_desc":
                    choicers = choicers.OrderByDescending(s => s.CQuestion);
                    break;
                case "Date":
                    choicers = choicers.OrderBy(s => s.Score);
                    break;
                case "date_desc":
                    choicers = choicers.OrderByDescending(s => s.Score);
                    break;
                default:
                    choicers = choicers.OrderBy(s => s.CQuestion);
                    break;
            }
            //for searchString

            if (!String.IsNullOrEmpty(searchString))
            {
                choicers = choicers.Where(s => s.CQuestion.Contains(searchString));
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);


            return View(choicers.ToPagedList(pageNumber, pageSize));

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
            var choicers = db.Choicers.Where(x => (x.IsSkip == true && x.FCUserID == userid));

            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                choicers = choicers.Where(w => w.CQuestion.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    choicers = choicers.OrderByDescending(w => w.CQuestion);
                    break;
                default:
                    choicers = choicers.OrderBy(w => w.CQuestion);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(choicers.ToPagedList(pageNumber, pageSize));
        }
        // share online
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
            var choicers = db.Choicers.Where(x => (x.IsShareOnline == true && x.FCUserID == userid));

            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                choicers = choicers.Where(w => w.CQuestion.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    choicers = choicers.OrderByDescending(w => w.CQuestion);
                    break;
                default:
                    choicers = choicers.OrderBy(w => w.CQuestion);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(choicers.ToPagedList(pageNumber, pageSize));
        }
        // GET: Choicer Answer
        public ActionResult Answer(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choicer choicer = db.Choicers.Find(id);
            if (choicer == null)
            {
                return HttpNotFound();
            }
            
            return View(choicer);
        }
        // GET: Choicer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choicer choicer = db.Choicers.Find(id);
            if (choicer == null)
            {
                return HttpNotFound();
            }
            return View(choicer);
        }

        // GET: Choicer/Create
        public ActionResult Create()
        {
            ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name");
          //  ViewBag.FCUserID = new SelectList(db.Users, "UserID", "Forename");
            return View();
        }

        // POST: Choicer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChoicerID,FCategoryID,FCUserID,CQuestion,Opt1,Opt2,Opt3,Opt4,Opt5,CAnswer,IsShareOnline,IsFavourite,IsSkip,Appearance,WAppearance,Score,AdditionalInfo")] Choicer choicer, bool checkResp = false)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {
                    if (checkResp == true)
                    {
                        choicer.IsShareOnline = true;
                    }
                    else
                    {
                        choicer.IsShareOnline = false;

                    }
                    if (Session["UserID"] != null)
                    {
                        string id = Session["UserID"].ToString();
                        choicer.FCUserID = Convert.ToInt32(id);
                    }

                    choicer.FCUserID = userid;

                    db.Choicers.Add(choicer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name", choicer.FCategoryID);
           // ViewBag.FCUserID = new SelectList(db.Users, "UserID", "Forename", choicer.FCUserID);
            //ViewBag.FCUserID = Session["UserID"].ToString();
            return View(choicer);
        }

        // GET: Choicer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choicer choicer = db.Choicers.Find(id);
            if (choicer.IsShareOnline == true)
            {
                ViewBag.chkShare = true;
            }
            else
            {
                ViewBag.chkShare = false;
            }
            if (choicer == null)
            {
                return HttpNotFound();
            }
            ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name", choicer.FCategoryID);
          //  ViewBag.FCUserID = new SelectList(db.Users, "UserID", "Forename", choicer.FCUserID);
            return View(choicer);
        }

        // POST: Choicer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChoicerID,FCategoryID,FCUserID,CQuestion,Opt1,Opt2,Opt3,Opt4,Opt5,CAnswer,IsShareOnline,IsFavourite,IsSkip,Appearance,WAppearance,Score,AdditionalInfo")] Choicer choicer, bool chkShare)
        {
            if (ModelState.IsValid)
            {
                ViewBag.chkShare = chkShare;
                if (chkShare)
                {
                    choicer.IsShareOnline = true;
                }
                else
                {
                    choicer.IsShareOnline = false;
                }
                choicer.FCUserID = userid;

                ViewBag.FCategoryID = new SelectList(db.Categories, "CategoryID", "Name", choicer.FCategoryID);
                db.Entry(choicer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          //  ViewBag.FCUserID = new SelectList(db.Users, "UserID", "Forename", choicer.FCUserID);
            return View(choicer);
        }

        // GET: Choicer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Choicer choicer = db.Choicers.Find(id);
            if (choicer == null)
            {
                return HttpNotFound();
            }
            return View(choicer);
        }

        // POST: Choicer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Choicer choicer = db.Choicers.Find(id);
            db.Choicers.Remove(choicer);
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
