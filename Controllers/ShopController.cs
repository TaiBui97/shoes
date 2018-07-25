using ShoesStore_agforl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace ShoesStore_agforl.Controllers
{
    public class ShopController : Controller
    {

        // GET: Shoes
        // tao 1 doi tuong chua toan bo CSDL tu dbQLBangiay
        dbBANGiayDataContext data = new dbBANGiayDataContext();
        private List<SanPham> Laygiaymoi(int count)
        {
            //   sap xe giam dan theo ngaycapnhat, laycount dong dau
            return data.SanPhams.OrderByDescending(a => a.NgayLaySP).Take(count).ToList();
        }
        private List<Banner> LayBanner(int count)
        {
            //   sap xe giam dan theo ngaycapnhat, laycount dong dau
            return data.Banners.OrderBy(a => a.MaBanner).Take(count).ToList();
        }
        public ActionResult BannerMoi()
        {
            var banner = LayBanner(3);
            return PartialView(banner);
        }

        public ActionResult Search(int MaSanPham = 0, String search = "")
        {
             if (MaSanPham != 0)
            {
                var model = data.SanPhams
                    .Where(p => p.MaSanPham == MaSanPham);
                return View(model);
            }
            else if (search != "")
            {
                var model = data.SanPhams
                    .Where(p => p.TenSanPham.Contains(search));
                return View(model);
            }
            return View(data.SanPhams);
        }
    
     
        public ActionResult Index(int ? page)
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);


            var giaymoi = Laygiaymoi(18);
            return View(giaymoi.ToPagedList(pageNum,pageSize));
        }
        public ActionResult Dacbiet()
        {
            var db = data.SanPhams.Where(a => a.DacBiet == true).Take(5);
            return PartialView("Dacbiet", db);
        }
        public ActionResult  SpHang()
        {
            var giay = from Hang in data.Hangs select Hang;
            return PartialView(giay);
        }
       
        public ActionResult SpLoai()
        {
            var giay = from Loai in data.Loais select Loai;
            return PartialView(giay);
        }
        public ActionResult SpTheohang(int id, int ? page)
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);

            var giay = from g in data.SanPhams where g.MaHangSanPham == id select g;
            return View(giay.ToPagedList(pageNum,pageSize));
        }
        public ActionResult SpTheoloai(int id, int ? page)
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);

            var giay = from g in data.SanPhams where g.MaLoai == id select g;
            return View(giay.ToPagedList(pageNum,pageSize));
        }
        public ActionResult BanneQC()
        {
            var bannerqc = (from s in data.Banners
                          where s.MaBanner == 4
                          select s).ToList();
            return PartialView(bannerqc);
        }
        public ActionResult BanneQC2()
        {
            var bannerqc2= (from s in data.Banners
                          where s.MaBanner == 5
                          select s).ToList();
            return PartialView(bannerqc2);
        }

        public ActionResult Bestseller(int ? page)
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);

            var giay = data.SanPhams.OrderByDescending(a => a.SoLuongCon<4).Take(9);
            return PartialView("Bestseller", giay.ToPagedList(pageNum, pageSize));
        }
    }
}   