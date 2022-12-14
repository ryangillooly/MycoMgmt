using System;

namespace MycoMgmt.DataStores.Neo4J
{
    public class Neo4JSettings
    {
        public Uri Neo4jConnection { get; set; }

        public string Neo4jUser { get; set; }

        public string Neo4jPassword { get; set; }

        public string Neo4jDatabase { get; set; }
    }
}