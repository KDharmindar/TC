

using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using tutioncloud.Helpers;
using tutioncloud.Models;
using tutioncloud.ViewModels;


namespace tutioncloud.Controllers
{
    [SessionExpire]
    public class WordutopiaController : Controller
    {

        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();
        private int userid = 0;
        private string usertype = string.Empty;

        public WordutopiaController() : this(new SessionManagement())
        {

        }
        public WordutopiaController(SessionManagement objSession)
        {
            userid = objSession.UserId;
            usertype = objSession.UserType;

        }

        // GET: Wordutopia
        //public ActionResult Index()
        //{
        //    var wordutopias = db.Wordutopias.Include(w => w.User);
        //    return View(wordutopias.ToList());
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

            IQueryable<Wordutopia> wordutopias = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                wordutopias = from w in db.Wordutopias
                              where w.FWUserID == userid
                              select w;
            }
            else if (usertype == "admin")
            {
                wordutopias = from w in db.Wordutopias
                              select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                wordutopias = wordutopias.Where(w => w.Word.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    wordutopias = wordutopias.OrderByDescending(w => w.Word);
                    break;
                default:
                    wordutopias = wordutopias.OrderBy(w => w.Word);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(wordutopias.ToPagedList(pageNumber, pageSize));
        }

        // Index  [HttpPost] for favourite and skip
        [HttpPost]
        public ActionResult Index(int[] Parameters, int[] SkipParameters, int[] TestWordsParameters, int[] IDs)
        {
            if (IDs != null)
            {
                var record = db.Wordutopias.Find(IDs[0]);
                var re = record.Word;
                // return Redirect("http://thesaurus.reference.com/browse/" + re);
                // return new  RedirectResult("http://www.google.com");
                return Json(new { url = "http://thesaurus.reference.com/browse/" + re });
                //  //   Response.Redirect("http://www.google.com");
            }
            // for favourite
            //  int l = Parameters.Length;
            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    var k = Parameters[i];
                    var q = (from word in db.Wordutopias
                             where word.WordID == k
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
                    var q = (from word in db.Wordutopias
                             where word.WordID == k
                             select word).SingleOrDefault();
                    q.IsSkip = true;
                    db.SaveChanges();

                }
            }

            // for add test words 
            if (TestWordsParameters != null)
            {
                //var countOfRows = db.WTests.Count();

                //var lastRow = db.WTests.Skip(countOfRows - 1).FirstOrDefault();
                // var last = db.WTests.LastOrDefault();
                var result = (from t in db.WTests
                              orderby t.WTestID descending
                              select t.WTestID).First();

                ViewBag.TestID = result;

                WordTest Wt = new WordTest();
                Wt.FWtestID = result;
                for (int j = 0; j < TestWordsParameters.Length; j++)
                {
                    var r = TestWordsParameters[j];


                    Wt.FWordID = r;
                    db.WordTests.Add(Wt);
                    db.SaveChanges();


                }
                // return RedirectToAction("Answer", "WTest",new {id = ViewBag.TestID});
            }
            else
            {
                // NO WORD HAS SELECTED .. PLZ SELECTED WORDS FOR TEST ????
            }
            return RedirectToAction("Index");
            //  int j = int(Parameters[1]);


        }

        // for Home View
        public ActionResult Home( string randomViewType = "normal")
        {

            var firstdata = (from s in db.Wordutopias where s.FWUserID == userid select s)
                            .OrderBy(x => Guid.NewGuid())
                            .FirstOrDefault();
            ViewBag.ViewType = randomViewType;
            if (firstdata == null)
            {
                return RedirectToAction("Welcome", "Wordutopia");
            }
            else
            {
                return View(InitializeWordutopiaViewModel(firstdata));
            }
        }
        // for Home next

        public JsonResult GetWordForHome(List<int> previousWordIds, string type, int currentWordId)
        {
            if (previousWordIds == null)
            {
                previousWordIds = new List<int>();
            }

            if (type.ToLower() == "next")
            {
                
                previousWordIds.Add(currentWordId);

                int totalWordCount = db.Wordutopias.Where(x => x.FWUserID == userid).Count();

                //start the loop again as all the words viewed
                if (totalWordCount == previousWordIds.Count())
                {
                    previousWordIds.Clear();
                }

                var model = GetNextWord(previousWordIds);

                model.ListOfPreviousWords = previousWordIds;

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            if (type.ToLower() == "previous")
            {
                int previousWordId = previousWordIds.Last();

                var model = GetPreviousWord(previousWordId);
                previousWordIds.RemoveAt(previousWordIds.Count - 1);

                model.ListOfPreviousWords = previousWordIds;

                return Json(model, JsonRequestBehavior.AllowGet);

            }


            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        
        private WordutopiaViewModel GetNextWord(List<int> previousWordIds)
        {
            var wordutopia = db.Wordutopias.Where(w => !previousWordIds.Contains(w.WordID) && w.FWUserID == userid)
                                            .OrderBy(x => Guid.NewGuid())
                                            .FirstOrDefault();

            return InitializeWordutopiaViewModel(wordutopia);

        }

        private WordutopiaViewModel GetPreviousWord(int previousWordId)
        {
            var wordutopia = db.Wordutopias.Where(x => x.WordID == previousWordId).FirstOrDefault();

            return InitializeWordutopiaViewModel(wordutopia);
        }


        private WordutopiaViewModel InitializeWordutopiaViewModel(Wordutopia model)
        {
            return new WordutopiaViewModel(model);
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

            var wordutopias = db.Wordutopias.Where(x => (x.IsFavourite == true && x.FWUserID == userid));

            //for sortOrder
            switch (sortOrder)
            {
                case "name_desc":
                    wordutopias = wordutopias.OrderByDescending(s => s.Word);
                    break;
                case "Date":
                    wordutopias = wordutopias.OrderBy(s => s.Score);
                    break;
                case "date_desc":
                    wordutopias = wordutopias.OrderByDescending(s => s.Score);
                    break;
                default:
                    wordutopias = wordutopias.OrderBy(s => s.Word);
                    break;
            }
            //for searchString

            if (!String.IsNullOrEmpty(searchString))
            {
                wordutopias = wordutopias.Where(s => s.Word.Contains(searchString));
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);


            return View(wordutopias.ToPagedList(pageNumber, pageSize));

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
            var wordutopias = db.Wordutopias.Where(x => (x.IsSkip == true && x.FWUserID == userid));

            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                wordutopias = wordutopias.Where(w => w.Word.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    wordutopias = wordutopias.OrderByDescending(w => w.Word);
                    break;
                default:
                    wordutopias = wordutopias.OrderBy(w => w.Word);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(wordutopias.ToPagedList(pageNumber, pageSize));
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
            var wordutopias = db.Wordutopias.Where(x => (x.IsShareOnline == true && x.FWUserID == userid));

            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                wordutopias = wordutopias.Where(w => w.Word.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    wordutopias = wordutopias.OrderByDescending(w => w.Word);
                    break;
                default:
                    wordutopias = wordutopias.OrderBy(w => w.Word);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(wordutopias.ToPagedList(pageNumber, pageSize));
        }
        // for review words
        public ActionResult ReviewWords(int? id)
        {

            int count = 0;
            List<Wordutopia> words = new List<Wordutopia>();
            var results = (from s in db.Wordutopias where s.FWUserID == userid select s);
            foreach (var r in results)
            {
                words.Add(r);
                count = count + 1;

                if (r.WordID == id)


                    break;
            }
            return View(words.ToList());
            // return View();
        }
        // GET: Wordutopia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wordutopia wordutopia = db.Wordutopias.Find(id);
            if (wordutopia == null)
            {
                return HttpNotFound();
            }
            return View(wordutopia);
        }

        // GET: Wordutopia/Create
        public ActionResult Create()
        {
            // ViewBag.FWUserID = new SelectList(db.Users, "UserID", "Forename");
            return View();
        }

        // POST: Wordutopia/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WordID,FWUserID,Word,Pronounciation,Phrase,Hint,Definition,Synonym,IsShareOnline,IsFavourite,IsSkip,Appearance,WAppreance,Score,AdditionInfo")] Wordutopia wordutopia, bool checkResp = false)
        {
            //ViewBag.FWUserID = new SelectList(db.Users, "UserID", "Forename", wordutopia.FWUserID);
            if (ModelState.IsValid)
            {
                if (Session["UserID"] != null)
                {

                    if (checkResp == true)
                    {
                        wordutopia.IsShareOnline = true;
                    }
                    else
                    {
                        wordutopia.IsShareOnline = false;

                    }
                    wordutopia.FWUserID = userid;
                    db.Wordutopias.Add(wordutopia);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }


            return View(wordutopia);
        }

        // GET: Wordutopia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wordutopia wordutopia = db.Wordutopias.Find(id);

            if (wordutopia.IsShareOnline == true)
            {
                ViewBag.chkShare = true;
            }
            else
            {
                ViewBag.chkShare = false;
            }
            if (wordutopia == null)
            {
                return HttpNotFound();
            }
            // ViewBag.FWUserID = new SelectList(db.Users, "UserID", "Forename", wordutopia.FWUserID);
            return View(wordutopia);
        }

        // POST: Wordutopia/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WordID,FWUserID,Word,Pronounciation,Phrase,Hint,Definition,Synonym,IsShareOnline,IsFavourite,IsSkip,Appearance,WAppreance,Score,AdditionInfo")] Wordutopia wordutopia, bool chkShare)
        {
            if (ModelState.IsValid)
            {
                ViewBag.chkShare = chkShare;
                if (chkShare)
                {
                    wordutopia.IsShareOnline = true;
                }
                else
                {
                    wordutopia.IsShareOnline = false;
                }
                wordutopia.FWUserID = userid;
                db.Entry(wordutopia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.FWUserID = new SelectList(db.Users, "UserID", "Forename", wordutopia.FWUserID);
            return View(wordutopia);
        }

        // GET: Wordutopia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wordutopia wordutopia = db.Wordutopias.Find(id);
            if (wordutopia == null)
            {
                return HttpNotFound();
            }
            return View(wordutopia);
        }

        // POST: Wordutopia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wordutopia wordutopia = db.Wordutopias.Find(id);
            db.Wordutopias.Remove(wordutopia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult AddToFavourite(int id)
        {
            var word = db.Wordutopias.Find(id);

            if (word != null)
            {
                word.IsFavourite = true;
                db.Entry(word).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Added successfully.");

            }

            return Json("Word already exists.");
        }
        public JsonResult AddToSkippedList(int id)
        {
            var word = db.Wordutopias.Find(id);

            if (word != null)
            {
                word.IsSkip = true;
                db.Entry(word).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Added successfully.");

            }

            return Json("Word already exists.");
        }

        public ActionResult RemoveFromFavouriteList(int id)
        {
            var word = db.Wordutopias.Find(id);

            if (word != null)
            {
                word.IsFavourite = false;
                db.Entry(word).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Favourite");

            }

            return RedirectToAction("Favourite");
        }
        public JsonResult RemoveFromSkippedList(int id)
        {
            var word = db.Wordutopias.Find(id);

            if (word != null)
            {
                word.IsSkip = false;
                db.Entry(word).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Removed successfully.");

            }

            return Json("Word already exists.");
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
