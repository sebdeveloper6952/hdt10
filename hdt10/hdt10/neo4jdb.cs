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

        public void AddPatient(string nombre, string telefono)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (a:Patient) " +
                                        "SET a.nombre = $nombre " +
                                        "SET a.telefono = $telefono " +
                                        "RETURN a",
                        new { nombre, telefono });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        public void AddDoctor(string nombre, int colegiado, string especialidad,
            string telefono)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (a:Person) " +
                                        "SET a.nombre = $nombre " +
                                        "SET a.colegiado = $colegiado"  + 
                                        "SET a.especialidad = $especialidad " +
                                        "SET a.telefono = $telefono " +
                                        "RETURN a",
                        new { nombre, colegiado, especialidad, telefono });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        public void AddMedicine(string nombre, string desde, string hasta, string dosis)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (m:Medicina) " +
                                        "SET m.nombre = $nombre " +
                                        "SET m.desde = $desde " +
                                        "SET m.hasta = $hasta " +
                                        "SET m.dosis = $dosis " +
                                        "RETURN m",
                        new { nombre, desde, hasta });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        public void PatientVisitsDoctor(string nombrePaciente, string nombreDoctor, string nombreMedicina,
            string fecha)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (p:Paciente {nombre:$nombrePaciente}), " +
                        "(d:Doctor {nombre:$nombreDoctor}) " + 
                        "CREATE (p)-[:VISITA]->(d) RETURN p, d",
                        new { nombrePaciente, nombreDoctor, nombreMedicina, fecha });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
