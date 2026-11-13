using System.Collections.Generic;

namespace Carpo.NetGen.Log
{
    /// <summary>
    /// Isti ugovor kao Carpo.UI.Connector.Abstract.Starter.Run: Logger ide u
    /// DependencyContainer.services pod ključem typeof(Logger). Get bez Add baca.
    /// </summary>
    public static class Spremnik
    {
        public static bool RegistracijaKaoCarpo()
        {
            bool bacio = false;
            try
            {
                Carpo.Core.Log.DependencyContainer.Get<Carpo.Core.Logger>();
            }
            catch (KeyNotFoundException)
            {
                bacio = true;
            }
            if (!bacio)
                return false;
            var log = new Carpo.Core.Logger(Carpo.Core.Log.LogLevel.Debug);
            Carpo.Core.Log.DependencyContainer.Add(log);
            return object.ReferenceEquals(Carpo.Core.Log.DependencyContainer.Get<Carpo.Core.Logger>(), log);
        }
    }
}
