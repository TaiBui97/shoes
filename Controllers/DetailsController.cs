using ShoesStore_agforl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoesStore_agforl.Controllers
{
    public class DetailsController : Controller
    {
        // GET: Details
        dbBANGiayDataContext data = new dbBANGiayDataContext();
        public ActionResult Index(int id)
        {
            var ct = from sp in data.SanPhams where sp.MaSanPham == id select sp;
            return View(ct.Single());
        }
        public ActionResult Mota(int id)
        {
            var mt = from sp in data.SanPhams where sp.MaSanPham == id select sp;
            return PartialView(mt);
        }
        public ActionResult Ramdom(int id)
        {
            var giay = from g in data.SanPhams where g.MaLoai == id select g;
            return PartialView(giay);


        }
      
        //public ActionResult Ramdom()
        //{
        //     var model = data.SanPhams.Where(p=>p.Ramdom>0).OrderBy(p=>Guid.NewGuid()).Take(4);
         //   return PartialView(model);
        //}
    }
}