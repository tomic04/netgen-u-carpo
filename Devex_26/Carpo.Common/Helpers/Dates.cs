using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Carpo.Common.Helpers
{
    public class Dates
    {
        public enum Predef { jucer, danas, sutra, ovajTjedan }

        public static List<Structure.Models.Dates.TreeMonths> GetMonthsBetweenDates(DateTime dateStart, DateTime dateEnd, string[] predefs)
        {
            Dictionary<int, string> dictMonths = GetMonthNames();

            List<Structure.Models.Dates.TreeMonths> list = new List<Structure.Models.Dates.TreeMonths>();

            Tuple<DateTime, DateTime> paramDates;

            if (predefs != null)
            {
                if (predefs.Contains(Predef.jucer.ToString()))
                {
                    paramDates = CalculateParameters(DateTime.Now.AddDays(-1).Date, Predef.jucer.ToString());
                    list.Add(new Structure.Models.Dates.TreeMonths { Id = Predef.jucer.ToString(), Year = DateTime.Now.Year, Month = DateTime.Now.Month, Name = "Jučer", ParentId = null, DatumOd = paramDates.Item1, DatumDo = paramDates.Item2 });
                }
                if (predefs.Contains(Predef.danas.ToString()))
                {
                    paramDates = CalculateParameters(DateTime.Now.Date, Predef.danas.ToString());
                    list.Add(new Structure.Models.Dates.TreeMonths { Id = Predef.danas.ToString(), Year = DateTime.Now.Year, Month = DateTime.Now.Month, Name = "Danas", ParentId = null, DatumOd = paramDates.Item1, DatumDo = paramDates.Item2 });
                }
                if (predefs.Contains(Predef.sutra.ToString()))
                {
                    paramDates = CalculateParameters(DateTime.Now.AddDays(1).Date, Predef.sutra.ToString());
                    list.Add(new Structure.Models.Dates.TreeMonths { Id = Predef.sutra.ToString(), Year = DateTime.Now.Year, Month = DateTime.Now.Month, Name = "Sutra", ParentId = null, DatumOd = paramDates.Item1, DatumDo = paramDates.Item2 });
                }
                if (predefs.Contains(Predef.ovajTjedan.ToString()))
                {
                    paramDates = CalculateParameters(DateTime.Now.Date, Predef.ovajTjedan.ToString());
                    list.Add(new Structure.Models.Dates.TreeMonths { Id = Predef.ovajTjedan.ToString(), Year = DateTime.Now.Year, Month = DateTime.Now.Month, Name = "Ovaj tjedan", ParentId = null, DatumOd = paramDates.Item1, DatumDo = paramDates.Item2 });
                }
            }

            for (DateTime dt = dateStart; dt <= dateEnd; dt = dt.AddMonths(1))
            {
                if (dt.Month == 1)
                {
                    paramDates = CalculateParameters(dt.Year, null);
                    list.Add(new Structure.Models.Dates.TreeMonths { Id = dt.Year.ToString(), Year = dt.Year, Month = dt.Month, Name = dt.Year.ToString(), ParentId = null, DatumOd = paramDates.Item1, DatumDo = paramDates.Item2 });
                }

                paramDates = CalculateParameters(dt.Year, dt.Month);
                list.Add(new Structure.Models.Dates.TreeMonths { Id = dt.Year + dt.Month.ToString("00"), Year = dt.Year, Month = dt.Month, Name = dictMonths[dt.Month], ParentId = dt.Year.ToString(), DatumOd = paramDates.Item1, DatumDo = paramDates.Item2 });
            }
            return list;
        }

        public static List<Structure.Models.Dates.TreeMonths> GetMonthsBetweenDates(DateTime dateStart, DateTime dateEnd)
        {
            return GetMonthsBetweenDates(dateStart, dateEnd, null);
        }

        public static Dictionary<int, string> GetMonthNames()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            for (int i = 1; i <= 12; i++)
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                dict.Add(i, monthName);
            }
            return dict;
        }

        private static Tuple<DateTime, DateTime> CalculateParameters(int year, int? month)
        {
            DateTime dat1, dat2;
            if (month == null)
            {
                dat1 = new DateTime(year, 1, 1);
                dat2 = new DateTime(year, 12, 31);
            }
            else
            {
                int monthInt = Convert.ToInt32(month);
                dat1 = new DateTime(year, monthInt, 1);
                dat2 = new DateTime(year, monthInt, DateTime.DaysInMonth(year, monthInt));
            }
            return Tuple.Create(dat1, dat2);
        }

        private static Tuple<DateTime, DateTime> CalculateParameters(DateTime datum, string predef)
        {
            DateTime dat1, dat2;

            if (predef == Predef.ovajTjedan.ToString())
            {
                int offset = datum.DayOfWeek - DayOfWeek.Monday; //ponedjeljak
                dat1 = datum.AddDays(-offset);
                dat2 = dat1.AddDays(6);
            }
            else if (predef == Predef.jucer.ToString()) //jučer
            {
                dat1 = dat2 = DateTime.Now.AddDays(-1).Date;
            }
            else if (predef == Predef.sutra.ToString())//sutra
            {
                dat1 = dat2 = DateTime.Now.AddDays(1).Date;
            }
            else //danas
            {
                dat1 = dat2 = DateTime.Now.Date;
            }
            return Tuple.Create(dat1, dat2);
        }
    }
}