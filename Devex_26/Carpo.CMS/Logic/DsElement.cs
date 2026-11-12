using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carpo.CMS.Logic
{
    internal class DsElement
    {
        public static IEnumerable<Models.ElementCheck> iEEl { get; set; }

        public static IEnumerable<Models.ElementCheck> GetElements()
        {
            if (iEEl == null)
            {
                DataTable dtEl = GetDTElements();
                iEEl = dtEl.AsEnumerable().Select(x => Helpers.ModelMapper.MapDataRowToElements(x)).OrderBy(x => x.Name);
            }
            return iEEl;
        }

        private static DataTable GetDTElements()
        {
            return Data.Element.GetElements();
        }

        public static List<Models.ElementCheck> GetElementByName(string element)
        {
            return GetElements().Where(x => x.Name == element).ToList();
        }

        // Child
        public static IEnumerable<Models.ElementCheck> GetChildElements()
        {
            if (iEEl == null)
            {
                DataTable dtEl = GetDTLnElements();
                iEEl = dtEl.AsEnumerable().Select(x => Helpers.ModelMapper.MapDataRowToElements(x)).OrderBy(x => x.Name);
            }
            return iEEl;
        }

        private static DataTable GetDTLnElements()
        {
            return Data.Element.GetLnElements();
        }

        public static List<Models.ElementCheck> GetChildElements(IEnumerable<Models.ElementCheck> parent, string parentName)
        {
            return parent.Where(x => x.ParentName == parentName).ToList();
        }
    }
}