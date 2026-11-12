using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.UI.Extensions
{
    internal static class ArrayExtensions
    {
        public static bool IsEqual<T>(this T[] array, T[] arrayB) where T : unmanaged
        {
            if (array.Length != arrayB.Length) return false;
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].Equals(arrayB[i])) return false;
            }
            return true;
        }
    }
}
