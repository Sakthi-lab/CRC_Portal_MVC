using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Models;
using PagedList;

namespace Portal.Controllers
{
    public class ContactsController : Controller
    {
        //
        // GET: /Contacts/
       private CustomerEntities db = new CustomerEntities();
        const int pageSize = 20;

       

    public ActionResult Index(string searchVal, string currentFilter, int? page, int dFilter = 1)
        {
          

            ViewBag.dFilter = dFilter;         

            var accounts = from m in db.NCSM_CRC_DUTY_ContactList
                           select m;

            //  currentFilter may be passed in from the paging links so we can reapply the search during the paging.
            if (!string.IsNullOrEmpty(currentFilter))
                searchVal = currentFilter;

            if (!string.IsNullOrEmpty(searchVal))
            {
                accounts = accounts.Where(s => s.ContactFullName.Contains(searchVal));
                ViewBag.currentFilter = searchVal;
            }
                                 

            return View(accounts.OrderBy(p => p.ContactID).ToPagedList((page ?? 1), pageSize));
        }

        [HttpPost]
        public ActionResult Index()
        {

            ViewBag.currentFilter = "";
            return View(db.NCSM_CRC_DUTY_ContactList.OrderBy(p => p.ContactID).ToPagedList(1, pageSize));
        }


        /*  public ActionResult(string search)
          {
              CustomerEntities db = new CustomerEntities();

              return View(db.NCSM_CRC_DUTY_ContactList.Where(x => x.DutyName.Contains(search)).ToList());
          }*/
        public ActionResult Add()
        {
             return View();
        }
       /* public ActionResult Index()
        {
            CustomerEntities db = new CustomerEntities();
            return View(db.CRC_DUTY_Customers_SearchCustomers(""));
        }*/

        [HttpPost]
        public ActionResult Search(string searchname)
        {
                       
            CustomerEntities db = new CustomerEntities();
            // var conts = db.NCSM_CRC_DUTY_ContactList.Where(w => w.ContactFullName.Contains(searchname)).ToList();
            var conts = db.NCSM_CRC_DUTY_ContactList.Where(w => w.ContactFullName == "test").ToList();
            var temp = db.CRC_DUTY_Customers_SearchCustomers(searchname);

            return View("Contacts", conts);
        }

        [HttpPost]
        public ActionResult AddNew(NCSM_CRC_DUTY_ContactList nc)
        {
            

            try
            {
                CustomerEntities db = new CustomerEntities();
                //var d = db.NCSM_CRC_DUTY_Officer.Where(w => w.Name == nc.DutyName).FirstOrDefault();
                NCSM_CRC_DUTY_ContactList ac = new NCSM_CRC_DUTY_ContactList
                {
              
                    ContactFullName=nc.ContactFullName,
                    ContactCell1 = nc.ContactCell1,
                    ContactCell2=nc.ContactCell2,
                    Residency = nc.Residency,
                    ContactCell3 = nc.ContactCell3,
                    ContactCell4 = nc.ContactCell4,
                    ContactCell5 = nc.ContactCell5,
                    // Sector=nc.Sector,
                    //  DutyName=nc.DutyName,
                    //  DutyID=d.ID,
                    Active =true
                };

                db.NCSM_CRC_DUTY_ContactList.Add(ac);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
             
            }

            return Redirect("../Contacts");
        }

        public ActionResult Edit(int id)
        {

            CustomerEntities db = new CustomerEntities();
            var acct = db.NCSM_CRC_DUTY_ContactList.Where(a => a.ContactID == id).FirstOrDefault();

            return View(acct);
        }


        [HttpPost]
        public ActionResult Update(int id, NCSM_CRC_DUTY_ContactList uc)
        {
            CustomerEntities db = new CustomerEntities();
            var acct = db.NCSM_CRC_DUTY_ContactList.Where(a => a.ContactID == id).FirstOrDefault();
          
            /*var d = db.NCSM_CRC_DUTY_Officer.Where(w => w.Name == uc.DutyName).FirstOrDefault();
            try
            {
                if (d.Equals(null))
                { */
                    acct.ContactFullName = uc.ContactFullName;

                    acct.ContactCell1 = uc.ContactCell1;
            acct.ContactCell2 = uc.ContactCell2;
            acct.Residency = uc.Residency;
            acct.ContactCell3 = uc.ContactCell3;
            acct.ContactCell4 = uc.ContactCell4;
            acct.ContactCell5 = uc.ContactCell5;
            //  acct.Sector = uc.Sector;
            // acct.DutyName = uc.DutyName;
            acct.Active = true;

              /*  }
                else
                {
                    acct.ContactFullName = uc.ContactFullName;

                    acct.ContactCell1 = uc.ContactCell1;
                  //  acct.Sector = uc.Sector;
                   // acct.DutyName = uc.DutyName;
                    //acct.DutyID = d.ID;
                    acct.Active = true;
                }
            }
            catch(Exception e)
            {

            }*/
            db.SaveChanges();

            // return RedirectToAction("ViewContacts", d);
            return RedirectToAction("");
        }

        public ActionResult Delete(int id)
        {
            CustomerEntities db = new CustomerEntities();
            var acct = db.NCSM_CRC_DUTY_ContactList.Where(a => a.ContactID == id).FirstOrDefault();
            var scheds = db.NCSM_CRC_DUTY_ContactList.Where(w => w.ContactID == id);
            var d = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == acct.DutyID).FirstOrDefault();
           var sch = db.NCSM_CRC_Schedule.Where(w => w.contactID == acct.ContactID).ToList();
            db.NCSM_CRC_DUTY_ContactList.Remove(acct);
            foreach(var sc in sch)
            {
                db.NCSM_CRC_Schedule.Remove(sc);

            }
            
           // db.C037_800_BOARDUP_OnCallList.RemoveRange(scheds);
            db.SaveChanges();

            // return RedirectToAction("ViewContacts", d);
            return RedirectToAction("");
        }


        public ActionResult ViewContacts(NCSM_CRC_DUTY_Officer d)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);

            return View(d);
        }

        public ActionResult BackToContacts(int id)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);

            CustomerEntities db = new CustomerEntities();
            var d = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == id).FirstOrDefault();

            //  return RedirectToAction("ViewContacts", d);
            return Redirect("../Contacts");
        }

    }
}
