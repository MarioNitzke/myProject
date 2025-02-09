using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITnetworkProjekt.Models
{
    public class InsuranceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuranceViewModelResources),
            ErrorMessageResourceName = "InsuredPersonIdRequired")]
        [Display(Name = "InsuredPersonIdLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        public int InsuredPersonId { get; set; }

        [Display(Name = "InsuredPersonIdLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        [ForeignKey(nameof(InsuredPersonId))]
        public InsuredPerson? InsuredPerson { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuranceViewModelResources),
            ErrorMessageResourceName = "PolicyTypeRequired")]
        [Display(Name = "PolicyTypeLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        public string PolicyType { get; set; } = "";

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuranceViewModelResources),
            ErrorMessageResourceName = "StartDateRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "StartDateLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuranceViewModelResources),
            ErrorMessageResourceName = "EndDateRequired")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "EndDateLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Models.InsuranceViewModelResources),
            ErrorMessageResourceName = "PremiumAmountRequired")]
        [Display(Name = "PremiumAmountLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        [Precision(18, 2)]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Resources.Models.InsuranceViewModelResources),
            ErrorMessageResourceName = "PremiumAmountRange")]
        public decimal? PremiumAmount { get; set; }

        [Display(Name = "CreatedDateLabel", ResourceType = typeof(Resources.Models.InsuranceViewModelResources))]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}