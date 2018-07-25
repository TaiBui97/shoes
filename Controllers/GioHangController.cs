using ShoesStore_agforl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ShoesStore_agforl.Controllers
{
    public class GioHangController : Controller
    {
        dbBANGiayDataContext db = new dbBANGiayDataContext();
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int iMaSanPham, string strURL)
        {
            //lay ra session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            //kiem tra danh sah nay ton tai trong seesion Giohang 
            Giohang sanpham = lstGiohang.Find(n => n.iMaSanPham == iMaSanPham);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMaSanPham);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
  
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Shoes");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGioHang(int iMaSP)
        {
          
            List<Giohang> lstGiohang = Laygiohang();
           
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSanPham == iMaSP);
        
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMaSanPham == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Shoes");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            //lay gio hang tu session
            List<Giohang> lstGiohang = Laygiohang();
            //kiem tra giay da co trong sesssion[Giohang]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMaSanPham == iMaSP);
            //neuton tai thi cho sua so luong
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Shoes");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            //kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Shoes");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);

        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            HoaDon hd = new HoaDon();
            KhachHang kh = (KhachHang)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            hd.MaKhachHang = kh.MaKhachHang;
            hd.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            hd.Ngaygiao = DateTime.Parse(ngaygiao);
            hd.Dathanhtoan = false;
            hd.TinhTrangGiaoHang = false;
            db.HoaDons.InsertOnSubmit(hd);
            db.SubmitChanges();
            foreach (var item in gh)
            {
                ChiTietHoaDon cthd = new ChiTietHoaDon();
                cthd.MaHoaDon = hd.MaHoaDon;
                cthd.MaSanPham = item.iMaSanPham;
                cthd.Soluong = item.iSoLuong;
                cthd.Dongia = (decimal)item.dGia;
                db.ChiTietHoaDons.InsertOnSubmit(cthd);
            }
            db.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");


        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
        public ActionResult IconGioHang()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult PartialDonHang()
        {
            return PartialView();
        }




    }
}