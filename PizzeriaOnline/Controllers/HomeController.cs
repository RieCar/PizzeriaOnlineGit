using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzeriaOnline.Models; 

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PizzeriaOnline.Controllers
{
    public class HomeController : Controller
    {
        private TomasosContext _context; 

        public HomeController(TomasosContext context)
        {
            _context = context; 
        }
        // GET: /<controller>/
        [Route("")]
        [Route("Start")]     
        public IActionResult Index()
        {          
            var model = _context.MatrattTyp.ToList(); 
            return View(model);
        }

        
        public IActionResult ViewMeny(int id)
        {
            var model = _context.Matratt.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt).Where(m => m.MatrattTyp == id).ToList();
            //var model = _context.Matratt.Where(m => m.MatrattTyp == id).ToList();
            return PartialView("_PartialDetailsFoodType", model);
        }

        [Route("Om-oss")]
        public IActionResult About()
        {
            var model = _context.MatrattTyp.ToList();
            return View(model);
        }
    }
}
