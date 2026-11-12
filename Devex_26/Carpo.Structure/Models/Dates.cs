using System;

namespace Carpo.Structure.Models.Dates
{
    public class TreeMonths
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Name { get; set; }
        public string ValueId { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
    }

    public class MonthNames
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}