namespace Carpo.NetGen.Log
{
    /// <summary>
    /// DependencyContainer.services ključ je typeof(Logger).
    /// Get je bacao KeyNotFoundException na indeksu. NetGen nema taj spremnik.
    /// </summary>
    public static class Spremnik
    {
        public static bool LoggerNijeObavezan()
        {
            return Carpo.Core.Log.DependencyContainer.Get<Carpo.Core.Logger>() == null;
        }
    }
}
