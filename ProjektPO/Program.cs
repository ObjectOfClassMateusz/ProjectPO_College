using System;
using System.Security.Principal;
using Assets;
using Models;
using System.Media;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

#pragma warning disable SYSLIB0011

namespace ProjektPO
{
    class Program
    {
        
        static List<Product> LoadAllProducts()
        {
            List<Product> products = new List<Product>();
            DirectoryInfo directory = new DirectoryInfo("data");
            FileInfo[] files = directory.GetFiles("*.prdx");

            foreach (FileInfo file in files)
            {
                using (var stream = File.OpenRead(file.FullName))
                {
                    try
                    {
                        var product = Serializer.Deserialize<Device>(stream);
                        products.Add(product);
                    }
                    catch 
                    {

                    }
                }
            }

            return products;
        }
       

        static List<Product> cart = new List<Product>();
        static void DisplaySearchResults(List<Product> found)
        {
            if (found.Count == 0)
            {
                Console.WriteLine("No products found");
                Console.WriteLine("Press any key to go back...");
                Console.ReadKey();
                Console.Clear();
                DisplayShop();
                return;
            }

            List<string> options = found.Select((p, index) =>
                $"{index + 1}. {p.Name} | {p.Price} zł").ToList();

            options.Add("Go back to menu");

            Control resultControl = new Control();
            resultControl.AddWindow(new Window(options, 2, 2));

            string choice = resultControl.DrawAndStart();

            if (choice == "Go back to menu")
            {
                Console.Clear();
                DisplayShop();
                return;
            }

            int selectedIndex = options.IndexOf(choice);
            if (selectedIndex >= 0 && selectedIndex < found.Count)
            {
                Product selected = found[selectedIndex];
                cart.Add(selected);
                Console.Clear();
                Console.WriteLine($"Added'{selected.Name}' to your cart.");
                Console.WriteLine("Press any key to go back...");
                Console.ReadKey();
            }

            Console.Clear();
            DisplayShop();
        }



        static void DisplaySearchByKeyword()
        {
            Console.Clear();
            Console.Write("Type keyword: ");
            string keyword = Console.ReadLine();
            var products = LoadAllProducts();
            var found = ProductSearch.SearchByKeyword(products, keyword);
            DisplaySearchResults(found);
        }

        static void DisplaySearchByCategory()
        {
            Console.Clear();
            Console.WriteLine("Categories:");
            foreach (var cat in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine($"- {cat}");
            }

            Console.Write("\nName selected category: ");
            string input = Console.ReadLine();

            if (Enum.TryParse(input, true, out Category selectedCategory))
            {
                var products = LoadAllProducts();
                var found = ProductSearch.SearchByCategory(products, selectedCategory);
                DisplaySearchResults(found);
            }
            else
            {
                Console.WriteLine("Incorrect category.");
                Console.ReadKey();
            }
        }
        static void DisplayProducts()
        {
            Control productsControl = new Control();
            DirectoryInfo directoryWithImages = new DirectoryInfo("img//Products");
            FileInfo[] Files = directoryWithImages.GetFiles("*.txt");
            List<string> file_names = new List<string>();
            foreach (FileInfo file in Files)
            {
                file_names.Add(file.Name);
            }
            file_names.Add("Return");

            productsControl.AddWindow(new Window(file_names, 2,2));
            switch (productsControl.DrawAndStart())
            {
                case "Return":
                    DisplayShop();
                    break;
                default:
                    break;
            }
        }
        
        static void DisplayShop()
        {
            Control shopControl = new Control();
            Console.BackgroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(1, 1);
            Console.WriteLine("Main Actions");
            Console.SetCursorPosition(20, 1);
            Console.WriteLine("Search By Category");
            Console.BackgroundColor = ConsoleColor.Black;

            shopControl.AddWindow(new Window(new List<string>()
            {
                "See Products",
                "Wish List",
                "Search by Name",
                "Search by Category",
                "Return to menu"
            }, 1, 2));

            shopControl.AddWindow(new Window(new List<string>()
            {
                "Add Product",
                "Edit Product",
                "Delete Product",
                "Make Discount",
                "Delete Discount"
            }, 20, 2));

            switch (shopControl.DrawAndStart())
            {
                case "See Products":
                    DisplayProducts();
                    break;
                case "Add Product":
                    Device device = new Device(
                        new Barcode(0b001),
                        "Blender GÖTZE & JENSEN HB950K Inox z krajarką w kostkę",
                        "Blender kuchenny to wielofunkcyjne urządzenie, które umożliwia szybkie miksowanie, blendowanie i rozdrabnianie składników. Idealny do przygotowywania koktajli, zup kremów, sosów i smoothie. Wyposażony w mocny silnik i ostrza ze stali nierdzewnej, zapewnia efektywną pracę i łatwość obsługi. Niezastąpiony w każdej nowoczesnej kuchni",
                        31.99M,
                        Category.Home,
                        new ASCIImage("Products//blender.txt"),
                        1000M,
                        230M,
                        1M,
                        1M
                    );
                    device.techSpecification = new Dictionary<string, string>()
                    {
                        { "Regulacja obrotów", "Mechaniczna-płynna" },
                        { "Funkcje", "Krojenie, Siekanie, Szatkowanie, Ubijanie piany, Ucieranie" }
                    };

                    using (var file = File.Create("data//blender.prdx"))
                    {
                        Serializer.Serialize(file, device);
                    }
                    break;
                case "Search by Name":
                    DisplaySearchByKeyword();
                    break;
                case "Search by Category":
                    DisplaySearchByCategory();
                    break;
                case "Return to menu":
                    DisplayStartMenu();
                    break;
                default:
                    break;
            }
        }

        static void DisplayStartMenu()
        {
            Control control = new Control();
            control.AddWindow(new Window(new List<string>()
            {
                "Enter the Shop",
                "Load wish list",
                "Exit"
            }, 35, 11, 20));

            ASCIImage Title = new ASCIImage("title.txt");
            ASCIImage TV    = new ASCIImage("tv.txt");
            Title.Display(10, 2);
            TV.Display(90, 2);
            switch (control.DrawAndStart())
            {
                case "Enter the Shop":
                    DisplayShop();
                    break;
                case "Load wish list":
                    Console.WriteLine("X");
                    break;
                case "Exit":
                    break;
            }
        }

        static void Main(string[] args)
        {
            //Byte k = 0b11111111;
            //Console.WriteLine(k);
            DisplayStartMenu();
        }
    }
}
/*
 * Sklep Internetowy
 * Celem projektu jest przygotowanie symulacji działania prostego sklepu internetowego
 * - przeglądanie oferty , dodawanie do koszyka i składanie zamówień
 * 
 * Wymaganie funkcjonalności:
 * -Przeglądanie katalogu produktów z ceną , opisem i dostępnością
 * -Dodawanie produktów do koszyka i ich usuwanie
 * -Finalizacja zamówienia z wyświetleniem podsumowania i łącznej ceny
 * -Obsługa stanu magazynowego - liczba sztuk danego produktu
 * -Możliwość dodawania/usuwania produktów przez administratora
 * -Ewentualnie: zapis zamówień do pliku , 
 * 
 * 
 * 
 * Projekt oceniany jest według następujących kryteriów:

A. Planowanie i organizacja projektu – 15 pkt
5 pkt – Wstępna prezentacja struktury klas i podziału 
na moduły (np. UML, pseudokod, szkic w kodzie).
5 pkt – Dokumentacja interfejsów programistycznych 
(API) (opis głównych metod i atrybutów, sposób komunikacji między komponentami).
5 pkt – Podział zadań w zespole (czy podział pracy jest logiczny,
czy każdy członek zespołu ma określone obowiązki).
B. Implementacja – 40 pkt
12 pkt – Ocena struktury obiektowej programu (poprawny podział na klasy, relacje między nimi, odpowiednie atrybuty, metody).
10 pkt – Poprawność implementacji (czy funkcjonalność została poprawnie zrealizowana, czy program działa bez istotnych błędów).
6 pkt – Jakość kodu (czytelność, zgodność z konwencjami, brak zbędnych powtórzeń, organizacja plików).
5 pkt – Obsługa błędów i wyjątków (czy program poprawnie obsługuje błędy, czy używa mechanizmu wyjątków tam, gdzie to konieczne).
4 pkt – Wykorzystanie zaawansowanych mechanizmów programowania obiektowego (polimorfizm, dziedziczenie, klasy abstrakcyjne i interfejsy, modularność, przeciążanie operatorów, tam gdzie to uzasadnione).
3 pkt – Interfejs użytkownika (jeśli dotyczy – czy program jest intuicyjny w obsłudze, czy dokumentacja pozwala go uruchomić i używać).
C. Prezentacja projektu – 5 pkt
3 pkt – Dokumentacja użytkownika i programisty 
(README, instrukcja uruchomienia, wyjaśnienie działania programu).
2 pkt – Umiejętność przedstawienia projektu
(czy studenci potrafią wyjaśnić działanie swojego
kodu i odpowiedzieć na pytania prowadzącego).
D. Kara za oddanie projektu po terminie
-5 pkt – za oddanie projektu po 14. zajęciach
W celu uzyskania zaliczenia, za projekt należy zdobyć przynajmniej 30 punktów!
 */

//zespół 3
//temat 2
//ps 4
//C#
