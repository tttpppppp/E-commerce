using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class ThongSoKyThuatController : Controller
    {
        // GET: Admin/ThongSoKyThuat
        public ActionResult Index(int id)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var thongSo = db.ThongSoKyThuats.FirstOrDefault(item => item.specificationId == id);
            return View(thongSo);
        }
        public ActionResult addThongSo() {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addThongSo(ThongSoKyThuat thongSoKyThuat)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            db.ThongSoKyThuats.Add(thongSoKyThuat);
            db.SaveChanges();
            ViewBag.success = "Thêm thông số thành công";
            return View();
        }
        public ActionResult listThongSo() {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var listThongSo = db.ThongSoKyThuats.ToList();
            return View(listThongSo);
        }
        public ActionResult deleteThongSo(int? id) {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var sp = db.Products.FirstOrDefault(item => item.specificationId == id);
            var thongso = db.ThongSoKyThuats.Find(id);
            if(sp != null)
            {
                thongso.status = 0;
            }
            else
            {
                db.ThongSoKyThuats.Remove(thongso);
            }
            db.SaveChanges();
            return RedirectToAction("listThongSo");
        } 
        public ActionResult updateThongSo(int? id)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var thongso = db.ThongSoKyThuats.Find(id);
            return View(thongso);
        }
        [HttpPost]
        public ActionResult updateThongSo(ThongSoKyThuat thongSoKyThuat)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var existingThongSo = db.ThongSoKyThuats.Find(thongSoKyThuat.specificationId);
            if (existingThongSo != null) {
                existingThongSo.screenTechnology = thongSoKyThuat.screenTechnology;
                existingThongSo.screenResolution = thongSoKyThuat.screenResolution;
                existingThongSo.screenSize = thongSoKyThuat.screenSize;
                existingThongSo.screenFeatures = thongSoKyThuat.screenFeatures;
                existingThongSo.refreshRate = thongSoKyThuat.refreshRate;
                existingThongSo.rearCamera = thongSoKyThuat.rearCamera;
                existingThongSo.rearCameraResolution = thongSoKyThuat.rearCameraResolution;
                existingThongSo.videoRecording = thongSoKyThuat.videoRecording;
                existingThongSo.flash = thongSoKyThuat.flash;
                existingThongSo.rearCameraFeatures = thongSoKyThuat.rearCameraFeatures;
                existingThongSo.frontCamera = thongSoKyThuat.frontCamera;
                existingThongSo.frontCameraResolution = thongSoKyThuat.frontCameraResolution;
                existingThongSo.operatingSystem = thongSoKyThuat.operatingSystem;
                existingThongSo.cpu = thongSoKyThuat.cpu;
                existingThongSo.ram = thongSoKyThuat.ram;
                existingThongSo.internalStorage = thongSoKyThuat.internalStorage;
                existingThongSo.mobileNetwork = thongSoKyThuat.mobileNetwork;
                existingThongSo.sim = thongSoKyThuat.sim;
                existingThongSo.wifi = thongSoKyThuat.wifi;
                existingThongSo.gps = thongSoKyThuat.gps;
                existingThongSo.bluetooth = thongSoKyThuat.bluetooth;
                existingThongSo.chargingPort = thongSoKyThuat.chargingPort;
                existingThongSo.headphoneJack = thongSoKyThuat.headphoneJack;
                existingThongSo.otherConnections = thongSoKyThuat.otherConnections;
                existingThongSo.batteryCapacity = thongSoKyThuat.batteryCapacity;
                existingThongSo.batteryTechnology = thongSoKyThuat.batteryTechnology;
                existingThongSo.advancedSecurity = thongSoKyThuat.advancedSecurity;
                existingThongSo.specialFeatures = thongSoKyThuat.specialFeatures;
                existingThongSo.waterResistance = thongSoKyThuat.waterResistance;
                existingThongSo.recording = thongSoKyThuat.recording;
                existingThongSo.radio = thongSoKyThuat.radio;
                existingThongSo.movieFormats = thongSoKyThuat.movieFormats;
                existingThongSo.musicFormats = thongSoKyThuat.musicFormats;
                existingThongSo.status = thongSoKyThuat.status;
                db.SaveChanges();
            }
            return RedirectToAction("listThongSo");
        }
    }
}