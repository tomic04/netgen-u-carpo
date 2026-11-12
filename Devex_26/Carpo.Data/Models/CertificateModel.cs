using Carpo.Data.Data.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Data.Models
{
    public class CertificateModel
    {
        [Column("pfx")]
        public string CertificateName {  get; set; }
        [Column("lozinkazapfx")]
        public string CertificatePassword { get; set; }
        [Column("URL")]
        public string FiscalURL { get; set; }
    }
}
