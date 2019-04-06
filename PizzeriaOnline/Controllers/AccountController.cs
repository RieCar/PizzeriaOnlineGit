using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzeriaOnline.Models;
using Microsoft.EntityFrameworkCore;
using System.Web; 


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PizzeriaOnline.Controllers
{
    public class AccountController : Controller
    {
        private TomasosContext _context;

        public AccountController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult LogOut()
        {        
            HttpContext.Session.Clear();
            TempData["Message"] = "Du är nu utloggad";
            return RedirectToAction("LogIn");
          
        }
        // GET: /<controller>/
        [Route("Medlem")]
        public IActionResult LogIn()
        {
            return View();
        }

        [Route("Medlem")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LogIn(Kund user)
        {
            Kund savedUsers;
            var errorCode = CheckUser(user);
            switch (errorCode)
            {
                case 1:
                    {
                        ViewBag.Message = "Användarnamn eller epostadess är fel";
                        return View();
                    }
                case 2:
                    {
                        ViewBag.Message = "Du är redan inloggad!";
                        return View();
                    }
                case 3:
                    {
                        ViewBag.Message = "Något gick fel. Har du registrerat ett konto?";
                        return View();
                    }
                case 4:
                    {
                        savedUsers = new Kund();
                        savedUsers = _context.Kund.SingleOrDefault(n => n.AnvandarNamn.Equals(user.AnvandarNamn));
                        var values = JsonConvert.SerializeObject(savedUsers);  // gör om produktlista till json
                        HttpContext.Session.SetString("Users", values); // här skapas sessionsvariabeln och värdena läggs in.                     
                        return RedirectToAction("ViewFood", "Order");
                    }
                default: return View();
            }
        }

        public int CheckUser(Kund user)
        {
            var currentCustomer = _context.Kund.SingleOrDefault(n => n.AnvandarNamn.Equals(user.AnvandarNamn));
            if(currentCustomer != null) { 
            bool isUser = currentCustomer.Losenord.Equals(user.Losenord);
            var usersValues = HttpContext.Session.GetString("Users");

            if (isUser && usersValues == null)
            { // loggar in användaren
                return 4;
            }
            else if (!isUser && usersValues == null)
            { //felaktigt lösenord
                return 1;
            }
            else if (isUser && usersValues != null)
            { // redan inloggad försöker logga in igen. 
                return 2;
            }
            else
            {
                return 3;
            }

            }
            else
            {
                return 3; 
            }
        }
        [Route("Bli-medlem")]
        public IActionResult RegisterAccount()
        {     
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult VerifyEmail(string email)
        {
           var isEmail = _context.Kund.SingleOrDefault(k => k.Email.Equals(email));
            var isAvailble = HttpContext.Session.GetString("Users");
            if(isAvailble != null)
            {
                var currentUser = JsonConvert.DeserializeObject<Kund>(isAvailble);
                var correctUser = currentUser.AnvandarNamn.Equals(isEmail.AnvandarNamn);
                if (isEmail != null && correctUser == false)
                {
                    return Json($"Email {email} is already in use.");
                }
                else
                {
                    return Json(true);
                }
            }
            else if(isAvailble == null)
            {
                if (isEmail != null)
                {
                    return Json($"Email {email} is already in use.");
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                return Json(true);
            }




        }

        [Route("Bli-medlem")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult RegisterAccount(Kund newCustomer)
        {
            var isEmail = _context.Kund.SingleOrDefault(k => k.Email.Equals(newCustomer.Email));
            var isUserName = _context.Kund.SingleOrDefault(k => k.AnvandarNamn.Equals(newCustomer.AnvandarNamn));
            bool isNotExisting = isEmail == null && isUserName == null;
            if (isNotExisting)
            {
                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Användaren är sparad. Du kan nu logga in";
                    //ViewBag.Message = "Användaren är sparad. Du kan nu logga in!";
                    _context.Kund.Add(newCustomer);
                    _context.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("LogIn");
                }
                else
                {
                    ViewBag.Message = "Något gick fel vid sparandet. Försök igen";
                    return View();
                }
            }
            else
            {
                TempData["Message"] = "Användarnamnet och/eller e-postadressen är redan registrerad. Testa att logga in";

                return RedirectToAction("LogIn");
            }

        }

        public IActionResult AccountSide()
        {
            Kund currentUser = new Kund();
            var isAvailble = HttpContext.Session.GetString("Users");
            if (isAvailble != null)
            {
                currentUser = JsonConvert.DeserializeObject<Kund>(isAvailble);
                return View(currentUser);
            }
            else
            {
                ViewBag.Message = "Du måste vara inloggad för att kunna använda denna sida";
                    return View(currentUser);
            }
        }

        [AutoValidateAntiforgeryToken]
        public IActionResult SaveChanges(Kund values)
        {
            if (ModelState.IsValid)
            {
                // hämta kund fr DB
                var kundDb = _context.Kund.SingleOrDefault(k => k.KundId == values.KundId);
                // sätt samtliga prop till modified och uppdatera till db
                _context.Entry(kundDb).CurrentValues.SetValues(values);            
                _context.SaveChanges();

                // uppdatera sessionvariabeln
                var updatedValues = JsonConvert.SerializeObject(values);
                HttpContext.Session.SetString("Users", updatedValues);

                return RedirectToAction("AccountSide");
            }
            else
            {
                ViewBag.Message = "Något gick fel";
                return RedirectToAction("AccountSide");
            }
           
        }
    }
}
