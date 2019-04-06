using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzeriaOnline.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }

        public int KundId { get; set; }

        [Required(ErrorMessage = "Du måste ange ett namn")]
        [StringLength(100,ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Du måste ange en adress")]
        [StringLength(50, ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        public string Gatuadress { get; set; }

        [Required(ErrorMessage = "Du måste ange ett postnummer")]
        [StringLength(20, ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "Du måste ange en postort")]
        [StringLength(100, ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        public string Postort { get; set; }

       
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Ange en giltig e-postadress")]
        [StringLength(50, ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        [Remote("VerifyEmail", "Account")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [StringLength(50, ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Du måste ange ett användarnamn")]
        [DisplayName("Användarnamn")]
        [StringLength(20, ErrorMessage = "{0} kan inte vara mer än {1} tecken")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Du måste ange ett lösenord")]
        [DataType(DataType.Password)]
        [DisplayName("Lösenord")]
        [StringLength(20, ErrorMessage = "{0} kan inte vara mer än {1} tecken",MinimumLength = 5)]
        public string Losenord { get; set; }

        public virtual ICollection<Bestallning> Bestallning { get; set; }
    }
}
