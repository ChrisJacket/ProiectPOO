namespace ProiectPOO;
using System;
using System.Collections.Generic;
using System.IO;
public class Magazin
{
    private List<Comanda> Comenzi { get; set; }
    private List<Produs> StocMagazin { get; set; }
    public List<User> Users { get; private set; }

    public Magazin()
    {
        Comenzi = new List<Comanda>();
        StocMagazin = new List<Produs>();
        Users = new List<User>();
    }
    
    public void LoadUsersFromFile()
    {
        string filePath = "users.txt";
    
        // Verificăm dacă fișierul există
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Fișierul cu utilizatori nu a fost găsit.");
            return;
        }

        try
        {
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var userData = line.Split(',');

                // Asigurăm că avem suficiente date
                if (userData.Length == 5)
                {
                    string surname = userData[0].Trim();
                    string name = userData[1].Trim();
                    string password = userData[2].Trim();
                    string email = userData[3].Trim();
                    string userTypeString = userData[4].Trim();

                    // Încercăm să convertim userTypeString în UserTypes
                    UserTypes userType;
                    if (Enum.TryParse(userTypeString, true, out userType))
                    {
                        // Creăm un utilizator de tipul corespunzător
                        User user;
                        if (userType == UserTypes.Admin)
                        {
                            user = new Admin(surname, name, password, email);
                        }
                        else
                        {
                            user = new Client(surname, name, password, email);
                        }

                        // Adăugăm utilizatorul în lista de utilizatori
                        Users.Add(user);
                    }
                    else
                    {
                        Console.WriteLine($"Tip utilizator invalid: {userTypeString}. Ignorăm această linie.");
                    }
                }
            }

            Console.WriteLine("Utilizatorii au fost încărcați cu succes.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea fișierului de utilizatori: {ex.Message}");
        }
    }

    
    
    
    public List<Comanda> LoadOrdersFromFile()
    {
        List<Comanda> orders = new List<Comanda>();

        try
        {
            string filePath = "orders.txt";

            // Verificăm dacă fișierul există, dacă nu, îl creăm
            if (!File.Exists(filePath))
            {
                // Creăm fișierul gol
                File.Create(filePath).Dispose();
                Console.WriteLine($"Fișierul {filePath} nu există. A fost creat.");
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var orderData = line.Split(',');

                if (orderData.Length < 9)
                {
                    Console.WriteLine("Linie incorectă în fișierul de comenzi: " + line);
                    continue;
                }

                string orderId = orderData[0];
                DateOnly orderDate = DateOnly.Parse(orderData[1]);
                OrderStatus orderStatus = Enum.Parse<OrderStatus>(orderData[2]);
                string customerName = orderData[3];
                string customerEmail = orderData[4];
                string city = orderData[5];
                string country = orderData[6];
                string postcode = orderData[7];
                
                ShippingAddress address = new ShippingAddress
                {
                    City = city,
                    Country = country,
                    Postcode = postcode,
                    Address = $"{orderData[5]} {orderData[6]}"
                };

                // Extragem produsele comandate
                Dictionary<Produs, int> productsOrdered = new Dictionary<Produs, int>();
                
                for (int i = 8; i < orderData.Length; i += 2)
                {
                    string productName = orderData[i];
                    int quantity = int.Parse(orderData[i + 1]);

                    // Vom crea un produs cu un preț fictiv, deoarece nu avem prețul real în fișier
                    // În mod normal, ar trebui să obținem produsul dintr-o bază de date sau să-l găsim într-o listă preexistentă
                    Produs product = new Produs(productName, productName, 10.0, 100, ProductCategory.Auto); // Exemplu de produs cu preț de 10.0
                    productsOrdered.Add(product, quantity);
                }

                // Creăm comanda
                Comanda order = new Comanda(productsOrdered, orderId, new Client(customerName, customerName, "password", customerEmail), orderStatus, address);
                orders.Add(order);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea fișierului de comenzi: {ex.Message}");
        }

        return orders;
    }
    public List<Produs> LoadProductsFromFile()
{
    List<Produs> products = new List<Produs>();

    try
    {
        string filePath = "products.txt";

        // Dacă fișierul nu există, îl creăm
        if (!File.Exists(filePath))
        {
            // Creăm fișierul gol
            File.Create(filePath).Dispose();
            Console.WriteLine($"Fișierul {filePath} nu exista. A fost creat.");
        }

        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var productData = line.Split(',');

            string id = productData[0];
            string name = productData[1];
            double price = double.Parse(productData[2]);
            int stock = int.Parse(productData[3]);
            ProductCategory category = Enum.Parse<ProductCategory>(productData[4]);

            // Creăm produsul și-l adăugăm în lista de produse
            Produs product = new Produs(id, name, price, stock, category);

            // Dacă există reduceri pentru produs, le adăugăm (dacă sunt specificate în fișier)
            if (productData.Length > 5)
            {
                string discountType = productData[5];

                if (discountType == "Percentage" && productData.Length > 6)
                {
                    int percentageDiscount = int.Parse(productData[6]);
                    product.AddDiscount(DiscountTypes.Percentage, percentageDiscount);
                }
                else if (discountType == "Constant" && productData.Length > 6)
                {
                    int constantDiscount = int.Parse(productData[6]);
                    product.AddDiscount(DiscountTypes.Constant, constantDiscount);
                }
                else if (discountType == "TwoPlusOne")
                {
                    product.AddDiscount(DiscountTypes.TwoPlusOne, 0);
                }
            }

            products.Add(product);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Eroare la citirea fișierului de produse: {ex.Message}");
    }

    return products;
}


    private string GenerateNextOrderId()
    {
        Random Randomizer = new Random();
        string randomID;
        do
            randomID = "O" + Randomizer.Next(100000, 999999).ToString(); // O de la order
        while (IdAlreadyExists(randomID));
        return randomID;
    }
    private string GenerateNextProductId()
    {
        Random Randomizer = new Random();
        string randomID;
        do
            randomID = "P" + Randomizer.Next(100000, 999999).ToString(); // O de la order
        while (IdAlreadyExists(randomID));
        return randomID;
    }
    private bool IdAlreadyExists(string proposedId)
    {
        if (proposedId[0] == 'O')
            foreach (Comanda comanda in Comenzi)
            {
                if (comanda.ID == proposedId) return true;
            }
        else if (proposedId[0] == 'P')
            foreach (Produs produs in StocMagazin)
            {
                if (produs.ID == proposedId) return true;
            }
        return false;
    }
    public void AddOrder(Dictionary<Produs, int> productsOrdered, Client recipient, OrderStatus status, ShippingAddress deliveryAddress)
    {
        Comanda NewOrder = new Comanda(productsOrdered, GenerateNextOrderId(), recipient, OrderStatus.BeingProcessed, deliveryAddress);
        if (OrderAlreadyExists(NewOrder))
            Console.WriteLine("Order has already been added!");
        else 
            Comenzi.Add(NewOrder);
    }
    private bool OrderAlreadyExists(Comanda comanda)
    {
        foreach(Comanda comanda_aux in Comenzi)
            if(comanda.ID == comanda_aux.ID) return true;
        return false;
    }
    public void AddProduct(string productName, double price, int stock, ProductCategory productCategory)
    {
        Produs NewProduct = new Produs(GenerateNextProductId(), productName, price, stock, productCategory);
        if (ProductAlreadyExists(NewProduct))
            Console.WriteLine("Product has already been added!");
        else
            StocMagazin.Add(NewProduct);
    }
    private bool ProductAlreadyExists(Produs produs)
    {
        foreach (Produs produs_aux in StocMagazin)
        {
            if (produs.ID == produs_aux.ID) return true;
            // aici poate putem face o chestie, daca produsul deja exista
            // astfel incat sa adauge stocul produsului nou la produsul care deja exista
        }
        return false;
    }
    public void EditOrder(Comanda comanda)
    {
        foreach(Comanda comanda_aux in Comenzi)
        {
            if(comanda_aux.ID == comanda.ID)
            {
                // logica de modificare a comenzii
                break;
            }
        }
    }
    public void EditProduct(Produs produs)
    {
        foreach (Produs produs_aux in StocMagazin)
        {
            if (produs_aux.ID == produs.ID)
            {
                // logica de modificare a produsului
                break;
            }
        }
    }

    public User AuthenticateUser()
    {
        string email, parola;
        while (true)
        {
            Console.Write("Email: ");
            email = Console.ReadLine();

            Console.Write("Parola: ");
            parola = Console.ReadLine();

            if (email != null && parola != null)
                break;
            else
                Console.WriteLine("Emailul sau parola nu pot fi campuri goale!\n");
        }

        Console.Clear();

        // Verifica daca exista utilizatorul in lista de utilizatori
        // "=>" a se citi "cu proprietatea ca"
        User user = Users.FirstOrDefault(u => u.VerifyUserCredentials(email, parola));

        if (user != null)
        {
            return user; // Utilizatorul există și autentificarea reușește
        }
        else
        {
            Console.WriteLine("Utilizatorul nu a fost găsit în sistem. Doriți să creați un cont nou? (da/nu)");
            string response = Console.ReadLine()?.Trim().ToLower();

            if (response == "da")
            {
                Console.Write("Introduceti prenumele: ");
                string firstName = Console.ReadLine()?.Trim();

                Console.Write("Introduceti numele de familie: ");
                string lastName = Console.ReadLine()?.Trim();
                

                // Creează un nou client
                var newClient = new Client(firstName, lastName, parola, email);

                // Adaugă utilizatorul în lista locală
                Users.Add(newClient);

                // Adaugă utilizatorul în fișier
                AddUserToFile(newClient);

                Console.WriteLine("Cont creat cu succes! Vă puteți autentifica acum.");
            }
        }
        return null;
    }
    
    //Adăugare clienti dacă nu sunt deja în fișier
    private void AddUserToFile(User user)
    {
        string filePath = "users.txt";

        try
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                // Formatează linia pentru fișier
                string userLine =
                    $"{user.LastName},{user.FirstName},{user.Password},{user.EmailAddress},{user.UserType}";
                sw.WriteLine(userLine);
            }

            Console.WriteLine("Utilizatorul a fost adăugat în fișier.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la scrierea în fișier: {ex.Message}");
        }
    }
}
