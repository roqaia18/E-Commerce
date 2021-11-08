using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task.Models;

namespace Task.Controllers
{
    public class CartController : Controller
    {
        EcommerceEntities Context = new EcommerceEntities();
        public ActionResult Index()
        {
            List<OrderDetail> cart = (List<OrderDetail>)this.Session["cart"];
            ViewBag.cart = cart;
            if (cart == null)
                ViewBag.total = 0;
            else
            {
               int totalTAx = cart.Sum(items => (items.Tax));
                ViewBag.totalTAx = totalTAx;
                ViewBag.total = cart.Sum(items => (((items.Item.Price - (items.Item.Price * items.Item.Discount) / 100)) * items.Quantity)+ totalTAx);
            }
            return View();
        }

        private int IsExist(int id)
        {
            List<OrderDetail> OrderDetails = (List<OrderDetail>)this.Session["cart"];
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                if (OrderDetails[i].Item.ID.Equals(id))
                {
                    return i;
                }

            }
            return -1;
        }

        public ActionResult Buy(int id)
        {
            Item item = Context.Items.Find(id);
            if (Session["cart"] == null)
            {
                List<OrderDetail> cart = new List<OrderDetail>();
                cart.Add(new OrderDetail()
                {
                    ItemId=item.ID,
                    Tax=10,
                    Quantity = 1,
                    Item = item
                });
                Session["cart"] = cart;
            }
            else
            {
                List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];
                //int index = IsExist(id);
                //if (index != -1)
                //{
                //    cart[index].Quantity++;
                //}
                //else
                //{
                //    cart.Add(new OrderDetail()
                //    {
                //        ItemId = item.ID,
                //        Tax = 10,
                //        Quantity = 1,
                //        Item = item
                //    });
                //}
                foreach (var x in cart.ToList())
                {
                    if (x.ItemId == item.ID)
                    {
                        if (item.QTY > x.Quantity)
                        {
                            x.Quantity += 1;
                        }
                        else
                            break;
                    }
                    else
                    {
                        cart.Add(new OrderDetail()
                        {
                            ItemId = item.ID,
                            Tax = 10,
                            //TotalPrice = item.Price,
                            Quantity = 1,
                            Item = item
                        });
                    }

                }

                Session["cart"] = cart;
            }

           return RedirectToAction("Index");


        }
        public ActionResult Remove(int id)
        {

            List<OrderDetail> cart = (List<OrderDetail>)this.Session["cart"];
            int index = IsExist(id);
            cart.RemoveAt(index);
            this.Session["cart"] = cart;
            return RedirectToAction("Index");
        }
        public ActionResult ChangeQuantity(int ItemId, int quantity)
        {
            List<OrderDetail> cart = (List<OrderDetail>)this.Session["cart"];
            OrderDetail item = cart.Where(i => i.ID == ItemId).First();
            item.Quantity = quantity;
            this.Session["cart"] = cart;

            return RedirectToAction("Index");
        }
    }
}