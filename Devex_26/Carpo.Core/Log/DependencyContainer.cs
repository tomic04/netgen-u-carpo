using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Core.Log
{
    public static class DependencyContainer
    {
        static Dictionary<Type, object> services = new Dictionary<Type, object>();
        public static void Add(object obj)
        {
            services[obj.GetType()] = obj;
        }
        public static T Get<T>()where T: class
        {
            object nadjen;
            if (!services.TryGetValue(typeof(T), out nadjen))
                return null;
            return (T)nadjen;
        }
    }
}
