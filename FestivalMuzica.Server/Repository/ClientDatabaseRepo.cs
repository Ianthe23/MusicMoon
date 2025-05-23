using log4net;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using FestivalMuzica.Server.Repository.Utils;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Server.Repository
{
    public class ClientDatabaseRepo : IClientRepo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClientDatabaseRepo));
        private readonly IDictionary<string, string> props;
        
        public ClientDatabaseRepo(IDictionary<string, string> props)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            log.Info("Initializing ClientDataBaseRepo");
        }

        public Client? FindByName(string name)
        {
            log.InfoFormat("Entering FindByName with name: {0}", name);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Client WHERE name=@name";
                cmd.Parameters.Add(new SQLiteParameter("@name", name));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string clientName = dataReader.GetString(1);
                        return new Client(id, clientName);
                    }
                }
            }
            log.InfoFormat("Exiting FindByName with name: {0}", name);
            return null;
        }

        public Client FindOne(long id)
        {
            log.InfoFormat("Entering FindOne with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Client WHERE id=@id";
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                using (var dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        return new Client(dataReader.GetInt64(0), dataReader.GetString(1));
                    }
                }
            }
            log.InfoFormat("Exiting FindOne with id: {0}", id);
            throw new ArgumentException($"Client with id {id} not found");
        }

        public IEnumerable<Client> FindAll()
        {
            log.InfoFormat("Entering FindAll");
            IDbConnection con = DBUtils.getConnection(props);
            IList<Client> clients = new List<Client>();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Client";
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        long id = dataReader.GetInt64(0);
                        string name = dataReader.GetString(1);
                        clients.Add(new Client(id, name));
                    }
                }
            }
            log.InfoFormat("Exiting FindAll with {0} clients", clients.Count);
            return clients;
        }

        public Client Save(Client entity)
        {
            log.InfoFormat("Entering Save");
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Client (name) VALUES (@name)";
                    cmd.Parameters.Add(new SQLiteParameter("@name", entity.Name));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Save");
                    return entity;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error saving entity: {0}", e.Message);
                throw;
            }
        }

        public Client Delete(long id)
        {
            log.InfoFormat("Entering Delete with id: {0}", id);
            IDbConnection con = DBUtils.getConnection(props);
            try
            {
                // First find the client to return
                Client client = FindOne(id);
                
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Client WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Delete with id: {0}", id);
                    return client;
                }   
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error deleting entity: {0}", e.Message);
                throw;
            }
        }

        public Client Update(Client entity)
        {
            log.InfoFormat("Entering Update with id: {0}", entity.Id);
            IDbConnection con = DBUtils.getConnection(props);
            try 
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Client SET name=@name WHERE id=@id";
                    cmd.Parameters.Add(new SQLiteParameter("@name", entity.Name));
                    cmd.Parameters.Add(new SQLiteParameter("@id", entity.Id));
                    cmd.ExecuteNonQuery();
                    log.InfoFormat("Exiting Update with id: {0}", entity.Id);
                    return entity;
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error updating entity: {0}", e.Message);
                throw;
            }
        }
    }
}