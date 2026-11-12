namespace Carpo.Structure.Models
{
    public class MenuItemTreeStructure
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string DisplayField { get; set; }
        public int ClickInd { get; set; }
    }
}