using Carpo.Common.Helpers;

namespace Carpo.Common
{
    public class Translator
    {
        public static bool isInitialized;

        public static TranslatorData translator;

        public static void Init(string _server, bool useTranslateLog)
        {
            translator = new TranslatorData(_server, false);
            JupiterTranslator.Translator.Init("jupitertranslations", _server);
            JupiterTranslator.Translator.LoadTranslations(translator.GetDefaultLanguageForLogin());
        }

        // TO DO - keepLength bi možda trebao biti = false
        public static string TranslateString(string str, bool keepLength = false)
        {
            return JupiterTranslator.Translator.TranslateString(str, GlobalName.Name);
        }

    }
}