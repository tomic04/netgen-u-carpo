using System;
using System.Data;

namespace Carpo.Specific.Matpos
{
    public class PriListKalkUrednikRabata
    {
        public PriListKalkUrednikRabata()
        {
        }

        public int NabPriDetId { get; set; }
        public decimal Rabat1P { get; set; }
        public decimal Rabat2P { get; set; }
        public decimal Rabat3P { get; set; }
        public decimal Rabat1I { get; set; }
        public decimal Rabat2I { get; set; }
        public decimal Rabat3I { get; set; }
        public decimal CarinaI { get; set; }
        public decimal TrosakI { get; set; }
        public decimal NabCijena { get; set; }
        public decimal NabVrij { get; set; }
        public decimal NCijena { get; set; }
        public decimal NVrijednost { get; set; }
        public string KojiRabat { get; set; }

        //sve preostale kolone potrebne za update spNabPlkCarpoDetAzur
        private int action = 5;

        public void CalculationRabat(int nabPriDetId, decimal rabat1P,
                                    decimal rabat2P,
                                    decimal rabat3P,
                                    string kojiRabat)
        {
            this.NabPriDetId = nabPriDetId;
            this.KojiRabat = kojiRabat;
            this.Rabat1P = rabat1P;
            this.Rabat2P = rabat2P;
            this.Rabat3P = rabat3P;

            FormulasRabat();
        }

        private void FormulasRabat()
        {
            DataTable dt = Data.Matpos.PriListKalkRabati.GetStavkaPLK(NabPriDetId).Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                switch (KojiRabat)
                {
                    case "rabatObicni":
                        Rabat1P = Rabat1P;
                        break;

                    case "rabatPodigni":
                        Rabat1P = Convert.ToDecimal(dt.Rows[0]["Rabat1P"]) * (1 + Rabat1P / 100);
                        Rabat2P = Convert.ToDecimal(dt.Rows[0]["Rabat2P"]) * (1 + Rabat2P / 100);
                        Rabat3P = Convert.ToDecimal(dt.Rows[0]["Rabat3P"]) * (1 + Rabat3P / 100);
                        break;

                    case "rabatPodigniPlus":
                        Rabat1P = Convert.ToDecimal(dt.Rows[0]["Rabat1P"]) + Rabat1P;
                        Rabat2P = Convert.ToDecimal(dt.Rows[0]["Rabat2P"]) + Rabat2P;
                        Rabat3P = Convert.ToDecimal(dt.Rows[0]["Rabat3P"]) + Rabat3P;
                        break;

                    case "rabatOstatak":
                        Rabat1P = Convert.ToDecimal(dt.Rows[0]["Rabat1P"]) + ((100 - Convert.ToDecimal(dt.Rows[0]["Rabat1P"])) * Rabat1P / 100);
                        Rabat2P = Convert.ToDecimal(dt.Rows[0]["Rabat2P"]) + ((100 - Convert.ToDecimal(dt.Rows[0]["Rabat2P"])) * Rabat2P / 100);
                        Rabat3P = Convert.ToDecimal(dt.Rows[0]["Rabat3P"]) + ((100 - Convert.ToDecimal(dt.Rows[0]["Rabat3P"])) * Rabat3P / 100);
                        break;
                }

                Rabat1I = Math.Round((decimal)dt.Rows[0]["FVrijednost"] * Rabat1P / 100, 4, MidpointRounding.AwayFromZero);

                Rabat2I = Math.Round((decimal)dt.Rows[0]["FVrijednost"] * (1 - Rabat1P / 100) * (Rabat2P / 100), 4, MidpointRounding.AwayFromZero);

                Rabat3I = Math.Round((decimal)dt.Rows[0]["FVrijednost"] * (1 - Rabat1P / 100) * (1 - Rabat2P / 100) * (Rabat3P / 100), 4, MidpointRounding.AwayFromZero);

                CarinaI = Math.Round(((decimal)dt.Rows[0]["FVrijednost"] - Rabat1I - Rabat2I - Rabat3I) * Convert.ToDecimal(dt.Rows[0]["CarinaP"]) / 100, 4, MidpointRounding.AwayFromZero);

                TrosakI = ((decimal)dt.Rows[0]["FVrijednost"] - Rabat1I - Rabat2I - Rabat3I + Convert.ToDecimal(dt.Rows[0]["Trosarina"]) + CarinaI) * Convert.ToDecimal(dt.Rows[0]["TrosakP"]) / 100;

                NVrijednost = (decimal)dt.Rows[0]["FVrijednost"] + TrosakI - Rabat1I - Rabat2I - Rabat3I + Convert.ToDecimal(dt.Rows[0]["PovratnaNaknada"]) + Convert.ToDecimal(dt.Rows[0]["Trosarina"]) + CarinaI + Convert.ToDecimal(dt.Rows[0]["PoticajnaNaknada"]);

                if (Convert.ToDecimal(dt.Rows[0]["Kolicina"]) != 0)
                {
                    NCijena = NVrijednost / Convert.ToDecimal(dt.Rows[0]["Kolicina"]);
                }
                else
                {
                    NCijena = 0;
                }

                NabVrij = (decimal)dt.Rows[0]["FVrijednost"] + TrosakI - Rabat1I - Rabat2I - Rabat3I + Convert.ToDecimal(dt.Rows[0]["Trosarina"]) + CarinaI;

                if (Convert.ToDecimal(dt.Rows[0]["Kolicina"]) != 0)
                {
                    NabCijena = NVrijednost / Convert.ToDecimal(dt.Rows[0]["Kolicina"]);
                }
                else
                {
                    NabCijena = 0;
                }

                string sqlText = "";
                //update stavku dokumenta s novim vrijednostima
                Data.Matpos.PriListKalkRabati.SpremiStavku(action, NabPriDetId, NCijena, Rabat1P, Rabat1I, Rabat2P, Rabat2I, Rabat3P, Rabat3I, TrosakI, NCijena, NVrijednost, CarinaI, out sqlText);
            }
        }

        public static object SendIntParamToDatabase(object objekt)
        {
            if (objekt == null)
                return DBNull.Value;
            else
                return objekt;
        }
    }
}