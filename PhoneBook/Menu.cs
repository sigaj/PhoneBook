using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsiazkaTelefoniczna
{
    internal class Menu
    {
        public void DisplayMenu<T>(List<T> items, Func<T, string> text, Action<T> callback, Action on_escape_callback = null)
        {
            int currentIndex = 0;

            while (true)
            {
                Console.Clear();

                for (int i = 0; i < items.Count; i++)
                {
                    if (i == currentIndex)
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine($"{i + 1}. {text(items[i])}");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.DownArrow:
                        currentIndex = (currentIndex + 1) % items.Count;
                        break;
                    case ConsoleKey.UpArrow:
                        currentIndex = (currentIndex - 1 + items.Count) % items.Count;
                        break;
                    case ConsoleKey.Enter:
                        callback(items[currentIndex]);
                        return;
                    case ConsoleKey.Escape:
                        on_escape_callback?.Invoke();
                        return;
                }
            }
        }
    }
}
