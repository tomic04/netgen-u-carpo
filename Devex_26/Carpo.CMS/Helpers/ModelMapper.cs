using Carpo.Common.Helpers;
using System;
using System.Data;
using System.Diagnostics;

namespace Carpo.CMS.Helpers
{
    internal class ModelMapper
    {
        public static Models.Attribute MapDataRowToAttribute(DataRow row)
        {
            if (row == null)
                return null;

            try
            {
                return new Models.Attribute
                {
                    AttrDescription = row["AttrDescription"].CoalesceStr(),
                    AttrName = row["AttrName"].ToString(),
                    AttrType = row["AttrType"].ToString(),
                    AttrValue = row["AttrValue"].CoalesceStr(),
                    CarpoDsAttributeId = Convert.ToInt32(row["CarpoDsAttributeId"]),
                    CarpoDsAttributeTypeId = Convert.ToInt32(row["CarpoDsAttributeTypeId"]),
                    CarpoDsElementId = Convert.ToInt32(row["CarpoDsElementId"]),
                    ElementName = row["ElementName"].ToString()
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                return null;
            }
        }

        public static Models.ElementCheck MapDataRowToElements(DataRow row)
        {
            if (row == null)
                return null;

            try
            {
                return new Models.ElementCheck
                {
                    Name = row["Name"].CoalesceStr(),
                    ParentName = row["ParentName"].CoalesceStr()
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                return null;
            }
        }
    }
}