using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;

namespace WebSale.Controllers
{
    public class CongNgheController : Controller
    {
        // GET: CongNghe
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();

        [Route("blogs/{slug}")]
        public ActionResult Index(string slug)
        {
            var category = db.DanhMucBlogs.FirstOrDefault(item => item.slug == slug);

            var namecate = category.ten;
            var categoryId = category.id;

            var listCategory = db.DanhMucBlogs.ToList();
            ViewBag.title = namecate;
            var listblogs = db.Blogs
                                .Include("DanhMucBlog")
                                .Include("MucLucs").OrderByDescending(item => item.ngay_dang).Take(4)
                                .ToList();

            var listblogboot = db.Blogs
                              .Include("DanhMucBlog")
                              .Include("MucLucs")
                              .Where(blog => blog.danh_muc_id == categoryId)
                              .ToList();

            ViewBag.listBlog = listblogs;
            ViewBag.listblogboot = listblogboot;

            ViewBag.blogCounts = db.DanhMucBlogs
            .Include(cate => cate.Blogs) 
            .Select(cate => new CategoryBlogCount
            {
                slug = cate.slug,
                BlogCount = cate.Blogs.Count() 
            })
            .ToList();


            ViewBag.listblog = new BlogModel
            {
                BlogDefault = listblogs.FirstOrDefault(),
                BlogList = listblogs.ToList()
            };
            return View(listCategory);
        }
        [Route("blogs/{slugcate}/{slug}")]
        public ActionResult chitiet(string slugcate, string slug)
        {
            // Find the category based on slugcate
            var category = db.DanhMucBlogs.FirstOrDefault(c => c.slug == slugcate);

            if (category == null)
            {
                return HttpNotFound();
            }

            // Find the blog based on slug
            var blog = db.Blogs.Include("MucLucs")
                               .FirstOrDefault(b => b.slug == slug && b.danh_muc_id == category.id);

            if (blog == null)
            {
                return HttpNotFound(); 
            }

            ViewBag.blogCounts = db.DanhMucBlogs
           .Include(cate => cate.Blogs)
           .Select(cate => new CategoryBlogCount
           {
               slug = cate.slug,
               BlogCount = cate.Blogs.Count()
           })
           .ToList();

            var listCategory = db.DanhMucBlogs.ToList();
            ViewBag.title = blog.tieu_de;
            ViewBag.listCategory = listCategory;
            ViewBag.categoryTitle = category.ten;

            return View(blog);
        }

    }
}