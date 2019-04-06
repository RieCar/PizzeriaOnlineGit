using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PizzeriaOnline.Models; 

namespace PizzeriaOnline.ViewModels
{
    public class OrderCheckOut
    {
        public Kund ShoppingUser { get; set; }
        public List<Matratt> FoodOrder { get; set; }
        public Bestallning TodoBestallning { get; set; }
        public BestallningMatratt kopplingMatAntal { get; set; }
        public List<BestallningMatratt> ListOfMatrattToOrder { get; set; }

        public OrderCheckOut()
        {
            FoodOrder = new List<Matratt>();
            ListOfMatrattToOrder = new List<BestallningMatratt>();
            TodoBestallning = new Bestallning(); 
            
        }
    }
}
