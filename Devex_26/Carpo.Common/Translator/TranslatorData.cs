using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Carpo.Common
{
    public class TranslatorData
    {
        public const string croatianLanguage = "hrv";
        public string CroatianLanguage { get { return croatianLanguage; } }

        public string defaultLanguage;
        public string module = "Translator";

        private bool useTranslateLog = false;

        private string server;

        private Dictionary<string, string> prijevodi = new Dictionary<string, string>();

        public TranslatorData(string _server, bool UseTranslateLog) //string language, string connectionString)
        {
            try
            {
                server = _server;
                useTranslateLog = UseTranslateLog;

                defaultLanguage = GetDefaultLanguageForLogin().ToLower();
                if (defaultLanguage != croatianLanguage)
                {
                    PopulateDictionary();
                }
            }
            catch
            {
                throw;
            }
        }

        public void PopulateDictionary()
        {
            using (SqlConnection conn = new SqlConnection(CreateConnection("JupiterTranslations")))
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    return;
                }

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.spPrijevodiQuery", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter par;
                    par = cmd.Parameters.Add("@jezik", SqlDbType.Char, 3);
                    par.Value = defaultLanguage;

                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr != null)
                    {
                        while (rdr.Read())
                            prijevodi.Add(rdr.GetString(0).ToLower(), rdr.GetString(1));
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private string SetCase(string s, bool firstUpper, bool allUpper)
        {
            if (allUpper)
                return s.ToUpper();
            if (firstUpper)
                return s.Substring(0, 1).ToUpper() + s.Substring(1);
            return s;
        }

        public string TranslateString(String str)
        {
            if (defaultLanguage == croatianLanguage)
                return str;

            string hasAmpersend = "", hasColon = "";
            bool allUpper = false, firstUpper = false;

            if (str == "")
                return str;

            if (str.StartsWith("&"))
            {
                str = str.TrimStart(new char[] { '&' });
                hasAmpersend = "&";
            }

            if (str.EndsWith(":"))
            {
                str = str.TrimEnd(new char[] { ':' });
                hasColon = ":";
            }

            allUpper = str.ToUpper() == str;
            firstUpper = str.ToUpper() != str && str.Substring(0, 1) == str.Substring(0, 1).ToUpper();

            if (prijevodi.ContainsKey(str))
                return hasAmpersend + SetCase(prijevodi[str], firstUpper, allUpper) + hasColon;

            if (prijevodi.ContainsKey(str.ToLower()))
                return hasAmpersend + SetCase(prijevodi[str.ToLower()], firstUpper, allUpper) + hasColon;

            // word breaker je sve sto nisu slova i brojke
            string CHARS = "1234567890qwertyuiopasdfghjklzxcvbnmšđčćžQWERTZUIOPASDFGHJKLYXCVBNMŠĐČĆŽ";

            List<string> ls = new List<string>();

            char[] cs = new char[str.Length];
            int csc = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (!CHARS.Contains(str[i].ToString()))
                {
                    if (csc > 0)
                    {
                        ls.Add(new string(cs));
                        //cs.Initialize();
                        cs = new char[str.Length];
                        csc = 0;
                    }
                    ls.Add(str[i].ToString());
                }
                else
                {
                    cs[csc++] = str[i];
                }
            }
            if (csc > 0)
            {
                ls.Add(new string(cs));
            }

            // stringovi u viticastim zagradama se ne prevode
            string beginStaticText = "{", endStaticText = "}";
            bool inStaticString = false;
            StringBuilder sb = new StringBuilder();
            StringBuilder failed = new StringBuilder();
            foreach (string s2 in ls)
            {
                string s;
                if (s2 == " ")
                    s = s2;
                else
                    s = s2.Replace('\0', ' ').TrimEnd();
                if (inStaticString)
                {
                    if (s != endStaticText)
                        sb.Append(s);
                    else
                        inStaticString = false;
                    continue;
                }
                else
                {
                    if (s == beginStaticText)
                    {
                        inStaticString = true;
                        continue;
                    }
                }

                if (prijevodi.ContainsKey(s.ToLower()))
                {
                    string sp = prijevodi[s.ToLower()];
                    // sva velika slova
                    if (s == s.ToUpper())
                        sb.Append(sp.ToUpper());
                    else
                    {
                        // prvo veliko slovo
                        if (s.Substring(0, 1) == s.Substring(0, 1).ToUpper())
                            sb.Append(sp.Substring(0, 1).ToUpper() + sp.Substring(1));
                        else
                        {
                            sb.Append(sp);
                        }
                    }
                }
                else
                {
                    sb.Append(s);
                    if (s != " ")
                    {
                        failed.Append(s);
                        failed.Append(",");
                    }
                }
            }

            if (useTranslateLog == true && failed.ToString() != "")
            {
                LogTranslation(str, failed.ToString());
            }

            return hasAmpersend + sb.ToString() + hasColon;
        }

        public string GetDefaultLanguageForLogin()
        {
            string r = croatianLanguage;
            using (SqlConnection conn = new SqlConnection(CreateConnection("JupiterSystem")))
            {
                try
                {
                    conn.Open();
                }
                catch
                {
                    return r;
                }

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.spGetDefaultLanguageForLogin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    object ret = cmd.ExecuteScalar();

                    r = ret.ToString();
                }
                catch
                { }

                return r;
            }
        }

        private bool LogTranslation(string phrase, string words)
        {
            using (SqlConnection conn = new SqlConnection(CreateConnection("JupiterTranslations")))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    return false;
                }

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.spTranslationLog", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@phrase", phrase);
                    cmd.Parameters.AddWithValue("@words", words);
                    cmd.Parameters.AddWithValue("@module", module);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        private string CreateConnection(string database)
        {
            SqlConnectionStringBuilder cBuilder = new SqlConnectionStringBuilder();

            cBuilder.ApplicationName = "Translator";
            cBuilder.ConnectTimeout = 10;
            cBuilder.InitialCatalog = database;
            cBuilder.DataSource = server;
            cBuilder.IntegratedSecurity = true;

            return cBuilder.ToString();
        }
    }
}