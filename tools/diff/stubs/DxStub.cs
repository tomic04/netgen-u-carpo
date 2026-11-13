// Minimalni nadomjestak DevExpress tipova da se Carpo.NetGen komande kompajliraju pod Mono
// za diferencijalni test. Ne koristi se u buildu.
namespace DevExpress.XtraPrinting
{
    public class PrintingSystem { }
    public class PageHeaderFooter { public object Content; public PageHeaderFooter() { } public PageHeaderFooter(object a, object b) { } }
    public class PageHeaderArea { }
    public class PrintableComponentLink
    {
        public PrintableComponentLink(PrintingSystem ps) { }
        public object PageHeaderFooter;
        public object Component;
        public bool Landscape;
        public void CreateDocument() { }
    }
}
