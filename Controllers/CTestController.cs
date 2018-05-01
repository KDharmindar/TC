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
    public class CTestController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

        static string[] definitions = new string[100];
        static string[] hint = new string[100];
        static string[] wordlist = new string[100];
        static int minute = 0;
        static int[] fwords = new int[100];
        static int fWordid = 0;
        static int turnfirst = 0;
        static int v = 0;
        static int true_count = 0;
        static int testid = 0;

        static int k = 0;
        // GET: WTest
        //public ActionResult Index()
        //{
        //    var wTests = db.WTests.Include(w => w.User);
        //    return View(wTests.ToList());
        //}

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

            IQueryable<CTest> cTests = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                cTests = from w in db.CTests
                         where w.FCTuserID == userid
                         select w;
            }
            else if (usertype == "admin")
            {
                cTests = from w in db.CTests
                         select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                cTests = cTests.Where(w => w.CTNote.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    cTests = cTests.OrderByDescending(w => w.CTNote);
                    break;
                default:
                    cTests = cTests.OrderBy(w => w.CTNote);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(cTests.ToPagedList(pageNumber, pageSize));
        }


        // WTest start tomorrow, send data to answer view wothout timer
        public ActionResult Answer(int? id)
        {
            var r = (from t in db.CTests
                     where t.FCTuserID == userid
                     orderby t.CTestID descending
                     select t).First();
            double time = Convert.ToDouble(r.Score);
            bool minute = r.TestTime ?? false;
            bool second = r.TimePerWord ?? false;
            if (minute == true)
            {
                if (Session["EndDate"] == null)
                {
                    Session["EndDate"] = DateTime.Now.AddMinutes(time).ToString("dd-MM-yyyy h:mm:ss tt");
                }
            }
            else if (second == true)
            {
                Session["EndDate"] = DateTime.Now.AddSeconds(time).ToString("dd-MM-yyyy h:mm:ss tt");
            }
            // Session["EndDate"] = DateTime.Now.AddMinutes(minute).ToString("dd-MM-yyyy h:mm:ss tt");

            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.EndDate = Session["EndDate"];
            int value = id ?? default(int);
            if (value == 1)
            {
                int wid = fwords[true_count];
                ChoiceTest wt = db.ChoiceTests.Where(t => (t.FChoicerID == wid) && (t.FCTestID == r.CTestID)).FirstOrDefault();
                wt.Result = true;
                db.SaveChanges();
                true_count = true_count + 1;
            }


            // var d = db.WordTests.Where(t => t.FWtestID==1).FirstOrDefault();  // working

            //int turnfirst = 0;
            if (turnfirst == 0)
            {
                v = 0;
                //testid = (from t in db.WTests
                //          orderby t.WTestID descending
                //          select t.WTestID).First();
                Choicer w = new Choicer();
                var IDs = db.ChoiceTests.Where(t => t.FCTestID == r.CTestID).ToArray().Select(t => t.FChoicerID);

                int index = 0;
                k = IDs.Count();  // k = total no words
                foreach (var c in IDs)
                {
                    w = db.Choicers.Find(c);
                    if(w.CAnswer==1)
                    {
                        definitions[index] = w.Opt1.ToString();
                    }
                    else if (w.CAnswer == 2)
                    {
                        definitions[index] = w.Opt2.ToString();
                    }
                    else if (w.CAnswer == 3)
                    {
                        definitions[index] = w.Opt3.ToString();
                    }
                    else if (w.CAnswer == 4)
                    {
                        definitions[index] = w.Opt4.ToString();
                    }
                    else if (w.CAnswer == 5)
                    {
                        definitions[index] = w.Opt5.ToString();
                    }
                    //definitions[index] = w.CAnswer.ToString(); // CAnswer is int so convert into string
                    hint[index] = w.Category.Name;      // Appearance colomn replace the hint and convert into string
                    wordlist[index] = w.CQuestion;
                    fwords[index] = w.ChoicerID;
                    index = index + 1;
                }
                turnfirst = 1;
            }
            if (v < k)
            {
                ViewBag.Definition = definitions[v];
                ViewBag.Hint = hint[v];
                ViewBag.Word = wordlist[v];
                v = v + 1;
            }
            else
            {
                ViewBag.Status = "complete";
                ViewBag.EndDate = null;
                return RedirectToAction("Result", new { id = r.CTestID });

            }
            return View();
        }
        public ActionResult Result(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var result = (from t in db.WTests
            //              orderby t.WTestID descending
            //              select t.WTestID).First();
            var cTest = db.ChoiceTests.Where(t => t.FCTestID == id).ToList();
            if (cTest == null)
            {
                return HttpNotFound();
            }
            float percentage = 0;
            int correct = db.ChoiceTests.Count(x => (x.FCTestID == id) && (x.Result == true));
            int total = db.ChoiceTests.Count(x => x.FCTestID == id);
            if (total > 0)
            {
                percentage = (correct * 100) / total;

                if (percentage < 40)
                {
                    ViewBag.Status = "Fail";
                }
                else
                {
                    ViewBag.Status = "Pass";
                }
            }
            else
            {
                ViewBag.Status = "No";
            }
            ViewBag.percentage = percentage;
            return View(cTest);
        }

        //// GET: CTest
        //public ActionResult Index()
        //{
        //    var cTests = db.CTests.Include(c => c.User);
        //    return View(cTests.ToList());
        //}

        // GET: CTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTest cTest = db.CTests.Find(id);
            if (cTest == null)
            {
                return HttpNotFound();
            }
            return View(cTest);
        }

        // GET: CTest/Create
        // GET: WTest/Create
        public ActionResult Create(string sortOrder, string currentFilter, string searchString, int? page)
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

            //ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename");
            //return View();
        }

        // POST: WTest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(int[] TestWordsParameters, string title, int rstatus, int time)  //string Title, bool? TotalTime , bool? Second, DateTime? Time
        {
            //   string name = Request.Form["Title"].ToString();

            CTest test = new CTest();
            if (TestWordsParameters != null)
            {

                test.CTNote = title;

                if (rstatus == 1)
                {
                    test.Score = time;
                    test.TestTime = true;

                }
                else if (rstatus == 2)
                {
                    test.Score = time;
                    test.TimePerWord = true;

                }
                for (int j = 0; j < TestWordsParameters.Length; j++)
                {
                    var r = TestWordsParameters[j];
                    ChoiceTest wt = new ChoiceTest();
                    wt.FChoicerID = r;
                    test.ChoiceTests.Add(wt);  // parent.childs.Add(child obj)
                }
                test.FCTuserID = userid;
                db.CTests.Add(test);
                db.SaveChanges();


            }

            // ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename", wTest.FWTUserID);
            return RedirectToAction("Create");
        }

        // GET: CTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTest cTest = db.CTests.Find(id);
            if (cTest == null)
            {
                return HttpNotFound();
            }
           // ViewBag.FCTuserID = new SelectList(db.Users, "UserID", "Forename", cTest.FCTuserID);
            return View(cTest);
        }

        // POST: CTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CTestID,FCTuserID,StartTime,EndTime,Score,CTNote,Status,TestTime,TimePerWord,Minutes,Seconds,Duration")] CTest cTest)
        {
            if (ModelState.IsValid)
            {
                cTest.FCTuserID = userid;
                db.Entry(cTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          //  ViewBag.FCTuserID = new SelectList(db.Users, "UserID", "Forename", cTest.FCTuserID);
            return View(cTest);
        }

        // GET: CTest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CTest cTest = db.CTests.Find(id);
            if (cTest == null)
            {
                return HttpNotFound();
            }
            return View(cTest);
        }

        // POST: CTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CTest cTest = db.CTests.Find(id);
            db.CTests.Remove(cTest);
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
