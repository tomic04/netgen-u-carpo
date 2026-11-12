using System.Collections.Generic;
using System.Linq;

namespace Carpo.Common.Helpers
{
    internal class Collections
    {
        public static void DictionaryUpdate(Dictionary<string, object> dic, object value, object newValue)
        {
            object val;
            var key = dic.FirstOrDefault(x => x.Value.ToString().ToLower() == value.ToString().ToLower()).Key;
            if (key != null)
                if (dic.TryGetValue(key, out val))
                {
                    dic[key] = newValue;
                }
        }

        public static void DictionaryRemove(Dictionary<string, object> dic)
        {
            foreach (var i in dic.Where(d => (d.Value.ToString().Substring(0, 1) == "#")).ToList())
            {
                dic.Remove(i.Key);
            }
        }
    }
}