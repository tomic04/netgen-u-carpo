using System;
using System.Globalization;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Isti ugovor kao ILCalc.CalcContext(Of Double) u Form2.EvaluateSTR 8286–8306:
    /// izraz u double, greška vraća prazan string. NetGen zatim zarez mijenja točkom.
    /// </summary>
    public static class Izraz
    {
        public static string Izracunaj(string izraz)
        {
            try
            {
                int i = 0;
                double v = Zbroj(izraz ?? "", ref i);
                if (i < (izraz ?? "").Length && !SamoRazmak(izraz, i))
                    return "";
                if (double.IsNaN(v) || double.IsInfinity(v))
                    return "";
                return v.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                return "";
            }
        }

        private static bool SamoRazmak(string s, int i)
        {
            for (int k = i; k < s.Length; k++)
            {
                if (!char.IsWhiteSpace(s[k]))
                    return false;
            }
            return true;
        }

        private static double Zbroj(string s, ref int i)
        {
            double v = Umnozak(s, ref i);
            while (true)
            {
                Preskoci(s, ref i);
                if (i >= s.Length)
                    return v;
                char c = s[i];
                if (c != '+' && c != '-')
                    return v;
                i++;
                double d = Umnozak(s, ref i);
                v = c == '+' ? v + d : v - d;
            }
        }

        private static double Umnozak(string s, ref int i)
        {
            double v = Potencija(s, ref i);
            while (true)
            {
                Preskoci(s, ref i);
                if (i >= s.Length)
                    return v;
                char c = s[i];
                if (c != '*' && c != '/' && c != '%')
                    return v;
                i++;
                double d = Potencija(s, ref i);
                if (c == '*')
                    v = v * d;
                else if (c == '/')
                    v = v / d;
                else
                    v = v % d;
            }
        }

        private static double Potencija(string s, ref int i)
        {
            double v = Unarni(s, ref i);
            Preskoci(s, ref i);
            if (i < s.Length && s[i] == '^')
            {
                i++;
                double d = Potencija(s, ref i);
                v = Math.Pow(v, d);
            }
            return v;
        }

        private static double Unarni(string s, ref int i)
        {
            Preskoci(s, ref i);
            if (i < s.Length && s[i] == '-')
            {
                i++;
                return -Unarni(s, ref i);
            }
            if (i < s.Length && s[i] == '+')
            {
                i++;
                return Unarni(s, ref i);
            }
            return Clan(s, ref i);
        }

        private static double Clan(string s, ref int i)
        {
            Preskoci(s, ref i);
            if (i >= s.Length)
                throw new FormatException();
            if (s[i] == '(')
            {
                i++;
                double v = Zbroj(s, ref i);
                Preskoci(s, ref i);
                if (i >= s.Length || s[i] != ')')
                    throw new FormatException();
                i++;
                return v;
            }
            if (char.IsLetter(s[i]))
            {
                int a = i;
                while (i < s.Length && char.IsLetter(s[i]))
                    i++;
                string ime = s.Substring(a, i - a);
                Preskoci(s, ref i);
                if (i < s.Length && s[i] == '(')
                {
                    i++;
                    double x = Zbroj(s, ref i);
                    double y = x;
                    bool dva = false;
                    Preskoci(s, ref i);
                    if (i < s.Length && s[i] == ',')
                    {
                        i++;
                        y = Zbroj(s, ref i);
                        dva = true;
                    }
                    Preskoci(s, ref i);
                    if (i >= s.Length || s[i] != ')')
                        throw new FormatException();
                    i++;
                    return Funkcija(ime, x, y, dva);
                }
                if (ime == "pi")
                    return Math.PI;
                if (ime == "e")
                    return Math.E;
                throw new FormatException();
            }
            return Broj(s, ref i);
        }

        private static double Funkcija(string ime, double x, double y, bool dva)
        {
            if (ime == "abs")
                return Math.Abs(x);
            if (ime == "sqrt")
                return Math.Sqrt(x);
            if (ime == "sin")
                return Math.Sin(x);
            if (ime == "cos")
                return Math.Cos(x);
            if (ime == "tan")
                return Math.Tan(x);
            if (ime == "log")
                return Math.Log(x);
            if (ime == "exp")
                return Math.Exp(x);
            if (ime == "min" && dva)
                return Math.Min(x, y);
            if (ime == "max" && dva)
                return Math.Max(x, y);
            throw new FormatException();
        }

        private static double Broj(string s, ref int i)
        {
            int a = i;
            while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.'))
                i++;
            if (a == i)
                throw new FormatException();
            string t = s.Substring(a, i - a);
            return double.Parse(t, CultureInfo.InvariantCulture);
        }

        private static void Preskoci(string s, ref int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;
        }
    }
}
