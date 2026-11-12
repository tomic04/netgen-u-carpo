using Carpo.Data.Data.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Data.Models
{
    public class OutlookModel
    {
        [Column("Ime")]
        public string Name { get; set; }
        [Column("Prezime")]
        public string Surname { get; set; }
        [Column("Title")]
        public string Title { get; set; }
        [Column("JobTitle")]
        public string JobTitle { get; set; }
        [Column("Company")]
        public string Company { get; set; }
        [Column("CompanyEmail")]
        public string CompanyEmail { get; set; }
        [Column("CompanyMobile")]
        public string CompanyMobile { get; set; }
        [Column("CompanyPhone")]
        public string CompanyHomeNumber { get; set; }
        [Column("AdresaCompany")]
        public string CompanyAddress { get; set; }
        [Column("CompanyCity")]
        public string CompanyCity { get; set; }
        [Column("CompanyHPT")]
        public string CompanyPostalCode { get; set; }
        [Column("PrivateMail")]
        public string PrivateEmail { get; set; }
        [Column("PrivateMobile")]
        public string PrivateMobile { get; set; }
        [Column("PrivatePhone")]
        public string PrivateHomeNumber { get; set; }
        [Column("PrivateAdresa")]
        public string PrivateAddress { get; set; }
        [Column("PrivateHPT")]
        public string PrivatePostalCode { get; set; }
        [Column("PrivateCity")]
        public string PrivateCity { get; set; }
    }
}
