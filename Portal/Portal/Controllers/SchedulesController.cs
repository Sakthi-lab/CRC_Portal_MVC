using Newtonsoft.Json.Linq;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class SchedulesController : Controller
    {
        //
        // GET: /Schedules/

        public ActionResult Index()
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);
            return View();
        }


        public ActionResult Add(NCSM_CRC_DUTY_Officer d)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);
            return View(d);
        }


        [HttpPost]
        public ActionResult AddNew(NCSM_CRC_Schedule ns)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);

            CustomerEntities db = new CustomerEntities();
            ViewBag.DupDateTimeSector = false;
            try
            {
                string TZ = ConfigurationManager.AppSettings["TZ"];
                NCSM_CRC_Schedule ads = new NCSM_CRC_Schedule();
                var sectorId = db.NCSM_CRC_DUTY_ContactList.Where(w => w.ContactID == ns.contactID).FirstOrDefault(); 
             
                ads.deptID = ns.ID;
                ads.contactID = ns.contactID;
                ads.start_ET = ns.start_ET;
                ads.end_ET = ns.end_ET;
                ads.start_UTC = TimeZoneInfo.ConvertTimeToUtc(ns.start_ET.Value, TimeZoneInfo.FindSystemTimeZoneById(TZ));
                ads.end_UTC = TimeZoneInfo.ConvertTimeToUtc(ns.end_ET.Value, TimeZoneInfo.FindSystemTimeZoneById(TZ));
                ads.rotated = false;
                ads.sector = ns.sector;

               // var sch = db.NCSM_CRC_Schedule.Where(w =>(( w.start_ET>=ns.start_ET && w.start_ET <= ns.end_ET) || (w.start_ET < ns.start_ET && w.end_ET>=ns.end_ET)) && w.sector==ns.sector).ToList();
           
          //      if (sch==null || sch.Count()==0)
             //   {
                    db.NCSM_CRC_Schedule.Add(ads);
                    db.SaveChanges();
                   
              //  }

              /*  else
                {
                    ViewBag.DupDateTimeSector = true;
                   // return View("Add");
                }*/
            }
            catch (Exception ex)
            {

                CommonTasks.EmailTasks.reportError("CRC Add Schdule Error", ex.ToString());

            }

            var dept = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == ns.ID).FirstOrDefault();
            ViewSchedule(dept);
            return View("ViewSchedule", dept);

        }

        public ActionResult Edit(NCSM_CRC_DUTY_Officer d)
        {
             if (Request.Cookies["loggedin"] != null)
                 Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);
                 
            CustomerEntities db = new CustomerEntities();
            var sched = db.NCSM_CRC_Schedule.Where(w => w.ID == d.ID).FirstOrDefault();

            return View(sched);
        }

        [HttpPost]
        public ActionResult Update(NCSM_CRC_Schedule us)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);
            CustomerEntities db = new CustomerEntities();
            string TZ = ConfigurationManager.AppSettings["TZ"];

            //  var ads = db.NCSM_CRC_Schedule.Where(w => w.ID == us.ID).FirstOrDefault();
            var ads = db.NCSM_CRC_Schedule.Where(a => a.ID == us.ID).FirstOrDefault();

            ads.contactID = us.contactID;
            ads.start_ET = us.start_ET;
            ads.end_ET = us.end_ET;
            ads.start_UTC = TimeZoneInfo.ConvertTimeToUtc(us.start_ET.Value, TimeZoneInfo.FindSystemTimeZoneById(TZ));
            ads.end_UTC = TimeZoneInfo.ConvertTimeToUtc(us.end_ET.Value, TimeZoneInfo.FindSystemTimeZoneById(TZ));
            ads.rotated = false;
            ads.deptID = us.deptID;

            ads.sector = us.sector;

            db.SaveChanges();
            var dept = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == us.deptID).FirstOrDefault();
            ViewSchedule(dept);
            return View("ViewSchedule", dept);
        }


        public ActionResult ViewSchedule(NCSM_CRC_DUTY_Officer d)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);
            CustomerEntities db = new CustomerEntities();
            ViewBag.DupDateTimeSector = false;
            var dept = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == d.ID).FirstOrDefault();
           var SectorNumber = db.NCSM_CRC_SectorsInSchdule_View.Select(s => s.sectornumber).ToList();
            ViewBag.missSecDate = false;
           ViewBag.SectorNumber = SectorNumber;
            ViewBag.checksector = true;
            
            ViewBag.datecheck = false;
            if (SectorNumber == null || SectorNumber.Count == 0)
            {
                ViewBag.checksector = false;
            }
            

            if (dept.ID != 1)
            {

                ViewBag.checksector = false;
            }
           
            ViewBag.allsector = string.Join(",", SectorNumber.ToArray());
           

            if (dept.Name !="Regular")
            {
                //var checkSch = db.NCSM_CRC_Schedule.Where(w => w.ID == d.ID);

               var targetDate = DateTime.Today;
                var endDate = targetDate.AddDays(6);
                var missingDates = new List<string>();
                var checkdates_new = new List<string>();
                //var checkdates = db.NCSM_CRC_Schedule.Where(w => w.deptID == d.ID).Select(s => s.start_ET).ToList();
                var checkdDates = db.NCSM_CRC_Schedule.Where(w => w.deptID == d.ID && ((w.start_ET >= targetDate && w.start_ET < endDate) || (w.start_ET < targetDate && w.end_ET >= targetDate))).ToList();
                var checkEndDates = db.NCSM_CRC_Schedule.Where(w => w.deptID == d.ID).Select(s => s.end_ET).ToList();
                                             
                while (targetDate <= endDate)
                {
                    bool missing = true;
                    foreach (var checkdates1 in checkdDates)
                    {
                        var checkdates_start = checkdates1.start_ET;
                        var checkdates_end = checkdates1.end_ET;
                        if (targetDate >= checkdates_start && targetDate <= checkdates_end)
                        {
                            missing = false;
                            break;
                        }

                    }
                    if (missing)
                    {
                        //if (!checkdates_new.Contains(targetDate.ToString("MM/dd/yyyy")))
                        missingDates.Add(targetDate.Year.ToString() + "/" + targetDate.Month.ToString() + "/" + targetDate.Day.ToString());
                    }
                    targetDate = targetDate.AddDays(1);
                }
                if (missingDates.Count > 0)
                {
                    foreach (var date in missingDates)

                        ViewBag.MissingDates = string.Join("\n,\n", missingDates.ToArray());
                    ViewBag.datecheck = true;
                }
                else
                {
                    ViewBag.MissingDates = "No Dates are missed for the scheduling";
                    ViewBag.datecheck = false;

                }
            }              
            
            return View(d);
        }



        public ActionResult BackToSchedule(int id)
        {

            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);

            CustomerEntities db = new CustomerEntities();
            var d = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == id).FirstOrDefault();

            return RedirectToAction("ViewSchedule", d);
        }



        public ActionResult Delete(int id)
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);

            CustomerEntities db = new CustomerEntities();
            var sched = db.NCSM_CRC_Schedule.Where(w => w.ID == id).FirstOrDefault();
            var dept = db.NCSM_CRC_DUTY_Officer.Where(w => w.ID == sched.deptID).FirstOrDefault(); ;

            db.NCSM_CRC_Schedule.Remove(sched);
            db.SaveChanges();
            ViewSchedule(dept);

            return View("ViewSchedule", dept);
        }

        public ActionResult SortSchedules()
        {
            if (Request.Cookies["loggedin"] != null)
                Response.Cookies["loggedin"].Expires = DateTime.Now.AddMinutes(15);

            var rslt = "";

            try
            {
                CustomerEntities db = new CustomerEntities();

                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string jsonContent = new StreamReader(req).ReadToEnd();
                JObject msg;
                msg = JObject.Parse(@jsonContent);

                if (msg.SelectToken("schedule") != null)
                {
                    var scheds = msg.SelectToken("schedule");
                    if (scheds.Count() > 0)
                    {
                        // add or update contacts
                        foreach (var sched in scheds)
                        {

                            // update category
                            var schedID = Int32.Parse(sched.SelectToken("schedRec").ToString());
                          //  var schedRec = db.C037_800_BOARDUP_OnCallList.Where(w => w.ID == schedID).FirstOrDefault();

                            //schedRec.Contact_Order = Int32.Parse(sched.SelectToken("order").ToString());

                            db.SaveChanges();
                        }
                    }


                }

                rslt = "{\"Result\":\"Success\"}";
            }
            catch (Exception)
            {
                rslt = "{\"Result\":\"Failed\"}";
            }
           
            return Content(rslt);

        }
    }
}
