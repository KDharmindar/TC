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
    public class QTestController : Controller
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
            IQueryable<QTest> qTests = null;
            ViewBag.CurrentFilter = searchString;
            if (usertype == "No")
            {
                qTests = from w in db.QTests
                         where w.FQTuserID == userid
                         select w;
            }
            else if (usertype == "admin")
            {
                qTests = from w in db.QTests
                         select w;
            }
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {


                qTests = qTests.Where(w => w.QTNote.Contains(searchString));

            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    qTests = qTests.OrderByDescending(w => w.QTNote);
                    break;
                default:
                    qTests = qTests.OrderBy(w => w.QTNote);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(qTests.ToPagedList(pageNumber, pageSize));
        }

        //public ActionResult Testhome()
        //{
        //    //if (Session["Rem_Time"] == null)
        //    //{
        //    //    Session["Rem_Time"] = DateTime.Now.AddMinutes(0.5).ToString("dd-MM-yyyy h:mm:ss tt");
        //    //}
        //    //ViewBag.Rem_Time = Session["Rem_Time"];

        //    //int testID = ViewBag.TestID;

        //    ViewBag.Message = "Modify this template to jump-start your MVC application.";
        //    return View();


        //}

        // WTest start tomorrow, send data to answer view wothout timer
        public ActionResult Answer(int? id)
        {
            var r = (from t in db.QTests
                     where t.FQTuserID == userid
                     orderby t.QTestID descending
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
                QuestTest wt = db.QuestTests.Where(t => (t.FQuesterID == wid) && (t.FQTestID == r.QTestID)).FirstOrDefault();
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
                Quester w = new Quester();
                var IDs = db.QuestTests.Where(t => t.FQTestID == r.QTestID).ToArray().Select(t => t.FQuesterID);

                int index = 0;
                k = IDs.Count();  // k = total no words
                foreach (var c in IDs)
                {
                    w = db.Questers.Find(c);
                    definitions[index] = w.Answer;
                    hint[index] = w.Hint;
                    wordlist[index] = w.Question;
                    fwords[index] = w.QuesterID;
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
                return RedirectToAction("Result", new { id = r.QTestID });

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
            var qTest = db.QuestTests.Where(t => t.FQTestID == id).ToList();
            if (qTest == null)
            {
                return HttpNotFound();
            }
            float percentage = 0;
            int correct = db.QuestTests.Count(x => (x.FQTestID == id) && (x.Result == true));
            int total = db.QuestTests.Count(x => x.FQTestID == id);
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
            return View(qTest);
        }

        // GET: QTest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QTest qTest = db.QTests.Find(id);
            if (qTest == null)
            {
                return HttpNotFound();
            }
            return View(qTest);
        }

        // GET: QTest/Create
        //public ActionResult Create()
        //{
        //    ViewBag.FQTuserID = new SelectList(db.Users, "UserID", "Forename");
        //    return View();
        //}

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

            QTest test = new QTest();
            if (TestWordsParameters != null)
            {

                test.QTNote = title;

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
                    QuestTest wt = new QuestTest();
                    wt.FQuesterID = r;
                    test.QuestTests.Add(wt);  // parent.childs.Add(child obj)
                }
                test.FQTuserID = userid;

                db.QTests.Add(test);
                db.SaveChanges();


            }

            // ViewBag.FWTUserID = new SelectList(db.Users, "UserID", "Forename", wTest.FWTUserID);
            return RedirectToAction("Create");
        }

        // GET: QTest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QTest qTest = db.QTests.Find(id);
            if (qTest == null)
            {
                return HttpNotFound();
            }
         //   ViewBag.FQTuserID = new SelectList(db.Users, "UserID", "Forename", qTest.FQTuserID);
            return View(qTest);
        }

        // POST: QTest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QTestID,FQTuserID,StartTime,EndTime,Score,QTNote,Status,TestTime,TimePerWord,Minutes,Seconds,Duration")] QTest qTest)
        {
            if (ModelState.IsValid)
            {
                qTest.FQTuserID = userid;
                db.Entry(qTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          //  ViewBag.FQTuserID = new SelectList(db.Users, "UserID", "Forename", qTest.FQTuserID);
            return View(qTest);
        }

        // GET: QTest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QTest qTest = db.QTests.Find(id);
            if (qTest == null)
            {
                return HttpNotFound();
            }
            return View(qTest);
        }

        // POST: QTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QTest qTest = db.QTests.Find(id);
            db.QTests.Remove(qTest);
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
