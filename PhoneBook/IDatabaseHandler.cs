using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using MySqlConnector;
using KsiazkaTelefoniczna;

namespace PhoneBook
{
    interface IDatabaseHandler
    {
        void Connect();
        List<Contact> GetAllContacts();
        void AddContact(string first_name, string last_name, string phone_number, string address);
        void EditContact(Contact contact);
        void DeleteContact(Contact contact);
        List<Contact> SearchContacts(string search_query);
    }
    class SQLiteHandler : IDatabaseHandler
    {
        private SQLiteConnection connection;
        private SQLiteConfig config;

        public SQLiteHandler(SQLiteConfig config)
        {
            this.config = config;
            Connect();
        }

        public void Connect()
        {
            if (!File.Exists(config.DatabaseName))
            {
                SQLiteConnection.CreateFile(config.DatabaseName);
            }

            connection = new SQLiteConnection($"Data Source={config.DatabaseName};Version=3;");
            connection.Open();

            string create_table_sql = $@"CREATE TABLE IF NOT EXISTS {config.TableName} (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                FirstName TEXT, 
                LastName TEXT, 
                PhoneNumber TEXT,
                Address TEXT)";

            using (SQLiteCommand command = new SQLiteCommand(create_table_sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public List<Contact> GetAllContacts()
        {
            List<Contact> contacts = new List<Contact>();
            string sql = $"SELECT * FROM {config.TableName}";

            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    contacts.Add(new Contact(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
                }
            }
            return contacts;
        }

        public void AddContact(string first_name, string last_name, string phone_number, string address)
        {
            string add_contact_sql = $"INSERT INTO {config.TableName}(FirstName, LastName, PhoneNumber, Address) VALUES (@firstName, @lastName, @phoneNumber, @address)";

            using (SQLiteCommand command = new SQLiteCommand(add_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@firstName", first_name);
                command.Parameters.AddWithValue("@lastName", last_name);
                command.Parameters.AddWithValue("@phoneNumber", phone_number);
                command.Parameters.AddWithValue("@address", address);
                command.ExecuteNonQuery();
            }
        }

        public void EditContact(Contact contact)
        {
            string update_contact_sql = $"UPDATE {config.TableName} SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNumber, Address = @address WHERE ID = @id";

            using (SQLiteCommand command = new SQLiteCommand(update_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@firstName", contact.FirstName);
                command.Parameters.AddWithValue("@lastName", contact.LastName);
                command.Parameters.AddWithValue("@phoneNumber", contact.PhoneNumber);
                command.Parameters.AddWithValue("@address", contact.Address);
                command.Parameters.AddWithValue("@id", contact.ID);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteContact(Contact contact)
        {
            string delete_contact_sql = $"DELETE FROM {config.TableName} WHERE ID = @id";

            using (SQLiteCommand command = new SQLiteCommand(delete_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@id", contact.ID);
                command.ExecuteNonQuery();
            }
        }
        public List<Contact> SearchContacts(string search_query)
        {
            List<Contact> contacts = new List<Contact>();
            string search_contact_sql = $"SELECT * FROM {config.TableName} WHERE FirstName LIKE @searchQuery OR LastName LIKE @searchQuery OR PhoneNumber LIKE @searchQuery OR Address LIKE @searchQuery";

            using (SQLiteCommand command = new SQLiteCommand(search_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@searchQuery", $"%{search_query}%");
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contacts.Add(new Contact(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
                    }
                }
            }

            return contacts;
        }
    }

    class MySQLHandler : IDatabaseHandler
    {
        private MySqlConnection connection;
        private MySQLConfig config;

        public MySQLHandler(MySQLConfig config)
        {
            this.config = config;
            Connect();
        }

        public void Connect()
        {
            try
            {
                connection = new MySqlConnection($"server={config.Server};database={config.Database};user={config.User};password={config.Password};");
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string create_table_sql = $@"CREATE TABLE IF NOT EXISTS `contacts` (
              `ID` int(11) NOT NULL AUTO_INCREMENT,
              `FirstName` text,
              `LastName` text,
              `PhoneNumber` text,
              `Address` text,
              PRIMARY KEY (`ID`)
            );";
            using (MySqlCommand command = new MySqlCommand(create_table_sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public List<Contact> GetAllContacts()
        {
            List<Contact> contacts = new List<Contact>();
            string sql = $"SELECT * FROM {config.TableName}";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    contacts.Add(new Contact(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
                }
            }
            return contacts;
        }

        public void AddContact(string first_name, string last_name, string phone_number, string address)
        {
            string add_contact_query = $"INSERT INTO {config.TableName}(FirstName, LastName, PhoneNumber, Address) VALUES (@firstName, @lastName, @phoneNumber, @address)";

            using (MySqlCommand command = new MySqlCommand(add_contact_query, connection))
            {
                command.Parameters.AddWithValue("@firstName", first_name);
                command.Parameters.AddWithValue("@lastName", last_name);
                command.Parameters.AddWithValue("@phoneNumber", phone_number);
                command.Parameters.AddWithValue("@address", address);
                command.ExecuteNonQuery();
            }
        }

        public void EditContact(Contact contact)
        {
            string update_contact_sql = $"UPDATE {config.TableName} SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNumber, Address = @address WHERE ID = @id";

            using (MySqlCommand command = new MySqlCommand(update_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@firstName", contact.FirstName);
                command.Parameters.AddWithValue("@lastName", contact.LastName);
                command.Parameters.AddWithValue("@phoneNumber", contact.PhoneNumber);
                command.Parameters.AddWithValue("@address", contact.Address);
                command.Parameters.AddWithValue("@id", contact.ID);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteContact(Contact contact)
        {
            string delete_contact_sql = $"DELETE FROM {config.TableName} WHERE ID = @id";

            using (MySqlCommand command = new MySqlCommand(delete_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@id", contact.ID);
                command.ExecuteNonQuery();
            }
        }

        public List<Contact> SearchContacts(string search_query)
        {
            List<Contact> contacts = new List<Contact>();
            string search_contact_sql = $"SELECT * FROM {config.TableName} WHERE FirstName LIKE @searchQuery OR LastName LIKE @searchQuery OR PhoneNumber LIKE @searchQuery OR Address LIKE @searchQuery";

            using (MySqlCommand command = new MySqlCommand(search_contact_sql, connection))
            {
                command.Parameters.AddWithValue("@searchQuery", $"%{search_query}%");
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contacts.Add(new Contact(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
                    }
                }
            }

            return contacts;
        }
    }

}
