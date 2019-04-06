using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PizzeriaOnline.Models; 

namespace PizzeriaOnline.ViewModels
{
    public class ViewModelMeny
    {
        public List<Matratt> Pizzor { get; set; }
        public List<Matratt> Pasta { get; set; }
        public List<Matratt> Sallad { get; set; }

        public ViewModelMeny()
        {
            Pizzor = new List<Matratt>(); 
            Pasta = new List<Matratt>();
            Sallad = new List<Matratt>();
        }
    }
}
