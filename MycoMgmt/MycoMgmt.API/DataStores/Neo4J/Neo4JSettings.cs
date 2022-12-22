using System;

namespace MycoMgmt.DataStores.Neo4J;

public class Neo4JSettings
{
    public Uri? Neo4JConnection { get; set; }

    public string? Neo4JUser { get; set; }

    public string? Neo4JPassword { get; set; }

    public string? Neo4JDatabase { get; set; }
}