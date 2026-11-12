using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carpo.Designer.Logic
{
    internal class DsAttribute
    {
        public static IEnumerable<Models.Attribute> iEAttr { get; set; }

        public static IEnumerable<Models.Attribute> GetAttr()
        {
            if (iEAttr == null)
            {
                DataTable dtAttr = GetDTAttributes();
                iEAttr = dtAttr.AsEnumerable().Select(x => Helpers.ModelMapper.MapDataRowToAttribute(x)).OrderBy(x => x.AttrName);
            }

            return iEAttr;
        }

        public static List<Models.Attribute> GetAttrByElement(string element)
        {
            return GetAttr().Where(x => x.ElementName == element).ToList();
        }

        public static object GetAttrDescription(string element, string attribute)
        {
            var attr = GetAttr().Where(x => x.ElementName == element && x.AttrName == attribute).FirstOrDefault();
            if (attr != null)
                return attr.AttrDescription;

            return null;
        }

        private static DataTable GetDTAttributes()
        {
            return Data.Attribute.GetAttributes();
        }
    }
}