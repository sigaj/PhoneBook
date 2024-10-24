using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace PhoneBook
{
    class DatabaseConfig
    {
        public string DatabaseType { get; set; }
        public MySQLConfig MySQL { get; set; }
        public SQLiteConfig SQLite { get; set; }

        public static DatabaseConfig LoadConfig(string file_path)
        {
            string json_string;
            if (!File.Exists(file_path) || new FileInfo(file_path).Length == 0)
            {
                File.WriteAllText(file_path, @"{
                    ""DatabaseType"": ""SQLite"",
                    ""MySQL"": {
                        ""Server"": ""localhost"",
                        ""Database"": ""phone_book"",
                        ""TableName"": ""contacts"",
                        ""User"": ""root"",
                        ""Password"": """"
                    },
                    ""SQLite"": {
                        ""DatabaseName"": ""phone_book.db"",
                        ""TableName"": ""contacts""
                    }
                }");
            }
            json_string = File.ReadAllText(file_path);
            return JsonSerializer.Deserialize<DatabaseConfig>(json_string);
        }
    }

    public class MySQLConfig
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string TableName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
    public class SQLiteConfig
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
    }
}
