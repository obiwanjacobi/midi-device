using System.Collections.Generic;
using System.IO;
using CannedBytes.Tools.DgmlBuilder;

namespace CannedBytes.Midi.Device.UnitTests;

public static class DgmlFactory
{
    public static void SaveGraph(SchemaNodeMap map, string targetPath)
    { 
        SaveGraph(map.RootNode, targetPath);
    }

    public static void SaveGraph(SchemaNode node, string targetPath)
    {
        var graph = CreateGraph(node);

        string fileName = targetPath + ".dgml";

        using FileStream stream = File.Create(fileName);
        DgmlSerializer.Serialize(stream, graph);
    }

    public static DirectedGraph CreateGraph(SchemaNode node)
    {
        string name = node.Field.Schema.Name.FullName;

        DirectedGraph graph = new()
        {
            Title = name,
            Categories = CreateCategories(),
            Nodes = CreateNodes(node),
            Links = CreateLinks(node),
        };

        return graph;
    }

    private static DirectedGraphLink[] CreateLinks(SchemaNode schemaNode)
    {
        List<DirectedGraphLink> links = new();

        foreach (var n in schemaNode.SelectNodes(n => n.Next))
        {
            if (n.Next is not null)
            {
                links.Add(new DirectedGraphLink()
                {
                    Source = BuildId(n),
                    Target = BuildId(n.Next),
                    Category1 = "Next",
                    Label = "Next",
                });
            }

            if (n.NextClone is not null)
            {
                links.Add(new DirectedGraphLink()
                {
                    Source = BuildId(n),
                    Target = BuildId(n.NextClone),
                    Category1 = "NextClone",
                    Label = "NextClone"
                });
            }

            if (n.NextSibling is not null)
            {
                links.Add(new DirectedGraphLink()
                {
                    Source = BuildId(n),
                    Target = BuildId(n.NextSibling),
                    Category1 = "NextSibling",
                    Label = "NextSibling"
                });
            }

            if (n.Parent is not null)
            {
                links.Add(new DirectedGraphLink()
                {
                    Source = BuildId(n),
                    Target = BuildId(n.Parent),
                    Category1 = "Parent",
                    Label = "Parent"
                });
            }
        }

        return links.ToArray();
    }

    // Fails when different nodes have the same name.
    private static string BuildId(SchemaNode schemaNode)
    {
        if (schemaNode.Field is not null)
        {
            return schemaNode.Field.Name.Name + " " + schemaNode.Key.ToString();
        }

        return schemaNode.FieldConverterPair.Converter.Name + " " + schemaNode.Key.ToString();
    }

    private static DirectedGraphNode[] CreateNodes(SchemaNode schemaNode)
    {
        List<DirectedGraphNode> nodes = new();

        CreateNode(nodes, schemaNode);
        CreateNodes(nodes, schemaNode.Children);

        return nodes.ToArray();
    }

    private static void CreateNodes(List<DirectedGraphNode> nodes, IEnumerable<SchemaNode> schemaNodes)
    {
        foreach (var schemaNode in schemaNodes)
        {
            CreateNode(nodes, schemaNode);

            CreateNodes(nodes, schemaNode.Children);
        }
    }

    private static void CreateNode(List<DirectedGraphNode> nodes, SchemaNode schemaNode)
    {
        nodes.Add(new DirectedGraphNode()
        {
            Id = BuildId(schemaNode),
            Description = schemaNode.ToString(),
            Category = CreateNodeCategories(schemaNode)
        });
    }

    private static DirectedGraphNodeCategory[] CreateNodeCategories(SchemaNode n)
    {
        List<DirectedGraphNodeCategory> cats = new();

        if (n.IsRecord)
        {
            cats.Add(new DirectedGraphNodeCategory() { Ref = "Record" });
        }
        if (n.IsClone)
        {
            cats.Add(new DirectedGraphNodeCategory() { Ref = "Clone" });
        }
        if (n.IsAddressMap)
        {
            cats.Add(new DirectedGraphNodeCategory() { Ref = "AddressMap" });
        }

        return cats.ToArray();
    }

    private static DirectedGraphCategory[] CreateCategories()
    {
        List<DirectedGraphCategory> list = new()
        {
            // Nodes
            new DirectedGraphCategory()
            {
                Id = "Record"
            },
            new DirectedGraphCategory()
            {
                Id = "Clone"
            },
            new DirectedGraphCategory()
            {
                Id = "AddressMap"
            },
            // Links
            new DirectedGraphCategory()
            {
                Id = "Parent"
            },
            new DirectedGraphCategory()
            {
                Id = "Child"
            },
            new DirectedGraphCategory()
            {
                Id = "Next"
            },
            new DirectedGraphCategory()
            {
                Id = "Previous"
            },
            new DirectedGraphCategory()
            {
                Id = "NextSibling"
            },
            new DirectedGraphCategory()
            {
                Id = "PreviousSibling"
            },
            new DirectedGraphCategory()
            {
                Id = "NextClone"
            },
            new DirectedGraphCategory()
            {
                Id = "PreviousClone"
            },
            new DirectedGraphCategory()
            {
                Id = "NextRecord"
            },
            new DirectedGraphCategory()
            {
                Id = "PreviousRecord"
            },
            new DirectedGraphCategory()
            {
                Id = "NextField"
            },
            new DirectedGraphCategory()
            {
                Id = "PreviousField"
            },
        };

        return list.ToArray();
    }
}
