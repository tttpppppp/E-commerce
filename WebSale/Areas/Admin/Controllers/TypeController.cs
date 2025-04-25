using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
namespace WebSale.Areas.Admin.Controllers
{
    public class TypeController : Controller
    {
        // GET: Admin/Type
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            mapType mapType = new mapType();
            var listtype = mapType.getType();
            return View(listtype);
        }
        public ActionResult addType() {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addType(Models.Type type)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(type.typeName))
            {
                ModelState.AddModelError("typeName", "Trường tên không được để trống!");
                return View();
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            type.created_at = DateTime.Now;
            type.updated_at = DateTime.Now;
            db.Types.Add(type);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult deleteType(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var type = db.Types.Find(id);
            var product = db.Products.Any(p => p.typeId == id);
            if (product)
            {
                type.status = 0;
            }
            else
            {
                db.Types.Remove(type);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult updateType(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var ty = db.Types.Find(id);
            return View(ty);
        }
        [HttpPost]
        public ActionResult updateType(Models.Type type)
        {
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var ty = db.Types.Find(type.typeId);
            ty.typeName = type.typeName;
            ty.status = type.status;
            ty.updated_at = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}