using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneBook
{
    internal class PhoneBook
    {
        private IDatabaseHandler database_handler = null;
        private Menu menu = new Menu();

        public PhoneBook()
        {
            try
            {
                var database_config = DatabaseConfig.LoadConfig("database_config.json");
                if (database_config.DatabaseType.ToLower() == "mysql")
                {
                    database_handler = new MySQLHandler(database_config.MySQL);
                }
                else if (database_config.DatabaseType.ToLower() == "sqlite")
                {
                    database_handler = new SQLiteHandler(database_config.SQLite);
                }
                else
                {
                    Console.WriteLine("nieznana baza danych, obslugiwane bazy to mysql i sqlite");
                    return;
                }
                database_handler.Connect();
            } catch (Exception ex)
            {
                Console.WriteLine("wystapil problem z polaczeniem z baza danych. sprawdz poprawnosc danych bazy");
                Console.WriteLine("Nazwa bledu: ");
                Console.WriteLine(ex.Message);
                return;
            }
            DisplayStartMenu();
        }
        public void DisplayContacts(List<Contact> contacts = null)
        {
            Console.Clear();
            try
            {
                contacts = contacts ?? database_handler.GetAllContacts(); 
            } catch (Exception ex)
            {
                Console.WriteLine("Wystapil blad z polaczeniem z baza danych");
                Console.WriteLine("Nazwa bledu: ");
                Console.WriteLine(ex);
                return;
            }
            if (contacts.Count == 0)
            {
                Console.WriteLine("brak kontaktoww, nacisnij jakikolwiek klawisz, aby wrocic");
                Console.ReadKey();
                DisplayStartMenu();
            }
            menu.DisplayMenu(contacts,
                contact => $"{contact.FirstName} {contact?.LastName} {contact?.PhoneNumber}, {contact?.Address}",
                contact => DisplayContactOptions(contact), DisplayStartMenu);
        }
        public void DisplayContactOptions(Contact contact)
        {
            Dictionary<string, Action> menu_options = new Dictionary<string, Action> {
                { "edytuj", () => EditContact(contact) },
                { "usun", () => DeleteContact(contact) },
            };
            menu.DisplayMenu(menu_options.Keys.ToList(),
                option => option,
                option => menu_options[option].Invoke(), () => DisplayContacts(null));
        }
        public void AddContact()
        {
            Console.Clear();
            Console.Write("imie: ");
            string first_name = Console.ReadLine();
            Console.Write("nazwisko: ");
            string last_name = Console.ReadLine();
            Console.Write("numer telefonu: ");
            string phone_number = Console.ReadLine();
            Console.Write("adres: ");
            string address = Console.ReadLine();

            database_handler.AddContact(first_name, last_name, phone_number, address);
            DisplayContacts();
        }
        public void EditContact(Contact contact)
        {
            Console.Clear();
            Console.Write("imie: ");
            SendKeys.SendWait(contact.FirstName);
            string new_first_name = Console.ReadLine();
            Console.Write("nazwisko: ");
            SendKeys.SendWait(contact.LastName);
            string new_last_name = Console.ReadLine();
            Console.Write("numer telefonu: ");
            SendKeys.SendWait(Regex.Replace(contact.PhoneNumber, "[+^%~()]", "{$0}"));
            string new_phone_number = Console.ReadLine();
            Console.Write("adres: ");
            SendKeys.SendWait(contact.Address);
            string new_address = Console.ReadLine();

            contact.FirstName = new_first_name;
            contact.LastName = new_last_name;
            contact.PhoneNumber = new_phone_number;
            contact.Address = new_address;

            database_handler.EditContact(contact);
            DisplayContacts();
        }
        public void DeleteContact(Contact contact)
        {
            Console.Clear();
            Console.WriteLine("czy na pewno usunac kontakt? (t/n)");
            var key = Console.ReadKey(false).Key;
            if (key == ConsoleKey.T)
            {
                database_handler.DeleteContact(contact);
                DisplayContacts();
            }
            else
            {
                DisplayContactOptions(contact);
            }
        }
        public void SearchContacts()
        {
            Console.Clear();
            Console.Write("wprowadz imie, nazwisko lub numer telefonu do wyszukania: ");
            string search_query = Console.ReadLine();

            List<Contact> found_contacts = database_handler.SearchContacts(search_query);

            if (found_contacts.Count > 0)
            {
                DisplayContacts(found_contacts);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("nie znaleziono zadnych kontaktow. kliknij dowolny klawisz, aby wyszukac ponownie.");
                Console.ReadKey();
                SearchContacts();
            }
        }

        public void DisplayStartMenu()
        {
            Dictionary<string, Action> menu_options = new Dictionary<string, Action> {
                { "wyswietl kontakty", () => DisplayContacts(null) },
                { "dodaj kontakt", () => AddContact() },
                { "wyszukaj kontakt", () => SearchContacts() },
            };
            menu.DisplayMenu(menu_options.Keys.ToList(),
                option => option,
                option => menu_options[option].Invoke(),
                () => Environment.Exit(0));
        }
    }
}
