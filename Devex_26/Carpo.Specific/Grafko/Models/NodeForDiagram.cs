using System.Collections.Generic;

namespace Carpo.Specific.Grafko.Models
{
    public class NodeForDiagram
    {
        public NodeForDiagram(string title, int xposition, int yposition, int width, int height, int id, int valueId, int vrstaDokId, string tablica, List<int> parents)
        {
            this.Title = title;
            this.xPosition = xposition;
            this.yPosition = yposition;
            this.Width = width;
            this.Height = height;
            this.Id = id;
            this.ValueId = valueId;
            this.VrstaDokId = vrstaDokId;
            this.Tablica = tablica;
            this.ParentIdsList = parents;
        }

        public string Title { get; set; }

        public int xPosition { get; set; }

        public int yPosition { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Id { get; set; }

        public int ValueId { get; set; }

        public int VrstaDokId { get; set; }

        public string Tablica { get; set; }

        public List<int> ParentIdsList { get; set; }
    }
}