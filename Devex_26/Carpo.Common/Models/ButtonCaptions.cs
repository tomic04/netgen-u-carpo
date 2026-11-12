namespace Carpo.Common.Models
{
    public static class ButtonCaptions
    {
        public static string yesCaption { get; set; } = Translator.TranslateString("Da");
        public static string noCaption { get; set; } = Translator.TranslateString("Ne");

        public static void SetValuesToDefault()
        {
            yesCaption = Translator.TranslateString("Da");
            noCaption = Translator.TranslateString("Ne");
        }
    }
}