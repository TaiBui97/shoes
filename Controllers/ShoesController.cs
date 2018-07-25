using ShoesStore_agforl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoesStore_agforl.Controllers
{
    public class ShoesController : Controller
    {
        // GET: Shoes
        dbBANGiayDataContext data = new dbBANGiayDataContext();
        
        public ActionResult Laygiayton()        
        {
            var giay = data.SanPhams.OrderByDescending(a => a.SoLuongCon).Take(4);
            return PartialView("Laygiayton", giay);
        }
        public ActionResult Laygiaydac()
        {
            var giay = data.SanPhams.OrderByDescending(a => a.Gia).Take(4);
            return PartialView("Laygiaydac", giay);
        }

        public ActionResult Laygiaycu()
        {
            var giay = data.SanPhams.OrderBy(a => a.NgayLaySP).Take(4);
            return PartialView("Laygiaycu", giay);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        [HttpGet]
        public ActionResult banner1()
        {
            var banner1 = (from s in data.QuangCaos
                           where s.MaQC == 1
                           select s).ToList();
            return PartialView(banner1);
        }
        public ActionResult banner()
        {         
            return View();
        }
        public ActionResult banner2()
        {
            var banner2 = (from s in data.QuangCaos
                           where s.MaQC == 3
                           select s).ToList();
            return PartialView(banner2);
        }
        public ActionResult banner3()
        {
            var banner3 = (from s in data.QuangCaos
                           where s.MaQC == 4
                           select s).ToList();
            return PartialView(banner3);
        }
        public ActionResult banner4()
        {
            var banner4 = (from s in data.QuangCaos
                           where s.MaQC == 2
                           select s).ToList();
            return PartialView(banner4);
        }
        public ActionResult contact()
        {
            return View();
        }
        public ActionResult Footer()
        {
            var footer = (from s in data.Footers                      
                           select s).ToList();
            return PartialView(footer);
        }
        public ActionResult slide()
        {
            var slide = (from s in data.Slides
                           where s.ID == 1
                           select s).ToList();
            return PartialView(slide);
        }
        public ActionResult slide2()
        {
            var slide2 = (from s in data.Slides
                          where s.ID == 2
                          select s).ToList();
            return PartialView(slide2);
        }
        public ActionResult slide3()
        {
            var slide3 = (from s in data.Slides
                          where s.ID == 3
                          select s).ToList();
            return PartialView(slide3);
        }
        public ActionResult slide4()
        {
            var slide4 = (from s in data.Slides
                          where s.ID == 4
                          select s).ToList();
            return PartialView(slide4);
        }
        public ActionResult slideview()
        {
            
            return View();
        }
    }
}   