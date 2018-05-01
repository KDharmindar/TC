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
    public class QuestShareController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();
        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

        // GET: QuestShare
        public ActionResult Index()
        {
            var questShares = db.QuestShares.Include(q => q.Quester);
            return View(questShares.ToList());
        }

        public ActionResult MainShare()
        {
            return View();
        }

        public ActionResult SharedWords(string sortOrder, string currentFilter, string searchString, int? page)
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

            var questShares = from w in db.QuestShares
                             select w;
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                questShares = questShares.Where(w => w.Password.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    questShares = questShares.OrderByDescending(w => w.Password);
                    break;
                default:
                    questShares = questShares.OrderBy(w => w.Password);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            // ViewBag.FWTUserID = new SelectList(db.Users.ToList(), "UserID", "Forename");
            // ViewBag.UserFrom = Session["UserID"];
            // ViewBag.Password = Guid.NewGuid().ToString().Substring(0, 8);

            return View(questShares.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult SaveDatabase(int[] IDs)
        {
            if (IDs != null)
            {
                //var countOfRows = db.WTests.Count();

                //var lastRow = db.WTests.Skip(countOfRows - 1).FirstOrDefault();
                // var last = db.WTests.LastOrDefault();




                Quester Qt = new Quester();

                for (int j = 0; j < IDs.Length; j++)
                {
                    var id = IDs[j];
                    var r = (from t in db.Questers
                             where t.QuesterID == id
                             select t).FirstOrDefault();
                    Qt.Question = r.Question;
                    Qt.Answer = r.Answer;
                    Qt.Hint = r.Hint;
                    Qt.AdditionalInfo = r.AdditionalInfo;
                    Qt.FQUserID = userid;  //Wr.FWuserid=Session["userID"]

                   
                    db.Questers.Add(Qt);
                    db.SaveChanges();


                }
                return RedirectToAction("Index", "Quester");
            }
            return View();
        }

        // GET: QuestShare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestShare questShare = db.QuestShares.Find(id);
            if (questShare == null)
            {
                return HttpNotFound();
            }
            return View(questShare);
        }

        // GET: QuestShare/Create
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

            ViewBag.CurrentFilter = searchString;

            var questers = from w in db.Questers where w.FQUserID == userid
                              select w;
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


            ViewBag.FWTUserID = new SelectList(db.Users.ToList(), "UserID", "Forename");
            // ViewBag.UserFrom = Session["UserID"];
            ViewBag.Password = Guid.NewGuid().ToString().Substring(0, 8);

            return View(questers.ToPagedList(pageNumber, pageSize));
        }

        // POST: WordShare/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult Create(int[] TestWordsParameters, string password, int UserTo)
        {
            // int userto,string password
            // ViewBag.FWTUserID = new SelectList(db.Wordutopias.ToList(), "WordID", "Word", wordShare.UserTo);
            //if (ModelState.IsValid)
            //{


            for (int j = 0; j < TestWordsParameters.Length; j++)
            {
                QuestShare Qshare = new QuestShare();
                Qshare.FQuesterID = TestWordsParameters[j];
                Qshare.UserFrom = userid;
                Qshare.UserTo = UserTo;
                Qshare.Password = password;

                db.QuestShares.Add(Qshare);
                db.SaveChanges();


            }

            return RedirectToAction("Create");
        }


        // GET: QuestShare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestShare questShare = db.QuestShares.Find(id);
            if (questShare == null)
            {
                return HttpNotFound();
            }
            ViewBag.FQuesterID = new SelectList(db.Questers, "QuesterID", "Question", questShare.FQuesterID);
            return View(questShare);
        }

        // POST: QuestShare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QShareID,FQuesterID,UserTo,UserFrom,Password,ShareDate")] QuestShare questShare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questShare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FQuesterID = new SelectList(db.Questers, "QuesterID", "Question", questShare.FQuesterID);
            return View(questShare);
        }

        // GET: QuestShare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestShare questShare = db.QuestShares.Find(id);
            if (questShare == null)
            {
                return HttpNotFound();
            }
            return View(questShare);
        }

        // POST: QuestShare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestShare questShare = db.QuestShares.Find(id);
            db.QuestShares.Remove(questShare);
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
