using System.Collections.Generic;

namespace CannedBytes.Tools.DgmlBuilder
{
    public class DgmlBuilder
    {
        public DgmlBuilder(string title)
        {
            New(title);
        }

        public DgmlBuilder(DirectedGraph graph)
        {
            DirectedGraph = graph;
            EnsureValid(DirectedGraph);
        }

        public DirectedGraph DirectedGraph { get; protected set; }

        public void New(string title)
        {
            DirectedGraph = new DirectedGraph()
            {
                Title = title
            };

            EnsureValid(DirectedGraph);
        }

        protected void EnsureValid(DirectedGraph graph)
        {
            if (graph.Categories == null) graph.Categories = new DirectedGraphCategory[0];
            if (graph.IdentifierAliases == null) graph.IdentifierAliases = new DirectedGraphAlias[0];
            if (graph.Nodes == null) graph.Nodes = new DirectedGraphNode[0];
            if (graph.Paths == null) graph.Paths = new DirectedGraphPath[0];
            if (graph.Properties == null) graph.Properties = new DirectedGraphProperty[0];
            if (graph.QualifiedNames == null) graph.QualifiedNames = new DirectedGraphName[0];
            if (graph.Styles == null) graph.Styles = new DirectedGraphStyle[0];
        }

        public DirectedGraphNode AddNode(string id)
        {
            DirectedGraphNode node = new()
            {
                Id = id
            };

            AddNode(node);

            return node;
        }

        public void AddNode(DirectedGraphNode node)
        {
            List<DirectedGraphNode> nodes = new(DirectedGraph.Nodes);

            nodes.Add(node);

            DirectedGraph.Nodes = nodes.ToArray();
        }

        public DirectedGraphLink AddLink(DirectedGraphNode source, DirectedGraphNode target)
        {
            return AddLink(source.Id, target.Id);
        }

        public DirectedGraphLink AddLink(string source, string target)
        {
            DirectedGraphLink link = new()
            {
                Source = source,
                Target = target,
            };

            AddLink(link);

            return link;
        }

        public void AddLink(DirectedGraphLink link)
        {
            List<DirectedGraphLink> links = new(DirectedGraph.Links);

            links.Add(link);

            DirectedGraph.Links = links.ToArray();
        }

        public DirectedGraphCategory AddCategory(string id)
        {
            DirectedGraphCategory cat = new()
            {
                Id = id
            };

            AddCategory(cat);

            return cat;
        }

        public void AddCategory(DirectedGraphCategory category)
        {
            List<DirectedGraphCategory> cats = new(DirectedGraph.Categories);

            cats.Add(category);

            DirectedGraph.Categories = cats.ToArray();
        }
    }
}
