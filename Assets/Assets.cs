using Models;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using System.Collections;
using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Serializers;

namespace Assets
{
    public static class ProductSearch
    {
        public static List<Product> SearchByKeyword(IEnumerable<Product> products, string keyword)
        {
            keyword = keyword.ToLower();

            return products.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(keyword)) ||
                (p.Description != null && p.Description.ToLower().Contains(keyword))
            ).ToList();
        }

        public static List<Product> SearchByCategory(IEnumerable<Product> products, Category category)
        {
            return products
                .Where(p => p != null)
                .Where(p => p.Category.Equals(category.ToString(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public class InputField
    {
        protected StringBuilder input { get; set; }
        public InputField()
        {
            input = new StringBuilder();
        }

        public string DrawAndStart(int x , int y)
        {
            ConsoleKeyInfo klawisz;
            int i = 0;
            while (true)
            {
                klawisz = Console.ReadKey(true);
                switch(klawisz.Key)
                {
                    case ConsoleKey.Backspace:
                        if(i != 0)
                        {
                            Console.SetCursorPosition(x + i - 1, y);
                            Console.Write(' ');
                            i--;
                        }
                        break;
                    case ConsoleKey.Enter:
                            return input.ToString();
                        break;
                    default:
                        input.Append(klawisz.KeyChar);
                        Console.SetCursorPosition(x + i, y);
                        Console.Write(klawisz.KeyChar);
                        i++;
                        break;
                }
            }
        }
    }

    public class Window
    {
        public int cornerX { get; }
        public int cornerY { get; }
        private int width { get; }
        private int height { get; }
        public int Width  { get { return width;  } }
        public int Height { get { return height; } }
        public List<string> Options = new List<string>();
        private string selectedOption;
        public string SelectedOption { get { return selectedOption; } }
        public Window(List<string> ops ,int startPlaceX , int startPlaceY, int width=0 , int height=0)
        {
            this.cornerX = startPlaceX;
            this.cornerY = startPlaceY;
            this.Options = ops;
            int longest_string = 0;
            foreach (string op in Options)
            {
                if (op.Length > longest_string)
                {
                    longest_string = op.Length;
                }
            }
            this.width = (longest_string + 2 > width) ? longest_string + 2 : width;
            this.height = this.Options.Count + 1 > height ? this.Options.Count + 1 : height;
        }
        public string DrawAndStart(int selctd=0)
        {
            ConsoleKeyInfo klawisz;
            int Selected = selctd;
            while (true)
            {
                Console.SetCursorPosition(cornerX + 1, cornerY + 1 + Selected);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(Options[Selected]);
                klawisz = Console.ReadKey(true);
                if (klawisz.Key == ConsoleKey.UpArrow && Selected != 0)
                {
                    Console.SetCursorPosition(cornerX + 1, cornerY + 1 + Selected);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(Options[Selected]);
                    Selected--;
                    Console.SetCursorPosition(cornerX + 1, cornerY + 1 + Selected);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(Options[Selected]);
                }
                if (klawisz.Key == ConsoleKey.DownArrow && Selected < Options.Count - 1)
                {
                    Console.SetCursorPosition(cornerX + 1, cornerY + 1 + Selected);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(Options[Selected]);
                    Selected++;
                    Console.SetCursorPosition(cornerX + 1, cornerY + 1 + Selected);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write(Options[Selected]);
                }
                if (klawisz.Key == ConsoleKey.Enter)
                {
                    selectedOption = Options[Selected];
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    return "r";
                }
                if (klawisz.Key == ConsoleKey.Tab)
                {
                    Console.SetCursorPosition(cornerX + 1, cornerY + 1 + Selected);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(Options[Selected]);
                    return "";
                }
            }
        }
    };

    public class Control
    {
        protected List<Window> windows;
        public Control()
        {
            windows = new List<Window>();
        }
        public void AddWindow(Window w)
        {
            windows.Add(w);
        }
        public string DrawAndStart(string exitRespone="")
        {
            foreach (Window window in windows) 
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(window.cornerX, window.cornerY);
                for (int j = 0; j < window.Width + 1; j++)
                {Console.Write("-");}
                for (int i = 1; i < window.Height; i++)
                {
                    Console.SetCursorPosition(window.cornerX, window.cornerY + i);
                    Console.Write("|");
                    if (i <= window.Options.Count)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(window.Options[i - 1]);
                    }
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(window.cornerX + window.Width, window.cornerY + i);
                    Console.Write("|");
                }
                Console.SetCursorPosition(window.cornerX, window.cornerY + window.Height);
                for (int j = 0; j < window.Width + 1; j++)
                {
                    Console.Write("-");
                }
            }
            for (int i = 0; true; i = (i + 1) % windows.Count)
            {
                string respone = windows[i].DrawAndStart();
                if(respone == "r")
                {
                    Console.Clear();
                    return windows[i].SelectedOption;
                }
            }
        }
    }

    [ProtoContract]
    public class ASCIImage
    {
        [ProtoMember(1)]
        private List<string> pixels;
        public void Display(int x , int y)
        {
            int i = 0;
            foreach (string k in pixels)
            {
                Console.SetCursorPosition(x, y + i);
                i++;
                Console.WriteLine(k);
            }
            
        }
        public ASCIImage(string txtfile) 
        {
            pixels = new List<string>();
            StreamReader reader = new StreamReader("img/" + txtfile);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                pixels.Add(line);
            }
            reader.Close();
        }

        public ASCIImage() { }
    }

    public class ProductStorage<T> : IEnumerable<T> where T : Product
    {
        protected T[] items;
        protected int count;
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {yield return item;}
        }
        IEnumerator IEnumerable.GetEnumerator()
        {return GetEnumerator();}
        public ProductStorage()
        {
            items = new T[0];
            count = 0;
        }
        public void Add(T item)
        {
            if (count == items.Length)
            {
                Array.Resize(ref items, items.Length + 1);
            }
            items[count] = item;
            count++;
        }
        public void Clear()
        {items = default;  count = 0;}
        public bool Remove(T item)
        {
            int index = Array.IndexOf(items, item, 0, count);
            if (index == -1)
            {
                return false;
            }
            var temp = new T[count - 1];
            bool x = false;//defines if we pass by forgotten element
            for (int i = 0; i < count - 1; i++)
            {
                if(i == index)
                {
                    x = true;
                }
                if(x)
                {
                    temp[i] = items[i + 1];
                }
                else
                {
                    temp[i] = items[i];
                }
            }
            this.Clear();
            items = temp;
            count = temp.Length;
            return true;
        }
        public bool Contains(T item)
        {
            return Array.IndexOf(items, item, 0, count) >= 0;
        }
        public T GetItem(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();
            return items[index];
        }
        public void PrintAll()
        {
            foreach (var item in items)
            {Console.WriteLine(item);}
        }
        public int Length()
        {return count;}
    }


    /*
https://app.diagrams.net/#G1nNPKJvPk5i6ZXqyXtRt_clXLcWH5EOcj#%7B%22pageId%22%3A%221r48irf5eILzQUGIeJtA%22%7D     
     */

    public static class DatabaseAction
    {
        public static void SerializeProduct()
        {

        }

        public static Product DeserializeProduct(string filePath)
        {
            if (File.Exists(filePath))
            {
                Device productToEdit;
                using (var file = File.OpenRead(filePath))
                {
                    productToEdit = Serializer.Deserialize<Device>(file);
                }
                return productToEdit;
            }
            else
            {
                throw new Exception("Haven't found file to deserialization");
            }
        }
    };

    public static class StringExtensions
    {
        public static string ToSafeFileName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return "unnamed";

            return string.Join("_", name.Split(Path.GetInvalidFileNameChars()))
                       .Replace(" ", "_")
                       .ToLower();
        }
    }

}
