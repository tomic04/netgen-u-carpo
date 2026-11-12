using System.Collections.Generic;

namespace Carpo.Designer.Helpers
{
    internal class ModelDataTransfer
    {
        public static void CopyElementModel(List<Models.Element> listTo, List<Models.Element> listFrom)
        {
            if (listTo.Count > 0)
                listTo.Clear();

            foreach (Models.Element el in listFrom)
            {
                listTo.Add(el);
            }
        }

        public static void CopyElementCheckModel(List<Models.ElementCheck> listTo, List<Models.ElementCheck> listFrom)
        {
            if (listTo.Count > 0)
                listTo.Clear();

            foreach (Models.ElementCheck el in listFrom)
            {
                listTo.Add(el);
            }
        }
    }
}