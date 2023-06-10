using System;
using System.Collections.Generic;

namespace CannedBytes.Tools.DgmlBuilder;

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

    protected static void EnsureValid(DirectedGraph graph)
    {
        graph.Categories ??= Array.Empty<DirectedGraphCategory>();
        graph.IdentifierAliases ??= Array.Empty<DirectedGraphAlias>();
        graph.Nodes ??= Array.Empty<DirectedGraphNode>();
        graph.Paths ??= Array.Empty<DirectedGraphPath>();
        graph.Properties ??= Array.Empty<DirectedGraphProperty>();
        graph.QualifiedNames ??= Array.Empty<DirectedGraphName>();
        graph.Styles ??= Array.Empty<DirectedGraphStyle>();
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
        List<DirectedGraphNode> nodes = new(DirectedGraph.Nodes)
        {
            node
        };

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
        List<DirectedGraphLink> links = new(DirectedGraph.Links)
        {
            link
        };

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
        List<DirectedGraphCategory> cats = new(DirectedGraph.Categories)
        {
            category
        };

        DirectedGraph.Categories = cats.ToArray();
    }
}
