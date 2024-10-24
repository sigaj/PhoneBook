using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook
{
    internal class Menu
    {
        public void DisplayMenu<T>(List<T> items, Func<T, string> text_formatter, Action<T> callback, Action on_escape_callback = null, int page_number = 1, int items_per_page = 10)
        {
            int current_index = 0;

            while (true)
            {
                Console.Clear();
                if (items.Count > items_per_page || page_number > 1)
                {
                    Console.WriteLine("uzyj strzalek, aby zmienic strone (←, →)");
                    Console.WriteLine();
                }

                var items_on_page = items.Skip((page_number - 1) * items_per_page).Take(items_per_page).ToList();
                for (int i = 0; i < items_on_page.Count; i++)
                {
                    if (i == current_index)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("> ");
                    }

                    Console.WriteLine($"{i + (page_number - 1) * items_per_page + 1}. {text_formatter(items_on_page[i])}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (items.Count > items_per_page || page_number > 1)
                {
                    for (int i = 0; i < items_per_page - items_on_page.Count + 1; i++)
                    {
                        Console.WriteLine();
                    }
                    Console.WriteLine($"strona {page_number} z {(items.Count / items_per_page) + 1}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        current_index += 1;
                        if (current_index >= items_on_page.Count)
                        {
                            current_index = 0;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        current_index -= 1;
                        if (current_index < 0 )
                        {
                            current_index = items_on_page.Count - 1;
                        }
                        break;
                    case ConsoleKey.Enter:
                        callback(items_on_page[current_index]);
                        return;
                    case ConsoleKey.Escape:
                        on_escape_callback?.Invoke();
                        return;
                    case ConsoleKey.LeftArrow:
                        page_number -= 1;
                        if (page_number < 1)
                        {
                            page_number = (items.Count / items_per_page) + 1;
                        }
                        DisplayMenu(items, text_formatter, callback, on_escape_callback, page_number, items_per_page);
                        return;
                    case ConsoleKey.RightArrow:
                        page_number += 1;
                        if (page_number > (items.Count / items_per_page) + 1)
                        {
                            page_number = 1;
                        }
                        DisplayMenu(items, text_formatter, callback, on_escape_callback, page_number, items_per_page);
                        return;
                }
            }
        }
    }
}
