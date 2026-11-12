namespace Carpo.Designer.Models
{
    internal class Attribute
    {
        public int CarpoDsAttributeId { get; set; }
        public string AttrName { get; set; }
        public string AttrDescription { get; set; }
        public int CarpoDsAttributeTypeId { get; set; }
        public string AttrType { get; set; }
        public string AttrValue { get; set; }
        public int CarpoDsElementId { get; set; }
        public string ElementName { get; set; }
    }
}