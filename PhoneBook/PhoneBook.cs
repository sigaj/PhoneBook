﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KsiazkaTelefoniczna
{
    internal class PhoneBook
    {
        private DatabaseHandler databaseHandler = null;
        private Menu menu = null;

        public PhoneBook()
        {
            databaseHandler = new DatabaseHandler();
            menu = new Menu();
            DisplayStartMenu();
        }
        public void DisplayContacts()
        {
            DisplayContacts(null);
        }
        public void DisplayContacts(List<Contact> contacts = null)
        {
            Console.Clear();
            contacts = contacts ?? databaseHandler.GetAllContacts();
            if (contacts.Count == 0)
            {
                Console.WriteLine("brak kontaktoww, nacisnij jakikolwiek klawisz, aby wrocic");
                Console.ReadKey();
                DisplayStartMenu();
            }
            menu.DisplayMenu(contacts,
                        contact => $"{contact.FirstName} {contact?.LastName} {contact?.PhoneNumber}",
                        contact => DisplayContactOptions(contact), DisplayStartMenu);
        }
        public void DisplayContactOptions(Contact contact)
        {
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action>{
                { "edytuj", () => EditContact(contact) },
                { "usun", () => DeleteContact(contact) },
            };
            menu.DisplayMenu(menuOptions.Keys.ToList(),
                        option => option,
                        option => menuOptions[option].Invoke(), DisplayContacts);
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

            databaseHandler.AddContact(first_name, last_name, phone_number);
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

            contact.FirstName = new_first_name;
            contact.LastName = new_last_name;
            contact.PhoneNumber = new_phone_number;

            databaseHandler.EditContact(contact);
            DisplayContacts();
        }
        public void DeleteContact(Contact contact)
        {
            Console.Clear();
            Console.WriteLine("czy na pewno usunac kontakt? (t/n)");
            var key = Console.ReadKey(false).Key;
            if (key == ConsoleKey.T)
            {
                databaseHandler.DeleteContact(contact);
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

            List<Contact> found_contacts = databaseHandler.SearchContacts(search_query);

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
            Dictionary<string, Action> menuOptions = new Dictionary<string, Action> {
                { "wyswietl kontakty", DisplayContacts },
                { "dodaj kontakt", AddContact },
                { "wyszukaj kontakt", SearchContacts },
            };
            menu.DisplayMenu(menuOptions.Keys.ToList(), option => option, option => menuOptions[option].Invoke());
        }
    }
}