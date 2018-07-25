using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoesStore_agforl.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;



namespace ShoesStore_agforl.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbBANGiayDataContext data = new dbBANGiayDataContext();
        // GET: /Admin/
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult err401()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {

            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {


                QuanTriVien ad = data.QuanTriViens.SingleOrDefault(n => n.TenDangNhap == tendn && n.MatKhau == matkhau);
                if (ad != null)
                {

                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        public List<String> ListRole
        {
            get
            {
                QuanTriVien qtv = (QuanTriVien)Session["Taikhoanadmin"];
                if (qtv != null)
                {
                    var roles = from grouprole in data.Groups
                                join credential in data.Credentials on grouprole.ID equals credential.UserGroupID
                                join role in data.Roles on credential.RoleID equals role.ID
                                where grouprole.ID == qtv.GroupID
                                select role.Name;
                    return roles.ToList();
                }
                else
                {
                    return null;

                }
            }
        }
        public ActionResult CountDH()
        {
            var Model = data.HoaDons.OrderByDescending(model => model.MaHoaDon);
            int count = Model.Count();

            return PartialView(Model.ToList());
        }

        public ActionResult Giay(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }

            if (!ListRole.Contains("MOD"))
            {
                return RedirectToAction("err401");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 5;
                return View(data.SanPhams.ToList().OrderBy(n => n.MaSanPham).ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult ThemmoiGiay()
        {
            //Dua du lieu vao dropdownList
            //Lay ds tu tabke chu de, sắp xep tang dan trheo ten chu de, chon lay gia tri Ma CD, hien thi thi Tenchude
            ViewBag.MaLoai = new SelectList(data.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaHangSanPham = new SelectList(data.Hangs.ToList().OrderBy(n => n.TenHangSanPham), "MaHangSanPham", "TenHangSanPham");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiGiay(SanPham sanpham, HttpPostedFileBase fileupload)
        {
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(data.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaHangSanPham = new SelectList(data.Hangs.ToList().OrderBy(n => n.TenHangSanPham), "MaHangSanPham", "TenHangSanPham");
            //Kiem tra duong dan file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images/shoes/"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }
                    sanpham.Anh = fileName;
                    //Luu vao CSDL
                    data.SanPhams.InsertOnSubmit(sanpham);
                    data.SubmitChanges();
                }
                return RedirectToAction("Giay");
            }
        }
        public ActionResult Chitietgiay(int id)
        {
            //Lay ra doi tuong sach theo ma
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSanPham == id);
            ViewBag.MaSanPham = sp.MaSanPham;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpGet]
        public ActionResult Xoagiay(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSanPham == id);
            ViewBag.MaSanPham = sp.MaSanPham;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }

        [HttpPost, ActionName("Xoagiay")]
        public ActionResult Xacnhanxoa(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSanPham == id);
            ViewBag.MaSanPham = sp.MaSanPham;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SanPhams.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("Giay");
        }

        [HttpGet]
        public string getImage(int id)
        {
            return data.SanPhams.SingleOrDefault(n => n.MaSanPham == id).Anh;
        }
        public ActionResult Suasp(int id)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSanPham == id);
            ViewBag.MaSanPham = sp.MaSanPham;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoai = new SelectList(data.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai", sp.MaLoai);
            ViewBag.MaHangSanPham = new SelectList(data.Hangs.ToList().OrderBy(n => n.TenHangSanPham), "MaHangSanPham", "TenHangSanPham", sp.MaHangSanPham);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Suasp(SanPham sp, HttpPostedFileBase fileUpload)
        {

            SanPham spm = data.SanPhams.SingleOrDefault(n => n.MaSanPham == sp.MaSanPham);
            ViewBag.MaSanPham = sp.MaSanPham;
            var MaLoai = sp.MaLoai;
            var MaHangSanPham = sp.MaHangSanPham;
            var Chitiet = sp.ChiTiet;
            var Gia = sp.Gia;
            var Soluongcon = sp.SoLuongCon;
            var DacBiet = sp.DacBiet;
            var Anh = sp.Anh;

            spm.Anh = Anh;
            spm.MaLoai = MaLoai;
            spm.MaHangSanPham = MaHangSanPham;
            spm.ChiTiet = Chitiet;
            spm.Gia = Gia;
            spm.SoLuongCon = Soluongcon;
            spm.DacBiet = DacBiet;

            if (ModelState.IsValid)
            {
                ViewBag.MaLoai = new SelectList(data.Loais.ToList().OrderBy(n => n.TenLoai), "Maloai", "TenLoai");
                ViewBag.MaHangSanPham = new SelectList(data.Hangs.ToList().OrderBy(n => n.TenHangSanPham), "MaHangSanPham", "TenHangSanPham");
                if (fileUpload == null)
                {
                    spm.Anh = getImage(sp.MaSanPham);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/shoes"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            sp.Anh = fileName;
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                            return View(sp);
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                            spm.Anh = fileName;
                        }
                    }
                }
                data.SubmitChanges();
            }
            return RedirectToAction("Giay");
        }
        public ActionResult Loai(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }

            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(data.Loais.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult themloai()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult themloai(Loai loai, HttpPostedFileBase fileUpload)
        {
            data.Loais.InsertOnSubmit(loai);
            data.SubmitChanges();
            return RedirectToAction("Loai", "Admin");
        }
        [HttpGet]
        public ActionResult Xoaloai(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Loai loai = data.Loais.SingleOrDefault(n => n.MaLoai == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(loai);
        }
        [HttpPost, ActionName("Xoaloai")]
        public ActionResult Xacnhanxoaloai(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Loai loai = data.Loais.SingleOrDefault(n => n.MaLoai == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Loais.DeleteOnSubmit(loai);
            data.SubmitChanges();
            return RedirectToAction("Loai", "Admin");
        }
        public ActionResult Sualoai(int id)
        {
            Loai loai = data.Loais.SingleOrDefault(n => n.MaLoai == id);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Sualoai(Loai loai, HttpPostedFileBase fileUpload)
        {

            Loai loaim = data.Loais.SingleOrDefault(n => n.MaLoai == loai.MaLoai);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            var Tenloai = loai.TenLoai;
            loaim.TenLoai = Tenloai;
            data.SubmitChanges();
            return RedirectToAction("Giay");
        }
        public ActionResult Hang(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }

            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(data.Hangs.ToList().OrderBy(n => n.MaHangSanPham).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult themhang()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult themhang(Hang hang, HttpPostedFileBase fileUpload)
        {
            data.Hangs.InsertOnSubmit(hang);
            data.SubmitChanges();
            return RedirectToAction("Hang", "Admin");
        }
        [HttpGet]
        public ActionResult Xoahang(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Hang hang = data.Hangs.SingleOrDefault(n => n.MaHangSanPham == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(hang);
        }
        [HttpPost, ActionName("Xoahang")]
        public ActionResult Xacnhanxoahang(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Hang hang = data.Hangs.SingleOrDefault(n => n.MaHangSanPham == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Hangs.DeleteOnSubmit(hang);
            data.SubmitChanges();
            return RedirectToAction("Hang", "Admin");
        }
        public ActionResult Suahang(int id)
        {
            Hang hang = data.Hangs.SingleOrDefault(n => n.MaHangSanPham == id);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hang);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Suahang(Hang hang, HttpPostedFileBase fileUpload)
        {

            Hang hangm = data.Hangs.SingleOrDefault(n => n.MaHangSanPham == hang.MaHangSanPham);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            var Tenhang = hang.TenHangSanPham;
            hangm.TenHangSanPham = Tenhang;
            data.SubmitChanges();
            return RedirectToAction("Hang");
        }
        public ActionResult Khachhang(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.KhachHangs.ToList().OrderBy(n => n.MaKhachHang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ChitietKH(int id)
        {
            //Lay ra doi tuong sach theo ma
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKhachHang == id);
            ViewBag.MaKhachHang = kh.MaKhachHang;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpGet]
        public ActionResult ThemKH()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemKH(KhachHang kh, HttpPostedFileBase fileUpload)
        {
            data.KhachHangs.InsertOnSubmit(kh);
            data.SubmitChanges();
            return RedirectToAction("Khachhang", "Admin");
        }
        [HttpGet]
        public ActionResult XoaKH(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKhachHang == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(kh);
        }
        [HttpPost, ActionName("XoaKH")]
        public ActionResult XacnhanxoaKH(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKhachHang == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.KhachHangs.DeleteOnSubmit(kh);
            data.SubmitChanges();
            return RedirectToAction("Khachhang", "Admin");
        }
        public ActionResult SuaKH(int id)
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
        public ActionResult SuaKH(KhachHang kh, HttpPostedFileBase fileUpload)
        {

            KhachHang khm = data.KhachHangs.SingleOrDefault(n => n.MaKhachHang == kh.MaKhachHang);
            ViewBag.Makh = kh.MaKhachHang;
            var tenkh = kh.TenKhachHang;
            var tk = kh.TaiKhoan;
            var mk = kh.MatKhau;
            var dc = kh.DiaChi;
            var sdt = kh.SoDienThoai;
            var email = kh.Email;
            var ns = kh.Ngaysinh;
            var tt = kh.TinhTrang;

            khm.TenKhachHang = tenkh;
            khm.TaiKhoan = tk;
            khm.MatKhau = mk;
            khm.DiaChi = dc;
            khm.SoDienThoai = sdt;
            khm.Email = email;
            khm.Ngaysinh = ns;
            khm.TinhTrang = tt;


            data.SubmitChanges();
            return RedirectToAction("Khachhang");
        }
        public ActionResult CountKH()
        {
            var Model = data.KhachHangs.OrderByDescending(model => model.MaKhachHang);
            int count = Model.Count();

            return PartialView(Model.ToList());
        }
        public ActionResult CountDDH()
        {
            var Model = data.HoaDons.OrderByDescending(model => model.TinhTrangGiaoHang);
            int count = Model.Count();

            return PartialView(Model.ToList());
        }
        public ActionResult Dondathang(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.HoaDons.ToList().OrderBy(n => n.MaHoaDon).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Chitietdonhang(int id)
        {
            //Lay ra doi tuong sach theo ma
            HoaDon sp = data.HoaDons.SingleOrDefault(n => n.MaHoaDon == id);
            ViewBag.MaHoaDon = sp.MaHoaDon;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        public ActionResult SuaDDH(int id)
        {
            HoaDon hang = data.HoaDons.SingleOrDefault(n => n.MaHoaDon == id);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hang);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SuaDDH(HoaDon hang, HttpPostedFileBase fileUpload)
        {

            HoaDon hangm = data.HoaDons.SingleOrDefault(n => n.MaHoaDon == hang.MaHoaDon);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            var thanhtoan = hang.Dathanhtoan;
            var tinhtrang = hang.TinhTrangGiaoHang;
            var ngaygiao = hang.Ngaygiao;




            hangm.Dathanhtoan = thanhtoan;
            hangm.TinhTrangGiaoHang = tinhtrang;
            hangm.Ngaygiao = ngaygiao;

            data.SubmitChanges();
            return RedirectToAction("Dondathang");
        }
        public ActionResult ctdonhang1()
        {
            return View(data.ChiTietHoaDons.ToList().OrderBy(n => n.MaHoaDon));
        }
        public ActionResult Banner(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }

            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(data.QuangCaos.ToList().OrderBy(n => n.MaQC).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Chitietbanner(int id)
        {
            //Lay ra doi tuong sach theo ma
            QuangCao sp = data.QuangCaos.SingleOrDefault(n => n.MaQC == id);
            ViewBag.MaQC = sp.MaQC;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        public string getImageGT(int id)
        {
            return data.QuangCaos.SingleOrDefault(n => n.MaQC == id).Anh;
        }
        public ActionResult SuaBanner(int id)
        {
            QuangCao sp = data.QuangCaos.SingleOrDefault(n => n.MaQC == id);
            ViewBag.MaQC = sp.MaQC;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SuaBanner(QuangCao sp, HttpPostedFileBase fileUpload)
        {
            QuangCao spm = data.QuangCaos.SingleOrDefault(n => n.MaQC == sp.MaQC);
            ViewBag.MaQC = sp.MaQC;
            var HieuGiay = sp.HieuGiay;
            var BaiViet = sp.BaiViet;
            var Url = sp.Url;
            var Anh = sp.Anh;

            spm.Anh = Anh;
            spm.HieuGiay = HieuGiay;
            spm.BaiViet = BaiViet;
            spm.Url = Url;
            spm.Anh = Anh;

            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    spm.Anh = getImageGT(sp.MaQC);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/shoes"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            sp.Anh = fileName;
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                            return View(sp);
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                            spm.Anh = fileName;
                        }
                    }
                }
                data.SubmitChanges();
            }
            return RedirectToAction("Giay");
        }
        public ActionResult Footer()
        {
            return View(data.Footers.ToList().OrderBy(n => n.FooterID));
        }


        [HttpGet]
        public ActionResult Xoafooter(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Footer hang = data.Footers.SingleOrDefault(n => n.FooterID == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(hang);
        }
        [HttpPost, ActionName("Xoafooter")]
        public ActionResult Xacnhanxoafooter(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Footer hang = data.Footers.SingleOrDefault(n => n.FooterID == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Footers.DeleteOnSubmit(hang);
            data.SubmitChanges();
            return RedirectToAction("Footer", "Admin");
        }
        public ActionResult SuaFooter(int id)
        {
            Footer hang = data.Footers.SingleOrDefault(n => n.FooterID == id);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hang);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SuaFooter(Footer hang, HttpPostedFileBase fileUpload)
        {

            Footer hangm = data.Footers.SingleOrDefault(n => n.FooterID == hang.FooterID);
            //ViewBag.MaDoChoi = dc.MaDoChoi;
            var phone = hang.Phone;
            var email = hang.Email;
            var location = hang.Location;
            var slogan = hang.Slogan;


            hangm.Phone = phone;
            hangm.Email = email;
            hangm.Location = location;
            hangm.Slogan = slogan;

            data.SubmitChanges();
            return RedirectToAction("Footer");
        }
        public ActionResult ChitietFooter(int id)
        {
            //Lay ra doi tuong sach theo ma
            Footer sp = data.Footers.SingleOrDefault(n => n.FooterID == id);
            ViewBag.FooterID = sp.FooterID;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        public ActionResult SlidePage(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }


            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(data.Slides.ToList().OrderBy(n => n.ID).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult ChitietSlide(int id)
        {
            //Lay ra doi tuong sach theo ma
            Slide sp = data.Slides.SingleOrDefault(n => n.ID == id);
            ViewBag.ID = sp.ID;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }

        [HttpGet]
        public string getImageSlide(int id)
        {
            return data.Slides.SingleOrDefault(n => n.ID == id).IMG;
        }
        public ActionResult EditSlide(int id)
        {
            Slide sp = data.Slides.SingleOrDefault(n => n.ID == id);
            ViewBag.ID = sp.ID;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditSlide(Slide sp, HttpPostedFileBase fileUpload)
        {

            Slide spm = data.Slides.SingleOrDefault(n => n.ID == sp.ID);
            var brand = sp.Brand;
            var slogan = sp.Slogan;
            var Anh = sp.IMG;

            spm.IMG = Anh;
            spm.Slogan = slogan;
            spm.Brand = brand;



            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    spm.IMG = getImageSlide(sp.ID);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            sp.IMG = fileName;
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                            return View(sp);
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                            spm.IMG = fileName;
                        }
                    }
                }
                data.SubmitChanges();
            }
            return RedirectToAction("SlidePage");
        }
        public ActionResult Contacts(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }


            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.Contacts.ToList().OrderBy(n => n.ID).ToPagedList(pageNumber, pageSize));

        }
        public ActionResult ChitietContact(int id)
        {
            //Lay ra doi tuong sach theo ma
            Contact sp = data.Contacts.SingleOrDefault(n => n.ID == id);
            ViewBag.ID = sp.ID;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpGet]
        public ActionResult XoaContact(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Contact hang = data.Contacts.SingleOrDefault(n => n.ID == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(hang);
        }
        [HttpPost, ActionName("XoaContact")]
        public ActionResult Xacnhanxoacontact(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Contact hang = data.Contacts.SingleOrDefault(n => n.ID == id);
            //ViewBag.MaLoai = dc.MaLoai;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Contacts.DeleteOnSubmit(hang);
            data.SubmitChanges();
            return RedirectToAction("Contacts", "Admin");
        }
        public ActionResult CountContact()
        {
            var Model = data.Contacts.OrderByDescending(model => model.ID);
            int count = Model.Count();

            return PartialView(Model.ToList());
        }
        public ActionResult BannerQC()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login");
            }



            return View(data.Banners.ToList().OrderBy(n => n.MaBanner));

        }
        public ActionResult ChitietBannerQC(int id)
        {
            //Lay ra doi tuong sach theo ma
            Banner sp = data.Banners.SingleOrDefault(n => n.MaBanner == id);
            ViewBag.MaBanner = sp.MaBanner;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpGet]
        public string getImageQC(int id)
        {
            return data.Banners.SingleOrDefault(n => n.MaBanner == id).Image;
        }
        public ActionResult SuaBannerQC(int id)
        {
            Banner sp = data.Banners.SingleOrDefault(n => n.MaBanner == id);
            ViewBag.MaBanner = sp.MaBanner;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SuaBannerQC(Banner sp, HttpPostedFileBase fileUpload)
        {

            Banner spm = data.Banners.SingleOrDefault(n => n.MaBanner == sp.MaBanner);
            ViewBag.MaSanPham = sp.MaBanner;
            var tittle = sp.title;
            var url = sp.Url;
            var Anh = sp.Image;

            spm.Image = Anh;
            spm.title = tittle;
            spm.Url = url;


            if (ModelState.IsValid)
            {
                if (fileUpload == null)
                {
                    spm.Image = getImageQC(sp.MaBanner);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/images/"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            sp.Image = fileName;
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                            return View(sp);
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                            spm.Image = fileName;
                        }
                    }
                }
                data.SubmitChanges();
            }
            return RedirectToAction("SuaBannerQC");
        }


    }
}