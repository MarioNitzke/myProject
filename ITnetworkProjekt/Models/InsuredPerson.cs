using System.ComponentModel.DataAnnotations;
using NuGet.Protocol.Core.Types;

namespace ITnetworkProjekt.Models
{
    public class InsuredPerson
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string SocialSecurityNumber { get; set; } = "";
        public string? UserId { get; set; } = "";
        public List<int>? InsuranceIds { get; set; }
    }
}


