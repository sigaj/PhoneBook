using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace KsiazkaTelefoniczna
{
    class DatabaseConfig
    {
        public string DatabaseType { get; set; }
        public MySQLConfig MySQL { get; set; }
        public SQLiteConfig SQLite { get; set; }

        public static DatabaseConfig LoadConfig(string file_path)
        {
            string json_string;
            if (!File.Exists(file_path))
            {
                File.Create(file_path);
                File.WriteAllText(file_path, @"{
                    ""DatabaseType"": ""MySQL"",
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
            try
            {
                json_string = File.ReadAllText(file_path);
            } catch (Exception ex) {
                Console.WriteLine("bład z plikiem konfiguracyjnym");
                throw ex;
            }
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
