using System;
using System.Security.Principal;
using Assets;
using Models;
using System.Media;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;


namespace ProjektPO
{
    class Program
    {
        static List<Product> LoadAllProducts()
        {
            List<Product> products = new List<Product>();
            DirectoryInfo directory = new DirectoryInfo("data");

            if (!directory.Exists)
            {
                Directory.CreateDirectory("data");
                return products;
            }

            foreach (var file in directory.GetFiles("*.prdx"))
            {
                try
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        var product = Serializer.Deserialize<Product>(stream); 
                        products.Add(product);
                        Console.WriteLine($"Loaded: {product.Name} ({product.Category})"); // Debug
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading {file.Name}: {ex.GetType().Name} - {ex.Message}");
                }
            }
            return products;
        }

        static List<Product> cart = new List<Product>();
        static void DisplaySearchResults(List<Product> found)
        {
            Console.WriteLine($"Debug: Found {found.Count} products");
            if (found.Count == 0)
            {
                Console.WriteLine("No products found");
                Console.WriteLine("Press any key to go back...");
                Console.ReadKey();
                Console.Clear();
                DisplayShop();
                return;
            }
            Console.Clear();
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


        static void EditProduct()
        {
            DirectoryInfo productDirectory = new DirectoryInfo("data");
            FileInfo[] productFiles = productDirectory.GetFiles("*.prdx");
            List<string> productNames = new List<string>();
            foreach (var file in productFiles)
            {
                productNames.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
            productNames.Add("Return");
            Control editControl = new Control();
            editControl.AddWindow(new Window(productNames, 2, 2));
            string selectedProduct = editControl.DrawAndStart();
            if (selectedProduct == "Return")
            {
                DisplayShop();
                return;
            }
            string filePath = $"data//{selectedProduct}.prdx";
            //productToEdit = DatabaseAction.DeserializeProduct(filePath);

            if (File.Exists(filePath))
            {
                Device productToEdit;
                using (var file = File.OpenRead(filePath))
                {
                    productToEdit = Serializer.Deserialize<Device>(file);
                }
                //Editing!
                productToEdit.setName("Edited Product Name");
                productToEdit.setPrice(50.99M);
                //
                using (var file = File.Create(filePath))
                {
                    Serializer.Serialize(file, productToEdit);
                }

                Console.WriteLine("Product edited successfully!");
                
            }
            else
            {
                Console.WriteLine("Error: Product file not found!");
            }
            //DisplayShop();
        }

        static void DisplayProducts()
        {
            Control productsControl = new Control();
            DirectoryInfo directoryWithImages = new DirectoryInfo("data");
            FileInfo[] Files = directoryWithImages.GetFiles("*.prdx");
            List<string> file_names = new List<string>();
            foreach (FileInfo file in Files)
            {
                file_names.Add(Path.GetFileNameWithoutExtension(file.Name));
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

        static void DeleteProduct()
        {
            DirectoryInfo productDirectory = new DirectoryInfo("data");
            FileInfo[] productFiles = productDirectory.GetFiles("*.prdx");
            List<string> productNames = new List<string>();
            foreach (var file in productFiles)
            {
                productNames.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
            productNames.Add("Return");
            Control deleteControl = new Control();
            deleteControl.AddWindow(new Window(productNames, 2, 2));
            string selectedProduct = deleteControl.DrawAndStart();
            if (selectedProduct == "Return")
            {
                return;
            }
            string filePath = $"data//{selectedProduct}.prdx";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("Product deleted successfully!");
            }
            else
            {
                Console.WriteLine("Error: Product file not found!");
            }
        }

        static void AddProduct()
        {
            Console.Clear();

            string name_;
            string description_;
            byte barcode_;
            decimal price_;
            Category category_;
            string imagePath_;

            InputField inputName = new InputField();
            InputField inputDescription = new InputField();
            InputField inputBarcode = new InputField();
            InputField inputPrice = new InputField();
            InputField inputImagePath = new InputField();

            Console.SetCursorPosition(4, 2);
            Console.Write("Enter product name:");
            name_ = inputName.DrawAndStart(4, 3);

            Console.SetCursorPosition(4, 4);
            Console.Write("Enter description:");
            description_ = inputDescription.DrawAndStart(4, 5);

            Console.SetCursorPosition(4, 6);
            Console.Write("Enter barcode (number):");
            while (!byte.TryParse(inputBarcode.DrawAndStart(4, 7), out barcode_))
            {
                Console.SetCursorPosition(4, 8);
                Console.Write("Invalid input! Please enter a number (0-255):");
                inputBarcode = new InputField();
            }

            Console.SetCursorPosition(4, 9);
            Console.Write("Enter price:");
            while (!decimal.TryParse(inputPrice.DrawAndStart(4, 10), out price_))
            {
                Console.SetCursorPosition(4, 11);
                Console.Write("Invalid price! Please enter a valid number:");
                inputPrice = new InputField();
            }

            Console.SetCursorPosition(4, 12);
            Console.Write("Select category:");
            Window categoryWindow = new Window(
                new List<string>()
                {
            "Home",
            "Kitchen",
            "TTLs",
            "Gaming",
            "Motor",
            "Accessory"
                }, 4, 13);

            string selectedCategoryStr = categoryWindow.DrawAndStart();

            try
            {
                category_ = (Category)Enum.Parse(typeof(Category), selectedCategoryStr);
            }
            catch
            {
                category_ = Category.Home; 
            }

            Console.SetCursorPosition(4, 20);
            Console.Write("Enter image path (e.g., 'Products/image.txt'):");
            imagePath_ = inputImagePath.DrawAndStart(4, 21);

            try
            {
                Device newProduct = new Device(
                    new Barcode(barcode_),
                    name_,
                    description_,
                    price_,
                    category_,
                    new ASCIImage(imagePath_),
                    0, 0, 0, 0 
                );

                string fileName = $"data/{name_.ToLower().Replace(" ", "_")}.prdx";
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    Serializer.Serialize(file, newProduct);
                    file.SetLength(file.Position); 
                }

                Console.Clear();
                Console.WriteLine($"Product '{name_}' added successfully!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Error adding product: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            DisplayShop(); 
        }

        static void DisplayShop()
        {
            Control shopControl = new Control();
            Console.BackgroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(1, 1);
            Console.WriteLine("Main Actions");
            Console.BackgroundColor = ConsoleColor.Black;
            shopControl.AddWindow(new Window(new List<string>()
            {
                "See Products",
                "Wish List",
                //"Order", order insiede wishlist mate
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
                "Delete Discount",
                "Add Product to Storage",
                "See Storage"
            }, 1, 10));

            switch (shopControl.DrawAndStart())
            {
                case "See Products":
                    DisplayProducts();
                    break;
                case "Add Product":
                    AddProduct();

                    //Product device = new Product(
                    //    new Barcode(0b111),
                    //    "Statyw Camrock CP-530 Czarny",
                    //    "Camrock CP-530 to kompaktowy, poręczny statyw. Idealny dla youtuberów, vlogerów, twórców wideo oraz pasjonatów fotografii. Z łatwością zmieści się w każdej podróżnej torbie czy plecaku.",
                    //    70.84M,
                    //    Category.Accesory,
                    //    new ASCIImage("Products//statyw.txt")
                    //    );
                    //device.techSpecification = new Dictionary<string, string>()
                    //{
                    //    { "Całkowity wymiar ekspozycji:", " 64 cm x 43" },
                    //    { "Zasilanie","Baterie" }
                    //};

                    //using (var file = File.Create("data//statyw.prdx"))
                    //{
                    //    Serializer.Serialize(file, device);
                   // }
                    //
                    break;
                case "Edit Product":
                    {
                        EditProduct();
                    }
                    break;

                case "Delete Product":
                    DeleteProduct();
                    break;
                case "Return to menu":
                    DisplayStartMenu();
                    break;
                case "Search by Name":
                    DisplaySearchByKeyword();
                    break;
                case "Search by Category":
                    DisplaySearchByCategory();
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
                    { 
                        DisplayShop();
                    }
                    break;
                case "Load wish list":
                    {
                        Console.WriteLine("X");
                    }
                    break;
                case "Exit":
                    { }
                    break;
            }
        }

        static void Main(string[] args)
        {
            /*
            Product device = new Product(
                        new Barcode(0b111),
                        "Statyw Camrock CP-530 Czarny",
                        "Camrock CP-530 to kompaktowy, poręczny statyw. Idealny dla youtuberów, vlogerów, twórców wideo oraz pasjonatów fotografii. Z łatwością zmieści się w każdej podróżnej torbie czy plecaku.",
                        70.84M,
                        Category.Accesory,
                        new ASCIImage("Products//statyw.txt")
                        );

            try
            {
                Console.WriteLine(((Device)device).getCurrent());
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }*/

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
