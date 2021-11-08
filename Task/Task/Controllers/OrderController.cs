using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task.Models;

namespace Task.Controllers
{
    public class OrderController : Controller
    {
        EcommerceEntities Context = new EcommerceEntities();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "User")]
        public ActionResult Create(int total)
        {
            ViewBag.cost = total;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Order order)
        {
            var user = this.User.Identity.Name;
            User User = Context.Users.Where(x => (x.Email == user)).FirstOrDefault();
            if (ModelState.IsValid)
            {
                order.CustomerID = User.ID;
                order.Due_Date = DateTime.Now.Date.AddDays(15);
                order.Status = "Open";
                Context.Orders.Add(order);
                Context.SaveChanges();
            }
            Session["Order"] = order;
            List < OrderDetail > OrderDetails= (List<OrderDetail>)this.Session["cart"];
            foreach (var OrderITem in OrderDetails)
            {
                OrderITem.Order = order;
                OrderITem.OrderID = order.ID;
                Item item = Context.Items.Find(OrderITem.Item.ID);
                item.QTY -= OrderITem.Quantity;
                Context.OrderDetails.Add(OrderITem);
                Context.SaveChanges();
            }
            this.Session["cart"] = null;
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public ActionResult Show()
        {
            Order order = (Order)this.Session["Order"];
            ViewBag.Total = order.Total_Price;
            List<OrderDetail> Detail = Context.OrderDetails.Where(x => (x.OrderID == order.ID)).ToList();
            foreach (var item in Detail)
            {
                item.Item = Context.Items.Where(id => (id.ID == item.ItemId)).FirstOrDefault();
            }
            ViewBag.items = Detail;
            return View();
        }
    }
}