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
    public class ChoiceShareController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

        // GET: ChoiceShare
        public ActionResult Index()
        {
            var choiceShares = db.ChoiceShares.Include(c => c.Choicer);
            return View(choiceShares.ToList());
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

            var choiceShares = from w in db.ChoiceShares
                              select w;
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                choiceShares = choiceShares.Where(w => w.Password.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    choiceShares = choiceShares.OrderByDescending(w => w.Password);
                    break;
                default:
                    choiceShares = choiceShares.OrderBy(w => w.Password);
                    break;
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            // ViewBag.FWTUserID = new SelectList(db.Users.ToList(), "UserID", "Forename");
            // ViewBag.UserFrom = Session["UserID"];
            // ViewBag.Password = Guid.NewGuid().ToString().Substring(0, 8);

            return View(choiceShares.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult SaveDatabase(int[] IDs)
        {
            if (IDs != null)
            {
                //var countOfRows = db.WTests.Count();

                //var lastRow = db.WTests.Skip(countOfRows - 1).FirstOrDefault();
                // var last = db.WTests.LastOrDefault();




                Choicer Ct = new Choicer();

                for (int j = 0; j < IDs.Length; j++)
                {
                    var id = IDs[j];
                    var r = (from t in db.Choicers
                             where t.ChoicerID == id
                             select t).FirstOrDefault();
                    Ct.CQuestion = r.CQuestion;
                    Ct.CAnswer = r.CAnswer;
                    Ct.Score = r.Score;
                    Ct.AdditionalInfo = r.AdditionalInfo;
                    Ct.FCUserID = userid;  //Wr.FWuserid=Session["userID"]


                    db.Choicers.Add(Ct);
                    db.SaveChanges();


                }
                return RedirectToAction("Index", "Choicer");
            }
            return View();
        }

        // GET: ChoiceShare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChoiceShare choiceShare = db.ChoiceShares.Find(id);
            if (choiceShare == null)
            {
                return HttpNotFound();
            }
            return View(choiceShare);
        }

        // GET: ChoiceShare/Create
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

            var choicers = from w in db.Choicers
                           where w.FCUserID == userid
                           select w;
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


            ViewBag.FWTUserID = new SelectList(db.Users.ToList(), "UserID", "Forename");
            // ViewBag.UserFrom = Session["UserID"];
            ViewBag.Password = Guid.NewGuid().ToString().Substring(0, 8);

            return View(choicers.ToPagedList(pageNumber, pageSize));
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
                ChoiceShare Cshare = new ChoiceShare();
                Cshare.FChoicerID = TestWordsParameters[j];
                Cshare.UserFrom = userid;
                Cshare.UserTo = UserTo;
                Cshare.Password = password;

                db.ChoiceShares.Add(Cshare);
                db.SaveChanges();


            }


            //}

            return RedirectToAction("Create");
        }


        // GET: ChoiceShare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChoiceShare choiceShare = db.ChoiceShares.Find(id);
            if (choiceShare == null)
            {
                return HttpNotFound();
            }
            ViewBag.FChoicerID = new SelectList(db.Choicers, "ChoicerID", "CQuestion", choiceShare.FChoicerID);
            return View(choiceShare);
        }

        // POST: ChoiceShare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CShareID,FChoicerID,UserTo,UserFrom,Password,ShareDate")] ChoiceShare choiceShare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(choiceShare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FChoicerID = new SelectList(db.Choicers, "ChoicerID", "CQuestion", choiceShare.FChoicerID);
            return View(choiceShare);
        }

        // GET: ChoiceShare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChoiceShare choiceShare = db.ChoiceShares.Find(id);
            if (choiceShare == null)
            {
                return HttpNotFound();
            }
            return View(choiceShare);
        }

        // POST: ChoiceShare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChoiceShare choiceShare = db.ChoiceShares.Find(id);
            db.ChoiceShares.Remove(choiceShare);
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
