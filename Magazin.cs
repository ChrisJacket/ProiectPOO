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
        string filePath = "products.txt";
        List<Produs> products = new List<Produs>();

        try
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var fields = line.Split('|');
                    string id = fields[0];
                    string name = fields[1];
                    double price = double.Parse(fields[2]);
                    int stock = int.Parse(fields[3]);
                    ProductCategory category = Enum.Parse<ProductCategory>(fields[4]);
                
                    Produs product = new Produs(id, name, price, stock, category);

                    // Aplica reducerile
                    if (!string.IsNullOrEmpty(fields[5]))
                    {
                        var discounts = fields[5].Split(',');
                        foreach (var discount in discounts)
                        {
                            DiscountTypes discountType = Enum.Parse<DiscountTypes>(discount);
                            int discountValue = 0;
                        
                            if (discountType == DiscountTypes.Percentage)
                                discountValue = int.Parse(fields[6]);
                            else if (discountType == DiscountTypes.Constant)
                                discountValue = int.Parse(fields[7]);

                            product.AddDiscount(discountType, discountValue);
                        }
                    }

                    products.Add(product);
                }
            }
            else
            {
                Console.WriteLine("Fișierul de produse nu există. Se va crea unul nou la prima salvare.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea produselor: {ex.Message}");
        }

        return products;
    }


    public void SignUpClient(Client ClientNou)
    {
        Users.Add(ClientNou);
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
        Comenzi.Add(NewOrder);
    }
    private bool ProductAlreadyExists(Admin admin, Produs produs)
    {
        foreach (Produs produs_aux in StocMagazin)
        {
            if (produs.Name == produs_aux.Name)
            {
                produs_aux.AddStock(admin, produs.Stock);
                return true;
            }
            // aici poate putem face o chestie, daca produsul deja exista
            // astfel incat sa adauge stocul produsului nou la produsul care deja exista
        }
        return false;
    }
    public void EditOrder(Comanda comanda)
    {
        foreach (Comanda comanda_aux in Comenzi)
        {
            if (comanda_aux.ID == comanda.ID)
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
        return Users.FirstOrDefault(u => u.VerifyUserCredentials(email, parola), null);
    }

    public void ManageStore(Admin ThisAdmin)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("1. Adauga produs in magazin");
            Console.WriteLine("2. Modifica produs");
            Console.WriteLine("3. Sterge produs");
            Console.WriteLine("4. Actualizeaza stocurile unui produs");
            Console.WriteLine("5. Adauga discount unui produs");
            if (ProductsWithLowStockExist() == true)
                Console.WriteLine("6. Afiseaza produsele cu stoc scazut (IMPORTANT)");
            Console.WriteLine("0. Iesire");

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddProduct(ThisAdmin);
                    break;

                case "2":
                    ModifyProduct(ThisAdmin);
                    break;

                case "3":
                    DeleteProduct(ThisAdmin);
                    break;

                case "4":
                    UpdateProductStock(ThisAdmin);
                    break;

                case "5":
                    AddProductDiscount(ThisAdmin);
                    break;

                case "6":
                    if (ProductsWithLowStockExist() == true)
                        ShowProductsWithLowStock();
                    else
                        Console.WriteLine("There are no products with low stock!");
                    break;
                case "0":
                    running = false;
                    break;

            }

        }
    }
    private void AddProduct(Admin admin)
    {
        string ProposedProductName;
        double ProposedProductPrice;
        int ProposedProductInitialStock;
        int ProposedProductCategory;
        bool conversion1, conversion2;
        while (true)
        {
            Console.WriteLine("Numele produsului:");
            ProposedProductName = Console.ReadLine();

            Console.WriteLine("Pretul produsului:");
            conversion1 = double.TryParse(Console.ReadLine(), out ProposedProductPrice);

            Console.WriteLine("Stocul initial al produsului:");
            conversion2 = int.TryParse(Console.ReadLine(), out ProposedProductInitialStock);

            Console.WriteLine("Categoria produsului:");
            int.TryParse(Console.ReadLine(), out ProposedProductCategory);

            if (!conversion1 || !conversion2)
                Console.WriteLine("Una sau mai multe valori nu a putut fi citita corect. Incercati din nou.");
            else if (ProposedProductName == null)
                Console.WriteLine("Produsul trebuie sa aiba un nume!");
            else
                break;
        }

        Produs NewProduct = new Produs(GenerateNextProductId(), ProposedProductName, ProposedProductPrice, ProposedProductInitialStock, (ProductCategory)ProposedProductCategory);
        if (ProductAlreadyExists(admin, NewProduct))
        {
            Console.WriteLine("Exista deja un produs cu acelasi nume! S-a adaugat stocul produsului de la intrare la produsul deja existent.");
        }
        else
            StocMagazin.Add(NewProduct);
    }

    private Produs? FindProduct()
    {
        string StringToSearch;
        Produs? ProductToFind = null;
        while (true)
        {
            Console.WriteLine("ID-ul sau numele produsului cautat:");
            StringToSearch = Console.ReadLine();

            if (StringToSearch != null)
                break;
        }

        foreach (Produs produs in StocMagazin)
        {
            if (produs.ID == StringToSearch || produs.Name == StringToSearch)
            {
                ProductToFind = produs;
                break;
            }
        }

        if (ProductToFind == null)
        {
            Console.WriteLine("Produsul cu ID-ul specificat nu a fost gasit.");
            return null;
        }

        return ProductToFind;
    }

    private void ModifyProduct(Admin admin)
    {

        Produs? ProductToModify = FindProduct();
        if (ProductToModify == null)
            return;

        bool running = true;
        while (running)
        {
            Console.WriteLine("Ce doriti sa modificati la produs?");
            Console.WriteLine("1. Numele");
            Console.WriteLine("2. Pretul");
            Console.WriteLine("3. Descrierea");
            Console.WriteLine("4. Iesire");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("Introduceti noul nume pentru produs:");
                    string NewProductName = Console.ReadLine();
                    ProductToModify.ModifyName(admin, NewProductName);
                    break;

                case "2":
                    Console.WriteLine("Introduceti noul pret al produsului:");
                    int.TryParse(Console.ReadLine(), out int NewProductPrice);
                    ProductToModify.ModifyPrice(admin, NewProductPrice);
                    break;

                case "3":
                    Console.WriteLine("Introduceti noua descriere a produsului:");
                    string NewProductDescription = Console.ReadLine();
                    ProductToModify.ModifyDescription(admin, NewProductDescription);
                    break;

                case "4":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Alegere invalida!");

                    break;
            }
        }
    }

    private void DeleteProduct(Admin admin)
    {
        Produs? ProductToDelete = FindProduct();

        if (ProductToDelete != null)
        {
            StocMagazin.Remove(ProductToDelete);
            Console.WriteLine($"Produsul {ProductToDelete.Name} cu ID-ul {ProductToDelete.ID} a fost sters!");
        }
    }

    private void UpdateProductStock(Admin admin)
    {
        Produs? ProductToUpdate = FindProduct();

        Console.WriteLine("Introduceti noua valoarea a stocului produsului:");
        int.TryParse(Console.ReadLine(), out int NewStock);

        if (ProductToUpdate != null)
            ProductToUpdate.ModifyStock(admin, NewStock);
    }

    private void AddProductDiscount(Admin admin)
    {
        Produs? ProductToDiscount = FindProduct();
        bool running = true;
        while (running)
        {
            Console.WriteLine("Ce tip de reducere doriti sa adaugati produsului?");
            Console.WriteLine("1. Oferta 2 + 1 gratis");
            Console.WriteLine("2. Procentaj din pretul actual");
            Console.WriteLine("3. Valoare fixa");
            Console.WriteLine("4. Anulare");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    ProductToDiscount.AddDiscount(DiscountTypes.TwoPlusOne, 0);
                    Console.WriteLine($"Oferta 2 + 1 gratis activata pentru produsul {ProductToDiscount.Name}, ID: {ProductToDiscount.ID}");
                    break;

                case "2":
                    int PercentageToDiscount;
                    Console.WriteLine("Ce procentaj din pretul produsului vreti sa fie redus?");
                    int.TryParse(Console.ReadLine(), out PercentageToDiscount);
                    ProductToDiscount.AddDiscount(DiscountTypes.Percentage, PercentageToDiscount);
                    break;

                case "3":
                    int FlatValueToDiscount;
                    Console.WriteLine("Ce valoare vreti sa fie redusa din pretul produsului?");
                    int.TryParse(Console.ReadLine(), out FlatValueToDiscount);
                    ProductToDiscount.AddDiscount(DiscountTypes.Constant, FlatValueToDiscount);
                    break;

                case "4":
                    running = false;
                    // exit
                    break;
            }
        }
    }

    private bool ProductsWithLowStockExist()
    {
        bool LowStockProductsExist = false;
        foreach (Produs produs in StocMagazin)
        {
            if (produs.Stock < 10)
            {
                LowStockProductsExist = true;
            }
        }
        return LowStockProductsExist;
    }

    private void ShowProductsWithLowStock()
    {
        foreach (Produs produs in StocMagazin)
        {
            if (produs.Stock < 10)
            {
                Console.WriteLine($"Produsul {produs.Name}, ID: {produs.ID} are stoc redus: {produs.Stock} bucati ramase");
            }
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public void ManageOrders(Admin admin)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Ce actiune doriti sa aplicati asupra comenzilor?");
            Console.WriteLine("1. Vizualizare");
            Console.WriteLine("2. Editare");
            Console.WriteLine("3. Niciuna - iesire");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    foreach (Comanda comanda in Comenzi)
                    {
                        int count = 1;
                        Console.WriteLine($"{count}. ID: {comanda.ID}\n " +
                            $"Produse comandate: {comanda.ProductsOrdered.Keys}, valoare totala {comanda.OrderPrice}\n " +
                            $"Plasata la data {comanda.PlacementDate}, status: {comanda.Status}");
                        count ++;
                    }
                    break;

                case "2":
                    bool localRunning = true;
                    string IdToSearch;
                    Comanda? OrderToFind = null;
                    while (true)
                    {
                        Console.WriteLine("ID-ul comenzii cautate:");
                        IdToSearch = Console.ReadLine();

                        if (IdToSearch != null)
                            break;
                    }

                    foreach (Comanda comanda in Comenzi)
                    {
                        if (comanda.ID == IdToSearch)
                        {
                            OrderToFind = comanda;
                            break;
                        }
                    }

                    if (OrderToFind == null)
                    {
                        Console.WriteLine("Comanda cu ID-ul specificat nu a fost gasita.");
                        return;
                    }

                    if (OrderToFind.Status == OrderStatus.Canceled)
                    {
                        Console.WriteLine("Aceasta comanda nu mai poate fi modificata, deoarece a fost anulata");
                    }
                    while (localRunning) { 
                    Console.WriteLine($"Statusul curent al comenzii este {OrderToFind.Status}");
                    Console.WriteLine("Ce status doriti sa ii dati?");
                    Console.WriteLine("1. Being processed");
                    Console.WriteLine("2. Sent");
                    Console.WriteLine("3. Delivered");
                    Console.WriteLine("4. Niciunul - anulare");
                    var StatusInput = Console.ReadLine();
                        switch (StatusInput)
                        {
                            case "1":
                                if (OrderToFind.Status == OrderStatus.BeingProcessed)
                                    Console.WriteLine("Comanda are deja acest status!");
                                else
                                    OrderToFind.UpdateStatus(admin, OrderStatus.BeingProcessed);
                                break;

                            case "2":
                                if (OrderToFind.Status == OrderStatus.Sent)
                                    Console.WriteLine("Comanda are deja acest status!");
                                else
                                    OrderToFind.UpdateStatus(admin, OrderStatus.Sent);
                                break;
                            case "3":
                                if (OrderToFind.Status == OrderStatus.Delivered)
                                    Console.WriteLine("Comanda are deja acest status!");
                                else
                                {
                                    OrderToFind.UpdateStatus(admin, OrderStatus.Delivered);
                                    OrderToFind.SetDeliveryDate();
                                }
                                break;
                            case "4":
                                // exit
                                break;
                        }
                    }
                break;
            }


        }
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
    public void SaveOrderToFile(Comanda order)
    {
        string filePath = "orders.txt";

        try
        {
            // Construim manual linia pentru fișier
            string products = string.Join(";", order.ProductsOrdered.Select(p => $"{p.Key.ID}:{p.Value}"));
            string address = $"{order.DeliveryAddress.City},{order.DeliveryAddress.Country},{order.DeliveryAddress.Postcode},{order.DeliveryAddress.Address}";
        
            string line = $"{order.ID}|{order.PlacementDate}|{order.Recipient.EmailAddress}|{order.Status}|{address}|{order.OrderPrice}|{products}";

            // Adăugăm linia în fișier
            File.AppendAllText(filePath, line + Environment.NewLine);
            Console.WriteLine("Comanda a fost salvată cu succes în fișier.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la salvarea comenzii: {ex.Message}");
        }
    }

    
    public void SaveProductToFile(Produs product)
    {
        string filePath = "products.txt";

        try
        {
            // Scrie produsul în format text în fișier
            File.AppendAllText(filePath, product.ToFileFormat() + Environment.NewLine);
            Console.WriteLine("Produsul a fost salvat cu succes în fișier.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la salvarea produsului: {ex.Message}");
        }
    }



    public void CreateSalesReport(Admin admin)
    {
        double TotalSalesFigure = 0, LastMonthSalesFigure = 0;
        int CanceledOrders = 0;
        Dictionary<Produs, int> ProductPopularityHashmap = new Dictionary<Produs, int>();

        foreach(Comanda comanda in Comenzi)
        {
            TotalSalesFigure += comanda.OrderPrice;
            if (comanda.PlacementDate > DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && comanda.Status != OrderStatus.Canceled)
                LastMonthSalesFigure += comanda.OrderPrice;

            if (comanda.Status == OrderStatus.Canceled)
                CanceledOrders++;

            foreach(var ProductIntPair in comanda.ProductsOrdered)
            {
                if (ProductPopularityHashmap.ContainsKey(ProductIntPair.Key))
                    ProductPopularityHashmap[ProductIntPair.Key] += ProductIntPair.Value;
                else
                    ProductPopularityHashmap.Add(ProductIntPair.Key, ProductIntPair.Value);
            }

        }
        Console.WriteLine($"Suma totala de vanzari a magazinului este {TotalSalesFigure}.\n");
        Console.WriteLine($"Magazinul a avut {Comenzi.Count} comenzi, dintre care doar {CanceledOrders} anulate.\n");
        Console.WriteLine($"Valoarea medie a unei comenzi este de {TotalSalesFigure / Comenzi.Count}.\n");
        Console.WriteLine($"Doar in ultimele 30 de zile, valoarea comenzilor a fost de {LastMonthSalesFigure}.\n");

        int MaxValue = ProductPopularityHashmap.Values.Max();

        if ( MaxValue > 0 )
            Console.WriteLine($"Cele mai vandute produse, cu vanzarea totala de {MaxValue} bucati:\n");
        foreach (var kvp in ProductPopularityHashmap)
        {
            if (kvp.Value == MaxValue && MaxValue != 0 )
                Console.WriteLine($"{kvp.Key.Name}, ID: {kvp.Key.ID}");
        }
            
    }
}
