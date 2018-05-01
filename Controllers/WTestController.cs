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
    public class WTestController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

       static string[] definitions = new string[100];
       static string[] hint = new string[100];
       static string[] wordlist = new string[100];
       static int minute=0;
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

            IQueryable<WTest> wTests = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                wTests = from w in db.WTests
                              where w.FWTUserID == userid
                              select w;
            }
            else if (usertype == "admin")
            {
                wTests = from w in db.WTests
                              select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                wTests = wTests.Where(w => w.TNote.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    wTests = wTests.OrderByDescending(w => w.TNote);
                    break;
                default:
                    wTests = wTests.OrderBy(w => w.TNote);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(wTests.ToPagedList(pageNumber, pageSize));
        }



        public ActionResult Testhome()
        {
            //if (Session["Rem_Time"] == null)
            //{
            //    Session["Rem_Time"] = DateTime.Now.AddMinutes(0.5).ToString("dd-MM-yyyy h:mm:ss tt");
            //}
            //ViewBag.Rem_Time = Session["Rem_Time"];

            //int testID = ViewBag.TestID;

            ViewBag.Message = "Modify this template to jump-start your MVC application.";
            return View();

           
        }



        // WTest start tomorrow, send data to answer view wothout timer
        public ActionResult Answer(int? id)
        {
            var r = (from t in db.WTests
                     where t.FWTUserID == userid
                     orderby t.WTestID descending
                     select t).First();
                double time = Convert.ToDouble(r.Score);
                bool minute = r.TestTime ?? false;
                bool second = r.TimePerWord ?? false;
               if( minute==true )
               {
                   if (Session["EndDate"] == null)
                   {
                       Session["EndDate"] = DateTime.Now.AddMinutes(time).ToString("dd-MM-yyyy h:mm:ss tt");
                   }
               }
               else if (second==true)
               {
                   Session["EndDate"] = DateTime.Now.AddSeconds(time).ToString("dd-MM-yyyy h:mm:ss tt");
               }
               // Session["EndDate"] = DateTime.Now.AddMinutes(minute).ToString("dd-MM-yyyy h:mm:ss tt");
           
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.EndDate = Session["EndDate"];
            int value = id ?? default(int);
            if(value==1)
            {
                int wid = fwords[true_count];
                WordTest wt = db.WordTests.Where(t => (t.FWordID == wid) && (t.FWtestID==r.WTestID)).FirstOrDefault(); 
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
                Wordutopia w = new Wordutopia();
                var IDs = db.WordTests.Where(t => t.FWtestID == r.WTestID).ToArray().Select(t => t.FWordID);
                
                int index = 0;
                k = IDs.Count();  // k = total no words
                foreach (var c in IDs)
                {
                    w = db.Wordutopias.Find(c);
                    definitions[index] = w.Definition;
                    hint[index] = w.Hint;
                    wordlist[index] = w.Word;
                    fwords[index] = w.WordID;
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
                  return RedirectToAction("Result", new { id = r.WTestID });
                
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
            var wTest = db.WordTests.Where(t => t.FWtestID == id).ToList();
            if (wTest == null)
            {
                return HttpNotFound();
            }
            float percentage=0;
            int correct = db.WordTests.Count(x => (x.FWtestID == id) && (x.Result == true));
            int total = db.WordTests.Count(x => x.FWtestID == id);
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
            return View(wTest);
        }
        // GET: WTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WTest wTest = db.WTests.Find(id);
            if (wTest == null)
            {
                return HttpNotFound();
            }
            return View(wTest);
        }

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
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(wordutopias.ToPagedList(pageNumber, pageSize));

            //ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename");
            //return View();
        }

        // POST: WTest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create(int[] TestWordsParameters, string title,int rstatus, int time)  //string Title, bool? TotalTime , bool? Second, DateTime? Time
        {
         //   string name = Request.Form["Title"].ToString();
            
                WTest test = new WTest();
                if (TestWordsParameters != null)
                {
                   
                    test.TNote = title;

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
                        WordTest wt = new  WordTest();
                        wt.FWordID = r;
                        test.WordTests.Add(wt);  // parent.childs.Add(child obj)
                    }
                    test.FWTUserID = userid;
                    db.WTests.Add(test);
                    db.SaveChanges();
                   
  
            }

           // ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename", wTest.FWTUserID);
            return RedirectToAction("Create");
        }

        // GET: WTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WTest wTest = db.WTests.Find(id);
            if (wTest == null)
            {
                return HttpNotFound();
            }
           // ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename", wTest.FWTUserID);
            return View(wTest);
        }

        // POST: WTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WTestID,FWTUserID,StartTime,EndTime,Score,TNote,Status,TestTime,TimePerWord,Minutes,Seconds,Duration")] WTest wTest)
        {
            if (ModelState.IsValid)
            {
                wTest.FWTUserID = userid;

                db.Entry(wTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename", wTest.FWTUserID);
            return View(wTest);
        }

        // GET: WTest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WTest wTest = db.WTests.Find(id);
            if (wTest == null)
            {
                return HttpNotFound();
            }
            return View(wTest);
        }

        // POST: WTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WTest wTest = db.WTests.Find(id);
            db.WTests.Remove(wTest);
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
