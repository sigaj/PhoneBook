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
            int currentIndex = 0;

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
                    if (i == currentIndex)
                        Console.ForegroundColor = ConsoleColor.Green;

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
                        currentIndex = (currentIndex + 1) % items_on_page.Count;
                        break;
                    case ConsoleKey.UpArrow:
                        currentIndex = (currentIndex - 1 + items_on_page.Count) % items_on_page.Count;
                        break;
                    case ConsoleKey.Enter:
                        callback(items_on_page[currentIndex]);
                        return;
                    case ConsoleKey.Escape:
                        on_escape_callback?.Invoke();
                        return;
                    case ConsoleKey.LeftArrow:
                        if (page_number - 1 > 0)
                        {
                            DisplayMenu(items, text_formatter, callback, on_escape_callback, page_number - 1, items_per_page);
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (page_number <= ((items.Count / items_per_page)))
                        {
                            DisplayMenu(items, text_formatter, callback, on_escape_callback, page_number + 1, items_per_page);
                        }
                        break;
                }
            }
        }
    }
}
