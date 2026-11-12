using DevExpress.Utils.Extensions;
using System;
using System.Collections.Generic;

namespace Carpo.Specific.Grafko.Models
{
    public class Node
    {
        public Node(int id, int valueId, int vrstaDokId, string tablica, int parentId, string childIds, string parentIds, string text)
        {
            this.Id = id;
            this.ValueId = valueId;
            this.VrstaDokId = vrstaDokId;
            this.Tablica = tablica;
            this.ParentId = parentId;
            this.ChildIds = childIds;
            this.ParentIds = parentIds;
            ChildIdsList = new List<int>();
            ParentIdsList = new List<int>();
            if (!string.IsNullOrWhiteSpace(childIds))
            {
                childIds.Split(',').ForEach(ci => ChildIdsList.Add(Int32.Parse(ci)));
            }
            if (!string.IsNullOrWhiteSpace(parentIds) && parentIds != "0")
            {
                parentIds.Split(',').ForEach(ci => ParentIdsList.Add(Int32.Parse(ci)));
            }
            this.Text = text;
        }

        public int Id { get; set; }

        public int ValueId { get; set; }

        public int VrstaDokId { get; set; }

        public string Tablica { get; set; }

        public int ParentId { get; set; }

        public string ChildIds { get; set; }

        public string ParentIds { get; set; }

        public List<int> ChildIdsList { get; set; }

        public List<int> ParentIdsList { get; set; }

        public int Generation { get; set; } = -1;

        public string Text { get; set; }
    }
}