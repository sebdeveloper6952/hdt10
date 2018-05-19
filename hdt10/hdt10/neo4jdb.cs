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
        // singleton
        public static neo4jdb instance = new neo4jdb("bolt://localhost:7687", "neo4j", "pass123");
        private readonly IDriver driver;

        private neo4jdb(string uri, string user, string password)
        {
            driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        /// <summary>
        /// Agrega un paciente con los datos especificados a la base de datos.
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="telefono"></param>
        public void AddPatient(string nombre, string telefono)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (a:Paciente) " +
                                        "SET a.nombre = $nombre " +
                                        "SET a.telefono = $telefono " +
                                        "RETURN a",
                        new { nombre, telefono });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        /// <summary>
        /// Agrega un doctor con los datos especificados a la base de datos.
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="colegiado"></param>
        /// <param name="especialidad"></param>
        /// <param name="telefono"></param>
        public void AddDoctor(string nombre, string colegiado, string especialidad,
            string telefono)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (a:Doctor) " +
                                        "SET a.nombre = $nombre " +
                                        "SET a.colegiado = $colegiado "  + 
                                        "SET a.especialidad = $especialidad " +
                                        "SET a.telefono = $telefono " +
                                        "RETURN a",
                        new { nombre, colegiado, especialidad, telefono });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        /// <summary>
        /// Agrega una medicina con los datos especificados a la base de datos.
        /// </summary>
        /// <param name="nombre"></param>
        public void AddMedicine(string nombre)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (m:Medicina) " +
                                        "SET m.nombre = $nombre " +
                                        "RETURN m",
                        new { nombre });
                    return result.Single()[0].As<string>();
                });
                Console.WriteLine(person);
            }
        }

        /// <summary>
        /// Relaciona un paciente con un doctor (VISITA), un paciente con una
        /// medicina (TOMA) y un doctor con una medicina (PRESCRIBE).
        /// </summary>
        /// <param name="paciente"></param>
        /// <param name="doctor"></param>
        /// <param name="medicina"></param>
        /// <param name="fecha"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="dosis"></param>
        public void PatientVisitsDoctor(string paciente, string doctor, string medicina,
            string fecha, string desde, string hasta, string dosis)
        {
                using (var session = driver.Session())
                {
                    var person = session.WriteTransaction(tx =>
                    {
                        try
                        {
                            var result = tx.Run("MATCH (p:Paciente {nombre:$paciente}), " +
                                "(d:Doctor {nombre:$doctor}), " +
                                "(m:Medicina {nombre:$medicina}) " +
                                "CREATE (p)-[:VISITA {fecha:$fecha}]->(d)" +
                                "-[:PRESCRIBE {desde:$desde, hasta:$hasta, dosis:$dosis}]->(m)<-[:TOMA]-(p) RETURN p",
                                new { paciente, doctor, medicina, fecha, desde, hasta, dosis });
                            return result.Single()[0].As<string>();
                        }
                        catch (Exception e) { return null; }
                    });
                }
        }

        /// <summary>
        /// Agrega una relacion entre dos personas (CONOCE).
        /// </summary>
        /// <param name="person0"></param>
        /// <param name="person1"></param>
        public void ConnectPersons(string person0, string person1)
        {
            using (var session = driver.Session())
            {
                var person = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (p0 {nombre:$person0}), " +
                        "(p1 {nombre:$person1}) " +  
                        "CREATE (p0)-[:CONOCE]->(p1)-[:CONOCE]->(p0) RETURN p0.nombre as nombre",
                        new { person0, person1 });
                    return result.Single()[0].As<string>();
                });
            }
        }

        /// <summary>
        /// Busca doctores con una especialidad especifica.
        /// especifica.
        /// </summary>
        /// <param name="especialidad"></param>
        /// <returns></returns>
        public IList<string> GetDoctorWithSpecialty(string especialidad)
        {
            List<string> results = new List<string>();
            using (var session = driver.Session())
            {
                session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (d:Doctor) " +
                        "WHERE d.especialidad = $especialidad " +
                        "RETURN d.nombre as nombre", new { especialidad });
                    foreach (var record in result)
                        results.Add(record["nombre"].As<string>());
                });
            }
            return results;
        }

        /// <summary>
        /// Recomendacion para pacientes, de doctores con una especialidad
        /// especifica.
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        public IList<string> RecommendDoctors(string persona)
        {
            List<string> results = new List<string>();
            using (var session = driver.Session())
            {
                session.WriteTransaction(tx =>
                {
                var result = tx.Run("MATCH (p:Paciente {nombre:$persona})" +
                    "-[:CONOCE*1..2]->(amigo)-[:VISITA]->" +
                    "(doc:Doctor) return doc.nombre as nombre", new { persona });
                    foreach (var record in result)
                        results.Add(record["nombre"].As<string>());
                });
            }
            return results;
        }

        /// <summary>
        /// Recomendacion para doctores, de pacientes.
        /// </summary>
        /// <param name="doctor"></param>
        /// <param name="especialidad"></param>
        /// <returns></returns>
        public IList<string> RecommendPatients(string doctor, string especialidad)
        {
            List<string> results = new List<string>();
            using (var session = driver.Session())
            {
                session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (d:Doctor {nombre:$doctor}), " + 
                        "(d)-[:CONOCE*1..2]->(doc:Doctor {especialidad:$especialidad}) " + 
                        "RETURN distinct doc.nombre as nombre", new { doctor, especialidad });
                    foreach (var record in result)
                        results.Add(record["nombre"].As<string>());
                });
            }
            return results;
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
