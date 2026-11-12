namespace Carpo.UI.Logic
{
    public class ParserDirect
    {
        public static object GetValue(Forms.Base.JupiterShared form, string value)
        {
            using (Parser parser = new Parser(form, null))
            {
                return parser.GetLogicPartValue(value, null);
            }
        }
    }
}