using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class LSPController : Controller
    {
        //
        // GET: /Departments/
        private CustomerEntities db = new CustomerEntities();

        public ActionResult Index(int id)
        {

            CustomerEntities db = new CustomerEntities();
            var SectorNumber = db.NCSM_CRC_SectorsInSchdule_View.ToList();
            ViewBag.SectorNumber = SectorNumber;
             return View(SectorNumber);
        }

    /*    public ActionResult Add()
        {

            return View();
        }


        [HttpPost]
        public ActionResult AddNew(C037_800_BOARDUP_LSP_Offices nd)
        {
            try
            {

                CustomerEntities db = new CustomerEntities();

                C037_800_BOARDUP_LSP_Offices ad = new C037_800_BOARDUP_LSP_Offices();
                ad.LSP_Name = nd.LSP_Name;
                ad.LSP_Office_Number = nd.LSP_Office_Number;

                db.C037_800_BOARDUP_LSP_Offices.Add(ad);
                db.SaveChanges();

            }
            catch (Exception ex)
            {

            }


            return Redirect("../LSP");

        }




        public ActionResult Edit(int id)
        {

            CustomerEntities db = new CustomerEntities();
            var dept = db.C037_800_BOARDUP_LSP_Offices.Where(a => a.LSP_ID == id).FirstOrDefault();


            return View(dept);
        }



        [HttpPost]
        public ActionResult Update(int id, C037_800_BOARDUP_LSP_Offices ud)
        {

            CustomerEntities db = new CustomerEntities();
            var dept = db.C037_800_BOARDUP_LSP_Offices.Where(a => a.LSP_ID == id).FirstOrDefault();

            dept.LSP_Name = ud.LSP_Name;
            dept.LSP_Office_Number = ud.LSP_Office_Number;

            db.SaveChanges();

            return Redirect("../../LSP");
        }

        public ActionResult Delete(int id)
        {

            CustomerEntities db = new CustomerEntities();
            var dept = db.C037_800_BOARDUP_LSP_Offices.Where(w => w.LSP_ID == id).FirstOrDefault();

           /* var conts = db.C037_800_BOARDUP_ContactList.Where(w => w.LSP_ID == id);
            var scheds = db.C037_800_BOARDUP_OnCallList.Where(w => w.LSP_ID == id);

            db.C037_800_BOARDUP_LSP_Offices.Remove(dept);
            db.C037_800_BOARDUP_OnCallList.RemoveRange(scheds);
            db.C037_800_BOARDUP_ContactList.RemoveRange(conts);
            db.SaveChanges();


            return Redirect("../../LSP");
        } */
        


    }
}
