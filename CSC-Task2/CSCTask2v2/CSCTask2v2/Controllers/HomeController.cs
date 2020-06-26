using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CSCTask2v2.Models;

namespace CSCTask2v2.Controllers
{
    public class HomeController : Controller
    {
        private Context _context;
        public HomeController()
        {
            _context = new Context();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List()
        {
            return Json(_context.Product.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(ProductModel product)
        {
            _context.Product.Add(product);
            _context.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetbyID(int ID)
        {
            return Json(_context.Product.FirstOrDefault(x => x.Id == ID), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(ProductModel product)
        {
            var data = _context.Product.FirstOrDefault(x => x.Id == product.Id);
            if (data != null)
            {
                data.Name = product.Name;
                data.Category = product.Category;
                data.Price = product.Price;
                _context.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(int ID)
        {
            var data = _context.Product.FirstOrDefault(x => x.Id == ID);
            _context.Product.Remove(data);
            _context.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}