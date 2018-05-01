using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tutioncloud.Models;
using System.IO;
using PagedList;

namespace tutioncloud.Controllers
{
    public class WordShareController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        int userid = (int)System.Web.HttpContext.Current.Session["UserID"];
        string usertype = (string)System.Web.HttpContext.Current.Session["UserType"];

      
        // GET: WordShare
        public ActionResult Index()
        {
            var wordShares = db.WordShares.Include(w => w.Wordutopia);
            return View(wordShares.ToList());
        }
        public ActionResult ShowWords(string searchString)
        {
            ViewBag.CurrentFilter = searchString;

                         
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                 var wordShares = from w in db.WordShares.Where(j => j.Password == searchString) select w;
                return RedirectToAction("SharedWords", wordShares.ToList());
            }
            return View();
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

            var wordShares = from w in db.WordShares
                              select w;
            // for search string
            if (!String.IsNullOrEmpty(searchString))
            {
                wordShares = wordShares.Where(w => w.Password.Contains(searchString));
            }
            // for sorting
            switch (sortOrder)
            {
                case "name_desc":
                    wordShares = wordShares.OrderByDescending(w => w.Password);
                    break;
               
            }
            // var wordutopias = db.Wordutopias.Include(w => w.User);
            // return View(wordutopias.ToList());
           


           // ViewBag.FWTUserID = new SelectList(db.Users.ToList(), "UserID", "Forename");
            // ViewBag.UserFrom = Session["UserID"];
           // ViewBag.Password = Guid.NewGuid().ToString().Substring(0, 8);

            return View(wordShares.ToList());
        }

        [HttpPost]
        public ActionResult SaveDatabase(int[] IDs)
        {
            if (IDs != null)
            {
                //var countOfRows = db.WTests.Count();

                //var lastRow = db.WTests.Skip(countOfRows - 1).FirstOrDefault();
                // var last = db.WTests.LastOrDefault();
               

               

                Wordutopia Wt = new Wordutopia();
               
                for (int j = 0; j < IDs.Length; j++)
                {
                    var id = IDs[j];
                     var r = (from t in db.Wordutopias
                              where t.WordID==id
                              select t).FirstOrDefault();
                    Wt.Word=r.Word;
                     Wt.Definition = r.Definition;
                     Wt.Appearance = r.Appearance;
                     Wt.AdditionInfo = r.AdditionInfo;
                     Wt.FWUserID = userid;  //Wr.FWuserid=Session["userID"]

                     Wt.Hint = r.Hint;
                     Wt.Phrase = r.Phrase;
                    db.Wordutopias.Add(Wt);
                    db.SaveChanges();


                }
                 return RedirectToAction("Index","Wordutopia");
            }
        return View();
        }
        // GET: WordShare/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordShare wordShare = db.WordShares.Find(id);
            if (wordShare == null)
            {
                return HttpNotFound();
            }
            return View(wordShare);
        }

        // GET: WordShare/Create
        //public ActionResult Create()
        //{
        //    ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word");
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

            ViewBag.CurrentFilter = searchString;

            var wordutopias = from w in db.Wordutopias where w.FWUserID == userid
                              select w;
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
           

            ViewBag.FWTUserID = new SelectList(db.Users.ToList(), "UserID", "Forename");
           // ViewBag.UserFrom = Session["UserID"];
            ViewBag.Password = Guid.NewGuid().ToString().Substring(0, 8);

          return View(wordutopias.ToPagedList(pageNumber, pageSize));
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
                    WordShare Wshare = new WordShare();
                    Wshare.FWordID = TestWordsParameters[j];
                    Wshare.UserFrom = userid;
                    Wshare.UserTo = UserTo;
                   Wshare.Password=password;
                     
                     db.WordShares.Add(Wshare);
                    db.SaveChanges();

                    
                }
                
               
            //}

                return RedirectToAction("Create");
        }

        // GET: WordShare/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordShare wordShare = db.WordShares.Find(id);
            if (wordShare == null)
            {
                return HttpNotFound();
            }
            ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word", wordShare.FWordID);
            return View(wordShare);
        }

        // POST: WordShare/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WShareID,FWordID,UserTo,UserFrom,Password,ShareDate")] WordShare wordShare)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wordShare).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FWordID = new SelectList(db.Wordutopias, "WordID", "Word", wordShare.FWordID);
            return View(wordShare);
        }

        // GET: WordShare/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WordShare wordShare = db.WordShares.Find(id);
            if (wordShare == null)
            {
                return HttpNotFound();
            }
            return View(wordShare);
        }

        // POST: WordShare/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WordShare wordShare = db.WordShares.Find(id);
            db.WordShares.Remove(wordShare);
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
