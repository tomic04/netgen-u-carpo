using System;
using System.Collections.Generic;
using System.Data;

namespace Carpo.Specific.Data
{
    internal class Virmani
    {
        public static void SpremiVirman(string connectionString, int action, int? virmaniid, int? virmanivrsteid, int? nalogodavacid, int? nalogodavacziroid,
           string nalogodavacziro, string pzvbrzad1, string pzvbrzad2, string svrha1, string svrha2, string svrha3, string svrha4, decimal? iznos,
           int? parziroid, string partneriziro, int? partneriid, string pzvbrod1, string pzvbrod2, DateTime? datum, int ssoperateriid, string spreman,
           int? valuteid, int indhitno, int indprimateljtip, int? valutepokricaid, int indtroskovna, int? sifranamjeneid, string bic, string bankanaziv,
           string bankaadresa, string bankamjesto, int? bankazemlja, int indprimatelj, int indplatitelj, string platitelj1, string platitelj2,
           string platitelj3, string primatelj1, string primatelj2, string primatelj3, out string sqlText)
        {
            try
            {
                string spName = "spCKntVirmaniAzur";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", 0 },
                    { "@VirmaniId", Kontor.Virmani.Virmani.SendIntParamToDatabase(virmaniid) },
                    { "@VirmaniVrsteId", Kontor.Virmani.Virmani.SendIntParamToDatabase(virmanivrsteid) },
                    { "@JedZavoda", "" },
                    { "@NalogodavacId", Kontor.Virmani.Virmani.SendIntParamToDatabase(nalogodavacid) },
                    { "@NalogodavacZiroId", Kontor.Virmani.Virmani.SendIntParamToDatabase(nalogodavacziroid) },
                    { "@NalogodavacZiro", Kontor.Virmani.Virmani.SendStringParamToDatabase(nalogodavacziro,25) },
                    { "@Nacinizvrs", "" },
                    { "@PzvbrZad1", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrzad1,4) },
                    { "@Pzvbrzad2", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrzad2,22) },
                    { "@Svrha1", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha1,35) },
                    { "@Svrha2", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha2,35) },
                    { "@Svrha3", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha3,35) },
                    { "@Svrha4", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha4,35) },
                    { "@Sifra1",  "" },
                    { "@Sifra2",  "" },
                    { "@Sifra3",  "" },
                    { "@Iznos", iznos },
                    { "@ParZiroId", Kontor.Virmani.Virmani.SendIntParamToDatabase(parziroid) },
                    { "@PartneriZiro", Kontor.Virmani.Virmani.SendStringParamToDatabase(partneriziro,34) },
                    { "@Partneriid", Kontor.Virmani.Virmani.SendIntParamToDatabase(partneriid) },
                    { "@PzvbrOd1", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrod1,4) },
                    { "@PzvbrOd2", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrod2,22) },
                    { "@Mjesto", "" },
                    { "@Datum", Kontor.Virmani.Virmani.SendIntParamToDatabase(datum) },
                    { "@ssOperateriId", ssoperateriid },
                    { "@DatumUnosa", DateTime.Now },
                    { "@KntPrometId", 0 },
                    { "@Spreman", spreman },
                    { "@ValuteId", Kontor.Virmani.Virmani.SendIntParamToDatabase(valuteid) },
                    { "@IndHitno", indhitno },
                    { "@IndPrimateljTip", indprimateljtip },
                    { "@ValutePokricaId", Kontor.Virmani.Virmani.SendIntParamToDatabase(valutepokricaid) },
                    { "@IndTroskovna", indtroskovna },
                    { "@SifraNamjeneID", Kontor.Virmani.Virmani.SendIntParamToDatabase(sifranamjeneid) },
                    { "@BIC", Kontor.Virmani.Virmani.SendStringParamToDatabase(bic,11) },
                    { "@BankaNaziv", Kontor.Virmani.Virmani.SendStringParamToDatabase(bankanaziv,70) },
                    { "@BankaAdresa", Kontor.Virmani.Virmani.SendStringParamToDatabase(bankaadresa,35) },
                    { "@BankaMjesto", Kontor.Virmani.Virmani.SendStringParamToDatabase(bankamjesto,35) },
                    { "@BankaZemlja", Kontor.Virmani.Virmani.SendIntParamToDatabase(bankazemlja) },
                    { "@IndPrimatelj", indprimatelj },
                    { "@IndPlatitelj", indplatitelj },
                    { "@Platitelj1", Kontor.Virmani.Virmani.SendStringParamToDatabase(platitelj1,50) },
                    { "@Platitelj2", Kontor.Virmani.Virmani.SendStringParamToDatabase(platitelj2,50) },
                    { "@Platitelj3", Kontor.Virmani.Virmani.SendStringParamToDatabase(platitelj3,50) },
                    { "@Primatelj1", Kontor.Virmani.Virmani.SendStringParamToDatabase(primatelj1,50) },
                    { "@Primatelj2", Kontor.Virmani.Virmani.SendStringParamToDatabase(primatelj2,50) },
                    { "@Primatelj3", Kontor.Virmani.Virmani.SendStringParamToDatabase(primatelj3,50) }
                };

                Carpo.Data.Common.ExecuteNonQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }

        public static void SpremiVirmaniVrste(string connectionString, int action, int? virmanivrsteid, string naziv, string oznaka, int? nalogodavacid,
           int? nalogodavacziroid, string nalogodavacziro, string pzvbrzad1, string pzvbrzad2, string svrha1, string svrha2, string svrha3, string svrha4,
           decimal? iznos, int? parziroid, string partneriziro, int? partneriid, string pzvbrod1, string pzvbrod2, DateTime? datum, int ssoperateriid,
           string spreman, int? valuteid, int indhitno, int indprimateljtip, int? valutepokricaid, int indtroskovna, int? sifranamjeneid, string bic,
           string bankanaziv, string bankaadresa, string bankamjesto, int? bankazemlja, int indprimatelj, int indplatitelj, string platitelj1,
           string platitelj2, string platitelj3, string primatelj1, string primatelj2, string primatelj3, out string sqlText)
        {
            try
            {
                string spName = "spCkntVirmaniVrsteAzur";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", 0 },
                    { "@VirmaniVrsteId", Kontor.Virmani.Virmani.SendIntParamToDatabase(virmanivrsteid) },
                    { "@Naziv", Kontor.Virmani.Virmani.SendStringParamToDatabase(naziv,50) },
                    { "@Tip",  "3" },
                    { "@Oznaka", Kontor.Virmani.Virmani.SendStringParamToDatabase(oznaka,1) },
                    { "@JedZavoda", "" },
                    { "@NalogodavacId", Kontor.Virmani.Virmani.SendIntParamToDatabase(nalogodavacid) },
                    { "@NalogodavacZiroId", Kontor.Virmani.Virmani.SendIntParamToDatabase(nalogodavacziroid) },
                    { "@NalogodavacZiro", Kontor.Virmani.Virmani.SendStringParamToDatabase(nalogodavacziro,25) },
                    { "@Nacinizvrs", "" },
                    { "@PzvbrZad1", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrzad1,4) },
                    { "@Pzvbrzad2", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrzad2,25) },
                    { "@Svrha1", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha1,30) },
                    { "@Svrha2", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha2,40) },
                    { "@Svrha3", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha3,40) },
                    { "@Svrha4", Kontor.Virmani.Virmani.SendStringParamToDatabase(svrha4,30) },
                    { "@Iznos", iznos },
                    { "@Sifra1",  "" },
                    { "@Sifra2",  "" },
                    { "@Sifra3",  "" },
                    { "@ParZiroId", Kontor.Virmani.Virmani.SendIntParamToDatabase(parziroid) },
                    { "@PartneriZiro", Kontor.Virmani.Virmani.SendStringParamToDatabase(partneriziro,25) },
                    { "@Partneriid", Kontor.Virmani.Virmani.SendIntParamToDatabase(partneriid) },
                    { "@PzvbrOd1", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrod1,4) },
                    { "@PzvbrOd2", Kontor.Virmani.Virmani.SendStringParamToDatabase(pzvbrod2,30) },
                    { "@Mjesto", "" },
                    { "@Datum", Kontor.Virmani.Virmani.SendIntParamToDatabase(datum) },
                    { "@ssOperateriId", ssoperateriid },
                    { "@DatumUnosa", DateTime.Now },
                    { "@Spreman", spreman },
                    { "@ValuteId", Kontor.Virmani.Virmani.SendIntParamToDatabase(valuteid) },
                    { "@IndHitno", indhitno },
                    { "@IndPrimateljTip", indprimateljtip },
                    { "@ValutePokricaId", Kontor.Virmani.Virmani.SendIntParamToDatabase(valutepokricaid) },
                    { "@IndTroskovna", indtroskovna },
                    { "@SifraNamjeneID", Kontor.Virmani.Virmani.SendIntParamToDatabase(sifranamjeneid) },
                    { "@BIC", Kontor.Virmani.Virmani.SendStringParamToDatabase(bic,11) },
                    { "@BankaNaziv", Kontor.Virmani.Virmani.SendStringParamToDatabase(bankanaziv,70) },
                    { "@BankaAdresa", Kontor.Virmani.Virmani.SendStringParamToDatabase(bankaadresa,35) },
                    { "@BankaMjesto", Kontor.Virmani.Virmani.SendStringParamToDatabase(bankamjesto,35) },
                    { "@BankaZemlja", Kontor.Virmani.Virmani.SendIntParamToDatabase(bankazemlja) },
                    { "@IndPrimatelj", indprimatelj },
                    { "@IndPlatitelj", indplatitelj },
                    { "@Platitelj1", Kontor.Virmani.Virmani.SendStringParamToDatabase(platitelj1,50) },
                    { "@Platitelj2", Kontor.Virmani.Virmani.SendStringParamToDatabase(platitelj2,50) },
                    { "@Platitelj3", Kontor.Virmani.Virmani.SendStringParamToDatabase(platitelj3,50) },
                    { "@Primatelj1", Kontor.Virmani.Virmani.SendStringParamToDatabase(primatelj1,50) },
                    { "@Primatelj2", Kontor.Virmani.Virmani.SendStringParamToDatabase(primatelj2,50) },
                    { "@Primatelj3", Kontor.Virmani.Virmani.SendStringParamToDatabase(primatelj3,50) }
                };

                Carpo.Data.Common.ExecuteNonQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetValute(string connectionString, out string sqlText)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                string spName = "spValuteQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 0 }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static int? GetHrkId(string connectionString, out string sqlText)
        {
            try
            {
                string spName = "spCKntVirmaniIzmijenaQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 10 }
                };

                return Convert.ToInt32(Carpo.Data.Common.ExecuteScalar(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30));
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetSifraNamjene(string connectionString, out string sqlText)
        {
            try
            {
                string spName = "spSifraNamjeneQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 0 }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetIBANComboData(string connectionString, int action, string iban, int partneriid, out string sqlText)
        {
            try
            {
                string spName = "spCKntVirmaniZiroDataGet";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", action },
                    { "@iban", iban },
                    { "@partneriid", partneriid }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetPartnerAdress(string connectionString, int partneriid, out string sqlText)
        {
            try
            {
                string spName = "spCKNTVirmaniAdresaGet";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@PartneriId", partneriid }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetData(string connectionString, int action, int id, out string sqlText)
        {
            try
            {
                string spName = "spCKntVirmaniIzmijenaQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", id }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static int CheckAndOrAddIban(string connectionString, int action, string ziro, int partneriid, out string sqlText)
        {
            try
            {
                string spName = "spCkntVirmaniDodajIBAN";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@Ziro", ziro },
                    { "@PartneriId", partneriid },
                };

                return Convert.ToInt32(Carpo.Data.Common.ExecuteScalar(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30));
            }
            catch
            {
                throw;
            }
        }

        public static object SwiftUpdate(string connectionString, string bic, int parziroid, out string sqlText)
        {
            try
            {
                string spName = "spCkntVirmaniDodajIBAN";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 1 },
                    { "@BIC", bic },
                    { "@ParZiroId", parziroid },
                };

                return Carpo.Data.Common.ExecuteScalar(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetPartneri(string connectionString, out string sqlText)
        {
            try
            {
                string spName = "spPartneriQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", 314 },
                    { "@Naziv", "" },
                    { "@ID", DBNull.Value },
                    { "@IDPartneri", DBNull.Value },
                    { "@DownUnder", DBNull.Value },
                    { "@SortKey", DBNull.Value },
                    { "@Sifra", DBNull.Value },
                    { "@MatBroj", DBNull.Value }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetBicData(string connectionString, int partneriId, int parziroid, out string sqlText)
        {
            try
            {
                string spName = "spCkntVirmaniParZiroQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@PartneriId", partneriId },
                    { "@VirmaniId", DBNull.Value },
                    { "@ParZiroId", parziroid }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }
        public static DataSet GetDataPDV(string connectionString, int action, int id, int operateriid, out string sqlText)
        {
            try
            {
                string spName = "spCKntVirmaniPDVQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", id },
                    { "@OperateriId", operateriid }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }
        public static object GetEnabledPDV(string connectionString, int action, int id, out string sqlText)
        {
            try
            {
                string spName = "spCKntVirmaniPDVQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", id }
                };

                return Carpo.Data.Common.ExecuteScalar(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);                
            }
            catch
            {
                throw;
            }
        }
        public static void SpremiVirmanPDV(string connectionString, int action, int id, decimal IznosRacuna, decimal Osnovica0, decimal ProlazneStavke,
            decimal NePod, decimal Osnovica25, decimal Odbiv25, decimal NeOdbiv25, decimal Osnovica13,decimal Odbiv13, decimal NeOdbiv13, decimal Osnovica5,
            decimal Odbiv5, decimal NeOdbiv5, int operateriid, out string sqlText, int Obavezno)
        {
            try
            {
                string spName = "spCKntVirmaniPDVAzur";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@ID", id },
                    { "@IznosRacuna", IznosRacuna },
                    { "@Osnovica0", Osnovica0 },
                    { "@ProlazneStavke", ProlazneStavke },
                    { "@NePod", NePod },
                    { "@Osnovica25", Osnovica25 },
                    { "@Odbiv25", Odbiv25 },
                    { "@NeOdbiv25", NeOdbiv25 },
                    { "@Osnovica13", Osnovica13 },
                    { "@Odbiv13", Odbiv13 },
                    { "@NeOdbiv13", NeOdbiv13 },
                    { "@Osnovica5", Osnovica5 },
                    { "@Odbiv5", Odbiv5 },
                    { "@NeOdbiv5", NeOdbiv5 },
                    { "@OperateriId", operateriid },
                    { "@ObvOstvarenjePlana", Obavezno}
                };

                Carpo.Data.Common.ExecuteNonQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }
        public static DataSet PostimajKontrolePDV(string connectionString, string action, decimal IznosRacuna, decimal Osnovica0, decimal ProlazneStavke,
            decimal NePod, decimal Osnovica25, decimal Odbiv25, decimal NeOdbiv25, decimal Osnovica13, decimal Odbiv13, decimal NeOdbiv13, decimal Osnovica5,
            decimal Odbiv5, decimal NeOdbiv5, int? pdv37id, out string sqlText)
        {
            try
            {
                string spName = "spCKntKnjigaUraPostimajKontrole";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", action },
                    { "@iznosspdv", IznosRacuna },
                    { "@neoporezivo", Osnovica0 },
                    { "@prolaznestavke", ProlazneStavke },
                    { "@nepod", NePod },
                    { "@osnovica25", Osnovica25 },
                    { "@odbiv25", Odbiv25 },
                    { "@neodbiv25", NeOdbiv25 },
                    { "@osnovica13", Osnovica13 },
                    { "@odbiv13", Odbiv13 },
                    { "@neodbiv13", NeOdbiv13 },
                    { "@osnovica5", Osnovica5 },
                    { "@odbiv5", Odbiv5 },
                    { "@neodbiv5", NeOdbiv5 },
                    { "@EUSlucaj", 0 },
                    { "@pdv37id", pdv37id }

                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }
    }
}