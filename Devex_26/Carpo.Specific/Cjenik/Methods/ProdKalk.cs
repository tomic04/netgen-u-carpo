using System;
using System.Windows.Forms;

namespace Carpo.Specific.Matpos
{
    public class ProdKalk
    {
        public ProdKalk()
        {
        }

        private bool isCijenaBod;

        //******input parametri - begin
        public decimal Kolicina { get; set; }

        public decimal Bod { get; set; }
        public decimal CijenaNetoBod { get; set; }
        public decimal CijenaNeto { get; set; }
        public decimal PorezPosto { get; set; }
        public decimal RabatPosto { get; set; }
        public decimal GridRabat { get; set; }
        public string KojiRabat { get; set; }
        //******input parametri - end

        public decimal VrijednostNeto { get; private set; }
        public decimal VrijednostNetoBod { get; private set; }
        public decimal Rabat { get; private set; }
        public decimal RabatBod { get; private set; }
        public decimal CassaPosto { get; private set; }
        public decimal Cassa { get; private set; }
        public decimal CassaBod { get; private set; }
        public decimal Porez { get; private set; }
        public decimal PorezBod { get; private set; }
        public decimal CijenaBruto { get; private set; }
        public decimal CijenaBrutoBod { get; private set; }
        public decimal VrijednostBruto { get; private set; }
        public decimal VrijednostBrutoBod { get; private set; }

        public void Calculation(decimal kolicina, decimal bod, decimal cijena, decimal porez, decimal rabat, decimal cassa, bool isCijenaBod)
        {
            this.Kolicina = kolicina;
            this.Bod = bod;
            this.PorezPosto = porez;
            this.RabatPosto = rabat;
            this.CassaPosto = cassa;
            this.isCijenaBod = isCijenaBod;
            if (this.isCijenaBod == true)
                this.CijenaNetoBod = cijena;
            else
                this.CijenaNeto = cijena;

            Formulas();
        }

        public void Test(string server, string database, string username, string password, int operateriid, string godina, string razdoblje, int slanje)
        {
            Form form = new Form();
            form.Show();
        }

        private void Formulas()
        {
            if (isCijenaBod == false)
                CijenaNetoBod = CijenaNeto / Bod;
            else
                CijenaNeto = CijenaNetoBod * Bod;

            VrijednostNetoBod = Decimal.Round(CijenaNetoBod * Kolicina, 2);
            VrijednostNeto = Decimal.Round(VrijednostNetoBod * Bod, 2);

            RabatBod = (VrijednostNetoBod * RabatPosto) / 100;
            Rabat = RabatBod * Bod;

            CassaBod = ((VrijednostNetoBod - RabatBod) * CassaPosto) / 100;
            Cassa = CassaBod * Bod;

            PorezBod = ((VrijednostNetoBod - RabatBod - CassaBod) * PorezPosto) / 100;
            Porez = PorezBod * Bod;

            CijenaBrutoBod = CijenaNetoBod + PorezBod;
            CijenaBruto = CijenaBrutoBod * Bod;

            VrijednostBrutoBod = VrijednostNetoBod - RabatBod - CassaBod + PorezBod;
            VrijednostBruto = VrijednostBrutoBod * Bod;

            //CijenaBrutoBod = VrijednostBrutoBod / 2; -- sokac 20180321
            CijenaBrutoBod = VrijednostBrutoBod / Kolicina;
            CijenaBruto = CijenaBrutoBod * Bod;
        }

        public void CalculationRabat(decimal kolicina, decimal bod, decimal cijena, decimal porez, decimal rabat, decimal cassa, bool isCijenaBod, decimal gridRabat, string kojiRabat)
        {
            this.Kolicina = kolicina;
            this.Bod = bod;
            this.PorezPosto = porez;
            this.RabatPosto = rabat;
            this.CassaPosto = cassa;
            this.isCijenaBod = isCijenaBod;
            this.GridRabat = gridRabat;
            this.KojiRabat = kojiRabat;
            if (this.isCijenaBod == true)
                this.CijenaNetoBod = cijena;
            else
                this.CijenaNeto = cijena;

            FormulasRabat();
        }

        private void FormulasRabat()
        {
            if (isCijenaBod == false)
                CijenaNetoBod = CijenaNeto / Bod;
            else
                CijenaNeto = CijenaNetoBod * Bod;

            VrijednostNetoBod = Decimal.Round(CijenaNetoBod * Kolicina, 2);
            VrijednostNeto = Decimal.Round(VrijednostNetoBod * Bod, 2);

            RabatBod = (VrijednostNetoBod * RabatPosto) / 100;
            Rabat = RabatBod * Bod;

            CassaBod = ((VrijednostNetoBod - RabatBod) * CassaPosto) / 100;
            Cassa = CassaBod * Bod;

            PorezBod = ((VrijednostNetoBod - RabatBod - CassaBod) * PorezPosto) / 100;
            Porez = PorezBod * Bod;

            CijenaBrutoBod = CijenaNetoBod + PorezBod;
            CijenaBruto = CijenaBrutoBod * Bod;

            VrijednostBrutoBod = VrijednostNetoBod - RabatBod - CassaBod + PorezBod;
            VrijednostBruto = VrijednostBrutoBod * Bod;

            //CijenaBrutoBod = VrijednostBrutoBod / 2; -- sokac 20180321
            CijenaBrutoBod = VrijednostBrutoBod / Kolicina;
            CijenaBruto = CijenaBrutoBod * Bod;

            switch (KojiRabat)
            {
                case "rabatObicni":
                    RabatPosto = RabatPosto;
                    break;

                case "rabatPodigni":
                    RabatPosto = GridRabat * (1 + RabatPosto / 100);
                    break;

                case "rabatPodigniPlus":
                    RabatPosto = GridRabat + RabatPosto;
                    break;

                case "rabatOstatak":
                    RabatPosto = GridRabat + ((100 - GridRabat) * RabatPosto / 100);
                    break;
            }
        }
    }
}