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
using PagedList;


namespace tutioncloud.Controllers
{
   // [Authorize(Roles = "Admin")]
 //  [Authorize]
    public class ContactController : Controller
    {
        private TuitionCloudDBEntities db = new TuitionCloudDBEntities();

        // GET: Contact
        //public ActionResult Index()
        //{
        //    var contacts = db.Contacts.Include(c => c.User);
        //    return View(contacts.ToList());
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
            var contacts = db.Contacts.Include(c => c.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(s => s.Name.Contains(searchString)
                                       || s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    contacts = contacts.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    contacts = contacts.OrderBy(s => s.Name);
                    break;
                case "date_desc":
                    contacts = contacts.OrderByDescending(s => s.Name);
                    break;
                default:  // Name ascending 
                    contacts = contacts.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(contacts.ToPagedList(pageNumber, pageSize));
        }


        // GET: Contact/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contact/Create
       [AllowAnonymous]
        public ActionResult Create()
        {
           // ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename");
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

         public ActionResult Create([Bind(Include = "ContactID,Name,Email,Description,SecurityCode,UserID")] Contact contact, string empty)
        {
            
            if (ModelState.IsValid)
            {
              
                // Code for validating the CAPTCHA  
                if (this.IsCaptchaValid("Captcha is valid"))
                {
                  
                    db.Contacts.Add(contact);
                    db.SaveChanges();
                    TempData["Success"] = "Your message has been submitted !";
                    return RedirectToAction("Create", "Contact");
                    // return RedirectToAction("Index");
                }
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename", contact.UserID);
                // for captcha
            ViewBag.ErrMessage = "Error: captcha is not valid.";  
            return View(contact);
        }

        // GET: Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename", contact.UserID);
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContactID,Name,Email,Description,SecurityCode,UserID")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "Forename", contact.UserID);
            return View(contact);
        }

        // GET: Contact/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
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
