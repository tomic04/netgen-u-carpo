// Ispis C# DijalogTok.Koraci vrste za svaki segment (mono). Ulaz: base64 segment po redu.
using System; using System.Text; using Carpo.NetGen.Komande;
internal static class LanacVoz
{
    private static int Main()
    {
        string red; var sb = new StringBuilder();
        while ((red = Console.In.ReadLine()) != null)
        {
            string s = Encoding.UTF8.GetString(Convert.FromBase64String(red));
            string v;
            try
            {
                var k = DijalogTok.Koraci(s);
                v = k.Count == 0 ? "prazno" : k[0].Vrsta;
            }
            catch (Exception ex) { v = "!" + ex.GetType().Name; }
            sb.Append(v).Append('\n');
        }
        Console.Out.Write(sb.ToString());
        return 0;
    }
}
