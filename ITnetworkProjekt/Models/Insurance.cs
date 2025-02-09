﻿namespace ITnetworkProjekt.Models
{
    public class Insurance
    {
        public int Id { get; set; }
        public int InsuredPersonID { get; set; }
        public InsuredPerson? InsuredPerson { get; set; }
        public string PolicyType { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? PremiumAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}


