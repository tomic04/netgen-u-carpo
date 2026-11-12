using System.Collections.Generic;

namespace Carpo.Specific.Grafko.Models
{
    public class DiagramGeneration
    {
        public DiagramGeneration(int generation)
        {
            this.Generation = generation;
        }

        public int VerticalStartPosition { get; set; }

        public int HorizontalStartPosition { get; set; }

        public int GenerationWidth { get; set; }

        public int GenerationHeight { get; set; }

        public int NumberOfNodesInGeneration { get; set; }

        public int Generation { get; set; }

        public List<Node> NodesInGeneration { get; set; }
    }
}