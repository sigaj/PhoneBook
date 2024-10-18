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
        private string database_name = "phone_book.db";
        private string table_name = "Contacts";

        public SQLiteHandler(SQLiteConfig config)
        {
            this.config = config;
            Connect();
        }

        public void Connect()
        {
            if (!File.Exists(database_name))
            {
                SQLiteConnection.CreateFile(database_name);
            }

            connection = new SQLiteConnection($"Data Source={database_name};Version=3;");
            connection.Open();

            string createTableSql = $@"CREATE TABLE IF NOT EXISTS {table_name} (
                ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                FirstName TEXT, 
                LastName TEXT, 
                PhoneNumber TEXT,
                Address TEXT)";

            using (SQLiteCommand command = new SQLiteCommand(createTableSql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public List<Contact> GetAllContacts()
        {
            List<Contact> contacts = new List<Contact>();
            string sql = $"SELECT * FROM {table_name}";

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
            string addContactQuery = $"INSERT INTO {table_name}(FirstName, LastName, PhoneNumber, Address) VALUES (@firstName, @lastName, @phoneNumber, @address)";

            using (SQLiteCommand command = new SQLiteCommand(addContactQuery, connection))
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
            string updateContactSql = $"UPDATE {table_name} SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNumber, Address = @address WHERE ID = @id";

            using (SQLiteCommand command = new SQLiteCommand(updateContactSql, connection))
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
            string deleteContactSql = $"DELETE FROM {table_name} WHERE ID = @id";

            using (SQLiteCommand command = new SQLiteCommand(deleteContactSql, connection))
            {
                command.Parameters.AddWithValue("@id", contact.ID);
                command.ExecuteNonQuery();
            }
        }
        public List<Contact> SearchContacts(string search_query)
        {
            List<Contact> contacts = new List<Contact>();
            string query = $"SELECT * FROM {table_name} WHERE FirstName LIKE @searchQuery OR LastName LIKE @searchQuery OR PhoneNumber LIKE @searchQuery OR Address LIKE @searchQuery";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
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
        private string connectionString;

        public MySQLHandler(MySQLConfig config)
        {
            this.config = config;
            Connect();
        }

        public void Connect()
        {
            connection = new MySqlConnection($"server={config.Server};database={config.Database};user={config.User};password={config.Password};");
            connection.Open();

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
            string addContactQuery = $"INSERT INTO {config.TableName}(FirstName, LastName, PhoneNumber, Address) VALUES (@firstName, @lastName, @phoneNumber, @address)";

            using (MySqlCommand command = new MySqlCommand(addContactQuery, connection))
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
            string updateContactSql = $"UPDATE {config.TableName} SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNumber, Address = @address WHERE ID = @id";

            using (MySqlCommand command = new MySqlCommand(updateContactSql, connection))
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
            string deleteContactSql = $"DELETE FROM {config.TableName} WHERE ID = @id";

            using (MySqlCommand command = new MySqlCommand(deleteContactSql, connection))
            {
                command.Parameters.AddWithValue("@id", contact.ID);
                command.ExecuteNonQuery();
            }
        }

        public List<Contact> SearchContacts(string search_query)
        {
            List<Contact> contacts = new List<Contact>();
            string query = $"SELECT * FROM {config.TableName} WHERE FirstName LIKE @searchQuery OR LastName LIKE @searchQuery OR PhoneNumber LIKE @searchQuery OR Address LIKE @searchQuery";

            using (MySqlCommand command = new MySqlCommand(query, connection))
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
