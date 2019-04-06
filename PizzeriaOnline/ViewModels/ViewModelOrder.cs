using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzeriaOnline.Models; 

namespace PizzeriaOnline.ViewModels
{
    public class ViewModelOrder
    {
        public List<Matratt> ListOfMatratt { get; set; }
        public List<MatrattTyp> ListOfTyper { get; set; }
        public Matratt Order { get; set; }
    }
}
