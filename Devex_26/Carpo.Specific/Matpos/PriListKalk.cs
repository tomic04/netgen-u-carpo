using System;

namespace Carpo.Specific.Matpos
{
    public class PriListKalk
    {
        public PriListKalk()
        { }

        //input varijable
        public string Tip { get; set; }

        public decimal Kolicina { get; set; }
        public decimal FakCijena { get; set; }
        public decimal FakVrijednost { get; set; }
        public decimal Tecaj { get; set; }
        public decimal DevCijena { get; set; }
        public decimal DevVrijednost { get; set; }
        public decimal Ambalaza_faktor { get; set; }
        public decimal Ambalaza_cijena { get; set; }
        public decimal NabCijena { get; set; }
        public decimal NabVrij { get; set; }
        public bool DevCijeEnabled { get; set; }
        public decimal Rabat1I { get; set; }
        public decimal Rabat2I { get; set; }
        public decimal Rabat3I { get; set; }
        public decimal Rabat1P { get; set; }
        public decimal Rabat2P { get; set; }
        public decimal Rabat3P { get; set; }
        public decimal CarinaI { get; set; }
        public decimal CarinaP { get; set; }
        public decimal TrosakI { get; set; }
        public decimal TrosakP { get; set; }
        public decimal Trosarina { get; set; }
        public decimal NCijena { get; set; }
        public decimal NVrijednost { get; set; }
        public decimal Poticaj { get; set; }
        public decimal Povrat { get; set; }
        public decimal FakVKn { get; set; }
        public decimal FakVE { get; set; }
        public decimal Marza { get; set; }
        public decimal MarzaP { get; set; }
        public decimal VPC { get; set; }
        public decimal MPC { get; set; }
        public decimal Rabat1IE { get; set; }
        public decimal Rabat2IE { get; set; }
        public decimal Rabat3IE { get; set; }

        /// <summary>
        /// tag parametri
        /// </summary>
        public decimal DevVrijednostTag { get; set; }

        public decimal FakVrijednostTag { get; set; }
        public decimal Rabat1ITag { get; set; }
        public decimal Rabat2ITag { get; set; }
        public decimal Rabat3ITag { get; set; }

        public void Calculation(string tip,
                                decimal kolicina,
                                decimal fakCijena,
                                decimal fakVrijednost,
                                decimal tecaj,
                                decimal devCijena,
                                decimal devVrijednost,
                                decimal ambalaza_faktor,
                                decimal ambalaza_cijena,
                                decimal nabCijena,
                                decimal nabVrij,
                                bool devCijeEnabled,
                                decimal rabat1I,
                                decimal rabat2I,
                                decimal rabat3I,
                                decimal rabat1P,
                                decimal rabat2P,
                                decimal rabat3P,
                                decimal carinaI,
                                decimal carinaP,
                                decimal trosakI,
                                decimal trosakP,
                                decimal trosarina,
                                decimal nVrijednost,
                                decimal poticaj,
                                decimal povrat,
                                decimal marza,
                                decimal marzaP,
                                decimal vpc,
                                decimal mpc,
                                decimal rabat1IE,
                                decimal rabat2IE,
                                decimal rabat3IE,

                                decimal devVrijednostTag,
                                decimal fakVrijednostTag,
                                decimal rabat1ITag,
                                decimal rabat2ITag,
                                decimal rabat3ITag

                            )
        {
            //binding svih vrijednosti sa forme
            this.Tip = tip;
            this.Kolicina = kolicina;
            this.FakCijena = fakCijena;
            this.FakVrijednost = fakVrijednost;
            this.Tecaj = tecaj;
            this.Ambalaza_cijena = ambalaza_cijena;
            this.Ambalaza_faktor = ambalaza_faktor;
            this.NabCijena = nabCijena;
            this.DevCijeEnabled = devCijeEnabled;
            this.Rabat1I = rabat1I;
            this.Rabat2I = rabat2I;
            this.Rabat3I = rabat3I;
            this.DevCijena = devCijena;
            this.DevVrijednost = devVrijednost;
            this.Rabat1P = rabat1P;
            this.Rabat2P = rabat2P;
            this.Rabat3P = rabat3P;
            this.CarinaI = carinaI;
            this.CarinaP = carinaP;
            this.TrosakP = trosakP;
            this.TrosakI = trosakI;
            this.Trosarina = trosarina;
            this.NVrijednost = nVrijednost;
            this.Poticaj = poticaj;
            this.Povrat = povrat;
            this.NabVrij = nabVrij;
            this.Marza = marza;
            this.MarzaP = marzaP;
            this.VPC = vpc;
            this.MPC = mpc;
            this.Rabat1IE = rabat1IE;
            this.Rabat2IE = rabat2IE;
            this.Rabat3IE = rabat3IE;

            this.DevVrijednostTag = devVrijednostTag;
            this.FakVrijednostTag = fakVrijednostTag;
            this.Rabat1ITag = rabat1ITag;
            this.Rabat2ITag = rabat2ITag;
            this.Rabat3ITag = rabat3ITag;
            //overloading prema tipu, poziv kunkcije za preracun ovisno o kontroli koja ima leave event

            if (Tip == "")
            {
                //Ovo je inicijalni load izračuna koji se okida nakon izbora robe
                obracunaj();
            }
            else
            {
                izracunPriListKalk();
            }
        }

        private void izracunPriListKalk()
        {
            try
            {
                if (Tip == null)
                    return;
                switch (Tip.ToString())
                {
                    case "kolicina":
                        KolicinaCalc();
                        break;

                    case "Fcijena":
                        FcijenaClac();
                        break;

                    case "Fvrijednost":
                        FvrijednostCalc();
                        break;

                    case "Rabat1P":
                        Rabat1PCalc();
                        break;

                    case "Rabat2P":
                        Rabat2PCalc();
                        break;

                    case "Rabat3P":
                        Rabat3PCalc();
                        break;

                    case "Rabat1I":
                        Rabat1ICalc();
                        break;

                    case "Rabat2I":
                        Rabat2ICalc();
                        break;

                    case "Rabat3I":
                        Rabat3ICalc();
                        break;

                    case "DevCijena":
                        DevCijenaCalc();
                        break;

                    case "DevVrijednost":
                        DevVrijednostCalc();
                        break;

                    case "CarinaI":
                        CarinaICalc();
                        break;

                    case "CarinaP":
                        CarinaPCalc();
                        break;

                    case "Trosarina":
                        TrosarinaCalc();
                        break;

                    case "TrosakP":
                        TrosakPCalc();
                        break;

                    case "TrosakI":
                        TrosakICalc();
                        break;

                    case "Povrat":
                        PovratCalc();
                        break;

                    case "Poticaj":
                        PoticajCalc();
                        break;
                }
            }
            catch (DivideByZeroException)
            {
                //Common.Helpers.JMessages.Error(ex.Message);
            }
            //srediVrijednosti();
        }

        private void obracunaj()
        {
            FakVrijednost = Kolicina * FakCijena;
            //srediVrijednosti();

            if (DevCijeEnabled == true)
            {
                if (Tecaj > 0)
                    ChkTecaj(Tecaj, Tip);
            }
            else
            {
                //getlast();
                //srediVrijednosti();
            }

            FakVrijednost = Kolicina * FakCijena;

            srediVrijednosti();

            FakVrijednost = Math.Round(Kolicina * FakCijena, 2, MidpointRounding.AwayFromZero);

            //Kolicina = doKolicina(ribaid);
        }

        /// <summary>
        /// glavna funkcija za preračun ostalih polja , sredivrijednosti()
        /// </summary>

        private void srediVrijednosti()
        {
            Rabat1I = Decimal.Round(FakVrijednost * Rabat1P / 100, 4, MidpointRounding.AwayFromZero);
            srediVrijednosti2();
            Rabat1I = Decimal.Round(FakVrijednost * Rabat1P / 100, 2, MidpointRounding.AwayFromZero);
        }

        private void srediVrijednosti2()
        {
            Rabat2I = Decimal.Round(FakVrijednost * (1 - Rabat1P / 100) * (Rabat2P / 100), 4, MidpointRounding.AwayFromZero);
            srediVrijednosti3a();
            Rabat2I = Decimal.Round(FakVrijednost * (1 - Rabat1P / 100) * (Rabat2P / 100), 2, MidpointRounding.AwayFromZero);
        }

        private void srediVrijednosti3a()
        {
            Rabat3I = Decimal.Round(FakVrijednost * (1 - Rabat1P / 100) * (1 - Rabat2P / 100) * (Rabat3P / 100), 4, MidpointRounding.AwayFromZero);
            srediVrijednosti3b();
            Rabat3I = Decimal.Round(FakVrijednost * (1 - Rabat1P / 100) * (1 - Rabat2P / 100) * (Rabat3P / 100), 2, MidpointRounding.AwayFromZero);
        }

        private void srediVrijednosti3b()
        {
            CarinaI = Decimal.Round((FakVrijednost - Rabat1I - Rabat2I - Rabat3I) * CarinaP / 100, 4, MidpointRounding.AwayFromZero);
            srediVrijednosti3();
            CarinaI = Decimal.Round(CarinaI, 2, MidpointRounding.AwayFromZero);
        }

        private void srediVrijednosti3()
        {
            TrosakI = Math.Round((FakVrijednost - Rabat1I - Rabat2I - Rabat3I + Trosarina + CarinaI) * TrosakP / 100, 4, MidpointRounding.AwayFromZero);
            srediVrijednosti4();
            srediVrijednosti6();
            TrosakI = Math.Round(TrosakI, 2, MidpointRounding.AwayFromZero);
        }

        private void srediVrijednosti4()
        {
            NVrijednost = FakVrijednost + TrosakI - Math.Round(Rabat1I, 2) - Math.Round(Rabat2I, 2) - Math.Round(Rabat3I, 2) + Povrat + Trosarina + CarinaI + Poticaj;

            if (Kolicina != 0)
            {
                NCijena = Math.Round(NVrijednost / Kolicina, 2, MidpointRounding.AwayFromZero);
                FakVKn = FakVrijednost - Math.Round(Rabat1I, 2, MidpointRounding.AwayFromZero) - Math.Round(Rabat2I, 2, MidpointRounding.AwayFromZero) - Math.Round(Rabat3I, 2, MidpointRounding.AwayFromZero);

                if (DevCijena > 0)
                {
                    FakVE = DevVrijednost - Rabat1IE - Rabat2IE - Rabat3IE;
                }
            }
            else
            {
                NCijena = 0;
                FakVKn = 0;
                FakVE = 0;
            }

            NVrijednost = Math.Round(NVrijednost, 2, MidpointRounding.AwayFromZero);
        }

        private void srediVrijednosti6()
        {
            NabVrij = FakVrijednost + Math.Round(TrosakI, 2) - Math.Round(Rabat1I, 2) - Math.Round(Rabat2I, 2) - Math.Round(Rabat3I, 2) + Math.Round(Trosarina, 2) + Math.Round(CarinaI, 2);
            if (Kolicina != 0)
            {
                NabCijena = Math.Round(NabVrij / Kolicina, 2, MidpointRounding.AwayFromZero);
                FakVKn = FakVrijednost - Math.Round(Rabat1I, 2, MidpointRounding.AwayFromZero) - Math.Round(Rabat2I, 2, MidpointRounding.AwayFromZero) - Math.Round(Rabat3I, 2, MidpointRounding.AwayFromZero);

                if (NabCijena != 0)
                {
                    Marza = Math.Round(VPC - NabCijena, 2, MidpointRounding.AwayFromZero);
                    MarzaP = Marza / Math.Round(NabCijena, 2, MidpointRounding.AwayFromZero) * 100;

                    BtnCjenik(true);
                }
                else
                {
                    Marza = 0;
                    MarzaP = 0;
                    FakVKn = 0;
                    Rabat1IE = 0;
                    Rabat2IE = 0;
                    Rabat3IE = 0;
                    FakVE = 0;
                    BtnCjenik(false);
                }

                if (DevCijena != 0)
                {
                    if (Tecaj != 0)
                    {
                        Rabat1IE = Math.Round(Math.Round(Rabat1I, 2, MidpointRounding.AwayFromZero) / Tecaj, 2, MidpointRounding.AwayFromZero);
                        Rabat2IE = Math.Round(Math.Round(Rabat2I, 2, MidpointRounding.AwayFromZero) / Tecaj, 2, MidpointRounding.AwayFromZero); ;
                        Rabat3IE = Math.Round(Math.Round(Rabat3I, 2, MidpointRounding.AwayFromZero) / Tecaj, 2, MidpointRounding.AwayFromZero);
                    }

                    FakVE = DevVrijednost - Rabat1IE - Rabat2IE - Rabat3IE;
                }
            }
            else
            {
                NabCijena = 0;
                Marza = 0;
                MarzaP = 0;
                FakVKn = 0;
                Rabat1IE = 0;
                Rabat2IE = 0;
                Rabat3IE = 0;
                FakVE = 0;
            }
        }

        private void KolicinaCalc()
        {
            FakVrijednost = Math.Round(Kolicina * FakCijena, 2, MidpointRounding.AwayFromZero);

            ChkTecaj(Tecaj, Tip);

            srediVrijednosti();

            IzracunajAmbalazu(Kolicina);

            //sredi vrijednost()
            srediVrijednosti();

            BtnCjenik();
        }

        private void FcijenaClac()
        {
            FakVrijednost = Math.Round(Kolicina * FakCijena, 2, MidpointRounding.AwayFromZero);

            ChkTecaj(Tecaj, Tip);

            //sredi vrijednost()
            srediVrijednosti();

            BtnCjenik();
        }

        private void FvrijednostCalc()
        {
            if (FakVrijednostTag != FakVrijednost)
            {
                if (Kolicina != 0)
                {
                    FakCijena = FakVrijednost / Kolicina;
                }
                else
                {
                    FakCijena = 0;
                }
                ChkTecaj(Tecaj, Tip);

                //sredi vrijednosti()
            }

            srediVrijednosti();
            DevCijena = Math.Round(DevVrijednost, 2) / Kolicina;
        }

        private void Rabat1PCalc()
        {
            srediVrijednosti();
        }

        private void Rabat2PCalc()
        {
            srediVrijednosti2();
        }

        private void Rabat3PCalc()
        {
            srediVrijednosti3a();
        }

        private void Rabat1ICalc()
        {
            if (Rabat1ITag != Rabat1I || Rabat1P == 0)
            {
                Rabat1P = Decimal.Round(100 * Rabat1I / FakVrijednost, 2, MidpointRounding.AwayFromZero);
            }

            srediVrijednosti2();
        }

        private void Rabat2ICalc()
        {
            if (Rabat2ITag != Rabat2I || Rabat2P == 0)
            {
                Rabat2P = Decimal.Round(100 * Rabat2I / (FakVrijednost - Rabat1I), 2, MidpointRounding.AwayFromZero);
            }

            srediVrijednosti3a();
        }

        private void Rabat3ICalc()
        {
            if (Rabat3ITag != Rabat3I || Rabat3P == 0)
            {
                Rabat3P = Decimal.Round(100 * Rabat3I / (FakVrijednost - Rabat1I - Rabat2I), 2, MidpointRounding.AwayFromZero);
            }

            srediVrijednosti3b();
        }

        private void DevCijenaCalc()
        {
            if (DevCijeEnabled != false)
            {
                DevVrijednost = ((Math.Round(DevCijena, 8, MidpointRounding.AwayFromZero) * Kolicina));
                FakCijena = Math.Round(DevCijena, 8, MidpointRounding.AwayFromZero) * Tecaj;
                FakVrijednost = decimal.Round(Math.Round(DevCijena, 8) * Kolicina * Tecaj, 2, MidpointRounding.AwayFromZero);

                srediVrijednosti();
            }
        }

        private void DevVrijednostCalc()
        {
            if (DevCijeEnabled != false)
            {
                if (DevVrijednostTag != DevVrijednost)
                {
                    if (Kolicina > 0)
                    {
                        DevCijena = DevVrijednost / Kolicina;
                    }
                    else
                    {
                        DevCijena = 0;
                    }
                }

                FakCijena = DevCijena * Tecaj;
                FakVrijednost = Decimal.Round(Math.Round(DevCijena, 8, MidpointRounding.AwayFromZero) * Kolicina * Tecaj, 2, MidpointRounding.AwayFromZero);

                srediVrijednosti();
            }
        }

        private void CarinaICalc()
        {
            if (Kolicina == 0 || FakCijena == 0)
                return;
            CarinaP = Decimal.Round(100 * CarinaI / (FakVrijednost - Rabat1I - Rabat2I - Rabat3I), 2, MidpointRounding.AwayFromZero);

            srediVrijednosti3();
        }

        private void CarinaPCalc()
        {
            srediVrijednosti3b();
        }

        private void TrosarinaCalc()
        {
            if (Kolicina == 0 || FakCijena == 0)
                return;
            srediVrijednosti3();
        }

        private void TrosakPCalc()
        {
            if (Kolicina == 0 || FakCijena == 0)
                return;
            srediVrijednosti3();
        }

        private void TrosakICalc()
        {
            if (Kolicina == 0 || FakCijena == 0)
                return;
            if ((FakVrijednost - Rabat1I - Rabat2I - Rabat3I) != 0)
                TrosakP = Decimal.Round(100 * TrosakI / (FakVrijednost - Rabat1I - Rabat2I - Rabat3I + CarinaI + Trosarina), 2, MidpointRounding.AwayFromZero);

            srediVrijednosti4();
            srediVrijednosti6();
        }

        private void PovratCalc()
        {
            if (Kolicina == 0 || FakCijena == 0)
                return;

            srediVrijednosti4();
        }

        private void PoticajCalc()
        {
            if (Kolicina == 0 || FakCijena == 0)
                return;

            srediVrijednosti4();
            srediVrijednosti6();
        }

        private void IzracunajAmbalazu(decimal kolicina)
        {
            if (Ambalaza_faktor * Ambalaza_cijena != 0)
            {
                Povrat = Math.Round(Kolicina / Ambalaza_faktor * Ambalaza_cijena, 2, MidpointRounding.AwayFromZero);
            }
        }

        private void ChkTecaj(decimal tecaj, string tip)
        {
            if (Tecaj == 0)
                return;
            if (Tecaj == 1)
            {
                DevCijena = 0;
                DevVrijednost = 0;
            }
            else
            {
                if (Tip == "kolicina")
                    FakCijena = Math.Round(DevCijena, 8, MidpointRounding.AwayFromZero) * Tecaj;
                if ((Tip == "Fcijena") || (Tip == "Fvrijednost"))
                    DevCijena = FakCijena / Tecaj;
                if (Tip == "")
                    if (DevCijena == 0)
                        DevCijena = FakVrijednost / Tecaj;
                DevVrijednost = Math.Round(DevCijena, 8, MidpointRounding.AwayFromZero) * Kolicina;
            }
        }

        private void BtnCjenik(bool? enable = false)
        {
            if (enable == true)
                //omoguci btnCjenik
                //onemoguci btnCjenik

                if (NabCijena == 0)
                {
                    //onemoguci bntCjenik
                    //inace omoguci
                }
        }
    }
}