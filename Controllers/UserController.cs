using ShoesStore_agforl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoesStore_agforl.Controllers
{
    public class UserController : Controller
    {
        dbBANGiayDataContext data = new dbBANGiayDataContext();
        // GET: NguoiDung
       
        [HttpPost]
        public ActionResult GetMail(FormCollection collection, MailKH p)
        {
            var mail = collection["Mail"];
            if(String.IsNullOrEmpty(mail))
            {
                ViewData["Loi"] = "Mail không được bỏ trống";

            }else
            {
                p.mail = mail;
                data.MailKHs.InsertOnSubmit(p);
                data.SubmitChanges();
               
            }
            return RedirectToAction("Index", "Shoes");
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KhachHang kh)
        {   
            var hoten = collection["Hoten"];
            var tendn = collection["TenDN"];
            var matkhau = collection["pwd"];
            var matkhaunhaplai = collection["pwd2"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var dienthoai = collection["Phone"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            
            if (tendn != null)
            {
               ViewData["Loi8"] = "Tên đăng nhập tồn tại";
            }
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập ";
            }
            if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi2"] = "Phải nhập email ";
            }
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập Mật khẩu ";
            }
            if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Lo4"] = "Phải nhập lại mật khẩu ";
            }
            if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = "Phải nhập địa chỉ";
            }
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi7"] = "Phải nhập họ và tên ";
            }
            if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập điện thoại ";
            }
            else
            {
                kh.TenKhachHang = hoten;
                kh.TaiKhoan = tendn;
                kh.MatKhau = matkhau;
                kh.Email = email;
                kh.DiaChi = diachi;
                kh.SoDienThoai = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("DangNhap");

            }
            return this.DangKy();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
           
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["pwd"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == matkhau);
                if (kh != null)
                {
                    //  ViewBag.Thongbao = "Chúc mừng bạn đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    
                    return RedirectToAction("GioHang", "GioHang");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    
            }
            return View();
        }
        public ActionResult Thongtinkhachhang(int id)
        {
            var ct = from kh in data.KhachHangs where kh.MaKhachHang == id select kh;
            return View(ct.Single());
        }
        public ActionResult Suathongtin(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKhachHang == id);

            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Suathongtin(KhachHang kh)
        {

            KhachHang khm = data.KhachHangs.SingleOrDefault(n => n.MaKhachHang == kh.MaKhachHang);
            ViewBag.Makh = kh.MaKhachHang;
            var tenkh = kh.TenKhachHang;          
            var mk = kh.MatKhau;
            var dc = kh.DiaChi;
            var sdt = kh.SoDienThoai;
            var email = kh.Email;
            var ns = kh.Ngaysinh;
     

            khm.TenKhachHang = tenkh;      
            khm.MatKhau = mk;
            khm.DiaChi = dc;
            khm.SoDienThoai = sdt;
            khm.Email = email;
            khm.Ngaysinh = ns;
           


            data.SubmitChanges();
            return RedirectToAction("Index","Shoes");
        }
        public ActionResult DangXuat()
        {
            
                Session["TaiKhoan"] = null;
            
              
            return Redirect("/");
        }

    }
}
