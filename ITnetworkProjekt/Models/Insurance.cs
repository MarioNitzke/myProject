using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Models
{
    public class Insurance
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vyberte pojistitele")]
        [Display(Name = "Pojistitel:")]
        public int InsuredPersonID { get; set; }
        [Display(Name = "Pojistitel:")]
        [ForeignKey(nameof(InsuredPersonID))]
        public InsuredPerson? InsuredPerson { get; set; }

        [Required(ErrorMessage = "Vyplňte typ pojištění")]
        [Display(Name = "Typ pojištění:")]
        public string PolicyType { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte datum začátek od:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Začátek od:")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Vyplňte datum konec do")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Konec do:")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Vyplňte částku pojištění")]
        [Display(Name = "Pojistná částka:")]
        [Precision(18, 2)]
        [Range(0, 999999.99, ErrorMessage = "Zadejte hodnotu mezi 0 a 999999.99.")]
        public decimal? PremiumAmount { get; set; }

        [Display(Name = "Datum vytvoření:")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
