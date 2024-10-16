using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KsiazkaTelefoniczna
{
    internal class DatabaseHandler
    {
        private SQLiteConnection connection;
        private string database_name = "ksiazkatelefoniczna.db";
        private string table_name = "Contacts";

        public DatabaseHandler()
        {
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
                PhoneNumber TEXT)";

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
                    contacts.Add(new Contact(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }
            }
            return contacts;
        }

        public void AddContact(string first_name, string last_name, string phone_number)
        {
            string addContactQuery = $"INSERT INTO {table_name}(FirstName, LastName, PhoneNumber) VALUES (@firstName, @lastName, @phoneNumber)";

            using (SQLiteCommand command = new SQLiteCommand(addContactQuery, connection))
            {
                command.Parameters.AddWithValue("@firstName", first_name);
                command.Parameters.AddWithValue("@lastName", last_name);
                command.Parameters.AddWithValue("@phoneNumber", phone_number);
                command.ExecuteNonQuery();
            }
        }

        public void EditContact(Contact contact)
        {
            string updateContactSql = $"UPDATE {table_name} SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNumber WHERE ID = @id";

            using (SQLiteCommand command = new SQLiteCommand(updateContactSql, connection))
            {
                command.Parameters.AddWithValue("@firstName", contact.FirstName);
                command.Parameters.AddWithValue("@lastName", contact.LastName);
                command.Parameters.AddWithValue("@phoneNumber", contact.PhoneNumber);
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
            string query = $"SELECT * FROM {table_name} WHERE FirstName LIKE @searchQuery OR LastName LIKE @searchQuery OR PhoneNumber LIKE @searchQuery";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchQuery", $"%{search_query}%");
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contacts.Add(new Contact(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                    }
                }
            }

            return contacts;
        }
    }

}
