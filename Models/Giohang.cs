using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoesStore_agforl.Models
{
    public class Giohang
    {
        dbBANGiayDataContext db = new dbBANGiayDataContext();
        public int iMaSanPham { set; get; }
        public string sTenSanPham { set; get; }
        public string sAnh { set; get; }
        public Double dGia { set; get; }
        public int iSoLuong { set; get; }
        public Double dThanhtien
        {
            get { return iSoLuong * dGia; }
        }
        //khoi tao gio hang theo masach duoc truyen vao voi soluong mac dinh la1
        public Giohang(int MaSanPham)
        {
            iMaSanPham = MaSanPham;
            SanPham sanpham = db.SanPhams.Single(n => n.MaSanPham == iMaSanPham);
            sTenSanPham = sanpham.TenSanPham;
            sAnh = sanpham.Anh;
            dGia = double.Parse(sanpham.Gia.ToString());
            iSoLuong = 1;
        }

    }
}