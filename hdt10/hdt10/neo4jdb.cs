using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;

namespace hdt10
{
    public class neo4jdb : IDisposable
    {
        public static neo4jdb instance = new neo4jdb("bolt://localhost:7687", "neo4j", "pass123");
        private readonly IDriver driver;

        private neo4jdb(string uri, string user, string password)
        {
            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
