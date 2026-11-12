using Carpo.Data;
using Carpo.Specific.Grafko.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carpo.Specific.Grafko
{
    public class Graf
    {
        private static List<Node> nodesList;
        private static List<DiagramGeneration> diagramGenerations;
        private static List<NodeForDiagram> nodesForDiagram;

        private static int gen = -1;

        private static int screenWidth = 860;
        private static int screenHeight = 649;

        private static int boxHeight = 50;
        private static int boxWidth = 120;

        private static int GlaId;
        private static string NazivTablice;
        private static int OperateriId;
        private static string Action;

        public static List<NodeForDiagram> CrtajGraf(int glaId, string nazivTablice, int operateriId, string action)
        {
            if (glaId == 0 || nazivTablice == "" || operateriId == 0) return null;

            GlaId = glaId;
            NazivTablice = nazivTablice;
            OperateriId = operateriId;
            Action = action;

            try
            {
                getNodes();
                setGenerations();
                setDiagramGenerations();
                setDiagramGenerationsPositions();
                generateNodesForDiagram();
            }
            catch (Exception err)
            {
                //Carpo.Common.LogWriter.LogLine("Grafko error" + err.Message);
                return null;
            }

            return nodesForDiagram == null ? new List<NodeForDiagram>() : nodesForDiagram;
        }

        private static void generateNodesForDiagram()
        {
            if (gen == -1) return;

            nodesForDiagram = new List<NodeForDiagram>();
            foreach (DiagramGeneration generation in diagramGenerations)
            {
                int i = 0;
                foreach (Node node in generation.NodesInGeneration)
                {
                    nodesForDiagram.Add(new NodeForDiagram(node.Text, generation.HorizontalStartPosition + ((generation.GenerationWidth - boxWidth) / 2), (((generation.GenerationHeight - boxHeight) / 2) + (generation.GenerationHeight * i++)), boxWidth, boxHeight, node.Id, node.ValueId, node.VrstaDokId, node.Tablica, node.ParentIdsList));
                }
            }
        }

        private static void setDiagramGenerationsPositions()
        {
            if (gen == -1) return;

            for (int i = 0; i <= gen; i++)
            {
                diagramGenerations.ElementAt(i).GenerationWidth = (screenWidth / (gen + 1)) - 5;
                diagramGenerations.ElementAt(i).GenerationHeight = (screenHeight / diagramGenerations.ElementAt(i).NumberOfNodesInGeneration) - 5;
                diagramGenerations.ElementAt(i).HorizontalStartPosition = i * diagramGenerations.ElementAt(i).GenerationWidth;
            }
        }

        private static void setDiagramGenerations()
        {
            if (gen == -1) return;

            diagramGenerations = new List<DiagramGeneration>();
            for (int i = 0; i <= gen; i++)
            {
                DiagramGeneration diagramGeneration = new DiagramGeneration(i);
                diagramGeneration.NodesInGeneration = nodesList.FindAll(n => n.Generation == i);
                diagramGeneration.NumberOfNodesInGeneration = diagramGeneration.NodesInGeneration.Count();
                diagramGenerations.Add(diagramGeneration);
            }
        }

        private static void getNodes()
        {
            string sql = String.Format("spPovezaniDokumenti_Grid @GlaId={0}, @Tablica={1}, @OperateriId={2}, @Action={3}", GlaId, NazivTablice, OperateriId, Action == "" ? "''" : Action);
            DataTable response = Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
            nodesList = new List<Node>();
            if (!response.HasErrors)
            {
                foreach (DataRow item in response.Rows)
                {
                    nodesList.Add(new Node(item.Field<int>("Id"), item.Field<int>("ValueId"), item.Field<int>("VrstaDokId"), item.Field<string>("Tablica"), item.Field<int>("ParentId"), item.Field<string>("ChildIds"), item.Field<string>("ParentIds"), item.Field<string>("Naziv")));
                }
                //nodesList.Add(new Node(123, 123, 124, "", "1316", "123 TEST djete od 1316"));
                //nodesList.Add(new Node(124, 124, 124, "", "1317, 7956", "124 TEST djete od 1317, 7956"));
                //nodesList.Add(new Node(125, 125, 124, "", "7956", "125 TEST djete od 7956"));
                //nodesList.Add(new Node(126, 126, 124, "", "123", "126 TEST djete od 123"));
                //nodesList.Add(new Node(127, 127, 124, "", "15668, 123", "127 TEST djete od 15668, 123"));
                //nodesList.Add(new Node(128, 128, 124, "", "15668", "128 TEST djete od 15668"));
            }
        }

        private static void setGenerations()
        {
            if (nodesList.Count == 0) return;
            gen = 0;

            // first generation
            foreach (Node node in nodesList)
            {
                if (node.ParentIds == "0")
                    node.Generation = 0;
            }

            int nodesInThisGeneration = 0;
            // other generations
            for (; ; )
            {
                foreach (Node node in nodesList.FindAll(n => n.Generation == gen))
                {
                    foreach (Node innerLoopNode in nodesList)
                    {
                        if (innerLoopNode.ParentIdsList.Contains(node.Id))
                        {
                            innerLoopNode.Generation = gen + 1;
                            nodesInThisGeneration++;
                        }
                    }
                }
                if (nodesInThisGeneration == 0) return;
                gen++;
                nodesInThisGeneration = 0;
            }
        }
    }
}