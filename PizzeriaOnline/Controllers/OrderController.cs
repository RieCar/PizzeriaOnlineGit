using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PizzeriaOnline.Models;
using PizzeriaOnline.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PizzeriaOnline.Controllers
{
    public class OrderController : Controller
    {
        private TomasosContext _context;

        public OrderController(TomasosContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult ViewFood()
        {
            ViewModelOrder model = new ViewModelOrder();
            model.ListOfMatratt = _context.Matratt.ToList();
            model.ListOfTyper = _context.MatrattTyp.ToList();
            return View(model);
        }

        public IActionResult ViewFoodToOrder(int id)
        {        
            var model = _context.Matratt.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt).Where(m => m.MatrattTyp == id).ToList();
            return PartialView("_PartialOrderFood", model);
        }

        public IActionResult AddToOrder(int id)
        {
            var currentFood = _context.Matratt.SingleOrDefault(n => n.MatrattId == id);
            List<Matratt> foodToOrder;

            if (HttpContext.Session.GetString("Order") == null)
            {
                foodToOrder = new List<Matratt>();
            }
            else
            {
                var existingValues = HttpContext.Session.GetString("Order");
                foodToOrder = JsonConvert.DeserializeObject<List<Matratt>>(existingValues); // gör om från json till objekt.

            }
            foodToOrder.Add(currentFood);
            var values = JsonConvert.SerializeObject(foodToOrder);  // gör om produktlista till json
            HttpContext.Session.SetString("Order", values); // här skapas sessionsvariabeln och värdena läggs in. 
            return RedirectToAction("ViewFood");
        }

        public IActionResult DoneAndCheckOut()
        {
            OrderCheckOut model = new OrderCheckOut();

            var user = HttpContext.Session.GetString("Users");
            var orderValues = HttpContext.Session.GetString("Order");

            if (user == null)
            {
                ViewBag.Message = "Du måste vara inloggad för att se denna sida";
                return RedirectToAction("LogIn", "Account");
            }
            else if(user != null && orderValues == null)
            {
                return RedirectToAction("ViewFood");
            }
            else
            {
                model.ListOfMatrattToOrder = GetKopplingsTabell();
                model.ShoppingUser = JsonConvert.DeserializeObject<Kund>(user);

                model.TodoBestallning.Totalbelopp = GetSum(model);


                return View(model);
            }
        }

        public List<BestallningMatratt> GetKopplingsTabell()
        {
            OrderCheckOut model = new OrderCheckOut();
            var orderValues = HttpContext.Session.GetString("Order"); // tar fram värden från sessionsvariabeln. 
            var matratter = JsonConvert.DeserializeObject<List<Matratt>>(orderValues); // gör om från json till objekt.
            foreach (var mat in matratter)
            {
                var isAlready = model.ListOfMatrattToOrder.SingleOrDefault(p => p.MatrattId == mat.MatrattId);
                if (isAlready == null)
                {
                    int antal = _context.Matratt.Count(m => m.MatrattId == mat.MatrattId);
                    BestallningMatratt koppling = new BestallningMatratt();
                    koppling.MatrattId = mat.MatrattId;
                    koppling.Antal = antal;
                    model.ListOfMatrattToOrder.Add(koppling);
                }
                else
                {
                    BestallningMatratt koppling = model.ListOfMatrattToOrder.SingleOrDefault(m => m.MatrattId == mat.MatrattId);
                    koppling.Antal = koppling.Antal + 1;
                }
            }
            return model.ListOfMatrattToOrder;
        }

        public int GetSum(OrderCheckOut model)
        {
            List<int> sum = new List<int>();
            var priset = 0;
            foreach (var ratt in model.ListOfMatrattToOrder)
            {
                var dish = _context.Matratt.SingleOrDefault(p => p.MatrattId == ratt.MatrattId);
                model.FoodOrder.Add(dish);
                priset = dish.Pris * ratt.Antal;
                sum.Add(priset);
            }
            var totalBelopp = sum.Sum();

            return model.TodoBestallning.Totalbelopp = totalBelopp; ;
        }
        public IActionResult RemovefromOrder(int id)
        {
            OrderCheckOut model = new OrderCheckOut();

            var orderValues = HttpContext.Session.GetString("Order"); // tar fram värden från sessionsvariabeln. 
            var foodToOrder = JsonConvert.DeserializeObject<List<Matratt>>(orderValues); // gör om från json till objekt.

            var isAlready = foodToOrder.First(p => p.MatrattId == id);

            foodToOrder.Remove(isAlready);
            var values = JsonConvert.SerializeObject(foodToOrder);  // gör om produktlista till json
            HttpContext.Session.SetString("Order", values);

            return RedirectToAction("DoneAndCheckOut");

        }

        public IActionResult PayCheckOut()
        {
            OrderCheckOut model = new OrderCheckOut();
            model.TodoBestallning.BestallningDatum = DateTime.Now;
            var user = HttpContext.Session.GetString("Users");
            var currentUser = JsonConvert.DeserializeObject<Kund>(user);
            model.ShoppingUser = _context.Kund.SingleOrDefault(k => k.AnvandarNamn.Equals(currentUser.AnvandarNamn));
            model.TodoBestallning.KundId = model.ShoppingUser.KundId;
            model.ListOfMatrattToOrder = GetKopplingsTabell();
            model.TodoBestallning.Totalbelopp = GetSum(model);
            _context.Add(model.TodoBestallning);
            _context.SaveChanges();
            var currentOrder = _context.Bestallning.OrderByDescending(d=>d.BestallningDatum).Where(k => k.KundId == model.ShoppingUser.KundId).Take(1);
            foreach(var matratt in model.ListOfMatrattToOrder)
            {
                matratt.BestallningId = model.TodoBestallning.BestallningId;
                _context.Add(matratt);
                _context.SaveChanges(); 
            }
            
            return View(model);
        }
    }

    //private List<SelectListItem> GetDropDown()
    //{
    //    List<SelectListItem> typer = new List<SelectListItem>();
    //    var matTyper = _context.MatrattTyp.ToList();
    //    foreach (var typ in matTyper)
    //    {
    //        typer.Add(new SelectListItem() { Text = typ.Beskrivning, Value = typ.Matratt.ToString() });
    //    }
    //    return typer;
    //}

}


