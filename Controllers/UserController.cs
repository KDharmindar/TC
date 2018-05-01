using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tutioncloud.Models;
using CaptchaMvc.HtmlHelpers;
using System.Web.Security;
using System.Data.Entity.Validation;
using PagedList;
//using System.Collections.Generic.IEnumerable;


namespace tutioncloud.Controllers
{
    public class UserController : Controller
    {



        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        public ActionResult Admin()
        {
            return View();
        }
        //public ActionResult Welcome()
        //{
        //    return View();
        //}

        // Login get

        public ActionResult Login()
        {
            ViewBag.FUserWebID = new SelectList(db.Websites, "WebID", "WebName");
            return View();
        }

        // Login post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {

            var users = db.Users.SingleOrDefault(u => u.Surname == user.Surname && u.Password == user.Password && u.FUserWebID == user.FUserWebID);
            //var users = db.Users.Single(u => u.Surname.Equals(user) && u.Password.Equals(user) && u.FUserWebID.Equals(user));
            // var users = db.Users.Single(u => u.Surname.Equals(user.Surname) && u.Password.Equals(user.Password) && u.FUserWebID.Equals(user.FUserWebID));
            if (users != null)
            {
                Session["UserID"] = (int)users.UserID;
                Session["UserName"] = Convert.ToString(users.Surname);
                Session["WebName"] = Convert.ToString(users.Website.WebName);
                Session["UserType"] = Convert.ToString(users.UserType);
                FormsAuthentication.SetAuthCookie(users.Surname, false);
                //string webName = Session["WebName"].ToString();
                //user.FUserWebID = Convert.ToInt32(webName);

                {

                    if (users.Surname == "stewart.gemmill")
                    {
                        return RedirectToAction("Admin", "User");
                    }

                    if (users.Website.WebName == "Wordutopia")
                    {
                        return RedirectToAction("Home", "Wordutopia");
                    }

                    if (users.Website.WebName == "Quester")
                    {
                        return RedirectToAction("Home", "Quester");

                    }

                    if (users.Website.WebName == "Choicer")
                    {
                        return RedirectToAction("Home", "Choicer");

                    }
                }

            }
            else
            {
                ModelState.AddModelError("Error", "UserName OR Password is wrong ");
            }
            ViewBag.FUserWebID = new SelectList(db.Websites, "WebID", "WebName", user.FUserWebID);
            //ViewData["FUserWebID"] = new SelectList(db.Websites, "WebID", "WebName");

            return View(users);

        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "User");
        }

        public ActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Manage(FormCollection Form)
        {
            string oldpass = Request.Form["OldPass"];
            string newpass = Request.Form["NewPass"];
            string confirmpass = Request.Form["ConfirmPass"];
            int userid = (int)Session["UserID"];
            // var users = db.Users.Single(u => u.Password == user.Password);
            //var result = from u in db.Users where u.Password == oldpass && u.FUserWebID ==userid select u.Surname;
            User result = (from p in db.Users
                           where p.UserID == userid
                           select p).SingleOrDefault();
            if (result != null)
            {
                if (newpass == confirmpass)
                {
                    try

                    {

                        result.Password = confirmpass;
                        // db.Entry(result).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                var msg = string.Concat("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }


                }
                return RedirectToAction("Login");
            }
            return View();
        }


        // GET: User
        //public ActionResult Index()
        //{
        //    var users = db.Users.Include(u => u.Website);
        //    return View(users.ToList());
        //}

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
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
            var users = db.Users.Include(u => u.Website);

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.Surname.Contains(searchString)
                                       || s.Username.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.Surname);
                    break;
                case "Date":
                    users = users.OrderBy(s => s.Surname);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(s => s.Surname);
                    break;
                default:  // Name ascending 
                    users = users.OrderBy(s => s.Surname);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.FUserWebID = new SelectList(db.Websites, "WebID", "WebName");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,FUserWebID,Forename,Surname,Username,Password,ConfirmPassword,SecurityCode,SubscribeDate,JoinDate,Amount,UserType")] User user, string empty)
        {
            if (ModelState.IsValid)
            {


                // Code for validating the CAPTCHA  
                if (this.IsCaptchaValid("Captcha is valid"))
                {
                    //Check username already exists


                    user.UserType = "No";
                    db.Users.Add(user);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    return RedirectToAction("Login");
                }
            }
            ViewBag.FUserWebID = new SelectList(db.Websites, "WebID", "WebName", user.FUserWebID);
            // for captcha
            ViewBag.ErrMessage = "Error: captcha is not valid.";

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.FUserWebID = new SelectList(db.Websites, "WebID", "WebName", user.FUserWebID);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FUserWebID,Forename,Surname,Username,Password,ConfirmPassword,SecurityCode,SubscribeDate,JoinDate,Amount,UserType")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FUserWebID = new SelectList(db.Websites, "WebID", "WebName", user.FUserWebID);
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
